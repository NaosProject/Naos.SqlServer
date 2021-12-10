// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamSchema.Sprocs.GetLatestRecordMetadataById.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Naos.CodeAnalysis.Recipes;
    using Naos.Database.Domain;
    using OBeautifulCode.Type;
    using static System.FormattableString;

    public static partial class StreamSchema
    {
        public partial class Sprocs
        {
            /// <summary>
            /// Stored procedure: GetLatestRecordMetadataById.
            /// </summary>
            public static class GetLatestRecordMetadataById
            {
                /// <summary>
                /// Gets the name.
                /// </summary>
                public static string Name => nameof(GetLatestRecordMetadataById);

                /// <summary>
                /// Input parameter names.
                /// </summary>
                [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Param", Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
                public enum InputParamName
                {
                    /// <summary>
                    /// The serialized object identifier
                    /// </summary>
                    StringSerializedId,

                    /// <summary>
                    /// The identifier assembly qualified name without version
                    /// </summary>
                    IdentifierTypeWithoutVersionIdQuery,

                    /// <summary>
                    /// The identifier assembly qualified name with version
                    /// </summary>
                    IdentifierTypeWithVersionIdQuery,

                    /// <summary>
                    /// The object assembly qualified name without version
                    /// </summary>
                    ObjectTypeWithoutVersionIdQuery,

                    /// <summary>
                    /// The object assembly qualified name with version
                    /// </summary>
                    ObjectTypeWithVersionIdQuery,

                    /// <summary>
                    /// The type version match strategy
                    /// </summary>
                    VersionMatchStrategy,

                    /// <summary>
                    /// The existing record not encountered strategy.
                    /// </summary>
                    RecordNotFoundStrategy,
                }

                /// <summary>
                /// Output parameter names.
                /// </summary>
                [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Param", Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
                public enum OutputParamName
                {
                    /// <summary>
                    /// The internal record identifier.
                    /// </summary>
                    InternalRecordId,

                    /// <summary>
                    /// The serialization kind
                    /// </summary>
                    SerializerRepresentationId,

                    /// <summary>
                    /// The identifier assembly qualified name with version
                    /// </summary>
                    IdentifierTypeWithVersionId,

                    /// <summary>
                    /// The object assembly qualified name with version
                    /// </summary>
                    ObjectTypeWithVersionId,

                    /// <summary>
                    /// The record's date and time.
                    /// </summary>
                    RecordDateTime,

                    /// <summary>
                    /// The object's date and time if it was a <see cref="IHaveTimestampUtc"/>.
                    /// </summary>
                    ObjectDateTime,

                    /// <summary>
                    /// Tag ids as CSV.
                    /// </summary>
                    TagIdsCsv,
                }

                /// <summary>
                /// Builds the execute stored procedure operation.
                /// </summary>
                /// <param name="streamName">Name of the stream.</param>
                /// <param name="stringSerializedId">The serialized object identifier.</param>
                /// <param name="identifierType">The identifier assembly qualified name with and without version.</param>
                /// <param name="objectType">The object assembly qualified name with and without version.</param>
                /// <param name="versionMatchStrategy">The type version match strategy.</param>
                /// <param name="recordNotFoundStrategy">The existing record not encountered strategy.</param>
                /// <returns>Operation to execute stored procedure.</returns>
                public static ExecuteStoredProcedureOp BuildExecuteStoredProcedureOp(
                    string streamName,
                    string stringSerializedId,
                    IdentifiedType identifierType,
                    IdentifiedType objectType,
                    VersionMatchStrategy versionMatchStrategy,
                    RecordNotFoundStrategy recordNotFoundStrategy)
                {
                    var sprocName = Invariant($"[{streamName}].[{nameof(GetLatestRecordMetadataById)}]");

                    var parameters = new List<SqlParameterRepresentationBase>()
                    {
                        new SqlInputParameterRepresentation<string>(nameof(InputParamName.StringSerializedId), Tables.Record.StringSerializedId.SqlDataType, stringSerializedId),
                        new SqlInputParameterRepresentation<int?>(nameof(InputParamName.IdentifierTypeWithoutVersionIdQuery), Tables.TypeWithoutVersion.Id.SqlDataType, identifierType?.IdWithoutVersion),
                        new SqlInputParameterRepresentation<int?>(nameof(InputParamName.IdentifierTypeWithVersionIdQuery), Tables.TypeWithVersion.Id.SqlDataType, identifierType?.IdWithVersion),
                        new SqlInputParameterRepresentation<int?>(nameof(InputParamName.ObjectTypeWithoutVersionIdQuery), Tables.TypeWithoutVersion.Id.SqlDataType, objectType?.IdWithoutVersion),
                        new SqlInputParameterRepresentation<int?>(nameof(InputParamName.ObjectTypeWithVersionIdQuery), Tables.TypeWithVersion.Id.SqlDataType, objectType?.IdWithVersion),
                        new SqlInputParameterRepresentation<string>(nameof(InputParamName.VersionMatchStrategy), new StringSqlDataTypeRepresentation(false, 50), versionMatchStrategy.ToString()),
                        new SqlInputParameterRepresentation<string>(nameof(InputParamName.RecordNotFoundStrategy), new StringSqlDataTypeRepresentation(false, 50), recordNotFoundStrategy.ToString()),
                        new SqlOutputParameterRepresentation<long>(nameof(OutputParamName.InternalRecordId), Tables.Record.Id.SqlDataType),
                        new SqlOutputParameterRepresentation<int>(nameof(OutputParamName.SerializerRepresentationId), Tables.SerializerRepresentation.Id.SqlDataType),
                        new SqlOutputParameterRepresentation<int>(nameof(OutputParamName.IdentifierTypeWithVersionId), Tables.TypeWithVersion.Id.SqlDataType),
                        new SqlOutputParameterRepresentation<int>(nameof(OutputParamName.ObjectTypeWithVersionId), Tables.TypeWithVersion.Id.SqlDataType),
                        new SqlOutputParameterRepresentation<DateTime>(nameof(OutputParamName.RecordDateTime), Tables.Record.RecordCreatedUtc.SqlDataType),
                        new SqlOutputParameterRepresentation<DateTime?>(nameof(OutputParamName.ObjectDateTime), Tables.Record.ObjectDateTimeUtc.SqlDataType),
                        new SqlOutputParameterRepresentation<string>(nameof(OutputParamName.TagIdsCsv), Tables.Record.TagIdsCsv.SqlDataType),
                    };

                    var result = new ExecuteStoredProcedureOp(sprocName, parameters);

                    return result;
                }

                /// <summary>
                /// Builds the creation script for put sproc.
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
{createOrModify} PROCEDURE [{streamName}].[{GetLatestRecordMetadataById.Name}](
  @{InputParamName.StringSerializedId} AS {Tables.Record.StringSerializedId.SqlDataType.DeclarationInSqlSyntax}
, @{InputParamName.IdentifierTypeWithoutVersionIdQuery} AS {Tables.TypeWithoutVersion.Id.SqlDataType.DeclarationInSqlSyntax}
, @{InputParamName.IdentifierTypeWithVersionIdQuery} AS {Tables.TypeWithVersion.Id.SqlDataType.DeclarationInSqlSyntax}
, @{InputParamName.ObjectTypeWithoutVersionIdQuery} AS {Tables.TypeWithoutVersion.Id.SqlDataType.DeclarationInSqlSyntax}
, @{InputParamName.ObjectTypeWithVersionIdQuery} AS {Tables.TypeWithVersion.Id.SqlDataType.DeclarationInSqlSyntax}
, @{InputParamName.VersionMatchStrategy} AS varchar(10)
, @{InputParamName.RecordNotFoundStrategy} AS varchar(50)
, @{OutputParamName.InternalRecordId} AS {Tables.Record.Id.SqlDataType.DeclarationInSqlSyntax} OUTPUT
, @{OutputParamName.SerializerRepresentationId} AS {Tables.SerializerRepresentation.Id.SqlDataType.DeclarationInSqlSyntax} OUTPUT
, @{OutputParamName.IdentifierTypeWithVersionId} AS {Tables.TypeWithVersion.Id.SqlDataType.DeclarationInSqlSyntax} OUTPUT
, @{OutputParamName.ObjectTypeWithVersionId} AS {Tables.TypeWithVersion.Id.SqlDataType.DeclarationInSqlSyntax} OUTPUT
, @{OutputParamName.ObjectDateTime} AS {Tables.Record.ObjectDateTimeUtc.SqlDataType.DeclarationInSqlSyntax} OUTPUT
, @{OutputParamName.RecordDateTime} AS {Tables.Record.RecordCreatedUtc.SqlDataType.DeclarationInSqlSyntax} OUTPUT
, @{OutputParamName.TagIdsCsv} AS {Tables.Record.TagIdsCsv.SqlDataType.DeclarationInSqlSyntax} OUTPUT
)
AS
BEGIN
    SELECT TOP 1
	   @{OutputParamName.SerializerRepresentationId} = [{Tables.Record.SerializerRepresentationId.Name}]
	 , @{OutputParamName.IdentifierTypeWithVersionId} = [{Tables.Record.IdentifierTypeWithVersionId.Name}]
	 , @{OutputParamName.ObjectTypeWithVersionId} = [{Tables.Record.ObjectTypeWithVersionId.Name}]
	 , @{OutputParamName.InternalRecordId} = [{Tables.Record.Id.Name}]
	 , @{OutputParamName.TagIdsCsv} = [{Tables.Record.TagIdsCsv.Name}]
	 , @{OutputParamName.RecordDateTime} = [{Tables.Record.RecordCreatedUtc.Name}]
	 , @{OutputParamName.ObjectDateTime} = [{Tables.Record.ObjectDateTimeUtc.Name}]
	FROM [{streamName}].[{Tables.Record.Table.Name}] WITH (NOLOCK)
	WHERE [{Tables.Record.StringSerializedId.Name}] = @{InputParamName.StringSerializedId}
    AND (
            -- No Type filter at all
            (@{InputParamName.IdentifierTypeWithoutVersionIdQuery} IS NULL AND @{InputParamName.IdentifierTypeWithVersionIdQuery} IS NULL AND @{InputParamName.ObjectTypeWithoutVersionIdQuery} IS NULL AND @{InputParamName.ObjectTypeWithVersionIdQuery} IS NULL)
            OR
            -- Specific Only Id
            (@{InputParamName.VersionMatchStrategy} = '{VersionMatchStrategy.SpecifiedVersion}' AND @{InputParamName.IdentifierTypeWithVersionIdQuery} IS NOT NULL AND @{InputParamName.ObjectTypeWithVersionIdQuery} IS NULL AND [{Tables.Record.IdentifierTypeWithVersionId.Name}] = @{InputParamName.IdentifierTypeWithVersionIdQuery})
            OR
            -- Specific Only Object
            (@{InputParamName.VersionMatchStrategy} = '{VersionMatchStrategy.SpecifiedVersion}' AND @{InputParamName.ObjectTypeWithVersionIdQuery} IS NOT NULL AND @{InputParamName.IdentifierTypeWithVersionIdQuery} IS NULL AND [{Tables.Record.ObjectTypeWithVersionId.Name}] = @{InputParamName.ObjectTypeWithVersionIdQuery})
            OR
            -- Specific Both
            (@{InputParamName.VersionMatchStrategy} = '{VersionMatchStrategy.SpecifiedVersion}' AND @{InputParamName.IdentifierTypeWithVersionIdQuery} IS NOT NULL AND @{InputParamName.ObjectTypeWithVersionIdQuery} IS NOT NULL AND [{Tables.Record.IdentifierTypeWithVersionId.Name}] = @{InputParamName.IdentifierTypeWithVersionIdQuery} AND [{Tables.Record.ObjectTypeWithVersionId.Name}] = @{InputParamName.ObjectTypeWithVersionIdQuery})
            OR
            -- Any Only Id
            (@{InputParamName.VersionMatchStrategy} = '{VersionMatchStrategy.Any}' AND @{InputParamName.IdentifierTypeWithoutVersionIdQuery} IS NOT NULL AND @{InputParamName.ObjectTypeWithoutVersionIdQuery} IS NULL AND [{Tables.Record.IdentifierTypeWithoutVersionId.Name}] = @{InputParamName.IdentifierTypeWithoutVersionIdQuery})
            OR
            -- Any Only Object
            (@{InputParamName.VersionMatchStrategy} = '{VersionMatchStrategy.Any}' AND @{InputParamName.ObjectTypeWithoutVersionIdQuery} IS NOT NULL AND @{InputParamName.IdentifierTypeWithoutVersionIdQuery} IS NULL AND [{Tables.Record.ObjectTypeWithoutVersionId.Name}] = @{InputParamName.ObjectTypeWithoutVersionIdQuery})
            OR
            -- Any Both
            (@{InputParamName.VersionMatchStrategy} = '{VersionMatchStrategy.Any}' AND @{InputParamName.IdentifierTypeWithoutVersionIdQuery} IS NOT NULL AND @{InputParamName.ObjectTypeWithoutVersionIdQuery} IS NOT NULL AND [{Tables.Record.IdentifierTypeWithoutVersionId.Name}] = @{InputParamName.IdentifierTypeWithoutVersionIdQuery} AND [{Tables.Record.ObjectTypeWithoutVersionId.Name}] = @{InputParamName.ObjectTypeWithoutVersionIdQuery})
        )
	ORDER BY [{Tables.Record.Id.Name}] DESC

    IF (@{OutputParamName.InternalRecordId} IS NULL)
    BEGIN
        SET @{OutputParamName.SerializerRepresentationId} = {Tables.SerializerRepresentation.NullId}
	    SET @{OutputParamName.IdentifierTypeWithVersionId} = {Tables.TypeWithVersion.NullId}
	    SET @{OutputParamName.ObjectTypeWithVersionId} = {Tables.TypeWithVersion.NullId}
	    SET @{OutputParamName.InternalRecordId} = {Tables.Record.NullId}
	    SET @{OutputParamName.ObjectDateTime} = NULL
	    SET @{OutputParamName.TagIdsCsv} = NULL
	    SET @{OutputParamName.RecordDateTime} = GETUTCDATE()
    END
END
			");

                    return result;
                }
            }
        }
    }
}
