// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamSchema.Sprocs.GetHandlingStatuses.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System.Collections.Generic;
    using Naos.Database.Domain;
    using OBeautifulCode.Type;
    using static System.FormattableString;

    public static partial class StreamSchema
    {
        public static partial class Sprocs
        {
            /// <summary>
            /// Stored procedure: GetHandlingStatuses.
            /// </summary>
            public static class GetHandlingStatuses
            {
                /// <summary>
                /// Gets the name of the stored procedure.
                /// </summary>
                public static string Name => nameof(GetHandlingStatuses);

                /// <summary>
                /// Input parameter names.
                /// </summary>
                public enum InputParamName
                {
                    /// <summary>
                    /// The concern.
                    /// </summary>
                    Concern,

                    /// <summary>
                    /// The internal record identifiers as CSV.
                    /// </summary>
                    InternalRecordIdsCsv,

                    /// <summary>
                    /// The identifier type identifiers as CSV.
                    /// </summary>
                    IdentifierTypeIdsCsv,

                    /// <summary>
                    /// The object type identifiers as CSV.
                    /// </summary>
                    ObjectTypeIdsCsv,

                    /// <summary>
                    /// The string identifiers to match as XML
                    /// </summary>
                    StringIdentifiersXml,

                    /// <summary>
                    /// The tag identifiers as CSV.
                    /// </summary>
                    TagsIdsCsv,

                    /// <summary>
                    /// The <see cref="Naos.Database.Domain.TagMatchStrategy"/>.
                    /// </summary>
                    TagMatchStrategy,

                    /// <summary>
                    /// The <see cref="OBeautifulCode.Type.VersionMatchStrategy"/>.
                    /// </summary>
                    VersionMatchStrategy,
                }

                /// <summary>
                /// Output parameter names.
                /// </summary>
                public enum OutputParamName
                {
                    /// <summary>
                    /// The internal record identifier to handling status map as XML.
                    /// </summary>
                    RecordIdHandlingStatusXml,
                }

                /// <summary>
                /// Builds the execute stored procedure operation.
                /// </summary>
                /// <param name="streamName">Name of the stream.</param>
                /// <param name="concern">Handling concern.</param>
                /// <param name="internalRecordIdsCsv">The internal record ids CSV.</param>
                /// <param name="identifierTypeIdsCsv">The identifier type ids CSV.</param>
                /// <param name="objectTypeIdsCsv">The object type ids CSV.</param>
                /// <param name="stringIdsToMatchXml">The string ids to match XML.</param>
                /// <param name="tagIdsCsv">The tag ids CSV.</param>
                /// <param name="tagMatchStrategy">The tag match strategy.</param>
                /// <param name="versionMatchStrategy">The version match strategy.</param>
                /// <returns>Operation to execute stored procedure.</returns>
                public static ExecuteStoredProcedureOp BuildExecuteStoredProcedureOp(
                    string streamName,
                    string concern,
                    string internalRecordIdsCsv,
                    string identifierTypeIdsCsv,
                    string objectTypeIdsCsv,
                    string stringIdsToMatchXml,
                    string tagIdsCsv,
                    TagMatchStrategy tagMatchStrategy,
                    VersionMatchStrategy versionMatchStrategy)
                {
                    var sprocName = Invariant($"[{streamName}].[{nameof(GetHandlingStatuses)}]");
                    var parameters = new List<ParameterDefinitionBase>()
                                     {
                                         new InputParameterDefinition<string>(
                                             nameof(InputParamName.Concern),
                                             Tables.Handling.Concern.SqlDataType,
                                             concern),
                                         new InputParameterDefinition<string>(
                                             nameof(InputParamName.InternalRecordIdsCsv),
                                             new StringSqlDataTypeRepresentation(false, StringSqlDataTypeRepresentation.MaxNonUnicodeLengthConstant),
                                             internalRecordIdsCsv),
                                         new InputParameterDefinition<string>(
                                             nameof(InputParamName.IdentifierTypeIdsCsv),
                                             new StringSqlDataTypeRepresentation(false, StringSqlDataTypeRepresentation.MaxNonUnicodeLengthConstant),
                                             identifierTypeIdsCsv),
                                         new InputParameterDefinition<string>(
                                             nameof(InputParamName.ObjectTypeIdsCsv),
                                             new StringSqlDataTypeRepresentation(false, StringSqlDataTypeRepresentation.MaxNonUnicodeLengthConstant),
                                             objectTypeIdsCsv),
                                         new InputParameterDefinition<string>(
                                             nameof(InputParamName.StringIdentifiersXml),
                                             new XmlSqlDataTypeRepresentation(),
                                             stringIdsToMatchXml),
                                         new InputParameterDefinition<string>(
                                             nameof(InputParamName.TagsIdsCsv),
                                             new StringSqlDataTypeRepresentation(false, StringSqlDataTypeRepresentation.MaxNonUnicodeLengthConstant),
                                             tagIdsCsv),
                                         new InputParameterDefinition<string>(
                                             nameof(InputParamName.TagMatchStrategy),
                                             new StringSqlDataTypeRepresentation(false, 40),
                                             tagMatchStrategy.ToString()),
                                         new InputParameterDefinition<string>(
                                             nameof(InputParamName.VersionMatchStrategy),
                                             new StringSqlDataTypeRepresentation(false, 20),
                                             versionMatchStrategy.ToString()),
                                         new OutputParameterDefinition<string>(
                                             nameof(OutputParamName.RecordIdHandlingStatusXml),
                                             new XmlSqlDataTypeRepresentation()),
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
                    const string recordIdsToConsiderTable = "RecordIdsToConsiderTable";
                    const string identifierTypesTable = "IdTypeIdentifiersTable";
                    const string objectTypesTable = "ObjectTypeIdentifiersTable";
                    const string stringSerializedIdsTable = "StringSerializedIdentifiersTable";
                    const string tagIdsTable = "TagIdsTable";

                    var createOrModify = asAlter ? "ALTER" : "CREATE";
                    var result = Invariant(
                        $@"
{createOrModify} PROCEDURE [{streamName}].[{Name}](
    @{nameof(InputParamName.Concern)} {Tables.Handling.Concern.SqlDataType.DeclarationInSqlSyntax}
 ,  @{nameof(InputParamName.InternalRecordIdsCsv)} {new StringSqlDataTypeRepresentation(false, StringSqlDataTypeRepresentation.MaxNonUnicodeLengthConstant).DeclarationInSqlSyntax}
 ,  @{nameof(InputParamName.IdentifierTypeIdsCsv)} {new StringSqlDataTypeRepresentation(false, StringSqlDataTypeRepresentation.MaxNonUnicodeLengthConstant).DeclarationInSqlSyntax}
 ,  @{nameof(InputParamName.ObjectTypeIdsCsv)} {new StringSqlDataTypeRepresentation(false, StringSqlDataTypeRepresentation.MaxNonUnicodeLengthConstant).DeclarationInSqlSyntax}
 ,  @{nameof(InputParamName.StringIdentifiersXml)} {new XmlSqlDataTypeRepresentation().DeclarationInSqlSyntax}
 ,  @{nameof(InputParamName.TagsIdsCsv)} {new StringSqlDataTypeRepresentation(false, StringSqlDataTypeRepresentation.MaxNonUnicodeLengthConstant).DeclarationInSqlSyntax}
 ,  @{nameof(InputParamName.TagMatchStrategy)} {new StringSqlDataTypeRepresentation(false, 40).DeclarationInSqlSyntax}
 ,  @{nameof(InputParamName.VersionMatchStrategy)} {new StringSqlDataTypeRepresentation(false, 20).DeclarationInSqlSyntax}
 ,  @{nameof(OutputParamName.RecordIdHandlingStatusXml)} {new XmlSqlDataTypeRepresentation().DeclarationInSqlSyntax} OUTPUT
  )
AS
BEGIN
    DECLARE @{recordIdsToConsiderTable} TABLE([{Tables.Record.Id.Name}] {Tables.Record.Id.SqlDataType.DeclarationInSqlSyntax} NOT NULL)
    INSERT INTO @{recordIdsToConsiderTable} ([{Tables.Record.Id.Name}]) VALUES
    SELECT value FROM STRING_SPLIT(@{InputParamName.InternalRecordIdsCsv}, ',')

    DECLARE @{identifierTypesTable} TABLE([{Tables.TypeWithVersion.Id.Name}] {Tables.TypeWithVersion.Id.SqlDataType.DeclarationInSqlSyntax} NOT NULL)
    INSERT INTO @{identifierTypesTable} ([{Tables.TypeWithVersion.Id.Name}]) VALUES
    SELECT value FROM STRING_SPLIT(@{InputParamName.IdentifierTypeIdsCsv}, ',')

    DECLARE @{objectTypesTable} TABLE([{Tables.TypeWithVersion.Id.Name}] {Tables.TypeWithVersion.Id.SqlDataType.DeclarationInSqlSyntax} NOT NULL)
    INSERT INTO @{objectTypesTable} ([{Tables.TypeWithVersion.Id.Name}]) VALUES
    SELECT value FROM STRING_SPLIT(@{InputParamName.ObjectTypeIdsCsv}, ',')

    DECLARE @{stringSerializedIdsTable} TABLE([{Tables.Record.StringSerializedId.Name}] {Tables.Record.StringSerializedId.SqlDataType.DeclarationInSqlSyntax} NOT NULL, [{Tables.TypeWithVersion.Id.Name}] {Tables.TypeWithVersion.Id.SqlDataType.DeclarationInSqlSyntax} NOT NULL)
    INSERT INTO @{stringSerializedIdsTable} ([{Tables.Record.StringSerializedId.Name}], [{Tables.TypeWithVersion.Id.Name}]) VALUES
    SELECT @{TagConversionTool.TagEntryKeyAttributeName}, @{TagConversionTool.TagEntryValueAttributeName}
    FROM [{streamName}].[{Funcs.GetTagsTableVariableFromTagsXml.Name}](@{InputParamName.StringIdentifiersXml}) 

    DECLARE @{tagIdsTable} TABLE([{Tables.Tag.Id.Name}] {Tables.Tag.Id.SqlDataType.DeclarationInSqlSyntax} NOT NULL)
    INSERT INTO @{tagIdsTable} ([{Tables.Tag.Id.Name}]) VALUES
    SELECT value FROM STRING_SPLIT(@{InputParamName.TagsIdsCsv}, ',')

    INSERT INTO @{recordIdsToConsiderTable}
    SELECT DISTINCT r.[{Tables.Record.Id.Name}]
	FROM [{streamName}].[{Tables.Record.Table.Name}] (NOLOCK) r
    LEFT JOIN [{recordIdsToConsiderTable}] ir ON
        r.[{Tables.Record.Id.Name}] =  ir.[{Tables.Record.Id.Name}]
    LEFT JOIN [{identifierTypesTable}] itwith ON
        r.[{Tables.Record.IdentifierTypeWithVersionId.Name}] = itwith.[{Tables.TypeWithVersion.Id.Name}] AND @{InputParamName.VersionMatchStrategy} = '{VersionMatchStrategy.SpecifiedVersion}'
    LEFT JOIN [{identifierTypesTable}] itwithout ON
        r.[{Tables.Record.IdentifierTypeWithoutVersionId.Name}] = itwithout.[{Tables.TypeWithoutVersion.Id.Name}] AND @{InputParamName.VersionMatchStrategy} = '{VersionMatchStrategy.Any}'
    LEFT JOIN [{objectTypesTable}] otwith ON
        r.[{Tables.Record.ObjectTypeWithVersionId.Name}] = otwith.[{Tables.TypeWithVersion.Id.Name}] AND @{InputParamName.VersionMatchStrategy} = '{VersionMatchStrategy.SpecifiedVersion}'
    LEFT JOIN [{objectTypesTable}] otwithout ON
        r.[{Tables.Record.ObjectTypeWithoutVersionId.Name}] = otwithout.[{Tables.TypeWithoutVersion.Id.Name}] AND @{InputParamName.VersionMatchStrategy} = '{VersionMatchStrategy.Any}'
    LEFT JOIN [{Tables.Record.Table.Name}] rt (NOLOCK) ON
        EXISTS (SELECT value FROM STRING_SPLIT(r.[{Tables.Record.TagIdsCsv.Name}], ',') INTERSECT SELECT [{Tables.Tag.Id.Name}] FROM @{tagIdsTable})
    WHERE
        r.[{Tables.Record.Id.Name}] IS NOT NULL

    SELECT h.[{Tables.Handling.RecordId}], h.[{Tables.Handling.Status}]
    FROM [{streamName}].[{Tables.Handling.Table.Name}] h
	LEFT JOIN [{streamName}].[{Tables.Handling.Table.Name}] h1
	    ON h.[{Tables.Handling.RecordId.Name}] = h1.[{Tables.Handling.RecordId.Name}] AND h.[{Tables.Handling.Id.Name}] < h1.[{Tables.Handling.Id.Name}]
    ORDER BY [{Tables.Handling.Id.Name}]
    FOR XML PATH ('{TagConversionTool.TagEntryElementName}'), ROOT('{TagConversionTool.TagSetElementName}'))
END");

                    return result;
                }
            }
        }
    }
}

