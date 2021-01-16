// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlStream.PutRecord.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Protocol.Client
{
    using System;
    using Naos.Database.Domain;
    using Naos.SqlServer.Domain;
    using static System.FormattableString;

    public partial class SqlStream
    {
        /// <inheritdoc />
        public override long Execute(
            PutRecordOp operation)
        {
            var sqlServerLocator = this.TryGetLocator(operation);
            var serializerRepresentation = this.GetIdAddIfNecessarySerializerRepresentation(sqlServerLocator, operation.Payload.SerializerRepresentation, operation.Payload.SerializationFormat);
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
                operation.Payload.SerializedPayload,
                operation.Metadata.ObjectTimestampUtc,
                tagIdsXml,
                operation.ExistingRecordEncounteredStrategy,
                operation.TypeVersionMatchStrategy);

            var sqlProtocol = this.BuildSqlOperationsProtocol(sqlServerLocator);
            var sprocResult = sqlProtocol.Execute(storedProcOp);

            var existingRecordId = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.PutRecord.OutputParamName.ExistingRecordId)].GetValue<long?>();

            if (existingRecordId != null)
            {
                switch (operation.ExistingRecordEncounteredStrategy)
                {
                    case ExistingRecordEncounteredStrategy.None:
                        throw new InvalidOperationException(Invariant($"Operation {nameof(ExistingRecordEncounteredStrategy)} was {operation.ExistingRecordEncounteredStrategy}; did not expect an {StreamSchema.Sprocs.PutRecord.OutputParamName.ExistingRecordId} value but got '{existingRecordId}'."));
                    case ExistingRecordEncounteredStrategy.ThrowIfFoundById:
                        throw new InvalidOperationException(
                            Invariant(
                                $"Operation {nameof(ExistingRecordEncounteredStrategy)} was {operation.ExistingRecordEncounteredStrategy}; expected to not find a record by identifier '{operation.Metadata.StringSerializedId}' yet found  {StreamSchema.Sprocs.PutRecord.OutputParamName.ExistingRecordId} '{existingRecordId}'."));
                    case ExistingRecordEncounteredStrategy.ThrowIfFoundByIdAndType:
                        throw new InvalidOperationException(
                            Invariant(
                                $"Operation {nameof(ExistingRecordEncounteredStrategy)} was {operation.ExistingRecordEncounteredStrategy}; expected to not find a record by identifier '{operation.Metadata.StringSerializedId}' and identifier type '{operation.Metadata.TypeRepresentationOfId.GetTypeRepresentationByStrategy(operation.TypeVersionMatchStrategy)}' and object type '{operation.Metadata.TypeRepresentationOfObject.GetTypeRepresentationByStrategy(operation.TypeVersionMatchStrategy)}' yet found  {StreamSchema.Sprocs.PutRecord.OutputParamName.ExistingRecordId} '{existingRecordId}'."));
                    case ExistingRecordEncounteredStrategy.ThrowIfFoundByIdAndTypeAndContent:
                        throw new InvalidOperationException(
                            Invariant(
                                $"Operation {nameof(ExistingRecordEncounteredStrategy)} was {operation.ExistingRecordEncounteredStrategy}; expected to not find a record by identifier '{operation.Metadata.StringSerializedId}' and identifier type '{operation.Metadata.TypeRepresentationOfId.GetTypeRepresentationByStrategy(operation.TypeVersionMatchStrategy)}' and object type '{operation.Metadata.TypeRepresentationOfObject.GetTypeRepresentationByStrategy(operation.TypeVersionMatchStrategy)}' and contents '{operation.Payload.SerializedPayload}' yet found  {StreamSchema.Sprocs.PutRecord.OutputParamName.ExistingRecordId} '{existingRecordId}'."));
                    case ExistingRecordEncounteredStrategy.DoNotWriteIfFoundById:
                        return StreamSchema.Tables.Record.NullId;
                    case ExistingRecordEncounteredStrategy.DoNotWriteIfFoundByIdAndType:
                        return StreamSchema.Tables.Record.NullId;
                    case ExistingRecordEncounteredStrategy.DoNotWriteIfFoundByIdAndTypeAndContent:
                        return StreamSchema.Tables.Record.NullId;
                    default:
                        throw new NotSupportedException(
                            Invariant($"{nameof(ExistingRecordEncounteredStrategy)} {operation.ExistingRecordEncounteredStrategy} is not supported."));
                }
            }
            else
            {
                var result = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.PutRecord.OutputParamName.Id)].GetValue<long>();
                return result;
            }
        }
    }
}
