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
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Runtime.InteropServices.ComTypes;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Naos.CodeAnalysis.Recipes;
    using Naos.Database.Domain;
    using Naos.Recipes.RunWithRetry;
    using Naos.SqlServer.Domain;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Collection.Recipes;
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
    /// SQL implementation of an <see cref="StandardStreamBase"/>.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Acceptable given it creates the stream.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = NaosSuppressBecause.CA1711_IdentifiersShouldNotHaveIncorrectSuffix_TypeNameAddedAsSuffixForTestsWhereTypeIsPrimaryConcern)]
    public partial class SqlStream : StandardStreamBase,
                                     ISyncAndAsyncReturningProtocol<GetOrAddIdentifiedSerializerRepresentationOp, int>,
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
        : base(name, serializerFactory, defaultSerializerRepresentation, defaultSerializationFormat, resourceLocatorProtocol)
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
            StandardGetNextUniqueLongOp operation)
        {
            var locator = this.ResourceLocatorProtocols.Execute(new GetResourceLocatorForUniqueIdentifierOp());
            var sqlServerLocator = locator as SqlServerLocator
                                ?? throw new NotSupportedException(Invariant($"{nameof(GetResourceLocatorForUniqueIdentifierOp)} should return a {nameof(SqlServerLocator)} and returned {locator?.GetType().ToStringReadable()}."));

            var storedProcOp = StreamSchema.Sprocs.GetNextUniqueLong.BuildExecuteStoredProcedureOp(this.Name);

            var sqlProtocol = this.BuildSqlOperationsProtocol(sqlServerLocator);
            var sprocResult = sqlProtocol.Execute(storedProcOp);

            long result = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.GetNextUniqueLong.OutputParamName.Value)].GetValueOfType<long>();

            return result;
        }

        /// <inheritdoc />
        public override StreamRecord Execute(
            StandardGetLatestRecordOp operation)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override void Execute(
            StandardDeleteStreamOp operation)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override void Execute(
            StandardPruneStreamOp operation)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override bool Execute(
            StandardDoesAnyExistByIdOp operation)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override IReadOnlyList<StreamRecord> Execute(
            StandardGetAllRecordsByIdOp operation)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override IReadOnlyList<StreamRecordMetadata> Execute(
            StandardGetAllRecordsMetadataByIdOp operation)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override string Execute(
            StandardGetLatestStringSerializedObjectByIdOp operation)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public async Task<int> ExecuteAsync(
            GetOrAddIdentifiedSerializerRepresentationOp operation)
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
        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "sprocResult", Justification = "Part of contract, could be a output parameter in future.")]
        public void Execute(
            CreateStreamUserOp operation)
        {
            var allLocators = this.ResourceLocatorProtocols.Execute(new GetAllResourceLocatorsOp());
            foreach (var resourceLocator in allLocators)
            {
                var sqlServerLocator = resourceLocator.ConfirmAndConvert<SqlServerLocator>();

                var roles = operation.ProtocolsToGrantAccessFor.Select(_ => StreamSchema.GetRoleNameFromProtocolType(_, this.Name)).ToList();
                var rolesCsv = roles.ToCsv();
                var storedProcOp = StreamSchema.Sprocs.CreateStreamUser.BuildExecuteStoredProcedureOp(
                    this.Name,
                    operation.UserName,
                    operation.ClearTextPassword,
                    rolesCsv);

                var sqlProtocol = this.BuildSqlOperationsProtocol(sqlServerLocator);
                var sprocResult = sqlProtocol.Execute(storedProcOp);
            }
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
