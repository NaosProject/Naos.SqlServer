// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlStream.SerializerRepresentationCaching.cs" company="Naos Project">
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
    using static System.FormattableString;
    using SerializationFormat = OBeautifulCode.Serialization.SerializationFormat;

    public partial class SqlStream
    {
        private readonly IDictionary<SerializerRepresentation, IdentifiedSerializerRepresentation> serializerDescriptionToDescribedSerializerMap = new Dictionary<SerializerRepresentation, IdentifiedSerializerRepresentation>();

        /// <summary>
        /// Gets the serializer representation from identifier (first by cache and then by database).
        /// </summary>
        /// <param name="sqlServerLocator">The resource locator.</param>
        /// <param name="serializerRepresentationId">The serializer representation identifier.</param>
        /// <returns>IdentifiedSerializerRepresentation.</returns>
        public IdentifiedSerializerRepresentation GetSerializerRepresentationFromId(
            SqlServerLocator sqlServerLocator,
            int serializerRepresentationId)
        {
            var storedProcOp = StreamSchema.Sprocs.GetSerializerRepresentationFromId.BuildExecuteStoredProcedureOp(
                this.Name,
                serializerRepresentationId);

            var sqlProtocol = this.BuildSqlOperationsProtocol(sqlServerLocator);
            var sprocResult = sqlProtocol.Execute(storedProcOp);

            SerializationKind serializationKind = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.GetSerializerRepresentationFromId.OutputParamName.SerializationKind)].GetValue<SerializationKind>();
            int configTypeWithVersionId = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.GetSerializerRepresentationFromId.OutputParamName.ConfigTypeWithVersionId)].GetValue<int>();
            CompressionKind compressionKind = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.GetSerializerRepresentationFromId.OutputParamName.CompressionKind)].GetValue<CompressionKind>();
            SerializationFormat serializationFormat = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.GetSerializerRepresentationFromId.OutputParamName.SerializationFormat)].GetValue<SerializationFormat>();

            var configType = this.GetTypeById(sqlServerLocator, configTypeWithVersionId, true);

            var rep = new SerializerRepresentation(serializationKind, configType.WithVersion, compressionKind);
            var result = new IdentifiedSerializerRepresentation(serializerRepresentationId, rep, serializationFormat);

            return result;
        }

        /// <summary>
        /// Gets the described serializer.
        /// </summary>
        /// <param name="resourceLocator">The stream locator in case it needs to look up.</param>
        /// <param name="serializerRepresentation">Optional <see cref="SerializerRepresentation"/>; default is DefaultSerializerRepresentation.</param>
        /// <param name="serializationFormat">Optional <see cref="SerializationFormat"/>; default is <see cref="SerializationFormat.String"/>.</param>
        /// <returns>DescribedSerializer.</returns>
        public IdentifiedSerializerRepresentation GetIdAddIfNecessarySerializerRepresentation(
            SqlServerLocator resourceLocator,
            SerializerRepresentation serializerRepresentation = null,
            SerializationFormat serializationFormat = SerializationFormat.String)
        {
            var localSerializerRepresentation = serializerRepresentation ?? this.DefaultSerializerRepresentation;
            if (this.serializerDescriptionToDescribedSerializerMap.ContainsKey(localSerializerRepresentation))
            {
                return this.serializerDescriptionToDescribedSerializerMap[localSerializerRepresentation];
            }

            var id = this.Execute(new GetIdAddIfNecessarySerializerRepresentationOp(resourceLocator, localSerializerRepresentation, serializationFormat));
            var result = new IdentifiedSerializerRepresentation(id, localSerializerRepresentation, serializationFormat);
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

            var sqlLocator = this.TryGetLocator(operation);
            var sqlProtocol = this.BuildSqlOperationsProtocol(sqlLocator);
            var sprocResult = sqlProtocol.Execute(storedProcOp);
            var result = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.GetIdAddIfNecessarySerializerRepresentation.OutputParamName.Id)]
                                    .GetValue<int>();
            return result;
        }
    }
}
