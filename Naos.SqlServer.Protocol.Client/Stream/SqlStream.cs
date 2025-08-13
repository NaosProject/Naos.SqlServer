// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlStream.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Protocol.Client
{
    using System;
    using System.Linq;
    using Naos.CodeAnalysis.Recipes;
    using Naos.Database.Domain;
    using Naos.SqlServer.Domain;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Serialization;
    using OBeautifulCode.Type;
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

        private SqlServerLocator TryGetLocator(ISpecifyResourceLocator locatorSpecification = null)
        {
            var locator = locatorSpecification?.SpecifiedResourceLocator?.ConfirmAndConvert<SqlServerLocator>()
                       ?? this.singleLocator
                       ?? throw new NotSupportedException(Invariant($"There is not a locator specified on {locatorSpecification?.ToString()} and there is not a single locator specified to fall back to."));

            return locator;
        }

        private static StreamRecordPayloadBase BuildStreamRecordPayload(
            StreamRecordItemsToInclude streamRecordItemsToInclude,
            SerializationFormat serializationFormat,
            byte[] binarySerializedObject,
            string stringSerializedObject)
        {
            StreamRecordPayloadBase result;

            if (streamRecordItemsToInclude == StreamRecordItemsToInclude.MetadataAndPayload)
            {
                switch (serializationFormat)
                {
                    case SerializationFormat.Binary:
                        result = new BinaryStreamRecordPayload(binarySerializedObject);
                        break;
                    case SerializationFormat.String:
                        result = new StringStreamRecordPayload(stringSerializedObject);
                        break;
                    default:
                        throw new NotSupportedException(Invariant($"{nameof(SerializationFormat)} {serializationFormat} is not supported."));
                }
            }
            else
            {
                result = new NullStreamRecordPayload();
            }

            return result;
        }
    }
}