/*
declare @RecordId bigint
set @RecordId = 3
declare @Concern varchar(50)
set @Concern = 'Handle'
declare @RecordIdHandlingStatusXml nvarchar(max)


    DECLARE @RecordIdsTable TABLE ([RecordId] [BIGINT] NOT NULL)
	IF (@RecordId IS NOT NULL)
	BEGIN
		INSERT @RecordIdsTable ([RecordId]) VALUES (@RecordId)
	END

	--IF (@RecordId IS NOT NULL)
	--BEGIN
	--	INSERT @RecordIdsTable ([RecordId])
	--	SELECT 
	--END

    DECLARE @RecordIdsDistinctTable TABLE ([RecordId] [BIGINT] NOT NULL)
	INSERT @RecordIdsDistinctTable ([RecordId])
	SELECT DISTINCT [RecordId] FROM @RecordIdsTable

    DECLARE @ResultsTable TABLE ([InternalRecordId] [VARCHAR](50) NOT NULL, [HandlingStatus] [NVARCHAR](450) NOT NULL)
    INSERT INTO @ResultsTable ([InternalRecordId], [HandlingStatus])
    SELECT CONVERT(VARCHAR(50), r.[Id]), ISNULL(h.[Status], 'None')
	FROM [Stream082].[Record] r WITH (NOLOCK)
	INNER JOIN @RecordIdsDistinctTable i
		ON i.[RecordId] = r.[Id]
    LEFT OUTER JOIN [Stream082].[Handling] h WITH (NOLOCK)
        ON h.[RecordId] = r.[Id] AND h.Concern = @Concern
    LEFT OUTER JOIN [Stream082].[Handling] h1 WITH (NOLOCK)
        ON h.[RecordId] = h1.[RecordId]  AND h.Concern = @Concern AND r.[Id] < h1.[Id]

    SELECT @RecordIdHandlingStatusXml = (SELECT
			e.InternalRecordId AS [@Key],
	        e.HandlingStatus AS [@Value]
        FROM @ResultsTable e
        FOR XML PATH ('Tag'), ROOT('Tags'))
SELECT @RecordIdHandlingStatusXml


 */
