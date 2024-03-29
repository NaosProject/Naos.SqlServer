﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlStream.StandardPutRecordOp.cs" company="Naos Project">
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
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Collection.Recipes;
    using OBeautifulCode.Serialization;
    using OBeautifulCode.String.Recipes;
    using static System.FormattableString;

    public partial class SqlStream
    {
        /// <inheritdoc />
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = NaosSuppressBecause.CA1506_AvoidExcessiveClassCoupling_DisagreeWithAssessment)]
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = NaosSuppressBecause.CA1502_AvoidExcessiveComplexity_DisagreeWithAssessment)]
        public override PutRecordResult Execute(
            StandardPutRecordOp operation)
        {
            operation.MustForArg(nameof(operation)).NotBeNull();

            var sqlServerLocator = this.TryGetLocator(operation);
            var payloadSerializationFormat = operation.Payload.GetSerializationFormat();
            var serializerRepresentation = this.GetIdAddIfNecessarySerializerRepresentation(sqlServerLocator, operation.Payload.SerializerRepresentation, payloadSerializationFormat);
            var identifierTypeIds = this.GetIdsAddIfNecessaryType(sqlServerLocator, operation.Metadata.TypeRepresentationOfId);
            var objectTypeIds = this.GetIdsAddIfNecessaryType(sqlServerLocator, operation.Metadata.TypeRepresentationOfObject);
            var tagIdsCsv = operation.Metadata.Tags == null
                ? null
                : this.GetIdsAddIfNecessaryTag(sqlServerLocator, operation.Metadata.Tags).Select(_ => _.ToStringInvariantPreferred()).ToCsv();

            var storedProcOp = StreamSchema.Sprocs.PutRecord.BuildExecuteStoredProcedureOp(
                this.Name,
                serializerRepresentation,
                identifierTypeIds,
                objectTypeIds,
                operation.InternalRecordId,
                operation.Metadata.StringSerializedId,
                payloadSerializationFormat == SerializationFormat.String ? operation.Payload.GetSerializedPayloadAsEncodedString() : null,
                payloadSerializationFormat == SerializationFormat.Binary ? operation.Payload.GetSerializedPayloadAsEncodedBytes() : null,
                operation.Metadata.ObjectTimestampUtc,
                tagIdsCsv,
                operation.ExistingRecordStrategy,
                operation.RecordRetentionCount,
                operation.VersionMatchStrategy);

            var sqlProtocol = this.BuildSqlOperationsProtocol(sqlServerLocator);
            var sprocResult = sqlProtocol.Execute(storedProcOp);

            var newRecordId = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.PutRecord.OutputParamName.Id)].GetValueOfType<long?>();
            var existingRecordIdsCsv = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.PutRecord.OutputParamName.ExistingRecordIdsCsv)].GetValueOfType<string>();
            var prunedRecordIdsCsv = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.PutRecord.OutputParamName.PrunedRecordIdsCsv)].GetValueOfType<string>();

            var existingRecordIds = existingRecordIdsCsv.FromCsv()?.Select(_ => long.Parse(_, CultureInfo.InvariantCulture)).ToList() ?? new List<long>();
            var prunedRecordIds = prunedRecordIdsCsv.FromCsv()?.Select(_ => long.Parse(_, CultureInfo.InvariantCulture)).ToList() ?? new List<long>();

            if (prunedRecordIds.Any() && !existingRecordIds.Any())
            {
                throw new InvalidOperationException(Invariant($"There were '{nameof(prunedRecordIds)}' but no '{nameof(existingRecordIds)}', this scenario should not occur, it is expected that the records pruned were in the existing set; pruned: '{prunedRecordIds.Select(_ => _.ToString(CultureInfo.InvariantCulture)).ToCsv()}'."));
            }

            if (existingRecordIds.Any())
            {
                switch (operation.ExistingRecordStrategy)
                {
                    case ExistingRecordStrategy.None:
                        throw new InvalidOperationException(Invariant($"Operation {nameof(ExistingRecordStrategy)} was {operation.ExistingRecordStrategy}; did not expect any '{nameof(existingRecordIds)}' value but got '{existingRecordIds.Select(_ => _.ToString(CultureInfo.InvariantCulture)).ToCsv()}'."));
                    case ExistingRecordStrategy.ThrowIfFoundById:
                        throw new InvalidOperationException(
                            Invariant(
                                $"Operation {nameof(ExistingRecordStrategy)} was {operation.ExistingRecordStrategy}; expected to not find a record by identifier '{operation.Metadata.StringSerializedId}' yet found  '{nameof(existingRecordIds)}' '{existingRecordIds.Select(_ => _.ToString(CultureInfo.InvariantCulture)).ToCsv()}'."));
                    case ExistingRecordStrategy.ThrowIfFoundByIdAndType:
                        throw new InvalidOperationException(
                            Invariant(
                                $"Operation {nameof(ExistingRecordStrategy)} was {operation.ExistingRecordStrategy}; expected to not find a record by identifier '{operation.Metadata.StringSerializedId}' and identifier type '{operation.Metadata.TypeRepresentationOfId.GetTypeRepresentationByStrategy(operation.VersionMatchStrategy)}' and object type '{operation.Metadata.TypeRepresentationOfObject.GetTypeRepresentationByStrategy(operation.VersionMatchStrategy)}' yet found  '{nameof(existingRecordIds)}': '{existingRecordIds.Select(_ => _.ToString(CultureInfo.InvariantCulture)).ToCsv()}'."));
                    case ExistingRecordStrategy.ThrowIfFoundByIdAndTypeAndContent:
                        throw new InvalidOperationException(
                            Invariant(
                                $"Operation {nameof(ExistingRecordStrategy)} was {operation.ExistingRecordStrategy}; expected to not find a record by identifier '{operation.Metadata.StringSerializedId}' and identifier type '{operation.Metadata.TypeRepresentationOfId.GetTypeRepresentationByStrategy(operation.VersionMatchStrategy)}' and object type '{operation.Metadata.TypeRepresentationOfObject.GetTypeRepresentationByStrategy(operation.VersionMatchStrategy)}' and contents '{operation.Payload}' yet found  '{nameof(existingRecordIds)}': '{existingRecordIds.Select(_ => _.ToString(CultureInfo.InvariantCulture)).ToCsv()}'."));
                    case ExistingRecordStrategy.DoNotWriteIfFoundById:
                    case ExistingRecordStrategy.DoNotWriteIfFoundByIdAndType:
                    case ExistingRecordStrategy.DoNotWriteIfFoundByIdAndTypeAndContent:
                        if (newRecordId != null)
                        {
                            throw new InvalidOperationException(Invariant($"Expect {nameof(ExistingRecordStrategy)} of {operation.ExistingRecordStrategy} to not write when there are existing ids found: '{existingRecordIds.Select(_ => _.ToString(CultureInfo.InvariantCulture)).ToCsv()}'; new record id: '{newRecordId}'."));
                        }

                        return new PutRecordResult(null, existingRecordIds, prunedRecordIds);
                    case ExistingRecordStrategy.PruneIfFoundById:
                    case ExistingRecordStrategy.PruneIfFoundByIdAndType:
                        return new PutRecordResult(newRecordId, existingRecordIds, prunedRecordIds);
                    default:
                        throw new NotSupportedException(
                            Invariant($"{nameof(ExistingRecordStrategy)} {operation.ExistingRecordStrategy} is not supported."));
                }
            }
            else
            {
                return new PutRecordResult(newRecordId);
            }
        }
    }
}
