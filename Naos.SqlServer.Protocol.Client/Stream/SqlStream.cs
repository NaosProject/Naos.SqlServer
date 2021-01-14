// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlStream.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Protocol.Client
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Runtime.InteropServices.ComTypes;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Naos.CodeAnalysis.Recipes;
    using Naos.Database.Domain;
    using Naos.Protocol.Domain;
    using Naos.Recipes.RunWithRetry;
    using Naos.SqlServer.Domain;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Compression;
    using OBeautifulCode.Database.Recipes;
    using OBeautifulCode.Enum.Recipes;
    using OBeautifulCode.Representation.System;
    using OBeautifulCode.Serialization;
    using OBeautifulCode.Type;
    using OBeautifulCode.Type.Recipes;
    using static System.FormattableString;
    using SerializationFormat = OBeautifulCode.Serialization.SerializationFormat;

    /// <summary>
    /// SQL implementation of an <see cref="StandardReadWriteStreamBase"/>.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Acceptable given it creates the stream.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = NaosSuppressBecause.CA1711_IdentifiersShouldNotHaveIncorrectSuffix_TypeNameAddedAsSuffixForTestsWhereTypeIsPrimaryConcern)]
    public partial class SqlStream : StandardReadWriteStreamBase,
                                     ISyncAndAsyncReturningProtocol<GetIdAddIfNecessarySerializerRepresentationOp, int>,
                                     ISyncAndAsyncVoidProtocol<CreateStreamUserOp>
    {
        private readonly SqlServerLocator singleLocator;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlStream"/> class.
        /// </summary>
        /// <param name="name">The name of the stream.</param>
        /// <param name="defaultConnectionTimeout">The default connection timeout.</param>
        /// <param name="defaultCommandTimeout">The default command timeout.</param>
        /// <param name="defaultSerializerRepresentation">Default serializer description to use.</param>
        /// <param name="defaultSerializationFormat">Default serializer format.</param>
        /// <param name="serializerFactory">The factory to get a serializer to use for objects.</param>
        /// <param name="resourceLocatorProtocol">The protocols for getting locators.</param>
        public SqlStream(
            string name,
            TimeSpan defaultConnectionTimeout,
            TimeSpan defaultCommandTimeout,
            SerializerRepresentation defaultSerializerRepresentation,
            SerializationFormat defaultSerializationFormat,
            ISerializerFactory serializerFactory,
            IResourceLocatorProtocols resourceLocatorProtocol)
        : base(name, resourceLocatorProtocol, serializerFactory, defaultSerializerRepresentation, defaultSerializationFormat)
        {
            name.MustForArg(nameof(name)).NotBeNullNorWhiteSpace();
            defaultSerializerRepresentation.MustForArg(nameof(defaultSerializerRepresentation)).NotBeNull();
            serializerFactory.MustForArg(nameof(serializerFactory)).NotBeNull();
            resourceLocatorProtocol.MustForArg(nameof(resourceLocatorProtocol)).NotBeNull();

            this.StreamRepresentation = new StreamRepresentation(this.Name);
            this.DefaultConnectionTimeout = defaultConnectionTimeout;
            this.DefaultCommandTimeout = defaultCommandTimeout;

            var allLocators = this.ResourceLocatorProtocols.Execute(new GetAllResourceLocatorsOp());
            this.singleLocator = allLocators.Count == 1 ? allLocators.Single().ConfirmAndConvert<SqlServerLocator>() : null;
        }

        /// <summary>
        /// Gets the default connection timeout.
        /// </summary>
        /// <value>The default connection timeout.</value>
        public TimeSpan DefaultConnectionTimeout { get; private set; }

        /// <summary>
        /// Gets the default command timeout.
        /// </summary>
        /// <value>The default command timeout.</value>
        public TimeSpan DefaultCommandTimeout { get; private set; }

        /// <inheritdoc />
        public override long Execute(
            GetNextUniqueLongOp operation)
        {
            var locator = this.ResourceLocatorProtocols.Execute(new GetResourceLocatorForUniqueIdentifierOp());
            var sqlServerLocator = locator as SqlServerLocator
                                ?? throw new NotSupportedException(Invariant($"{nameof(GetResourceLocatorForUniqueIdentifierOp)} should return a {nameof(SqlServerLocator)} and returned {locator?.GetType().ToStringReadable()}."));

            var storedProcOp = StreamSchema.Sprocs.GetNextUniqueLong.BuildExecuteStoredProcedureOp(this.Name);

            var sqlProtocol = this.BuildSqlOperationsProtocol(sqlServerLocator);
            var sprocResult = sqlProtocol.Execute(storedProcOp);

            long result = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.GetNextUniqueLong.OutputParamName.Value)].GetValue<long>();

            return result;
        }

        /// <inheritdoc />
        public override StreamRecord Execute(
            GetLatestRecordOp operation)
        {
            throw new NotImplementedException();
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

            if (internalRecordId == -1)
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

        /// <inheritdoc />
        public override IReadOnlyList<StreamRecordHandlingEntry> Execute(
            GetHandlingHistoryOfRecordOp operation)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override HandlingStatus Execute(
            GetHandlingStatusOfRecordsByIdOp operation)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override HandlingStatus Execute(
            GetHandlingStatusOfRecordSetByTagOp operation)
        {
            if (operation.HandlingStatusCompositionStrategy != null)
            {
                operation
                   .HandlingStatusCompositionStrategy
                   .IgnoreCancel
                   .MustForArg(Invariant($"{nameof(GetHandlingStatusOfRecordSetByTagOp)}.{nameof(HandlingStatusCompositionStrategy)}.{nameof(HandlingStatusCompositionStrategy.IgnoreCancel)}"))
                   .BeFalse(Invariant($"{nameof(HandlingStatusCompositionStrategy)}.{nameof(HandlingStatusCompositionStrategy.IgnoreCancel)} is not supported."));
            }

            if (operation.TagMatchStrategy != null)
            {
                operation.TagMatchStrategy.ScopeOfFindSet
                         .MustForArg(Invariant($"{nameof(GetHandlingStatusOfRecordSetByTagOp)}.{nameof(TagMatchStrategy)}.{nameof(TagMatchStrategy.ScopeOfFindSet)}"))
                         .BeEqualTo(Database.Domain.TagMatchScope.All);

                operation.TagMatchStrategy.ScopeOfTarget
                         .MustForArg(Invariant($"{nameof(GetHandlingStatusOfRecordSetByTagOp)}.{nameof(TagMatchStrategy)}.{nameof(TagMatchStrategy.ScopeOfTarget)}"))
                         .BeEqualTo(Database.Domain.TagMatchScope.Any);
            }

            var allLocators = this.ResourceLocatorProtocols.Execute(new GetAllResourceLocatorsOp());
            var statusPerLocator = new List<HandlingStatus>();
            foreach (var resourceLocator in allLocators)
            {
                var sqlServerLocator = resourceLocator.ConfirmAndConvert<SqlServerLocator>();
                var sqlProtocol = this.BuildSqlOperationsProtocol(sqlServerLocator);
                var tagIds = this.GetIdsAddIfNecessaryTag(sqlServerLocator, operation.TagsToMatch);
                var tagIdsXml = TagConversionTool.GetTagsXmlString(tagIds.ToOrdinalDictionary());
                var op = StreamSchema.Sprocs.GetCompositeHandlingStatus.BuildExecuteStoredProcedureOp(
                    this.Name,
                    operation.Concern,
                    tagIdsXml: tagIdsXml);

                var sprocResult = sqlProtocol.Execute(op);

                HandlingStatus status = sprocResult
                                                 .OutputParameters[StreamSchema.Sprocs.GetCompositeHandlingStatus.OutputParamName.Status.ToString()]
                                                 .GetValue<HandlingStatus>();
                statusPerLocator.Add(status);
            }

            var result = statusPerLocator.ReduceToCompositeHandlingStatus(operation.HandlingStatusCompositionStrategy);
            return result;
        }

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

        /// <inheritdoc />
        public override void Execute(
            BlockRecordHandlingOp operation)
        {
            var locator = this.ResourceLocatorProtocols.Execute(new GetResourceLocatorForUniqueIdentifierOp());
            var sqlServerLocator = locator as SqlServerLocator
                                ?? throw new NotSupportedException(Invariant($"{nameof(GetResourceLocatorForUniqueIdentifierOp)} should return a {nameof(SqlServerLocator)} and returned {locator?.GetType().ToStringReadable()}."));
            var tagIdsXml = TagConversionTool.GetTagsXmlString(
                operation.Tags == null
                    ? new Dictionary<string, string>()
                    : this.GetIdsAddIfNecessaryTag(sqlServerLocator, operation.Tags).ToOrdinalDictionary());
            var storedProcOp = StreamSchema.Sprocs.PutHandling.BuildExecuteStoredProcedureOp(
                this.Name,
                Concerns.RecordHandlingConcern,
                operation.Details,
                Concerns.GlobalBlockingRecordId,
                HandlingStatus.Blocked,
                typeof(HandlingStatus).GetAllPossibleEnumValues()
                                      .Cast<HandlingStatus>()
                                      .Except(
                                           new[]
                                           {
                                               HandlingStatus.Blocked,
                                           })
                                      .ToList(),
                tagIdsXml);

            var sqlProtocol = this.BuildSqlOperationsProtocol(sqlServerLocator);
            var sprocResult = sqlProtocol.Execute(storedProcOp);
            sprocResult.MustForOp(nameof(sprocResult)).NotBeNull();
        }

        /// <inheritdoc />
        public override void Execute(
            CancelBlockedRecordHandlingOp operation)
        {
            var locator = this.ResourceLocatorProtocols.Execute(new GetResourceLocatorForUniqueIdentifierOp());
            var sqlServerLocator = locator as SqlServerLocator
                                ?? throw new NotSupportedException(Invariant($"{nameof(GetResourceLocatorForUniqueIdentifierOp)} should return a {nameof(SqlServerLocator)} and returned {locator?.GetType().ToStringReadable()}."));
            var tagIdsXml = TagConversionTool.GetTagsXmlString(
                operation.Tags == null
                    ? new Dictionary<string, string>()
                    : this.GetIdsAddIfNecessaryTag(sqlServerLocator, operation.Tags).ToOrdinalDictionary());
            var storedProcOp = StreamSchema.Sprocs.PutHandling.BuildExecuteStoredProcedureOp(
                this.Name,
                Concerns.RecordHandlingConcern,
                operation.Details,
                Concerns.GlobalBlockingRecordId,
                HandlingStatus.Requested,
                new[] { HandlingStatus.Blocked },
                tagIdsXml);

            var sqlProtocol = this.BuildSqlOperationsProtocol(sqlServerLocator);
            var sprocResult = sqlProtocol.Execute(storedProcOp);
            sprocResult.MustForOp(nameof(sprocResult)).NotBeNull();
        }

        /// <inheritdoc />
        public override void Execute(
            CancelHandleRecordExecutionRequestOp operation)
        {
            var sqlServerLocator = this.TryGetLocator(operation);
            var tagIdsXml = TagConversionTool.GetTagsXmlString(
                operation.Tags == null
                    ? new Dictionary<string, string>()
                    : this.GetIdsAddIfNecessaryTag(sqlServerLocator, operation.Tags).ToOrdinalDictionary());

            var storedProcOp = StreamSchema.Sprocs.PutHandling.BuildExecuteStoredProcedureOp(
                this.Name,
                operation.Concern,
                operation.Details,
                operation.Id,
                HandlingStatus.Canceled,
                new[] { HandlingStatus.Requested, HandlingStatus.Running, HandlingStatus.Failed },
                tagIdsXml);

            var sqlProtocol = this.BuildSqlOperationsProtocol(sqlServerLocator);
            var sprocResult = sqlProtocol.Execute(storedProcOp);
            sprocResult.MustForOp(nameof(sprocResult)).NotBeNull();
        }

        /// <inheritdoc />
        public override void Execute(
            CancelRunningHandleRecordExecutionOp operation)
        {
            var sqlServerLocator = this.TryGetLocator(operation);
            var tagIdsXml = TagConversionTool.GetTagsXmlString(
                operation.Tags == null
                    ? new Dictionary<string, string>()
                    : this.GetIdsAddIfNecessaryTag(sqlServerLocator, operation.Tags).ToOrdinalDictionary());

            var storedProcOp = StreamSchema.Sprocs.PutHandling.BuildExecuteStoredProcedureOp(
                this.Name,
                operation.Concern,
                operation.Details,
                operation.Id,
                HandlingStatus.CanceledRunning,
                new[] { HandlingStatus.Running },
                tagIdsXml);

            var sqlProtocol = this.BuildSqlOperationsProtocol(sqlServerLocator);
            var sprocResult = sqlProtocol.Execute(storedProcOp);
            sprocResult.MustForOp(nameof(sprocResult)).NotBeNull();
        }

        /// <inheritdoc />
        public override void Execute(
            CompleteRunningHandleRecordExecutionOp operation)
        {
            var sqlServerLocator = this.TryGetLocator(operation);
            var tagIdsXml = TagConversionTool.GetTagsXmlString(
                operation.Tags == null
                    ? new Dictionary<string, string>()
                    : this.GetIdsAddIfNecessaryTag(sqlServerLocator, operation.Tags).ToOrdinalDictionary());

            var storedProcOp = StreamSchema.Sprocs.PutHandling.BuildExecuteStoredProcedureOp(
                this.Name,
                operation.Concern,
                operation.Details,
                operation.Id,
                HandlingStatus.Completed,
                new[] { HandlingStatus.Running },
                tagIdsXml);

            var sqlProtocol = this.BuildSqlOperationsProtocol(sqlServerLocator);
            var sprocResult = sqlProtocol.Execute(storedProcOp);
            sprocResult.MustForOp(nameof(sprocResult)).NotBeNull();
        }

        /// <inheritdoc />
        public override void Execute(
            FailRunningHandleRecordExecutionOp operation)
        {
            var sqlServerLocator = this.TryGetLocator(operation);
            var tagIdsXml = TagConversionTool.GetTagsXmlString(
                operation.Tags == null
                    ? new Dictionary<string, string>()
                    : this.GetIdsAddIfNecessaryTag(sqlServerLocator, operation.Tags).ToOrdinalDictionary());

            var storedProcOp = StreamSchema.Sprocs.PutHandling.BuildExecuteStoredProcedureOp(
                this.Name,
                operation.Concern,
                operation.Details,
                operation.Id,
                HandlingStatus.Failed,
                new[] { HandlingStatus.Running },
                tagIdsXml);

            var sqlProtocol = this.BuildSqlOperationsProtocol(sqlServerLocator);
            var sprocResult = sqlProtocol.Execute(storedProcOp);
            sprocResult.MustForOp(nameof(sprocResult)).NotBeNull();
        }

        /// <inheritdoc />
        public override void Execute(
            SelfCancelRunningHandleRecordExecutionOp operation)
        {
            var sqlServerLocator = this.TryGetLocator(operation);
            var tagIdsXml = TagConversionTool.GetTagsXmlString(
                operation.Tags == null
                    ? new Dictionary<string, string>()
                    : this.GetIdsAddIfNecessaryTag(sqlServerLocator, operation.Tags).ToOrdinalDictionary());

            var storedProcOp = StreamSchema.Sprocs.PutHandling.BuildExecuteStoredProcedureOp(
                this.Name,
                operation.Concern,
                operation.Details,
                operation.Id,
                HandlingStatus.SelfCanceledRunning,
                new[] { HandlingStatus.Running },
                tagIdsXml);

            var sqlProtocol = this.BuildSqlOperationsProtocol(sqlServerLocator);
            var sprocResult = sqlProtocol.Execute(storedProcOp);
            sprocResult.MustForOp(nameof(sprocResult)).NotBeNull();
        }

        /// <inheritdoc />
        public override void Execute(
            RetryFailedHandleRecordExecutionOp operation)
        {
            var sqlServerLocator = this.TryGetLocator(operation);
            var tagIdsXml = TagConversionTool.GetTagsXmlString(
                operation.Tags == null
                    ? new Dictionary<string, string>()
                    : this.GetIdsAddIfNecessaryTag(sqlServerLocator, operation.Tags).ToOrdinalDictionary());

            var storedProcOp = StreamSchema.Sprocs.PutHandling.BuildExecuteStoredProcedureOp(
                this.Name,
                operation.Concern,
                operation.Details,
                operation.Id,
                HandlingStatus.RetryFailed,
                new[] { HandlingStatus.Failed },
                tagIdsXml);

            var sqlProtocol = this.BuildSqlOperationsProtocol(sqlServerLocator);
            var sprocResult = sqlProtocol.Execute(storedProcOp);
            sprocResult.MustForOp(nameof(sprocResult)).NotBeNull();
        }

        /// <inheritdoc />
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Acceptable given it creates the streams.")]
        public override void Execute(
            CreateStreamOp operation)
        {
            var allLocators = this.ResourceLocatorProtocols.Execute(new GetAllResourceLocatorsOp());
            foreach (var locator in allLocators)
            {
                if (locator is SqlServerLocator sqlLocator)
                {
                    using (var connection = sqlLocator.OpenSqlConnection(this.DefaultConnectionTimeout))
                    {
                        // TODO: should we use a transaction here?
                        var streamAlreadyExists = connection.HasAtLeastOneRowWhenReading(
                            FormattableString.Invariant($"select * from sys.schemas where name = '{this.Name}'"));

                        if (streamAlreadyExists)
                        {
                            switch (operation.ExistingStreamEncounteredStrategy)
                            {
                                case ExistingStreamEncounteredStrategy.Overwrite:
                                    throw new NotSupportedException(FormattableString.Invariant(
                                        $"Overwriting streams is not currently supported; stream '{this.Name}' already exists, {nameof(operation)}.{nameof(operation.ExistingStreamEncounteredStrategy)} was set to {ExistingStreamEncounteredStrategy.Overwrite}."));
                                case ExistingStreamEncounteredStrategy.Throw:
                                    throw new InvalidDataException(FormattableString.Invariant($"Stream '{this.Name}' already exists, {nameof(operation)}.{nameof(operation.ExistingStreamEncounteredStrategy)} was set to {ExistingStreamEncounteredStrategy.Throw}."));
                                case ExistingStreamEncounteredStrategy.Skip:
                                    break;
                            }
                        }
                        else
                        {
                            var creationScripts = new[]
                                                  {
                                                      StreamSchema.BuildCreationScriptForSchema(this.Name),
                                                      StreamSchema.Tables.NextUniqueLong.BuildCreationScript(this.Name),
                                                      StreamSchema.Tables.TypeWithoutVersion.BuildCreationScript(this.Name),
                                                      StreamSchema.Tables.TypeWithVersion.BuildCreationScript(this.Name),
                                                      StreamSchema.Tables.SerializerRepresentation.BuildCreationScript(this.Name),
                                                      StreamSchema.Tables.Tag.BuildCreationScript(this.Name),
                                                      StreamSchema.Tables.Record.BuildCreationScript(this.Name),
                                                      StreamSchema.Tables.RecordTag.BuildCreationScript(this.Name),
                                                      StreamSchema.Tables.Handling.BuildCreationScript(this.Name),
                                                      StreamSchema.Tables.HandlingTag.BuildCreationScript(this.Name),
                                                      StreamSchema.Funcs.GetStatusSortOrderTableVariable.BuildCreationScript(this.Name),
                                                      StreamSchema.Funcs.GetTagsTableVariableFromTagsXml.BuildCreationScript(this.Name),
                                                      StreamSchema.Funcs.GetTagsTableVariableFromTagIdsXml.BuildCreationScript(this.Name),
                                                      StreamSchema.Sprocs.GetIdAddIfNecessaryTypeWithoutVersion.BuildCreationScript(this.Name),
                                                      StreamSchema.Sprocs.GetIdAddIfNecessaryTypeWithVersion.BuildCreationScript(this.Name),
                                                      StreamSchema.Sprocs.GetTypeFromId.BuildCreationScript(this.Name),
                                                      StreamSchema.Sprocs.GetIdAddIfNecessarySerializerRepresentation.BuildCreationScript(this.Name),
                                                      StreamSchema.Sprocs.GetSerializerRepresentationFromId.BuildCreationScript(this.Name),
                                                      StreamSchema.Sprocs.GetIdsAddIfNecessaryTagSet.BuildCreationScript(this.Name),
                                                      StreamSchema.Sprocs.GetTagSetFromIds.BuildCreationScript(this.Name),
                                                      StreamSchema.Sprocs.GetLatestRecordMetadataById.BuildCreationScript(this.Name),
                                                      StreamSchema.Sprocs.GetLatestRecordById.BuildCreationScript(this.Name),
                                                      StreamSchema.Sprocs.GetNextUniqueLong.BuildCreationScript(this.Name),
                                                      StreamSchema.Sprocs.PutRecord.BuildCreationScript(this.Name),
                                                      StreamSchema.Sprocs.PutHandling.BuildCreationScript(this.Name),
                                                      StreamSchema.Sprocs.GetCompositeHandlingStatus.BuildCreationScript(this.Name),
                                                      StreamSchema.Sprocs.TryHandleRecord.BuildCreationScript(this.Name),
                                                  };

                            foreach (var script in creationScripts)
                            {
                                connection.ExecuteNonQuery(script);
                            }
                        }
                    }
                }
                else
                {
                    throw SqlServerLocator.BuildInvalidLocatorException(locator.GetType());
                }
            }
        }

        /// <inheritdoc />
        public override void Execute(
            DeleteStreamOp operation)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override void Execute(
            PruneBeforeInternalRecordDateOp operation)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override void Execute(
            PruneBeforeInternalRecordIdOp operation)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override bool Execute(
            DoesAnyExistByIdOp operation)
        {
            throw new NotImplementedException();
        }

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
            if (internalRecordId == -1)
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
        public override IReadOnlyList<StreamRecord> Execute(
            GetAllRecordsByIdOp operation)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override IReadOnlyList<StreamRecordMetadata> Execute(
            GetAllRecordsMetadataByIdOp operation)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public async Task<int> ExecuteAsync(
            GetIdAddIfNecessarySerializerRepresentationOp operation)
        {
            var syncResult = this.Execute(operation);
            return await Task.FromResult(syncResult);
        }

        /// <summary>
        /// Builds the SQL operations protocol.
        /// </summary>
        /// <param name="sqlLocator">The SQL locator.</param>
        /// <returns>IProtocolSqlOperations.</returns>
        public IProtocolSqlOperations BuildSqlOperationsProtocol(
            SqlServerLocator sqlLocator)
        {
            var result = new SqlOperationsProtocol(sqlLocator, this.DefaultConnectionTimeout, this.DefaultCommandTimeout);
            return result;
        }

        /// <inheritdoc />
        public override IStreamRepresentation StreamRepresentation { get; }

        /// <inheritdoc />
        public void Execute(
            CreateStreamUserOp operation)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public async Task ExecuteAsync(
            CreateStreamUserOp operation)
        {
            this.Execute(operation);
            await Task.FromResult(true); // just for await
        }

        private SqlServerLocator TryGetLocator(ISpecifyResourceLocator locatorSpecification = null)
        {
            var locator = locatorSpecification?.SpecifiedResourceLocator?.ConfirmAndConvert<SqlServerLocator>()
                       ?? this.singleLocator
                       ?? throw new NotSupportedException(Invariant($"There is not a locator specified on {locatorSpecification?.ToString()} and there is not a single locator specified to fall back to."));

            return locator;
        }
    }
}
