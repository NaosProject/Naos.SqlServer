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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sprocs", Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
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
                [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Param", Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
                public enum InputParamName
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
                [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Param", Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
                public enum OutputParamName
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
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamName.SerializedObjectId), Tables.Object.SerializedObjectId.DataType, serializedObjectId),
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamName.ObjectAssemblyQualifiedNameWithoutVersion), Tables.TypeWithoutVersion.AssemblyQualifiedName.DataType, objectAssemblyQualifiedNameWithoutVersion),
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamName.ObjectAssemblyQualifiedNameWithVersion), Tables.TypeWithVersion.AssemblyQualifiedName.DataType, objectAssemblyQualifiedNameWithVersion),
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamName.TypeVersionMatchStrategy), new StringSqlDataTypeRepresentation(false, 50), typeVersionMatchStrategy.ToString()),
                                         new SqlOutputParameterRepresentation<string>(nameof(OutputParamName.SerializationConfigAssemblyQualifiedNameWithoutVersion), Tables.TypeWithoutVersion.AssemblyQualifiedName.DataType),
                                         new SqlOutputParameterRepresentation<SerializationKind>(nameof(OutputParamName.SerializationKind), Tables.SerializerRepresentation.SerializationKind.DataType),
                                         new SqlOutputParameterRepresentation<SerializationFormat>(nameof(OutputParamName.SerializationFormat), Tables.SerializerRepresentation.SerializationFormat.DataType),
                                         new SqlOutputParameterRepresentation<CompressionKind>(nameof(OutputParamName.CompressionKind), Tables.SerializerRepresentation.CompressionKind.DataType),
                                         new SqlOutputParameterRepresentation<UnregisteredTypeEncounteredStrategy>(nameof(OutputParamName.UnregisteredTypeEncounteredStrategy), Tables.SerializerRepresentation.UnregisteredTypeEncounteredStrategy.DataType),
                                         new SqlOutputParameterRepresentation<string>(nameof(OutputParamName.SerializedObjectString), Tables.Object.SerializedObjectString.DataType),
                                         new SqlOutputParameterRepresentation<byte[]>(nameof(OutputParamName.SerializedObjectBinary), Tables.Object.SerializedObjectBinary.DataType),
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
  @{nameof(InputParamName.SerializedObjectId)} AS nvarchar(450)
, @{nameof(InputParamName.ObjectAssemblyQualifiedNameWithoutVersion)} AS nvarchar(2000)
, @{nameof(InputParamName.ObjectAssemblyQualifiedNameWithVersion)} AS nvarchar(2000)
, @{nameof(InputParamName.TypeVersionMatchStrategy)} AS varchar(10)
, @{nameof(OutputParamName.SerializationConfigAssemblyQualifiedNameWithoutVersion)} AS nvarchar(2000) OUTPUT
, @{nameof(OutputParamName.SerializationKind)} AS varchar(50) OUTPUT
, @{nameof(OutputParamName.SerializationFormat)} AS varchar(50) OUTPUT
, @{nameof(OutputParamName.CompressionKind)} AS varchar(50) OUTPUT
, @{nameof(OutputParamName.UnregisteredTypeEncounteredStrategy)} AS varchar(50) OUTPUT
, @{nameof(OutputParamName.SerializedObjectString)} AS nvarchar(MAX) OUTPUT
, @{nameof(OutputParamName.SerializedObjectBinary)} AS varbinary(MAX) OUTPUT
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
	 , @{nameof(OutputParamName.SerializedObjectString)} = [SerializedObjectString]
	 , @{nameof(OutputParamName.SerializedObjectBinary)} = [SerializedObjectBinary]
	FROM [{streamName}].[Object]
	WHERE [SerializedObjectId] = @SerializedObjectId
	ORDER BY [Id] DESC
--check for record count and update contract to have an understanding of nothing found
	DECLARE @SerializationConfigTypeWithoutVersionId int
	SELECT 
		@SerializationConfigTypeWithoutVersionId = [{nameof(Tables.SerializerRepresentation.SerializationConfigurationTypeWithoutVersionId)}] 
	  , @{nameof(OutputParamName.SerializationKind)} = [{nameof(Tables.SerializerRepresentation.SerializationKind)}]
	  , @{nameof(OutputParamName.SerializationFormat)} = [{nameof(Tables.SerializerRepresentation.SerializationFormat)}]
	  , @{nameof(OutputParamName.CompressionKind)} = [{nameof(Tables.SerializerRepresentation.CompressionKind)}]
	  , @{nameof(OutputParamName.UnregisteredTypeEncounteredStrategy)} = [{nameof(Tables.SerializerRepresentation.UnregisteredTypeEncounteredStrategy)}]
	FROM [{streamName}].[SerializerRepresentation] WHERE [Id] = @SerializerRepresentationId

	SELECT @{nameof(OutputParamName.SerializationConfigAssemblyQualifiedNameWithoutVersion)} = [AssemblyQualifiedName] FROM [{streamName}].[TypeWithoutVersion] WHERE [Id] = @SerializationConfigTypeWithoutVersionId
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
