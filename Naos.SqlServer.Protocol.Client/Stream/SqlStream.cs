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
        private readonly IDictionary<SerializerRepresentation, DescribedSerializer> serializerDescriptionToDescribedSerializerMap = new Dictionary<SerializerRepresentation, DescribedSerializer>();
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
            throw new NotImplementedException();
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
            var storedProcOp = StreamSchema.Sprocs.GetLatestRecordById.BuildExecuteStoredProcedureOp(
                this.Name,
                operation.StringSerializedId,
                operation.IdentifierType.ToWithAndWithoutVersion(),
                operation.ObjectType.ToWithAndWithoutVersion(),
                TypeVersionMatchStrategy.Any);

            var sqlProtocol = this.BuildSqlOperationsProtocol(sqlServerLocator);
            var sprocResult = sqlProtocol.Execute(storedProcOp);

            long internalRecordId = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.GetLatestRecordById.OutputParamName.InternalRecordId)].GetValue<long>();
            SerializationKind serializationKind = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.GetLatestRecordById.OutputParamName.SerializationKind)].GetValue<SerializationKind>();
            SerializationFormat serializationFormat = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.GetLatestRecordById.OutputParamName.SerializationFormat)].GetValue<SerializationFormat>();
            string serializationConfigAssemblyQualifiedNameWithoutVersion = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.GetLatestRecordById.OutputParamName.SerializationConfigAssemblyQualifiedNameWithoutVersion)].GetValue<string>();
            CompressionKind compressionKind = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.GetLatestRecordById.OutputParamName.CompressionKind)].GetValue<CompressionKind>();
            string identifierAssemblyQualifiedNameWithVersion = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.GetLatestRecordById.OutputParamName.IdentifierAssemblyQualifiedNameWithVersion)].GetValue<string>();
            string objectAssemblyQualifiedNameWithVersion = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.GetLatestRecordById.OutputParamName.ObjectAssemblyQualifiedNameWithVersion)].GetValue<string>();
            string serializedObjectString = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.GetLatestRecordById.OutputParamName.StringSerializedObject)].GetValue<string>();
            DateTime recordTimestampRaw = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.GetLatestRecordById.OutputParamName.RecordDateTime)].GetValue<DateTime>();
            DateTime? objectTimestampRaw = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.GetLatestRecordById.OutputParamName.ObjectDateTime)].GetValue<DateTime?>();
            string tagsXml = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.GetLatestRecordById.OutputParamName.TagsXml)].GetValue<string>();

            var tags = TagConversionTool.GetTagsFromXmlString(tagsXml);
            var configType = serializationConfigAssemblyQualifiedNameWithoutVersion
               .ToTypeRepresentationFromAssemblyQualifiedName();
            var serializerRepresentation = new SerializerRepresentation(serializationKind, configType, compressionKind);

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

            var identifierType = identifierAssemblyQualifiedNameWithVersion.ToTypeRepresentationFromAssemblyQualifiedName().ToWithAndWithoutVersion();
            var objectType = objectAssemblyQualifiedNameWithVersion.ToTypeRepresentationFromAssemblyQualifiedName().ToWithAndWithoutVersion();
            var metadata = new StreamRecordMetadata(
                operation.StringSerializedId,
                serializerRepresentation,
                identifierType,
                objectType,
                tags,
                recordTimestamp,
                objectTimestamp);

            var payload = new DescribedSerialization(objectType.WithVersion, serializedObjectString, serializerRepresentation, serializationFormat);

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
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override StreamRecord Execute(
            TryHandleRecordOp operation)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override long Execute(
            PutRecordOp operation)
        {
            var sqlServerLocator = this.TryGetLocator(operation);
            var identifierAssemblyQualifiedNameWithoutVersion = operation.Metadata.TypeRepresentationOfId.WithoutVersion.BuildAssemblyQualifiedName();
            var identifierAssemblyQualifiedNameWithVersion = operation.Metadata.TypeRepresentationOfId.WithVersion.BuildAssemblyQualifiedName();
            var objectAssemblyQualifiedNameWithoutVersion = operation.Metadata.TypeRepresentationOfObject.WithoutVersion.BuildAssemblyQualifiedName();
            var objectAssemblyQualifiedNameWithVersion = operation.Metadata.TypeRepresentationOfObject.WithVersion.BuildAssemblyQualifiedName();
            var describedSerializer = this.GetDescribedSerializer(sqlServerLocator, operation.Payload.SerializerRepresentation, operation.Payload.SerializationFormat);
            var tagsXml = TagConversionTool.GetTagsXmlString(operation.Metadata.Tags);

            var storedProcOp = StreamSchema.Sprocs.PutRecord.BuildExecuteStoredProcedureOp(
                this.Name,
                identifierAssemblyQualifiedNameWithoutVersion,
                identifierAssemblyQualifiedNameWithVersion,
                objectAssemblyQualifiedNameWithoutVersion,
                objectAssemblyQualifiedNameWithVersion,
                describedSerializer.SerializerRepresentationId,
                operation.Metadata.StringSerializedId,
                operation.Payload.SerializedPayload,
                operation.Metadata.ObjectTimestampUtc,
                tagsXml);

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
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override void Execute(
            CancelBlockedRecordHandlingOp operation)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override void Execute(
            CancelHandleRecordExecutionRequestOp operation)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override void Execute(
            CancelRunningHandleRecordExecutionOp operation)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override void Execute(
            CompleteRunningHandleRecordExecutionOp operation)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override void Execute(
            FailRunningHandleRecordExecutionOp operation)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override void Execute(
            SelfCancelRunningHandleRecordExecutionOp operation)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override void Execute(
            RetryFailedHandleRecordExecutionOp operation)
        {
            throw new NotImplementedException();
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
                                                      StreamSchema.Tables.TypeWithoutVersion.BuildCreationScript(this.Name),
                                                      StreamSchema.Tables.TypeWithVersion.BuildCreationScript(this.Name),
                                                      StreamSchema.Tables.SerializerRepresentation.BuildCreationScript(this.Name),
                                                      StreamSchema.Tables.Object.BuildCreationScript(this.Name),
                                                      StreamSchema.Tables.Tag.BuildCreationScript(this.Name),
                                                      StreamSchema.Sprocs.GetIdAddIfNecessaryTypeWithoutVersion.BuildCreationScript(this.Name),
                                                      StreamSchema.Sprocs.GetIdAddIfNecessaryTypeWithVersion.BuildCreationScript(this.Name),
                                                      StreamSchema.Sprocs.GetIdAddIfNecessarySerializerRepresentation.BuildCreationScript(this.Name),
                                                      StreamSchema.Sprocs.PutRecord.BuildCreationScript(this.Name),
                                                      StreamSchema.Sprocs.GetLatestRecordById.BuildCreationScript(this.Name),
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
        public override StreamRecordMetadata Execute(
            GetLatestRecordMetadataByIdOp operation)
        {
            throw new NotImplementedException();
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

        /// <summary>
        /// Gets the described serializer.
        /// </summary>
        /// <param name="resourceLocator">The stream locator in case it needs to look up.</param>
        /// <param name="serializerRepresentation">Optional <see cref="SerializerRepresentation"/>; default is DefaultSerializerRepresentation.</param>
        /// <param name="serializationFormat">Optional <see cref="SerializationFormat"/>; default is <see cref="SerializationFormat.String"/>.</param>
        /// <returns>DescribedSerializer.</returns>
        public DescribedSerializer GetDescribedSerializer(
            SqlServerLocator resourceLocator,
            SerializerRepresentation serializerRepresentation = null,
            SerializationFormat serializationFormat = SerializationFormat.String)
        {
            var localSerializerRepresentation = serializerRepresentation ?? this.DefaultSerializerRepresentation;
            if (this.serializerDescriptionToDescribedSerializerMap.ContainsKey(localSerializerRepresentation))
            {
                return this.serializerDescriptionToDescribedSerializerMap[localSerializerRepresentation];
            }

            var serializer = this.SerializerFactory.BuildSerializer(
                localSerializerRepresentation);

            var serializerDescriptionId = this.Execute(new GetIdAddIfNecessarySerializerRepresentationOp(resourceLocator, localSerializerRepresentation, serializationFormat));
            var result = new DescribedSerializer(localSerializerRepresentation, serializationFormat, serializer, serializerDescriptionId);
            this.serializerDescriptionToDescribedSerializerMap.Add(localSerializerRepresentation, result);
            return result;
        }

        /// <inheritdoc />
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Should dispose correctly.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "Built internally and should be safe from injection.")]
        public int Execute(
            GetIdAddIfNecessarySerializerRepresentationOp operation)
        {
            var serializationConfigurationTypeWithoutVersion = operation.SerializerRepresentation.SerializationConfigType.RemoveAssemblyVersions().BuildAssemblyQualifiedName();
            var serializationConfigurationTypeWithVersion = operation.SerializerRepresentation.SerializationConfigType.BuildAssemblyQualifiedName();

            var storedProcOp = StreamSchema.Sprocs.GetIdAddIfNecessarySerializerRepresentation.BuildExecuteStoredProcedureOp(
                this.Name,
                serializationConfigurationTypeWithoutVersion,
                serializationConfigurationTypeWithVersion,
                operation.SerializerRepresentation.SerializationKind,
                operation.SerializationFormat,
                operation.SerializerRepresentation.CompressionKind,
                UnregisteredTypeEncounteredStrategy.Attempt);

            var locator = operation.ResourceLocator;
            if (!(locator is ISqlServerLocator sqlLocator))
            {
                throw new NotSupportedException(FormattableString.Invariant($"Cannot support locator of type: {locator.GetType().ToStringReadable()}"));
            }

            var sqlProtocol = this.BuildSqlOperationsProtocol(sqlLocator);
            var sprocResult = sqlProtocol.Execute(storedProcOp);
            var result = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.GetIdAddIfNecessarySerializerRepresentation.OutputParamName.Id)]
                                    .GetValue<int>();
            return result;
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
            ISqlServerLocator sqlLocator)
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
        public Task ExecuteAsync(
            CreateStreamUserOp operation)
        {
            throw new NotImplementedException();
        }

        private SqlServerLocator TryGetLocator(ISpecifyResourceLocator locatorSpecification = null)
        {
            var locator = locatorSpecification?.SpecifiedResourceLocator.ConfirmAndConvert<SqlServerLocator>()
                       ?? this.singleLocator
                       ?? throw new NotSupportedException(Invariant($"There is not a locator specified on {locatorSpecification?.ToString()} and there is not a single locator specified to fall back to."));

            return locator;
        }
    }
}
