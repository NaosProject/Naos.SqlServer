// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlStream.TypeCaching.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Protocol.Client
{
    using System;
    using System.Collections.Concurrent;
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

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Acceptable given it creates the stream.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = NaosSuppressBecause.CA1711_IdentifiersShouldNotHaveIncorrectSuffix_TypeNameAddedAsSuffixForTestsWhereTypeIsPrimaryConcern)]
    public partial class SqlStream
    {
        private readonly ConcurrentDictionary<int, TypeRepresentationWithAndWithoutVersion> assemblyQualifiedNameWithVersionIdToIdentifiedTypeMap = new ConcurrentDictionary<int, TypeRepresentationWithAndWithoutVersion>();
        private readonly ConcurrentDictionary<string, IdentifiedType> assemblyQualifiedNameWithVersionToIdentifiedTypeMap = new ConcurrentDictionary<string, IdentifiedType>();

        /// <summary>
        /// Gets the type of the ids add if necessary.
        /// </summary>
        /// <param name="locator">The locator.</param>
        /// <param name="typeRepresentation">The type representation.</param>
        /// <returns>IdentifiedType.</returns>
        public IdentifiedType GetIdsAddIfNecessaryType(SqlServerLocator locator, TypeRepresentationWithAndWithoutVersion typeRepresentation)
        {
            if (typeRepresentation == null)
            {
                return null;
            }

            var assemblyQualifiedNameWithVersion = typeRepresentation.WithVersion.BuildAssemblyQualifiedName();
            var found = this.assemblyQualifiedNameWithVersionToIdentifiedTypeMap.TryGetValue(assemblyQualifiedNameWithVersion, out var result);
            if (found)
            {
                return result;
            }
            else
            {
                var assemblyQualifiedNameWithoutVersion = typeRepresentation.WithoutVersion.BuildAssemblyQualifiedName();

                var sqlProtocol = this.BuildSqlOperationsProtocol(locator);
                var storedProcWithoutVersionOp = StreamSchema.Sprocs.GetIdAddIfNecessaryTypeWithoutVersion.BuildExecuteStoredProcedureOp(
                    this.Name,
                    assemblyQualifiedNameWithoutVersion);

                var sprocResultWithoutVersion = sqlProtocol.Execute(storedProcWithoutVersionOp);
                var withoutVersionId = sprocResultWithoutVersion
                                      .OutputParameters[nameof(StreamSchema.Sprocs.GetIdAddIfNecessaryTypeWithoutVersion.OutputParamName.Id)]
                                      .GetValueOfType<int>();
                var storedProcWithVersionOp = StreamSchema.Sprocs.GetIdAddIfNecessaryTypeWithVersion.BuildExecuteStoredProcedureOp(
                    this.Name,
                    assemblyQualifiedNameWithVersion);
                var sprocResultWithVersion = sqlProtocol.Execute(storedProcWithVersionOp);
                var withVersionId = sprocResultWithVersion
                                   .OutputParameters[nameof(StreamSchema.Sprocs.GetIdAddIfNecessaryTypeWithVersion.OutputParamName.Id)]
                                   .GetValueOfType<int>();

                var item = new IdentifiedType(withoutVersionId, withVersionId);
                this.assemblyQualifiedNameWithVersionToIdentifiedTypeMap.TryAdd(assemblyQualifiedNameWithVersion, item);
                var newFound = this.assemblyQualifiedNameWithVersionToIdentifiedTypeMap.TryGetValue(assemblyQualifiedNameWithVersion, out var newResult);
                newFound.MustForOp("failedToFindSerializationRepresentationAfterAdding").BeTrue();
                return newResult;
            }
        }

        /// <summary>
        /// Gets the type by identifier.
        /// </summary>
        /// <param name="locator">The locator.</param>
        /// <param name="typeWithVersionId">The type with version identifier.</param>
        /// <param name="versioned">if set to <c>true</c> [versioned].</param>
        /// <returns>TypeRepresentationWithAndWithoutVersion.</returns>
        public TypeRepresentationWithAndWithoutVersion GetTypeById(
            SqlServerLocator locator,
            int typeWithVersionId,
            bool versioned)
        {
            versioned.MustForArg(nameof(versioned)).BeTrue("Can't allow until I can confirm the that a null versioned type rep doesn't matter.");
            var found = this.assemblyQualifiedNameWithVersionIdToIdentifiedTypeMap.TryGetValue(typeWithVersionId, out var result);
            if (found)
            {
                return result;
            }
            else
            {
                var storedProcOp = StreamSchema.Sprocs.GetTypeFromId.BuildExecuteStoredProcedureOp(
                    this.Name,
                    typeWithVersionId,
                    versioned);

                var sqlProtocol = this.BuildSqlOperationsProtocol(locator);
                var sprocResult = sqlProtocol.Execute(storedProcOp);
                var assemblyQualifiedName = sprocResult
                                           .OutputParameters[nameof(StreamSchema.Sprocs.GetTypeFromId.OutputParamName.AssemblyQualifiedName)]
                                           .GetValueOfType<string>();
                var typeRep = assemblyQualifiedName.ToTypeRepresentationFromAssemblyQualifiedName();
                var item = typeRep.ToWithAndWithoutVersion();
                this.assemblyQualifiedNameWithVersionIdToIdentifiedTypeMap.TryAdd(typeWithVersionId, item);

                var newFound = this.assemblyQualifiedNameWithVersionIdToIdentifiedTypeMap.TryGetValue(typeWithVersionId, out var newResult);
                newFound.MustForOp("failedToFindSerializationRepresentationAfterAdding").BeTrue();
                return newResult;
            }
        }
    }
}
