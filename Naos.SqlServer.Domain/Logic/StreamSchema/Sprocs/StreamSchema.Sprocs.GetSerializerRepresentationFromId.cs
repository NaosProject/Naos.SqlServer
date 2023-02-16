// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamSchema.Sprocs.GetSerializerRepresentationFromId.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System;
    using System.Collections.Generic;
    using OBeautifulCode.Compression;
    using OBeautifulCode.Serialization;
    using static System.FormattableString;

    public static partial class StreamSchema
    {
        public static partial class Sprocs
        {
            /// <summary>
            /// Stored procedure: GetSerializerRepresentationFromId.
            /// </summary>
            public static class GetSerializerRepresentationFromId
            {
                /// <summary>
                /// Gets the name of the stored procedure.
                /// </summary>
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
                /// <returns>Operation to execute stored procedure.</returns>
                public static ExecuteStoredProcedureOp BuildExecuteStoredProcedureOp(
                    string streamName,
                    int serializerRepresentationId)
                {
                    var sprocName = FormattableString.Invariant($"[{streamName}].{nameof(GetSerializerRepresentationFromId)}");
                    var parameters = new List<ParameterDefinitionBase>()
                                     {
                                         new InputParameterDefinition<int>(
                                             nameof(InputParamName.Id),
                                             Tables.SerializerRepresentation.Id.SqlDataType,
                                             serializerRepresentationId),
                                         new OutputParameterDefinition<SerializationKind>(
                                             nameof(OutputParamName.SerializationKind),
                                             Tables.SerializerRepresentation.SerializationKind.SqlDataType),
                                         new OutputParameterDefinition<int>(
                                             nameof(OutputParamName.ConfigTypeWithVersionId),
                                             Tables.SerializerRepresentation.SerializationConfigurationTypeWithVersionId.SqlDataType),
                                         new OutputParameterDefinition<CompressionKind>(
                                             nameof(OutputParamName.CompressionKind),
                                             Tables.SerializerRepresentation.CompressionKind.SqlDataType),
                                         new OutputParameterDefinition<SerializationFormat>(
                                             nameof(OutputParamName.SerializationFormat),
                                             Tables.SerializerRepresentation.SerializationFormat.SqlDataType),
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
                    var createOrModify = asAlter ? "CREATE OR ALTER" : "CREATE";
                    var result = Invariant(
                        $@"
{createOrModify} PROCEDURE [{streamName}].[{GetSerializerRepresentationFromId.Name}](
    @{InputParamName.Id} {Tables.SerializerRepresentation.Id.SqlDataType.DeclarationInSqlSyntax}
  , @{OutputParamName.SerializationKind} {Tables.SerializerRepresentation.SerializationKind.SqlDataType.DeclarationInSqlSyntax} OUTPUT
  , @{OutputParamName.ConfigTypeWithVersionId} {Tables.SerializerRepresentation.SerializationConfigurationTypeWithVersionId.SqlDataType.DeclarationInSqlSyntax} OUTPUT
  , @{OutputParamName.CompressionKind} {Tables.SerializerRepresentation.CompressionKind.SqlDataType.DeclarationInSqlSyntax} OUTPUT
  , @{OutputParamName.SerializationFormat} {Tables.SerializerRepresentation.SerializationFormat.SqlDataType.DeclarationInSqlSyntax} OUTPUT
)
AS
BEGIN
SELECT
	    @{OutputParamName.SerializationKind} = [{Tables.SerializerRepresentation.SerializationKind.Name}]
	  , @{OutputParamName.ConfigTypeWithVersionId} = [{Tables.SerializerRepresentation.SerializationConfigurationTypeWithVersionId.Name}]
	  , @{OutputParamName.CompressionKind} = [{Tables.SerializerRepresentation.CompressionKind.Name}]
	  , @{OutputParamName.SerializationFormat} = [{Tables.SerializerRepresentation.SerializationFormat.Name}]
	FROM [{streamName}].[{Tables.SerializerRepresentation.Table.Name}] WHERE [{Tables.SerializerRepresentation.Id.Name}] = @{InputParamName.Id}

END");

                    return result;
                }
            }
        }
    }
}