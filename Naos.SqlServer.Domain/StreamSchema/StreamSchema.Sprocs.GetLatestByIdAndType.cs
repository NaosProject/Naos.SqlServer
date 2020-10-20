// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamSchema.Sprocs.GetLatestByIdAndType.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Naos.CodeAnalysis.Recipes;
    using Naos.Protocol.Domain;
    using OBeautifulCode.Compression;
    using OBeautifulCode.Serialization;

    /// <summary>
    /// Stream schema.
    /// </summary>
    public static partial class StreamSchema
    {
        /// <summary>
        /// Stored procedures.
        /// </summary>
        public partial class Sprocs
        {
            /// <summary>
            /// Stored procedure: GetLatestByIdAndType.
            /// </summary>
            public static class GetLatestByIdAndType
            {
                /// <summary>
                /// Input parameter names.
                /// </summary>
                public enum InputParamNames
                {
                    /// <summary>
                    /// The serialized object identifier
                    /// </summary>
                    SerializedObjectId,

                    /// <summary>
                    /// The object assembly qualified name without version
                    /// </summary>
                    ObjectAssemblyQualifiedNameWithoutVersion,

                    /// <summary>
                    /// The object assembly qualified name with version
                    /// </summary>
                    ObjectAssemblyQualifiedNameWithVersion,

                    /// <summary>
                    /// The type version match strategy
                    /// </summary>
                    TypeVersionMatchStrategy,
                }

                /// <summary>
                /// Output parameter names.
                /// </summary>
                public enum OutputParamNames
                {
                    /// <summary>
                    /// The serialization configuration assembly qualified name without version
                    /// </summary>
                    SerializationConfigAssemblyQualifiedNameWithoutVersion,

                    /// <summary>
                    /// The serialization kind
                    /// </summary>
                    SerializationKind,

                    /// <summary>
                    /// The serialization format
                    /// </summary>
                    SerializationFormat,

                    /// <summary>
                    /// The compression kind
                    /// </summary>
                    CompressionKind,

                    /// <summary>
                    /// The unregistered type encountered strategy.
                    /// </summary>
                    UnregisteredTypeEncounteredStrategy,

                    /// <summary>
                    /// The serialized object string
                    /// </summary>
                    SerializedObjectString,

                    /// <summary>
                    /// The serialized object binary
                    /// </summary>
                    SerializedObjectBinary,
                }

                /// <summary>
                /// Builds the execute stored procedure operation.
                /// </summary>
                /// <param name="streamName">Name of the stream.</param>
                /// <param name="serializedObjectId">The serialized object identifier.</param>
                /// <param name="objectAssemblyQualifiedNameWithoutVersion">The object assembly qualified name without version.</param>
                /// <param name="objectAssemblyQualifiedNameWithVersion">The object assembly qualified name with version.</param>
                /// <param name="typeVersionMatchStrategy">The type version match strategy.</param>
                /// <returns>ExecuteStoredProcedureOp.</returns>
                public static ExecuteStoredProcedureOp BuildExecuteStoredProcedureOp(
                    string streamName,
                    string serializedObjectId,
                    string objectAssemblyQualifiedNameWithoutVersion,
                    string objectAssemblyQualifiedNameWithVersion,
                    TypeVersionMatchStrategy typeVersionMatchStrategy)
                {
                    var sprocName = FormattableString.Invariant($"[{streamName}].{nameof(GetLatestByIdAndType)}");

                    var parameters = new List<SqlParameterRepresentationBase>()
                                     {
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamNames.SerializedObjectId), Tables.Object.SerializedObjectId.DataType, serializedObjectId),
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamNames.ObjectAssemblyQualifiedNameWithoutVersion), Tables.TypeWithoutVersion.AssemblyQualifiedName.DataType, objectAssemblyQualifiedNameWithoutVersion),
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamNames.ObjectAssemblyQualifiedNameWithVersion), Tables.TypeWithVersion.AssemblyQualifiedName.DataType, objectAssemblyQualifiedNameWithVersion),
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamNames.TypeVersionMatchStrategy), new StringSqlDataTypeRepresentation(false, 50), typeVersionMatchStrategy.ToString()),
                                         new SqlOutputParameterRepresentation<string>(nameof(OutputParamNames.SerializationConfigAssemblyQualifiedNameWithoutVersion), Tables.TypeWithoutVersion.AssemblyQualifiedName.DataType),
                                         new SqlOutputParameterRepresentation<SerializationKind>(nameof(OutputParamNames.SerializationKind), Tables.SerializerRepresentation.SerializationKind.DataType),
                                         new SqlOutputParameterRepresentation<SerializationFormat>(nameof(OutputParamNames.SerializationFormat), Tables.SerializerRepresentation.SerializationFormat.DataType),
                                         new SqlOutputParameterRepresentation<CompressionKind>(nameof(OutputParamNames.CompressionKind), Tables.SerializerRepresentation.CompressionKind.DataType),
                                         new SqlOutputParameterRepresentation<UnregisteredTypeEncounteredStrategy>(nameof(OutputParamNames.UnregisteredTypeEncounteredStrategy), Tables.SerializerRepresentation.UnregisteredTypeEncounteredStrategy.DataType),
                                         new SqlOutputParameterRepresentation<string>(nameof(OutputParamNames.SerializedObjectString), Tables.Object.SerializedObjectString.DataType),
                                         new SqlOutputParameterRepresentation<byte[]>(nameof(OutputParamNames.SerializedObjectBinary), Tables.Object.SerializedObjectBinary.DataType),
                                     };

                    var parameterNameToRepresentationMap = parameters.ToDictionary(k => k.Name, v => v);

                    var result = new ExecuteStoredProcedureOp(sprocName, parameterNameToRepresentationMap);

                    return result;
                }

                /// <summary>
                /// Builds the creation script for put sproc.
                /// </summary>
                /// <param name="streamName">Name of the stream.</param>
                /// <returns>System.String.</returns>
                [System.Diagnostics.CodeAnalysis.SuppressMessage(
                    "Microsoft.Naming",
                    "CA1702:CompoundWordsShouldBeCasedCorrectly",
                    MessageId = "ForGet",
                    Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
                [System.Diagnostics.CodeAnalysis.SuppressMessage(
                    "Microsoft.Naming",
                    "CA1704:IdentifiersShouldBeSpelledCorrectly",
                    MessageId = "Sproc",
                    Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
                public static string BuildCreationScript(
                    string streamName)
                {
                    var result = FormattableString.Invariant(
                        $@"
CREATE PROCEDURE [{streamName}].GetLatestByIdAndType(
  @{nameof(InputParamNames.SerializedObjectId)} AS nvarchar(450)
, @{nameof(InputParamNames.ObjectAssemblyQualifiedNameWithoutVersion)} AS nvarchar(2000)
, @{nameof(InputParamNames.ObjectAssemblyQualifiedNameWithVersion)} AS nvarchar(2000)
, @{nameof(InputParamNames.TypeVersionMatchStrategy)} AS varchar(10)
, @{nameof(OutputParamNames.SerializationConfigAssemblyQualifiedNameWithoutVersion)} AS nvarchar(2000) OUTPUT
, @{nameof(OutputParamNames.SerializationKind)} AS varchar(50) OUTPUT
, @{nameof(OutputParamNames.SerializationFormat)} AS varchar(50) OUTPUT
, @{nameof(OutputParamNames.CompressionKind)} AS varchar(50) OUTPUT
, @{nameof(OutputParamNames.UnregisteredTypeEncounteredStrategy)} AS varchar(50) OUTPUT
, @{nameof(OutputParamNames.SerializedObjectString)} AS nvarchar(MAX) OUTPUT
, @{nameof(OutputParamNames.SerializedObjectBinary)} AS varbinary(MAX) OUTPUT
)
AS
BEGIN

    DECLARE @SerializerRepresentationId int   
	DECLARE @ObjectTypeWithoutVersionId int
	DECLARE @ObjectTypeWithVersionId int
    SELECT TOP 1
	   @SerializerRepresentationId = [SerializerRepresentationId]
	 , @ObjectTypeWithoutVersionId = [ObjectTypeWithoutVersionId]
	 , @ObjectTypeWithVersionId = [ObjectTypeWithVersionId]
	 , @{nameof(OutputParamNames.SerializedObjectString)} = [SerializedObjectString]
	 , @{nameof(OutputParamNames.SerializedObjectBinary)} = [SerializedObjectBinary]
	FROM [{streamName}].[Object]
	WHERE [SerializedObjectId] = @SerializedObjectId
	ORDER BY [Id] DESC
--check for record count and update contract to have an understanding of nothing found
	DECLARE @SerializationConfigTypeWithoutVersionId int
	SELECT 
		@SerializationConfigTypeWithoutVersionId = [{nameof(Tables.SerializerRepresentation.SerializationConfigurationTypeWithoutVersionId)}] 
	  , @{nameof(OutputParamNames.SerializationKind)} = [{nameof(Tables.SerializerRepresentation.SerializationKind)}]
	  , @{nameof(OutputParamNames.SerializationFormat)} = [{nameof(Tables.SerializerRepresentation.SerializationFormat)}]
	  , @{nameof(OutputParamNames.CompressionKind)} = [{nameof(Tables.SerializerRepresentation.CompressionKind)}]
	  , @{nameof(OutputParamNames.UnregisteredTypeEncounteredStrategy)} = [{nameof(Tables.SerializerRepresentation.UnregisteredTypeEncounteredStrategy)}]
	FROM [{streamName}].[SerializerRepresentation] WHERE [Id] = @SerializerRepresentationId

	SELECT @{nameof(OutputParamNames.SerializationConfigAssemblyQualifiedNameWithoutVersion)} = [AssemblyQualifiedName] FROM [{streamName}].[TypeWithoutVersion] WHERE [Id] = @SerializationConfigTypeWithoutVersionId
	SELECT @ObjectAssemblyQualifiedNameWithoutVersion = [AssemblyQualifiedName] FROM [{streamName}].[TypeWithoutVersion] WHERE [Id] = @ObjectTypeWithoutVersionId
	SELECT @ObjectAssemblyQualifiedNameWithVersion = [AssemblyQualifiedName] FROM [{streamName}].[TypeWithVersion] WHERE [Id] = @ObjectTypeWithVersionId
END

			");

                    return result;
                }
            }
        }
    }
}
