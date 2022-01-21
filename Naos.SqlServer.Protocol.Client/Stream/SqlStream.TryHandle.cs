// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlStream.TryHandle.cs" company="Naos Project">
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
    using OBeautifulCode.DateTime.Recipes;
    using OBeautifulCode.Serialization;
    using OBeautifulCode.String.Recipes;
    using OBeautifulCode.Type;
    using static System.FormattableString;

    public partial class SqlStream
    {
        /// <inheritdoc />
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = NaosSuppressBecause.CA1506_AvoidExcessiveClassCoupling_DisagreeWithAssessment)]
        public override TryHandleRecordResult Execute(
            StandardTryHandleRecordOp operation)
        {
            operation.MustForArg(nameof(operation)).NotBeNull();

            var sqlServerLocator = this.TryGetLocator(operation);
            var convertedRecordFilter = this.ConvertRecordFilter(operation.RecordFilter, sqlServerLocator);
            var tagIdsForEntryCsv = this.GetIdsAddIfNecessaryTag(
                                                             sqlServerLocator,
                                                             operation.Tags ?? new List<NamedValue<string>>())
                                                        .Select(_ => _.ToStringInvariantPreferred())
                                                        .ToCsv();
            var storedProcOp = StreamSchema.Sprocs.TryHandleRecord.BuildExecuteStoredProcedureOp(
                                                this.Name,
                                                operation.Concern,
                                                operation.Details ?? Invariant($"Created by {nameof(StandardTryHandleRecordOp)}."),
                                                convertedRecordFilter,
                                                tagIdsForEntryCsv,
                                                operation.OrderRecordsBy,
                                                operation.MinimumInternalRecordId,
                                                operation.InheritRecordTags);

            var sqlProtocol = this.BuildSqlOperationsProtocol(sqlServerLocator);
            var sprocResult = sqlProtocol.Execute(storedProcOp);

            int shouldHandle = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.TryHandleRecord.OutputParamName.ShouldHandle)].GetValueOfType<int>();
            int isBlocked = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.TryHandleRecord.OutputParamName.IsBlocked)].GetValueOfType<int>();
            if (shouldHandle != 1)
            {
                return new TryHandleRecordResult(null, isBlocked == 1);
            }

            long internalRecordId = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.TryHandleRecord.OutputParamName.InternalRecordId)].GetValueOfType<long>();
            int serializerRepresentationId = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.TryHandleRecord.OutputParamName.SerializerRepresentationId)].GetValueOfType<int>();
            int identifierTypeWithVersionId = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.TryHandleRecord.OutputParamName.IdentifierTypeWithVersionId)].GetValueOfType<int>();
            int objectTypeWithVersionId = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.TryHandleRecord.OutputParamName.ObjectTypeWithVersionId)].GetValueOfType<int>();
            string stringSerializedId = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.TryHandleRecord.OutputParamName.StringSerializedId)].GetValueOfType<string>();
            string stringSerializedObject = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.TryHandleRecord.OutputParamName.StringSerializedObject)].GetValueOfType<string>();
            byte[] binarySerializedObject = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.TryHandleRecord.OutputParamName.BinarySerializedObject)].GetValueOfType<byte[]>();
            DateTime recordTimestampRaw = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.TryHandleRecord.OutputParamName.RecordDateTime)].GetValueOfType<DateTime>();
            DateTime? objectTimestampRaw = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.TryHandleRecord.OutputParamName.ObjectDateTime)].GetValueOfType<DateTime?>();
            string recordTagIdsCsv = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.TryHandleRecord.OutputParamName.TagIdsCsv)].GetValueOfType<string>();

            var identifiedSerializerRepresentation = this.GetSerializerRepresentationFromId(sqlServerLocator, serializerRepresentationId);
            var identifierType = this.GetTypeById(sqlServerLocator, identifierTypeWithVersionId, true);
            var objectType = this.GetTypeById(sqlServerLocator, objectTypeWithVersionId, true);
            var tagIds = recordTagIdsCsv?.FromCsv().Select(_ => long.Parse(_, CultureInfo.InvariantCulture)).ToList();
            var tags = tagIds == null ? null : this.GetTagsByIds(sqlServerLocator, tagIds);

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
