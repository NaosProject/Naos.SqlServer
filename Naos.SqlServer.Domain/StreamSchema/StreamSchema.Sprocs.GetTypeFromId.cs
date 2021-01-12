// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamSchema.Sprocs.GetTypeFromId.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Container for schema.
    /// </summary>
    public static partial class StreamSchema
    {
        /// <summary>
        /// Stored procedures.
        /// </summary>
        public static partial class Sprocs
        {
            /// <summary>
            /// Class TypeWithVersion.
            /// </summary>
            public static class GetTypeFromId
            {
                /// <summary>
                /// Gets the name of the stored procedure.
                /// </summary>
                /// <value>The name of the stored procedure.</value>
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
                /// <returns>ExecuteStoredProcedureOp.</returns>
                public static ExecuteStoredProcedureOp BuildExecuteStoredProcedureOp(
                    string streamName,
                    int typeId,
                    bool versioned)
                {
                    var sprocName = FormattableString.Invariant($"[{streamName}].{nameof(GetTypeFromId)}");
                    var parameters = new List<SqlParameterRepresentationBase>()
                                     {
                                         new SqlInputParameterRepresentation<int>(nameof(InputParamName.Id), versioned ? Tables.TypeWithVersion.Id.DataType : Tables.TypeWithoutVersion.Id.DataType, typeId),
                                         new SqlInputParameterRepresentation<int>(nameof(InputParamName.Versioned), new IntSqlDataTypeRepresentation(), versioned ? 1 : 0),
                                         new SqlOutputParameterRepresentation<string>(nameof(OutputParamName.AssemblyQualifiedName), versioned ? Tables.TypeWithVersion.AssemblyQualifiedName.DataType : Tables.TypeWithoutVersion.AssemblyQualifiedName.DataType),
                                     };

                    var parameterNameToDetailsMap = parameters.ToDictionary(k => k.Name, v => v);

                    var result = new ExecuteStoredProcedureOp(sprocName, parameterNameToDetailsMap);

                    return result;
                }

                /// <summary>
                /// Builds the name of the put stored procedure.
                /// </summary>
                /// <param name="streamName">Name of the stream.</param>
                /// <returns>Name of the put stored procedure.</returns>
                public static string BuildCreationScript(
                    string streamName)
                {
                    return FormattableString.Invariant(
                        $@"
CREATE PROCEDURE [{streamName}].{Name}(
    @{InputParamName.Id} {Tables.TypeWithVersion.Id.DataType.DeclarationInSqlSyntax}
  , @{InputParamName.Versioned} [BIT]
  , @{OutputParamName.AssemblyQualifiedName} {Tables.TypeWithVersion.AssemblyQualifiedName.DataType.DeclarationInSqlSyntax} OUTPUT
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
                }
            }
        }
    }
}