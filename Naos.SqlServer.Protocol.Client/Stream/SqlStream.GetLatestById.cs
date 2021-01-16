// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlStream.GetLatestById.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Protocol.Client
{
    using System;
    using System.Linq;
    using Naos.Database.Domain;
    using Naos.SqlServer.Domain;
    using OBeautifulCode.Serialization;
    using static System.FormattableString;

    public partial class SqlStream
    {
        /// <inheritdoc />
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "internalRecordId", Justification = "Leaving for debugging and future use.")]
        public override StreamRecordMetadata Execute(
            GetLatestRecordMetadataByIdOp operation)
        {
            var sqlServerLocator = this.TryGetLocator(operation);
            var identifierTypeQuery = this.GetIdsAddIfNecessaryType(sqlServerLocator, operation.IdentifierType?.ToWithAndWithoutVersion());
            var objectTypeQuery = this.GetIdsAddIfNecessaryType(sqlServerLocator, operation.ObjectType?.ToWithAndWithoutVersion());

            var storedProcOp = StreamSchema.Sprocs.GetLatestRecordMetadataById.BuildExecuteStoredProcedureOp(
                this.Name,
                operation.StringSerializedId,
                identifierTypeQuery,
                objectTypeQuery,
                operation.TypeVersionMatchStrategy,
                operation.ExistingRecordNotEncounteredStrategy);

            var sqlProtocol = this.BuildSqlOperationsProtocol(sqlServerLocator);
            var sprocResult = sqlProtocol.Execute(storedProcOp);

            long internalRecordId = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.GetLatestRecordMetadataById.OutputParamName.InternalRecordId)].GetValue<long>();
            if (internalRecordId == StreamSchema.Tables.Record.NullId)
            {
                switch (operation.ExistingRecordNotEncounteredStrategy)
                {
                    case ExistingRecordNotEncounteredStrategy.ReturnDefault:
                        return null;
                    case ExistingRecordNotEncounteredStrategy.Throw:
                        throw new InvalidOperationException(
                            Invariant(
                                $"Expected stream {this.StreamRepresentation} to contain a matching record for {operation}, none was found and {nameof(operation.ExistingRecordNotEncounteredStrategy)} is '{operation.ExistingRecordNotEncounteredStrategy}'."));
                    default:
                        throw new NotSupportedException(
                            Invariant($"{nameof(ExistingRecordNotEncounteredStrategy)} {operation.ExistingRecordNotEncounteredStrategy} is not supported."));
                }
            }

            int serializerRepresentationId = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.GetLatestRecordMetadataById.OutputParamName.SerializerRepresentationId)].GetValue<int>();
            int identifierTypeWithVersionId = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.GetLatestRecordMetadataById.OutputParamName.IdentifierTypeWithVersionId)].GetValue<int>();
            int objectTypeWithVersionId = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.GetLatestRecordMetadataById.OutputParamName.ObjectTypeWithVersionId)].GetValue<int>();
            DateTime recordTimestampRaw = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.GetLatestRecordMetadataById.OutputParamName.RecordDateTime)].GetValue<DateTime>();
            DateTime? objectTimestampRaw = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.GetLatestRecordMetadataById.OutputParamName.ObjectDateTime)].GetValue<DateTime?>();
            string tagIdsXml = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.GetLatestRecordMetadataById.OutputParamName.TagIdsXml)].GetValue<string>();

            var identifiedSerializerRepresentation = this.GetSerializerRepresentationFromId(sqlServerLocator, serializerRepresentationId);
            var identifierType = this.GetTypeById(sqlServerLocator, identifierTypeWithVersionId, true);
            var objectType = this.GetTypeById(sqlServerLocator, objectTypeWithVersionId, true);
            var tagIds = TagConversionTool.GetTagsFromXmlString(tagIdsXml);
            var tags = this.GetTagsByIds(sqlServerLocator, tagIds.Values.ToList());

            var recordTimestamp = new DateTime(
                recordTimestampRaw.Year,
                recordTimestampRaw.Month,
                recordTimestampRaw.Day,
                recordTimestampRaw.Hour,
                recordTimestampRaw.Minute,
                recordTimestampRaw.Second,
                recordTimestampRaw.Millisecond,
                DateTimeKind.Utc);

            var objectTimestamp = objectTimestampRaw == null ? (DateTime?)null : new DateTime(
                objectTimestampRaw.Value.Year,
                objectTimestampRaw.Value.Month,
                objectTimestampRaw.Value.Day,
                objectTimestampRaw.Value.Hour,
                objectTimestampRaw.Value.Minute,
                objectTimestampRaw.Value.Second,
                objectTimestampRaw.Value.Millisecond,
                DateTimeKind.Utc);

            var metadata = new StreamRecordMetadata(
                operation.StringSerializedId,
                identifiedSerializerRepresentation.SerializerRepresentation,
                identifierType,
                objectType,
                tags,
                recordTimestamp,
                objectTimestamp);

            return metadata;
        }

        /// <inheritdoc />
        public override StreamRecord Execute(
            GetLatestRecordByIdOp operation)
        {
            var sqlServerLocator = this.TryGetLocator(operation);
            var identifierTypeQuery = this.GetIdsAddIfNecessaryType(sqlServerLocator, operation.IdentifierType?.ToWithAndWithoutVersion());
            var objectTypeQuery = this.GetIdsAddIfNecessaryType(sqlServerLocator, operation.ObjectType?.ToWithAndWithoutVersion());

            var storedProcOp = StreamSchema.Sprocs.GetLatestRecordById.BuildExecuteStoredProcedureOp(
                this.Name,
                operation.StringSerializedId,
                identifierTypeQuery,
                objectTypeQuery,
                operation.TypeVersionMatchStrategy,
                operation.ExistingRecordNotEncounteredStrategy);

            var sqlProtocol = this.BuildSqlOperationsProtocol(sqlServerLocator);
            var sprocResult = sqlProtocol.Execute(storedProcOp);

            long internalRecordId = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.GetLatestRecordById.OutputParamName.InternalRecordId)].GetValue<long>();

            if (internalRecordId == StreamSchema.Tables.Record.NullId)
            {
                switch (operation.ExistingRecordNotEncounteredStrategy)
                {
                    case ExistingRecordNotEncounteredStrategy.ReturnDefault:
                        return null;
                    case ExistingRecordNotEncounteredStrategy.Throw:
                        throw new InvalidOperationException(
                            Invariant(
                                $"Expected stream {this.StreamRepresentation} to contain a matching record for {operation}, none was found and {nameof(operation.ExistingRecordNotEncounteredStrategy)} is '{operation.ExistingRecordNotEncounteredStrategy}'."));
                    default:
                        throw new NotSupportedException(
                            Invariant($"{nameof(ExistingRecordNotEncounteredStrategy)} {operation.ExistingRecordNotEncounteredStrategy} is not supported."));
                }
            }

            int serializerRepresentationId = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.GetLatestRecordById.OutputParamName.SerializerRepresentationId)].GetValue<int>();
            int identifierTypeWithVersionId = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.GetLatestRecordById.OutputParamName.IdentifierTypeWithVersionId)].GetValue<int>();
            int objectTypeWithVersionId = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.GetLatestRecordById.OutputParamName.ObjectTypeWithVersionId)].GetValue<int>();
            string serializedObjectString = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.GetLatestRecordById.OutputParamName.StringSerializedObject)].GetValue<string>();
            DateTime recordTimestampRaw = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.GetLatestRecordById.OutputParamName.RecordDateTime)].GetValue<DateTime>();
            DateTime? objectTimestampRaw = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.GetLatestRecordById.OutputParamName.ObjectDateTime)].GetValue<DateTime?>();
            string tagIdsXml = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.GetLatestRecordById.OutputParamName.TagIdsXml)].GetValue<string>();

            var identifiedSerializerRepresentation = this.GetSerializerRepresentationFromId(sqlServerLocator, serializerRepresentationId);
            var identifierType = this.GetTypeById(sqlServerLocator, identifierTypeWithVersionId, true);
            var objectType = this.GetTypeById(sqlServerLocator, objectTypeWithVersionId, true);
            var tagIds = TagConversionTool.GetTagsFromXmlString(tagIdsXml);
            var tags = this.GetTagsByIds(sqlServerLocator, tagIds.Values.ToList());

            var recordTimestamp = new DateTime(
                recordTimestampRaw.Year,
                recordTimestampRaw.Month,
                recordTimestampRaw.Day,
                recordTimestampRaw.Hour,
                recordTimestampRaw.Minute,
                recordTimestampRaw.Second,
                recordTimestampRaw.Millisecond,
                DateTimeKind.Utc);

            var objectTimestamp = objectTimestampRaw == null ? (DateTime?)null : new DateTime(
                objectTimestampRaw.Value.Year,
                objectTimestampRaw.Value.Month,
                objectTimestampRaw.Value.Day,
                objectTimestampRaw.Value.Hour,
                objectTimestampRaw.Value.Minute,
                objectTimestampRaw.Value.Second,
                objectTimestampRaw.Value.Millisecond,
                DateTimeKind.Utc);

            var metadata = new StreamRecordMetadata(
                operation.StringSerializedId,
                identifiedSerializerRepresentation.SerializerRepresentation,
                identifierType,
                objectType,
                tags,
                recordTimestamp,
                objectTimestamp);

            var payload = new DescribedSerialization(objectType.WithVersion, serializedObjectString, identifiedSerializerRepresentation.SerializerRepresentation, identifiedSerializerRepresentation.SerializationFormat);

            var result = new StreamRecord(internalRecordId, metadata, payload);
            return result;
        }
    }
}
