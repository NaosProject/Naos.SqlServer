// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlStream.PutRecord.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Protocol.Client
{
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
                operation.ExistingRecordEncounteredStrategy);

            var sqlProtocol = this.BuildSqlOperationsProtocol(sqlServerLocator);
            var sprocResult =
                sqlProtocol.Execute(
                    storedProcOp); // should this be returning with the ID??? Dangerous b/c it blurs the contract, opens avenues for coupling and misuse...

            var result = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.PutRecord.OutputParamName.Id)].GetValue<long>();
            return result;
        }
    }
}
