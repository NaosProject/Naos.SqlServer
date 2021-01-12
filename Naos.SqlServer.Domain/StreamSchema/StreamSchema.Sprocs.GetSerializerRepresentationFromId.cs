﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamSchema.Sprocs.GetSerializerRepresentationFromId.cs" company="Naos Project">
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
            public static class GetSerializerRepresentationFromId
            {
                /// <summary>
                /// Gets the name of the stored procedure.
                /// </summary>
                /// <value>The name of the stored procedure.</value>
                public static string Name => nameof(GetSerializerRepresentationFromId);

                /// <summary>
                /// Input parameter names.
                /// </summary>
                public enum InputParamName
                {
                    /// <summary>
                    /// The identifier.
                    /// </summary>
                    Id,
                }

                /// <summary>
                /// Output parameter names.
                /// </summary>
                public enum OutputParamName
                {
                    /// <summary>
                    /// The serialization kind.
                    /// </summary>
                    SerializationKind,

                    /// <summary>
                    /// The configuration type with version identifier.
                    /// </summary>
                    ConfigTypeWithVersionId,

                    /// <summary>
                    /// The compression kind.
                    /// </summary>
                    CompressionKind,

                    /// <summary>
                    /// The serialization format.
                    /// </summary>
                    SerializationFormat,
                }

                /// <summary>
                /// Builds the execute stored procedure operation.
                /// </summary>
                /// <param name="streamName">Name of the stream.</param>
                /// <param name="serializerRepresentationId">The identifier.</param>
                /// <returns>ExecuteStoredProcedureOp.</returns>
                public static ExecuteStoredProcedureOp BuildExecuteStoredProcedureOp(
                    string streamName,
                    int serializerRepresentationId)
                {
                    var sprocName = FormattableString.Invariant($"[{streamName}].{nameof(GetSerializerRepresentationFromId)}");
                    var parameters = new List<SqlParameterRepresentationBase>()
                                     {
                                         new SqlInputParameterRepresentation<int>(
                                             nameof(InputParamName.Id),
                                             Tables.SerializerRepresentation.Id.DataType,
                                             serializerRepresentationId),
                                         new SqlOutputParameterRepresentation<string>(
                                             nameof(OutputParamName.SerializationKind),
                                             Tables.SerializerRepresentation.SerializationKind.DataType),
                                         new SqlOutputParameterRepresentation<int>(
                                             nameof(OutputParamName.ConfigTypeWithVersionId),
                                             Tables.SerializerRepresentation.SerializationConfigurationTypeWithVersionId.DataType),
                                         new SqlOutputParameterRepresentation<string>(
                                             nameof(OutputParamName.CompressionKind),
                                             Tables.SerializerRepresentation.CompressionKind.DataType),
                                         new SqlOutputParameterRepresentation<string>(
                                             nameof(OutputParamName.SerializationFormat),
                                             Tables.SerializerRepresentation.SerializationFormat.DataType),
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
    @{InputParamName.Id} {Tables.SerializerRepresentation.Id.DataType.DeclarationInSqlSyntax}
  , @{OutputParamName.SerializationKind} {Tables.SerializerRepresentation.SerializationKind} OUTPUT
  , @{OutputParamName.ConfigTypeWithVersionId} {Tables.SerializerRepresentation.SerializationConfigurationTypeWithVersionId} OUTPUT
  , @{OutputParamName.CompressionKind} {Tables.SerializerRepresentation.CompressionKind} OUTPUT
  , @{OutputParamName.SerializationFormat} {Tables.SerializerRepresentation.SerializationFormat} OUTPUT
)
AS
BEGIN
SELECT 
	    @{OutputParamName.SerializationKind} = [{Tables.SerializerRepresentation.SerializationKind.Name}]
	  , @{OutputParamName.SerializationFormat} = [{Tables.SerializerRepresentation.SerializationFormat.Name}]
	  , @{OutputParamName.CompressionKind} = [{Tables.SerializerRepresentation.CompressionKind.Name}]
	  , @{OutputParamName.SerializationFormat} = [{Tables.SerializerRepresentation.SerializationFormat.Name}]
	FROM [{streamName}].[{Tables.SerializerRepresentation.Table.Name}] WHERE [{Tables.SerializerRepresentation.Id.Name}] = @{InputParamName.Id}

END");
                }
            }
        }
    }
}