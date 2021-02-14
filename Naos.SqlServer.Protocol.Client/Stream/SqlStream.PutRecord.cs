// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlStream.PutRecord.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Protocol.Client
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using Naos.CodeAnalysis.Recipes;
    using Naos.Database.Domain;
    using Naos.SqlServer.Domain;
    using OBeautifulCode.Collection.Recipes;
    using OBeautifulCode.Serialization;
    using static System.FormattableString;

    public partial class SqlStream
    {
        /// <inheritdoc />
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = NaosSuppressBecause.CA1506_AvoidExcessiveClassCoupling_DisagreeWithAssessment)]
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = NaosSuppressBecause.CA1502_AvoidExcessiveComplexity_DisagreeWithAssessment)]
        public override PutRecordResult Execute(
            PutRecordOp operation)
        {
            var sqlServerLocator = this.TryGetLocator(operation);
            var payloadSerializationFormat = operation.Payload.GetSerializationFormat();
            var serializerRepresentation = this.GetIdAddIfNecessarySerializerRepresentation(sqlServerLocator, operation.Payload.SerializerRepresentation, payloadSerializationFormat);
            var identifierTypeIds = this.GetIdsAddIfNecessaryType(sqlServerLocator, operation.Metadata.TypeRepresentationOfId);
            var objectTypeIds = this.GetIdsAddIfNecessaryType(sqlServerLocator, operation.Metadata.TypeRepresentationOfObject);
            var tagIds = this.GetIdsAddIfNecessaryTag(sqlServerLocator, operation.Metadata.Tags);
            var tagIdsDictionary = tagIds.ToOrdinalDictionary();
            var tagIdsXml = TagConversionTool.GetTagsXmlString(tagIdsDictionary);

            var storedProcOp = StreamSchema.Sprocs.PutRecord.BuildExecuteStoredProcedureOp(
                this.Name,
                serializerRepresentation,
                identifierTypeIds,
                objectTypeIds,
                operation.Metadata.StringSerializedId,
                payloadSerializationFormat == SerializationFormat.String ? operation.Payload.GetSerializedPayloadAsEncodedString() : null,
                payloadSerializationFormat == SerializationFormat.Binary ? operation.Payload.GetSerializedPayloadAsEncodedBytes() : null,
                operation.Metadata.ObjectTimestampUtc,
                tagIdsXml,
                operation.ExistingRecordEncounteredStrategy,
                operation.RecordRetentionCount,
                operation.TypeVersionMatchStrategy);

            var sqlProtocol = this.BuildSqlOperationsProtocol(sqlServerLocator);
            var sprocResult = sqlProtocol.Execute(storedProcOp);

            var newRecordId = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.PutRecord.OutputParamName.Id)].GetValue<long?>();
            var existingRecordIdsXml = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.PutRecord.OutputParamName.ExistingRecordIdsXml)].GetValue<string>();
            var prunedRecordIdsXml = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.PutRecord.OutputParamName.PrunedRecordIdsXml)].GetValue<string>();

            var existingRecordIds = TagConversionTool.GetTagsFromXmlString(existingRecordIdsXml)?.Select(_ => long.Parse(_.Value, CultureInfo.InvariantCulture)).ToList() ?? new List<long>();
            var prunedRecordIds = TagConversionTool.GetTagsFromXmlString(prunedRecordIdsXml)?.Select(_ => long.Parse(_.Value, CultureInfo.InvariantCulture)).ToList() ?? new List<long>();

            if (prunedRecordIds.Any() && !existingRecordIds.Any())
            {
                throw new InvalidOperationException(Invariant($"There were '{nameof(prunedRecordIds)}' but no '{nameof(existingRecordIds)}', this scenario should not occur, it is expected that the records pruned were in the existing set; pruned: '{prunedRecordIds.Select(_ => _.ToString(CultureInfo.InvariantCulture)).ToCsv()}'."));
            }

            if (existingRecordIds.Any())
            {
                switch (operation.ExistingRecordEncounteredStrategy)
                {
                    case ExistingRecordEncounteredStrategy.None:
                        throw new InvalidOperationException(Invariant($"Operation {nameof(ExistingRecordEncounteredStrategy)} was {operation.ExistingRecordEncounteredStrategy}; did not expect any '{nameof(existingRecordIds)}' value but got '{existingRecordIds.Select(_ => _.ToString(CultureInfo.InvariantCulture)).ToCsv()}'."));
                    case ExistingRecordEncounteredStrategy.ThrowIfFoundById:
                        throw new InvalidOperationException(
                            Invariant(
                                $"Operation {nameof(ExistingRecordEncounteredStrategy)} was {operation.ExistingRecordEncounteredStrategy}; expected to not find a record by identifier '{operation.Metadata.StringSerializedId}' yet found  '{nameof(existingRecordIds)}' '{existingRecordIds.Select(_ => _.ToString(CultureInfo.InvariantCulture)).ToCsv()}'."));
                    case ExistingRecordEncounteredStrategy.ThrowIfFoundByIdAndType:
                        throw new InvalidOperationException(
                            Invariant(
                                $"Operation {nameof(ExistingRecordEncounteredStrategy)} was {operation.ExistingRecordEncounteredStrategy}; expected to not find a record by identifier '{operation.Metadata.StringSerializedId}' and identifier type '{operation.Metadata.TypeRepresentationOfId.GetTypeRepresentationByStrategy(operation.TypeVersionMatchStrategy)}' and object type '{operation.Metadata.TypeRepresentationOfObject.GetTypeRepresentationByStrategy(operation.TypeVersionMatchStrategy)}' yet found  '{nameof(existingRecordIds)}': '{existingRecordIds.Select(_ => _.ToString(CultureInfo.InvariantCulture)).ToCsv()}'."));
                    case ExistingRecordEncounteredStrategy.ThrowIfFoundByIdAndTypeAndContent:
                        throw new InvalidOperationException(
                            Invariant(
                                $"Operation {nameof(ExistingRecordEncounteredStrategy)} was {operation.ExistingRecordEncounteredStrategy}; expected to not find a record by identifier '{operation.Metadata.StringSerializedId}' and identifier type '{operation.Metadata.TypeRepresentationOfId.GetTypeRepresentationByStrategy(operation.TypeVersionMatchStrategy)}' and object type '{operation.Metadata.TypeRepresentationOfObject.GetTypeRepresentationByStrategy(operation.TypeVersionMatchStrategy)}' and contents '{operation.Payload}' yet found  '{nameof(existingRecordIds)}': '{existingRecordIds.Select(_ => _.ToString(CultureInfo.InvariantCulture)).ToCsv()}'."));
                    case ExistingRecordEncounteredStrategy.DoNotWriteIfFoundById:
                    case ExistingRecordEncounteredStrategy.DoNotWriteIfFoundByIdAndType:
                    case ExistingRecordEncounteredStrategy.DoNotWriteIfFoundByIdAndTypeAndContent:
                        if (newRecordId != null)
                        {
                            throw new InvalidOperationException(Invariant($"Expect {nameof(ExistingRecordEncounteredStrategy)} of {operation.ExistingRecordEncounteredStrategy} to not write when there are existing ids found: '{existingRecordIds.Select(_ => _.ToString(CultureInfo.InvariantCulture)).ToCsv()}'; new record id: '{newRecordId}'."));
                        }

                        return new PutRecordResult(null, existingRecordIds, prunedRecordIds);
                    case ExistingRecordEncounteredStrategy.PruneIfFoundById:
                    case ExistingRecordEncounteredStrategy.PruneIfFoundByIdAndType:
                        return new PutRecordResult(newRecordId, existingRecordIds, prunedRecordIds);
                    default:
                        throw new NotSupportedException(
                            Invariant($"{nameof(ExistingRecordEncounteredStrategy)} {operation.ExistingRecordEncounteredStrategy} is not supported."));
                }
            }
            else
            {
                return new PutRecordResult(newRecordId);
            }
        }
    }
}
