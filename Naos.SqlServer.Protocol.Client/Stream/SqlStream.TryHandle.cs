// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlStream.TryHandle.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Protocol.Client
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using Naos.CodeAnalysis.Recipes;
    using Naos.Database.Domain;
    using Naos.SqlServer.Domain;
    using OBeautifulCode.DateTime.Recipes;
    using OBeautifulCode.Serialization;
    using static System.FormattableString;

    public partial class SqlStream
    {
        /// <inheritdoc />
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = NaosSuppressBecause.CA1506_AvoidExcessiveClassCoupling_DisagreeWithAssessment)]
        public override TryHandleRecordResult Execute(
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
                tagIdsForEntryXml,
                operation.MinimumInternalRecordId,
                operation.InheritRecordTags);

            var sqlProtocol = this.BuildSqlOperationsProtocol(sqlServerLocator);
            var sprocResult = sqlProtocol.Execute(storedProcOp);

            int shouldHandle = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.TryHandleRecord.OutputParamName.ShouldHandle)].GetValue<int>();
            int isBlocked = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.TryHandleRecord.OutputParamName.IsBlocked)].GetValue<int>();
            if (shouldHandle != 1)
            {
                return new TryHandleRecordResult(null, isBlocked == 1);
            }

            long internalRecordId = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.TryHandleRecord.OutputParamName.InternalRecordId)].GetValue<long>();
            int serializerRepresentationId = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.TryHandleRecord.OutputParamName.SerializerRepresentationId)].GetValue<int>();
            int identifierTypeWithVersionId = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.TryHandleRecord.OutputParamName.IdentifierTypeWithVersionId)].GetValue<int>();
            int objectTypeWithVersionId = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.TryHandleRecord.OutputParamName.ObjectTypeWithVersionId)].GetValue<int>();
            string stringSerializedId = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.TryHandleRecord.OutputParamName.StringSerializedId)].GetValue<string>();
            string stringSerializedObject = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.TryHandleRecord.OutputParamName.StringSerializedObject)].GetValue<string>();
            byte[] binarySerializedObject = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.TryHandleRecord.OutputParamName.BinarySerializedObject)].GetValue<byte[]>();
            DateTime recordTimestampRaw = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.TryHandleRecord.OutputParamName.RecordDateTime)].GetValue<DateTime>();
            DateTime? objectTimestampRaw = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.TryHandleRecord.OutputParamName.ObjectDateTime)].GetValue<DateTime?>();
            string tagIdsXml = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.TryHandleRecord.OutputParamName.TagIdsXml)].GetValue<string>();

            var identifiedSerializerRepresentation = this.GetSerializerRepresentationFromId(sqlServerLocator, serializerRepresentationId);
            var identifierType = this.GetTypeById(sqlServerLocator, identifierTypeWithVersionId, true);
            var objectType = this.GetTypeById(sqlServerLocator, objectTypeWithVersionId, true);
            var tagIds = TagConversionTool.GetTagsFromXmlString(tagIdsXml);
            var tags = tagIds == null ? null : this.GetTagsByIds(sqlServerLocator, tagIds.Select(_ => long.Parse(_.Value, CultureInfo.InvariantCulture)).ToList());

            var recordTimestamp = recordTimestampRaw.ToUtc();

            var objectTimestamp = objectTimestampRaw == null ? (DateTime?)null : objectTimestampRaw.ToUtc();

            var metadata = new StreamRecordMetadata(
                stringSerializedId,
                identifiedSerializerRepresentation.SerializerRepresentation,
                identifierType,
                objectType,
                tags,
                recordTimestamp,
                objectTimestamp);

            DescribedSerializationBase payload;
            switch (identifiedSerializerRepresentation.SerializationFormat)
            {
                case SerializationFormat.Binary:
                    payload = new BinaryDescribedSerialization(objectType.WithVersion, identifiedSerializerRepresentation.SerializerRepresentation, binarySerializedObject);
                    break;
                case SerializationFormat.String:
                    payload = new StringDescribedSerialization(objectType.WithVersion, identifiedSerializerRepresentation.SerializerRepresentation, stringSerializedObject);
                    break;
                default:
                    throw new NotSupportedException(Invariant($"{nameof(SerializationFormat)} {identifiedSerializerRepresentation.SerializationFormat} is not supported."));
            }

            var record = new StreamRecord(internalRecordId, metadata, payload);
            var result = new TryHandleRecordResult(record);
            return result;
        }
    }
}
