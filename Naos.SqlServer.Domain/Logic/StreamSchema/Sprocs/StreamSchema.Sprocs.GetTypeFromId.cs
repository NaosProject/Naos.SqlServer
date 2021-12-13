// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamSchema.Sprocs.GetTypeFromId.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System.Collections.Generic;
    using static System.FormattableString;

    public static partial class StreamSchema
    {
        public static partial class Sprocs
        {
            /// <summary>
            /// Stored procedure: GetTypeFromId.
            /// </summary>
            public static class GetTypeFromId
            {
                /// <summary>
                /// Gets the name of the stored procedure.
                /// </summary>
                public static string Name => nameof(GetTypeFromId);

                /// <summary>
                /// Input parameter names.
                /// </summary>
                public enum InputParamName
                {
                    /// <summary>
                    /// The identifier.
                    /// </summary>
                    Id,

                    /// <summary>
                    /// Including version.
                    /// </summary>
                    Versioned,
                }

                /// <summary>
                /// Output parameter names.
                /// </summary>
                public enum OutputParamName
                {
                    /// <summary>
                    /// The serialization kind.
                    /// </summary>
                    AssemblyQualifiedName,
                }

                /// <summary>
                /// Builds the execute stored procedure operation.
                /// </summary>
                /// <param name="streamName">Name of the stream.</param>
                /// <param name="typeId">The identifier.</param>
                /// <param name="versioned">Including version.</param>
                /// <returns>Operation to execute stored procedure.</returns>
                public static ExecuteStoredProcedureOp BuildExecuteStoredProcedureOp(
                    string streamName,
                    int typeId,
                    bool versioned)
                {
                    var sprocName = Invariant($"[{streamName}].{nameof(GetTypeFromId)}");

                    var parameters = new List<SqlParameterDefinitionBase>()
                                     {
                                         new SqlInputParameterDefinition<int>(nameof(InputParamName.Id), versioned ? Tables.TypeWithVersion.Id.SqlDataType : Tables.TypeWithoutVersion.Id.SqlDataType, typeId),
                                         new SqlInputParameterDefinition<int>(nameof(InputParamName.Versioned), new IntSqlDataTypeRepresentation(), versioned ? 1 : 0),
                                         new SqlOutputParameterDefinition<string>(nameof(OutputParamName.AssemblyQualifiedName), versioned ? Tables.TypeWithVersion.AssemblyQualifiedName.SqlDataType : Tables.TypeWithoutVersion.AssemblyQualifiedName.SqlDataType),
                                     };

                    var result = new ExecuteStoredProcedureOp(sprocName, parameters);

                    return result;
                }

                /// <summary>
                /// Builds the name of the put stored procedure.
                /// </summary>
                /// <param name="streamName">Name of the stream.</param>
                /// <param name="asAlter">An optional value indicating whether or not to generate a ALTER versus CREATE; DEFAULT is false and will generate a CREATE script.</param>
                /// <returns>Creation script for creating the stored procedure.</returns>
                public static string BuildCreationScript(
                    string streamName,
                    bool asAlter = false)
                {
                    var createOrModify = asAlter ? "ALTER" : "CREATE";
                    var result = Invariant(
                        $@"
{createOrModify} PROCEDURE [{streamName}].[{GetTypeFromId.Name}](
    @{InputParamName.Id} {Tables.TypeWithVersion.Id.SqlDataType.DeclarationInSqlSyntax}
  , @{InputParamName.Versioned} [BIT]
  , @{OutputParamName.AssemblyQualifiedName} {Tables.TypeWithVersion.AssemblyQualifiedName.SqlDataType.DeclarationInSqlSyntax} OUTPUT
)
AS
BEGIN
    IF (@{InputParamName.Versioned} = 1)
    BEGIN
        SELECT
	            @{OutputParamName.AssemblyQualifiedName} = [{Tables.TypeWithVersion.AssemblyQualifiedName.Name}]
	        FROM [{streamName}].[{Tables.TypeWithVersion.Table.Name}] WHERE [{Tables.TypeWithVersion.Id.Name}] = @{InputParamName.Id}
    END
    ELSE
    BEGIN
        SELECT
	            @{OutputParamName.AssemblyQualifiedName} = [{Tables.TypeWithoutVersion.AssemblyQualifiedName.Name}]
	        FROM [{streamName}].[{Tables.TypeWithoutVersion.Table.Name}] WHERE [{Tables.TypeWithoutVersion.Id.Name}] = @{InputParamName.Id}
    END
END");

                    return result;
                }
            }
        }
    }
}