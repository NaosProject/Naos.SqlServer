// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlStream.TypeCaching.cs" company="Naos Project">
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
    public partial class SqlStream
    {
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

            var sqlProtocol = this.BuildSqlOperationsProtocol(locator);

            // TODO: add caching by assembly qualified name
            var assemblyQualifiedNameWithoutVersion = typeRepresentation.WithoutVersion.BuildAssemblyQualifiedName();
            var storedProcWithoutVersionOp = StreamSchema.Sprocs.GetIdAddIfNecessaryTypeWithoutVersion.BuildExecuteStoredProcedureOp(
                                                              this.Name,
                                                              assemblyQualifiedNameWithoutVersion);

            var sprocResultWithoutVersion = sqlProtocol.Execute(storedProcWithoutVersionOp);
            var withoutVersionId = sprocResultWithoutVersion.OutputParameters[nameof(StreamSchema.Sprocs.GetIdAddIfNecessaryTypeWithoutVersion.OutputParamName.Id)].GetValue<int>();

            // TODO: add caching by assembly qualified name
            var assemblyQualifiedNameWithVersion = typeRepresentation.WithVersion.BuildAssemblyQualifiedName();
            var storedProcWithVersionOp = StreamSchema.Sprocs.GetIdAddIfNecessaryTypeWithVersion.BuildExecuteStoredProcedureOp(
                this.Name,
                assemblyQualifiedNameWithVersion);
            var sprocResultWithVersion = sqlProtocol.Execute(storedProcWithVersionOp);
            var withVersionId = sprocResultWithVersion.OutputParameters[nameof(StreamSchema.Sprocs.GetIdAddIfNecessaryTypeWithVersion.OutputParamName.Id)].GetValue<int>();

            var result = new IdentifiedType(withoutVersionId, withVersionId);
            return result;
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
            var storedProcOp = StreamSchema.Sprocs.GetTypeFromId.BuildExecuteStoredProcedureOp(
                this.Name,
                typeWithVersionId,
                versioned);

            var sqlProtocol = this.BuildSqlOperationsProtocol(locator);
            var sprocResult = sqlProtocol.Execute(storedProcOp);
            var assemblyQualifiedName = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.GetTypeFromId.OutputParamName.AssemblyQualifiedName)].GetValue<string>();
            var typeRep = assemblyQualifiedName.ToTypeRepresentationFromAssemblyQualifiedName();
            var result = typeRep.ToWithAndWithoutVersion();

            return result;
        }
    }
}
