﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlStream.TryHandle.cs" company="Naos Project">
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
        public override StreamRecord Execute(
            TryHandleRecordOp operation)
        {
            var sqlServerLocator = this.TryGetLocator(operation);
            var identifierTypeQuery = operation.IdentifierType == null
                ? null
                : this.GetIdsAddIfNecessaryType(sqlServerLocator, operation.IdentifierType.ToWithAndWithoutVersion());
            var objectTypeQuery = operation.ObjectType == null
                ? null
                : this.GetIdsAddIfNecessaryType(sqlServerLocator, operation.ObjectType.ToWithAndWithoutVersion());
            var tagIdsForEntryXml = operation.Tags == null
                ? null
                : TagConversionTool.GetTagsXmlString(this.GetIdsAddIfNecessaryTag(sqlServerLocator, operation.Tags).ToOrdinalDictionary());

            var storedProcOp = StreamSchema.Sprocs.TryHandleRecord.BuildExecuteStoredProcedureOp(
                this.Name,
                operation.Concern,
                operation.Details ?? Invariant($"Created by {nameof(TryHandleRecordOp)}."),
                identifierTypeQuery,
                objectTypeQuery,
                operation.OrderRecordsStrategy,
                operation.TypeVersionMatchStrategy,
                tagIdsForEntryXml);

            var sqlProtocol = this.BuildSqlOperationsProtocol(sqlServerLocator);
            var sprocResult = sqlProtocol.Execute(storedProcOp);

            int shouldHandle = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.TryHandleRecord.OutputParamName.ShouldHandle)].GetValue<int>();
            if (shouldHandle != 1)
            {
                return null;
            }

            long internalRecordId = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.TryHandleRecord.OutputParamName.InternalRecordId)].GetValue<long>();
            int serializerRepresentationId = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.TryHandleRecord.OutputParamName.SerializerRepresentationId)].GetValue<int>();
            int identifierTypeWithVersionId = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.TryHandleRecord.OutputParamName.IdentifierTypeWithVersionId)].GetValue<int>();
            int objectTypeWithVersionId = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.TryHandleRecord.OutputParamName.ObjectTypeWithVersionId)].GetValue<int>();
            string stringSerializedId = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.TryHandleRecord.OutputParamName.StringSerializedId)].GetValue<string>();
            string stringSerializedObject = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.TryHandleRecord.OutputParamName.StringSerializedObject)].GetValue<string>();
            DateTime recordTimestampRaw = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.TryHandleRecord.OutputParamName.RecordDateTime)].GetValue<DateTime>();
            DateTime? objectTimestampRaw = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.TryHandleRecord.OutputParamName.ObjectDateTime)].GetValue<DateTime?>();
            string tagIdsXml = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.TryHandleRecord.OutputParamName.TagIdsXml)].GetValue<string>();

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
                stringSerializedId,
                identifiedSerializerRepresentation.SerializerRepresentation,
                identifierType,
                objectType,
                tags,
                recordTimestamp,
                objectTimestamp);

            var payload = new DescribedSerialization(objectType.WithVersion, stringSerializedObject, identifiedSerializerRepresentation.SerializerRepresentation, identifiedSerializerRepresentation.SerializationFormat);

            var result = new StreamRecord(internalRecordId, metadata, payload);
            return result;
        }
    }
}