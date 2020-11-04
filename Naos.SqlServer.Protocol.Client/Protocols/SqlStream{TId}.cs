// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlStream{TId}.cs" company="Naos Project">
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
    using System.Runtime.InteropServices.ComTypes;
    using System.Threading.Tasks;
    using Naos.CodeAnalysis.Recipes;
    using Naos.Database.Domain;
    using Naos.Protocol.Domain;
    using Naos.Recipes.RunWithRetry;
    using Naos.SqlServer.Domain;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Database.Recipes;
    using OBeautifulCode.Representation.System;
    using OBeautifulCode.Serialization;
    using OBeautifulCode.Type;
    using OBeautifulCode.Type.Recipes;
    using SerializationFormat = OBeautifulCode.Serialization.SerializationFormat;

    /// <summary>
    /// SQL implementation of an <see cref="IStream" />.
    /// </summary>
    /// <typeparam name="TId">Type of the key.</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Acceptable given it creates the stream.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = NaosSuppressBecause.CA1711_IdentifiersShouldNotHaveIncorrectSuffix_TypeNameAddedAsSuffixForTestsWhereTypeIsPrimaryConcern)]
    public partial class SqlStream<TId> : StreamBase<TId>, ISyncAndAsyncReturningProtocol<GetIdAddIfNecessarySerializerRepresentationOp, int>
    {
        private readonly IDictionary<SerializerRepresentation, DescribedSerializer> serializerDescriptionToDescribedSerializerMap = new Dictionary<SerializerRepresentation, DescribedSerializer>();
        private readonly IStreamRepresentation<TId> streamRepresentation;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlStream{TId}"/> class.
        /// </summary>
        /// <param name="name">The name of the stream.</param>
        /// <param name="defaultConnectionTimeout">The default connection timeout.</param>
        /// <param name="defaultCommandTimeout">The default command timeout.</param>
        /// <param name="defaultSerializerRepresentation">Default serializer description to use.</param>
        /// <param name="defaultSerializationFormat">Default serializer format.</param>
        /// <param name="serializerFactory">The factory to get a serializer to use for objects.</param>
        /// <param name="resourceLocatorProtocol">The protocols for getting locators.</param>
        /// <param name="protocolGetIdByTypeProtocol">Optional, id extractor protocols by type; DEFAULT will look for implementation of <see cref="IIdentifiable"/> on the object written.</param>
        /// <param name="protocolGetTagsByTypeProtocol">Optional tag extractor protocols by type; DEFAULT will look for implementation of <see cref="IHaveTags"/> on the object written.</param>
        public SqlStream(
            string name,
            TimeSpan defaultConnectionTimeout,
            TimeSpan defaultCommandTimeout,
            SerializerRepresentation defaultSerializerRepresentation,
            SerializationFormat defaultSerializationFormat,
            ISerializerFactory serializerFactory,
            IProtocolResourceLocator<TId> resourceLocatorProtocol,
            ISyncAndAsyncReturningProtocol<GetProtocolByTypeOp, IProtocol> protocolGetIdByTypeProtocol = null,
            ISyncAndAsyncReturningProtocol<GetProtocolByTypeOp, IProtocol> protocolGetTagsByTypeProtocol = null)
        : base(name, resourceLocatorProtocol, protocolGetIdByTypeProtocol, protocolGetTagsByTypeProtocol)
        {
            name.MustForArg(nameof(name)).NotBeNullNorWhiteSpace();
            defaultSerializerRepresentation.MustForArg(nameof(defaultSerializerRepresentation)).NotBeNull();
            serializerFactory.MustForArg(nameof(serializerFactory)).NotBeNull();
            resourceLocatorProtocol.MustForArg(nameof(resourceLocatorProtocol)).NotBeNull();

            this.streamRepresentation = new StreamRepresentation<TId>(this.Name);
            this.DefaultConnectionTimeout = defaultConnectionTimeout;
            this.DefaultCommandTimeout = defaultCommandTimeout;
            this.DefaultSerializerRepresentation = defaultSerializerRepresentation;
            this.SerializerFactory = serializerFactory;
            this.DefaultSerializationFormat = defaultSerializationFormat;
        }

        /// <inheritdoc />
        public override IStreamRepresentation<TId> StreamRepresentation => this.streamRepresentation;

        /// <summary>
        /// Gets the default serializer description.
        /// </summary>
        /// <value>The default serializer description.</value>
        public SerializerRepresentation DefaultSerializerRepresentation { get; private set; }

        /// <summary>
        /// Gets the default serialization format.
        /// </summary>
        /// <value>The default serialization format.</value>
        public SerializationFormat DefaultSerializationFormat { get; private set; }

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

        /// <summary>
        /// Gets the type of the key.
        /// </summary>
        /// <value>The type of the key.</value>
        public Type IdType => typeof(TId);

        /// <summary>
        /// Gets the serializer factory.
        /// </summary>
        /// <value>The serializer factory.</value>
        public ISerializerFactory SerializerFactory { get; private set; }

        /// <inheritdoc />
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Acceptable given it creates the streams.")]
        public override void Execute(
            CreateStreamOp<TId> operation)
        {
            var streamRepresentation = operation.StreamRepresentation;
            if (streamRepresentation.Name != this.Name)
            {
                throw new ArgumentException(FormattableString.Invariant($"Cannot create a stream using a stream with mismatching name, confirm this is the stream you're intending to create; this.Name '{this.Name}' op.StreamRepresentation.Name '{streamRepresentation.Name}'."));
            }

            var allLocators = this.ResourceLocatorProtocol.Execute(new GetAllResourceLocatorsOp());
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
                                                      StreamSchema.Sprocs.PutObject.BuildCreationScript(this.Name),
                                                      StreamSchema.Sprocs.GetLatestByIdAndType.BuildCreationScript(this.Name),
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
        public override async Task ExecuteAsync(
            CreateStreamOp<TId> operation)
        {
            await Task.Run(() => this.Execute(operation));
        }

        /// <inheritdoc />
        public override void Execute(
            DeleteStreamOp<TId> operation)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override async Task ExecuteAsync(
            DeleteStreamOp<TId> operation)
        {
            await Task.Run(() => this.Execute(operation));
        }

        /// <inheritdoc />
        public override ISyncAndAsyncVoidProtocol<PutOp<TObject>> BuildPutProtocol<TObject>()
        {
            return new SqlStreamObjectOperationsProtocol<TId, TObject>(this);
        }

        /// <inheritdoc />
        public override ISyncAndAsyncReturningProtocol<GetLatestByIdAndTypeOp<TId, TObject>, TObject> BuildGetLatestByIdAndTypeProtocol<TObject>()
        {
            return new SqlStreamObjectOperationsProtocol<TId, TObject>(this);
        }

        /// <summary>
        /// Gets the described serializer.
        /// </summary>
        /// <param name="resourceLocator">The stream locator in case it needs to look up.</param>
        /// <returns>DescribedSerializer.</returns>
        public DescribedSerializer GetDescribedSerializer(
            SqlServerLocator resourceLocator)
        {
            if (this.serializerDescriptionToDescribedSerializerMap.ContainsKey(this.DefaultSerializerRepresentation))
            {
                return this.serializerDescriptionToDescribedSerializerMap[this.DefaultSerializerRepresentation];
            }

            var serializer = this.SerializerFactory.BuildSerializer(
                this.DefaultSerializerRepresentation);

            var serializerDescriptionId = this.Execute(new GetIdAddIfNecessarySerializerRepresentationOp(resourceLocator, this.DefaultSerializerRepresentation, this.DefaultSerializationFormat));
            var result = new DescribedSerializer(this.DefaultSerializerRepresentation, this.DefaultSerializationFormat, serializer, serializerDescriptionId);
            this.serializerDescriptionToDescribedSerializerMap.Add(this.DefaultSerializerRepresentation, result);
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
            var result = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.GetIdAddIfNecessarySerializerRepresentation.OutputParamNames.Id)]
                                    .GetValue<int>();
            return result;
        }

        /// <inheritdoc />
        public async Task<int> ExecuteAsync(
            GetIdAddIfNecessarySerializerRepresentationOp operation)
        {
            return await Task.FromResult(this.Execute(operation));
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
    }
}
