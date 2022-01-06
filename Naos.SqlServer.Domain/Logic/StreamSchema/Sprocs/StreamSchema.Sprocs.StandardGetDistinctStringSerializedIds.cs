// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamSchema.Sprocs.StandardGetDistinctStringSerializedIds.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Naos.CodeAnalysis.Recipes;
    using Naos.Database.Domain;
    using OBeautifulCode.Type;
    using static System.FormattableString;

    public static partial class StreamSchema
    {
        public partial class Sprocs
        {
            /// <summary>
            /// Stored procedure: StandardGetDistinctStringSerializedIds.
            /// </summary>
            public static class StandardGetDistinctStringSerializedIds
            {
                /// <summary>
                /// Gets the name.
                /// </summary>
                public static string Name => nameof(StandardGetDistinctStringSerializedIds);

                /// <summary>
                /// Input parameter names.
                /// </summary>
                [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Param", Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
                public enum InputParamName
                {
                    /// <summary>
                    /// The tag identifiers as CSV of tags to match.
                    /// </summary>
                    TagIdsCsv,

                    /// <summary>
                    /// The identifier assembly qualified name without version of the object type to consider a deprecated event.
                    /// </summary>
                    DeprecatedIdEventTypeWithoutVersionId,

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
                }

                /// <summary>
                /// Output parameter names.
                /// </summary>
                [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Param", Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
                public enum OutputParamName
                {
                    /// <summary>
                    /// The distinct string identifiers in XML of the discovered matching records.
                    /// </summary>
                    StringIdentifiersXml,
                }

                /// <summary>
                /// Builds the execute stored procedure operation.
                /// </summary>
                /// <param name="streamName">Name of the stream.</param>
                /// <param name="tagIdsCsv">The identifiers of the tags as CSV.</param>
                /// <param name="deprecatedEventType">The type of the event to consider a deprecated identifier.</param>
                /// <param name="identifierType">The identifier assembly qualified name with and without version.</param>
                /// <param name="objectType">The object assembly qualified name with and without version.</param>
                /// <param name="versionMatchStrategy">The type version match strategy.</param>
                /// <returns>Operation to execute stored procedure.</returns>
                public static ExecuteStoredProcedureOp BuildExecuteStoredProcedureOp(
                    string streamName,
                    string tagIdsCsv,
                    IdentifiedType deprecatedEventType,
                    IdentifiedType identifierType,
                    IdentifiedType objectType,
                    VersionMatchStrategy versionMatchStrategy)
                {
                    var sprocName = FormattableString.Invariant($"[{streamName}].[{nameof(StandardGetDistinctStringSerializedIds)}]");

                    var parameters = new List<ParameterDefinitionBase>()
                                     {
                                         new InputParameterDefinition<string>(nameof(InputParamName.TagIdsCsv), Tables.Record.StringSerializedId.SqlDataType, tagIdsCsv),
                                         new InputParameterDefinition<int?>(nameof(InputParamName.DeprecatedIdEventTypeWithoutVersionId), Tables.TypeWithoutVersion.Id.SqlDataType, deprecatedEventType?.IdWithoutVersion),
                                         new InputParameterDefinition<int?>(nameof(InputParamName.IdentifierTypeWithoutVersionIdQuery), Tables.TypeWithoutVersion.Id.SqlDataType, identifierType?.IdWithoutVersion),
                                         new InputParameterDefinition<int?>(nameof(InputParamName.IdentifierTypeWithVersionIdQuery), Tables.TypeWithVersion.Id.SqlDataType, identifierType?.IdWithVersion),
                                         new InputParameterDefinition<int?>(nameof(InputParamName.ObjectTypeWithoutVersionIdQuery), Tables.TypeWithoutVersion.Id.SqlDataType, objectType?.IdWithoutVersion),
                                         new InputParameterDefinition<int?>(nameof(InputParamName.ObjectTypeWithVersionIdQuery), Tables.TypeWithVersion.Id.SqlDataType, objectType?.IdWithVersion),
                                         new InputParameterDefinition<string>(nameof(InputParamName.VersionMatchStrategy), new StringSqlDataTypeRepresentation(false, 50), versionMatchStrategy.ToString()),
                                         new OutputParameterDefinition<string>(nameof(OutputParamName.StringIdentifiersXml), new XmlSqlDataTypeRepresentation()),
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
                    string streamName,
                    bool asAlter = false)
                {
                    var createOrModify = asAlter ? "ALTER" : "CREATE";

                    const string resultTableName = "ResultsTable";

                    var result = Invariant(
                        $@"
{createOrModify} PROCEDURE [{streamName}].[{StandardGetDistinctStringSerializedIds.Name}](
  @{InputParamName.TagIdsCsv} AS {Tables.Record.TagIdsCsv.SqlDataType.DeclarationInSqlSyntax}
, @{InputParamName.DeprecatedIdEventTypeWithoutVersionId} AS {Tables.TypeWithoutVersion.Id.SqlDataType.DeclarationInSqlSyntax}
, @{InputParamName.IdentifierTypeWithoutVersionIdQuery} AS {Tables.TypeWithoutVersion.Id.SqlDataType.DeclarationInSqlSyntax}
, @{InputParamName.IdentifierTypeWithVersionIdQuery} AS {Tables.TypeWithVersion.Id.SqlDataType.DeclarationInSqlSyntax}
, @{InputParamName.ObjectTypeWithoutVersionIdQuery} AS {Tables.TypeWithoutVersion.Id.SqlDataType.DeclarationInSqlSyntax}
, @{InputParamName.ObjectTypeWithVersionIdQuery} AS {Tables.TypeWithVersion.Id.SqlDataType.DeclarationInSqlSyntax}
, @{InputParamName.VersionMatchStrategy} AS {new StringSqlDataTypeRepresentation(false, 10).DeclarationInSqlSyntax}
, @{OutputParamName.StringIdentifiersXml} AS {new XmlSqlDataTypeRepresentation().DeclarationInSqlSyntax} OUTPUT
)
AS
BEGIN
    DECLARE @{resultTableName} TABLE ([{Tables.Record.StringSerializedId.Name}] {Tables.Record.StringSerializedId.SqlDataType.DeclarationInSqlSyntax} NULL)
    INSERT INTO @{resultTableName} ([{Tables.Record.StringSerializedId.Name}])
    SELECT DISTINCT r.{Tables.Record.StringSerializedId.Name}
	FROM [{streamName}].[{Tables.Record.Table.Name}] r WITH (NOLOCK)
    LEFT OUTER JOIN [{streamName}].[{Tables.Record.Table.Name}] r1 WITH (NOLOCK)
        ON r.[{Tables.Record.StringSerializedId.Name}] = r1.[{Tables.Record.StringSerializedId.Name}] AND r.[{Tables.Record.Id.Name}] < r1.[{Tables.Record.Id.Name}]
	WHERE
        r1.[{Tables.Record.Id.Name}] IS NULL
        AND
        (
            -- No Type filter at all
            (@{InputParamName.IdentifierTypeWithoutVersionIdQuery} IS NULL AND @{InputParamName.IdentifierTypeWithVersionIdQuery} IS NULL AND @{InputParamName.ObjectTypeWithoutVersionIdQuery} IS NULL AND @{InputParamName.ObjectTypeWithVersionIdQuery} IS NULL)
            OR
            -- Specific Only Id
            (@{InputParamName.VersionMatchStrategy} = '{VersionMatchStrategy.SpecifiedVersion}' AND @{InputParamName.IdentifierTypeWithVersionIdQuery} IS NOT NULL AND @{InputParamName.ObjectTypeWithVersionIdQuery} IS NULL AND r.{Tables.Record.IdentifierTypeWithVersionId.Name} = @{InputParamName.IdentifierTypeWithVersionIdQuery})
            OR
            -- Specific Only Object
            (@{InputParamName.VersionMatchStrategy} = '{VersionMatchStrategy.SpecifiedVersion}' AND @{InputParamName.ObjectTypeWithVersionIdQuery} IS NOT NULL AND @{InputParamName.IdentifierTypeWithVersionIdQuery} IS NULL AND r.{Tables.Record.ObjectTypeWithVersionId.Name} = @{InputParamName.ObjectTypeWithVersionIdQuery})
            OR
            -- Specific Both
            (@{InputParamName.VersionMatchStrategy} = '{VersionMatchStrategy.SpecifiedVersion}' AND @{InputParamName.IdentifierTypeWithVersionIdQuery} IS NOT NULL AND @{InputParamName.ObjectTypeWithVersionIdQuery} IS NOT NULL AND r.{Tables.Record.IdentifierTypeWithVersionId.Name} = @{InputParamName.IdentifierTypeWithVersionIdQuery} AND r.{Tables.Record.ObjectTypeWithVersionId.Name} = @{InputParamName.ObjectTypeWithVersionIdQuery})
            OR
            -- Any Only Id
            (@{InputParamName.VersionMatchStrategy} = '{VersionMatchStrategy.Any}' AND @{InputParamName.IdentifierTypeWithoutVersionIdQuery} IS NOT NULL AND @{InputParamName.ObjectTypeWithoutVersionIdQuery} IS NULL AND r.{Tables.Record.IdentifierTypeWithoutVersionId.Name} = @{InputParamName.IdentifierTypeWithoutVersionIdQuery})
            OR
            -- Any Only Object
            (@{InputParamName.VersionMatchStrategy} = '{VersionMatchStrategy.Any}' AND @{InputParamName.ObjectTypeWithoutVersionIdQuery} IS NOT NULL AND @{InputParamName.IdentifierTypeWithoutVersionIdQuery} IS NULL AND r.{Tables.Record.ObjectTypeWithoutVersionId.Name} = @{InputParamName.ObjectTypeWithoutVersionIdQuery})
            OR
            -- Any Both
            (@{InputParamName.VersionMatchStrategy} = '{VersionMatchStrategy.Any}' AND @{InputParamName.IdentifierTypeWithoutVersionIdQuery} IS NOT NULL AND @{InputParamName.ObjectTypeWithoutVersionIdQuery} IS NOT NULL AND r.{Tables.Record.IdentifierTypeWithoutVersionId.Name} = @{InputParamName.IdentifierTypeWithoutVersionIdQuery} AND r.{Tables.Record.ObjectTypeWithoutVersionId.Name} = @{InputParamName.ObjectTypeWithoutVersionIdQuery})
        )
        AND
        ((r.[{Tables.Record.ObjectTypeWithoutVersionId.Name}] <> @{InputParamName.DeprecatedIdEventTypeWithoutVersionId}) OR (@{InputParamName.DeprecatedIdEventTypeWithoutVersionId} IS NULL))

        SELECT @{OutputParamName.StringIdentifiersXml} = (SELECT
            ROW_NUMBER() OVER (ORDER BY e.[{Tables.Record.StringSerializedId.Name}], ISNULL(e.[{Tables.Record.StringSerializedId.Name}], '{TagConversionTool.NullCanaryValue}')) AS [@{TagConversionTool.TagEntryKeyAttributeName}],
	        e.{Tables.Record.StringSerializedId.Name} AS [@{TagConversionTool.TagEntryValueAttributeName}]
        FROM @{resultTableName} e
        FOR XML PATH ('{TagConversionTool.TagEntryElementName}'), ROOT('{TagConversionTool.TagSetElementName}'))
END

			");

                    return result;
                }
            }
        }
    }
}
