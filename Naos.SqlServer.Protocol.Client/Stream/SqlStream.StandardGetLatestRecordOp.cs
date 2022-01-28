// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlStream.StandardGetLatestRecordOp.cs" company="Naos Project">
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
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.DateTime.Recipes;
    using OBeautifulCode.Serialization;
    using OBeautifulCode.String.Recipes;
    using static System.FormattableString;

    public partial class SqlStream
    {
        /// <inheritdoc />
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = NaosSuppressBecause.CA1506_AvoidExcessiveClassCoupling_DisagreeWithAssessment)]
        public override StreamRecord Execute(
            StandardGetLatestRecordOp operation)
        {
            operation.MustForArg(nameof(operation)).NotBeNull();

            var sqlServerLocator = this.TryGetLocator(operation);
            var convertedRecordFilter = this.ConvertRecordFilter(operation.RecordFilter, sqlServerLocator);

            var storedProcOp = StreamSchema.Sprocs.GetLatestRecord.BuildExecuteStoredProcedureOp(
                this.Name,
                convertedRecordFilter,
                operation.RecordNotFoundStrategy,
                operation.StreamRecordItemsToInclude);

            var sqlProtocol = this.BuildSqlOperationsProtocol(sqlServerLocator);
            var sprocResult = sqlProtocol.Execute(storedProcOp);

            long internalRecordId = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.GetLatestRecord.OutputParamName.InternalRecordId)].GetValueOfType<long>();

            if (internalRecordId == StreamSchema.Tables.Record.NullId)
            {
                switch (operation.RecordNotFoundStrategy)
                {
                    case RecordNotFoundStrategy.ReturnDefault:
                        return null;
                    case RecordNotFoundStrategy.Throw:
                        throw new InvalidOperationException(
                            Invariant(
                                $"Expected stream {this.StreamRepresentation} to contain a matching record for {operation}, none was found and {nameof(operation.RecordNotFoundStrategy)} is '{operation.RecordNotFoundStrategy}'."));
                    default:
                        throw new NotSupportedException(
                            Invariant($"{nameof(RecordNotFoundStrategy)} {operation.RecordNotFoundStrategy} is not supported."));
                }
            }

            int serializerRepresentationId = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.GetLatestRecord.OutputParamName.SerializerRepresentationId)].GetValueOfType<int>();
            int identifierTypeWithVersionId = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.GetLatestRecord.OutputParamName.IdentifierTypeWithVersionId)].GetValueOfType<int>();
            int objectTypeWithVersionId = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.GetLatestRecord.OutputParamName.ObjectTypeWithVersionId)].GetValueOfType<int>();
            string stringSerializedId = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.GetLatestRecord.OutputParamName.StringSerializedId)].GetValueOfType<string>();
            string stringSerializedObject = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.GetLatestRecord.OutputParamName.StringSerializedObject)].GetValueOfType<string>();
            byte[] binarySerializedObject = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.GetLatestRecord.OutputParamName.BinarySerializedObject)].GetValueOfType<byte[]>();
            DateTime recordTimestampRaw = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.GetLatestRecord.OutputParamName.RecordDateTime)].GetValueOfType<DateTime>();
            DateTime? objectTimestampRaw = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.GetLatestRecord.OutputParamName.ObjectDateTime)].GetValueOfType<DateTime?>();
            string tagIdsCsv = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.GetLatestRecord.OutputParamName.TagIdsCsv)].GetValueOfType<string>();

            var identifiedSerializerRepresentation = this.GetSerializerRepresentationFromId(sqlServerLocator, serializerRepresentationId);
            var identifierType = this.GetTypeById(sqlServerLocator, identifierTypeWithVersionId, true);
            var objectType = this.GetTypeById(sqlServerLocator, objectTypeWithVersionId, true);
            var tagIds = tagIdsCsv?.FromCsv().Select(_ => long.Parse(_, CultureInfo.InvariantCulture)).ToList();
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
            if (operation.StreamRecordItemsToInclude == StreamRecordItemsToInclude.MetadataAndPayload)
            {
                switch (identifiedSerializerRepresentation.SerializationFormat)
                {
                    case SerializationFormat.Binary:
                        payload = new BinaryDescribedSerialization(
                            objectType.WithVersion,
                            identifiedSerializerRepresentation.SerializerRepresentation,
                            binarySerializedObject);
                        break;
                    case SerializationFormat.String:
                        payload = new StringDescribedSerialization(
                            objectType.WithVersion,
                            identifiedSerializerRepresentation.SerializerRepresentation,
                            stringSerializedObject);
                        break;
                    default:
                        throw new NotSupportedException(
                            Invariant($"{nameof(SerializationFormat)} {identifiedSerializerRepresentation.SerializationFormat} is not supported."));
                }
            }
            else
            {
                payload = new NullDescribedSerialization(metadata.TypeRepresentationOfObject.WithVersion, metadata.SerializerRepresentation);
            }

            var result = new StreamRecord(internalRecordId, metadata, payload);
            return result;
        }
    }
}
