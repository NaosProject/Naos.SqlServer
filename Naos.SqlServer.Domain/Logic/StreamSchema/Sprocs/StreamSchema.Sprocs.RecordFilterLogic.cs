// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamSchema.Sprocs.RecordFilterLogic.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System.Diagnostics.CodeAnalysis;
    using Naos.CodeAnalysis.Recipes;
    using Naos.Database.Domain;
    using OBeautifulCode.Type;
    using static System.FormattableString;

    public static partial class StreamSchema
    {
        public static partial class Sprocs
        {
            /// <summary>
            /// Share logic for record filter use.
            /// </summary>
            public static class RecordFilterLogic
            {
                /// <summary>
                /// Shared input parameter names.
                /// </summary>
                [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Param", Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
                public enum InputParamName
                {
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
                    /// The string identifiers to match as XML (key is string identifier and value is the appropriate type identifier per the <see cref="OBeautifulCode.Type.VersionMatchStrategy"/>).
                    /// </summary>
                    StringIdentifiersXml,

                    /// <summary>
                    /// The record tag identifiers as CSV.
                    /// </summary>
                    TagIdsToMatchCsv,

                    /// <summary>
                    /// The handling tag identifiers as CSV.
                    /// </summary>
                    HandlingTagIdsToMatchCsv,

                    /// <summary>
                    /// The <see cref="RecordFilter.TagMatchStrategy"/>.
                    /// </summary>
                    TagMatchStrategy,

                    /// <summary>
                    /// The <see cref="RecordFilter.VersionMatchStrategy"/>.
                    /// </summary>
                    VersionMatchStrategy,

                    /// <summary>
                    /// The deprecated identifier event type identifiers as CSV.
                    /// </summary>
                    DeprecatedIdEventTypeIdsCsv,

                    /// <summary>
                    /// The <see cref="RecordsToFilterCriteria.RecordsToFilterSelectionStrategy"/>.
                    /// </summary>
                    RecordsToFilterSelectionStrategy,

                    /// <summary>
                    /// The <see cref="RecordsToFilterCriteria.VersionMatchStrategy"/>.
                    /// </summary>
                    RecordsToFilterVersionMatchStrategy,
                }

                /// <summary>
                /// Builds the stored procedure snippet to be introduced to populate a table of internal record identifiers based on the shared <see cref="InputParamName"/>'s which are a decomposed <see cref="RecordFilter"/> converted to a <see cref="RecordFilterConvertedForStoredProcedure"/>.
                /// </summary>
                /// <param name="streamName">Name of the stream.</param>
                /// <param name="recordIdsToConsiderTable">The record ids to consider table.</param>
                /// <param name="includeHandlingTags">A value indicating whether or not to add handling tag logic.</param>
                /// <param name="includeRecordsToFilterCriteria">A value indicating whether or not to include logic to honor a specified <see cref="RecordsToFilterCriteria"/>.</param>
                /// <returns>Injection SQL for filtering.</returns>
                public static string BuildRecordFilterToBuildRecordsToConsiderTable(
                    string streamName,
                    string recordIdsToConsiderTable,
                    bool includeHandlingTags,
                    bool includeRecordsToFilterCriteria)
                {
                    const string recordTagIdsTable = "RecordTagIdsTable";
                    const string handlingTagIdsTable = "HandlingTagIdsTable";
                    const string isRecordIdsToConsiderTableInitializedBit = "IsRecordIdsToConsiderTableInitialized";
                    var handlingTagMixIn = !includeHandlingTags
                        ? string.Empty
                        : Invariant(
                            $@"
    -----------------------------------------------------------------
    -- BEGIN APPLY HANDLING TAG FILTER
    IF (@{InputParamName.HandlingTagIdsToMatchCsv} IS NOT NULL)
    BEGIN
        DECLARE @{handlingTagIdsTable} TABLE([{Tables.Tag.Id.Name}] {new BigIntSqlDataTypeRepresentation().DeclarationInSqlSyntax} NOT NULL)
        INSERT INTO @{handlingTagIdsTable} ([{Tables.Tag.Id.Name}])
        SELECT VALUE FROM STRING_SPLIT(@{InputParamName.HandlingTagIdsToMatchCsv}, ',')

        DECLARE @HandlingTagCount INT
        SELECT @HandlingTagCount = COUNT([{Tables.Tag.Id.Name}]) FROM @{handlingTagIdsTable}

        IF (@{isRecordIdsToConsiderTableInitializedBit} = 1)
        BEGIN
          DELETE FROM @{recordIdsToConsiderTable} WHERE [{Tables.Record.Id.Name}] NOT IN
            (
                SELECT DISTINCT h.[{Tables.Handling.RecordId.Name}] AS [{Tables.Record.Id.Name}]
                FROM [{streamName}].[{Tables.HandlingTag.Table.Name}] ht WITH (NOLOCK)
                INNER JOIN @{handlingTagIdsTable} tids ON tids.[{Tables.Tag.Id.Name}] = ht.[{Tables.HandlingTag.TagId.Name}]
                INNER JOIN [{streamName}].[{Tables.Handling.Table.Name}] h WITH (NOLOCK) ON h.[{Tables.Handling.Id.Name}] = ht.[{Tables.HandlingTag.HandlingId.Name}]
                GROUP BY h.[{Tables.Handling.Id.Name}], h.[{Tables.Handling.RecordId.Name}]
                HAVING COUNT(h.[{Tables.Handling.RecordId.Name}]) = @HandlingTagCount
            )
        END
        ELSE
        BEGIN
            SET @{isRecordIdsToConsiderTableInitializedBit} = 1
            INSERT INTO @{recordIdsToConsiderTable} ([{Tables.Record.Id.Name}])
            (
                SELECT DISTINCT h.[{Tables.Handling.RecordId.Name}] AS [{Tables.Record.Id.Name}]
                FROM [{streamName}].[{Tables.HandlingTag.Table.Name}] ht WITH (NOLOCK)
                INNER JOIN @{handlingTagIdsTable} tids ON tids.[{Tables.Tag.Id.Name}] = ht.[{Tables.HandlingTag.TagId.Name}]
                INNER JOIN [{streamName}].[{Tables.Handling.Table.Name}] h WITH (NOLOCK) ON h.[{Tables.Handling.Id.Name}] = ht.[{Tables.HandlingTag.HandlingId.Name}]
                GROUP BY h.[{Tables.Handling.Id.Name}], h.[{Tables.Handling.RecordId.Name}]
                HAVING COUNT(h.[{Tables.Handling.RecordId.Name}]) = @HandlingTagCount
            )
        END
    END
    -- END APPLY HANDLING TAG FILTER
    -----------------------------------------------------------------
");

                    var recordsToFilterCriteriaMixIn = !includeRecordsToFilterCriteria
                        ? string.Empty
                        : Invariant(
                            $@"
    -----------------------------------------------------------------
    -- BEGIN APPLY RECORDS TO FILTER CRITERIA
    -- The .NET client guarantees that the only other supported value is '{RecordsToFilterSelectionStrategy.All}'.
    -- In that case, there's nothing to do.  If @{isRecordIdsToConsiderTableInitializedBit} = 0, the remaining
    -- filters consider the universe of records available for filtering to be all records.
    IF (@{InputParamName.RecordsToFilterSelectionStrategy} = '{RecordsToFilterSelectionStrategy.LatestById}')
    BEGIN
        INSERT INTO @{recordIdsToConsiderTable}
        SELECT Max(r.[{Tables.Record.Id.Name}])
        FROM [{streamName}].[{Tables.Record.Table.Name}] r WITH (NOLOCK)
        GROUP BY r.[{Tables.Record.StringSerializedId.Name}]

        SET @{isRecordIdsToConsiderTableInitializedBit} = 1
    END
    ELSE
    BEGIN
        IF (@{InputParamName.RecordsToFilterSelectionStrategy} = '{RecordsToFilterSelectionStrategy.LatestByIdAndObjectType}')
        BEGIN
            IF (@{InputParamName.VersionMatchStrategy} = '{VersionMatchStrategy.Any}')
            BEGIN
                INSERT INTO @{recordIdsToConsiderTable}
                SELECT Max(r.[{Tables.Record.Id.Name}])
                FROM [{streamName}].[{Tables.Record.Table.Name}] r WITH (NOLOCK)
                GROUP BY r.[{Tables.Record.StringSerializedId.Name}], r.[{Tables.Record.ObjectTypeWithoutVersionId.Name}]
            END
            ELSE
            BEGIN
                INSERT INTO @{recordIdsToConsiderTable}
                SELECT Max(r.[{Tables.Record.Id.Name}])
                FROM [{streamName}].[{Tables.Record.Table.Name}] r WITH (NOLOCK)
                GROUP BY r.[{Tables.Record.StringSerializedId.Name}], r.[{Tables.Record.ObjectTypeWithVersionId.Name}]
            END

            SET @{isRecordIdsToConsiderTableInitializedBit} = 1
        END
    END

    -- END APPLY RECORDS TO FILTER SELECTION STRATEGY
    -----------------------------------------------------------------
");

                    var result = Invariant(
                        $@"
    --------------------------------------------------------------------------------------
    -- BEGIN RECORD FILTER INJECTED SQL - CHANGE IN BUILDING CODE AND PATCH SPROCS      --
    --------------------------------------------------------------------------------------
    /*
    TODO:
       * Consider branch logic for common combos that could be consolidated
       * Consider limiting queries when @{isRecordIdsToConsiderTableInitializedBit} to only Ids in @RecordIdsToConsider, how to deal with delete?
       * Consider copying values of @RecordIdsToConsider to a new table and truncating and overwriting (makes limiting easier), what is perf?
       * remove distinct where possible
    */
    -----------------------------------------------------------------

    -----------------------------------------------------------------
    -- BEGIN PARAM CHECK
    -- Skipping these checks for performance purposes
    -- The .NET client will guarantee non-empty CSV, non-empty XML, and supported Enum values.
    -- Further, .NET client guarantees that CSVs contain distinct values.
    --IF (@{InputParamName.InternalRecordIdsCsv} = '')
    --BEGIN
    --  RAISERROR (15600,-1,-1, '{nameof(RecordFilterLogic)}.@{InputParamName.InternalRecordIdsCsv}')
    --END
    --IF (@{InputParamName.IdentifierTypeIdsCsv} = '')
    --BEGIN
    --  RAISERROR (15600,-1,-1, '{nameof(RecordFilterLogic)}.@{InputParamName.IdentifierTypeIdsCsv}')
    --END
    --IF (@{InputParamName.ObjectTypeIdsCsv} = '')
    --BEGIN
    --  RAISERROR (15600,-1,-1, '{nameof(RecordFilterLogic)}.@{InputParamName.ObjectTypeIdsCsv}')
    --END
    -- IF (@{InputParamName.StringIdentifiersXml} IS NOT NULL) and (@{InputParamName.StringIdentifiersXml}.EXISTS('*') = 0) -- DOES EXISTS('*') do slow XML stuff?
    -- BEGIN
        -- RAISERROR (15600,-1,-1, '{nameof(RecordFilterLogic)}.@{InputParamName.StringIdentifiersXml}')
    -- END
    --IF (@{InputParamName.TagIdsToMatchCsv} = '')
    --BEGIN
    --  RAISERROR (15600,-1,-1, '{nameof(RecordFilterLogic)}.@{InputParamName.TagIdsToMatchCsv}')
    --END
    --IF (@{InputParamName.HandlingTagIdsToMatchCsv} = '')
    --BEGIN
    --  RAISERROR (15600,-1,-1, '{nameof(RecordFilterLogic)}.@{InputParamName.HandlingTagIdsToMatchCsv}')
    --END
    --IF (@{InputParamName.TagMatchStrategy} <> '{TagMatchStrategy.RecordContainsAllQueryTags}')
    --BEGIN
    --  RAISERROR (15600,-1,-1, '{nameof(RecordFilterLogic)}.@{InputParamName.TagMatchStrategy}')
    --END
    --IF (@{InputParamName.VersionMatchStrategy} <> '{VersionMatchStrategy.SpecifiedVersion}' AND @{InputParamName.VersionMatchStrategy} <> '{VersionMatchStrategy.Any}')
    --BEGIN
    --  RAISERROR (15600,-1,-1, '{nameof(RecordFilterLogic)}.@{InputParamName.VersionMatchStrategy}')
    --END
    --IF (@{InputParamName.DeprecatedIdEventTypeIdsCsv} = '')
    --BEGIN
    --  RAISERROR (15600,-1,-1, '{nameof(RecordFilterLogic)}.@{InputParamName.DeprecatedIdEventTypeIdsCsv}')
    --END
    --IF (@{InputParamName.RecordsToFilterSelectionStrategy} <> '{RecordsToFilterSelectionStrategy.All}' AND @{InputParamName.RecordsToFilterSelectionStrategy} <> '{RecordsToFilterSelectionStrategy.LatestById}' AND @{InputParamName.RecordsToFilterSelectionStrategy} <> '{RecordsToFilterSelectionStrategy.LatestByIdAndObjectType}')
    --BEGIN
    --  RAISERROR (15600,-1,-1, '{nameof(RecordFilterLogic)}.@{InputParamName.RecordsToFilterSelectionStrategy}')
    --END
    --IF (@{InputParamName.RecordsToFilterVersionMatchStrategy} <> '{VersionMatchStrategy.SpecifiedVersion}' AND @{InputParamName.VersionMatchStrategy} <> '{VersionMatchStrategy.Any}')
    --BEGIN
    --  RAISERROR (15600,-1,-1, '{nameof(RecordFilterLogic)}.@{InputParamName.RecordsToFilterVersionMatchStrategy}')
    --END
    -- END PARAM CHECK
    -----------------------------------------------------------------

    -----------------------------------------------------------------
    -- BEGIN DECLARE ASSETS
    DECLARE @{recordIdsToConsiderTable} TABLE([{Tables.Record.Id.Name}] {Tables.Record.Id.SqlDataType.DeclarationInSqlSyntax} NOT NULL)
    DECLARE @{isRecordIdsToConsiderTableInitializedBit} BIT
    SET @{isRecordIdsToConsiderTableInitializedBit} = 0
    -- END DECLARE ASSETS
    -----------------------------------------------------------------

{recordsToFilterCriteriaMixIn}

    -----------------------------------------------------------------
    -- BEGIN APPLY INTERNAL RECORD ID FILTER
    IF (@{InputParamName.InternalRecordIdsCsv} IS NOT NULL)
    BEGIN
        IF (@{isRecordIdsToConsiderTableInitializedBit} = 1)
        BEGIN
            DELETE FROM @{recordIdsToConsiderTable} WHERE [{Tables.Record.Id.Name}] NOT IN
            (
                SELECT DISTINCT VALUE FROM STRING_SPLIT(@{InputParamName.InternalRecordIdsCsv}, ',')
            )
        END
        ELSE
        BEGIN
            SET @{isRecordIdsToConsiderTableInitializedBit} = 1
            INSERT INTO @{recordIdsToConsiderTable} ([{Tables.Record.Id.Name}])
            (
                SELECT DISTINCT VALUE FROM STRING_SPLIT(@{InputParamName.InternalRecordIdsCsv}, ',')
            )
        END
    END
    -- END APPLY INTERNAL RECORD ID FILTER
    -----------------------------------------------------------------

    -----------------------------------------------------------------
    -- BEGIN APPLY STRING SERIALIZED ID FILTER
    IF (@{InputParamName.StringIdentifiersXml} IS NOT NULL)
    BEGIN
        IF (@{isRecordIdsToConsiderTableInitializedBit} = 1)
        BEGIN
            IF (@{InputParamName.VersionMatchStrategy} = '{VersionMatchStrategy.Any}')
            BEGIN
                DELETE FROM @{recordIdsToConsiderTable} WHERE [{Tables.Record.Id.Name}] NOT IN
                (
                    SELECT DISTINCT r.[{Tables.Record.Id.Name}]
                    FROM [{streamName}].[{Tables.Record.Table.Name}] r WITH (NOLOCK)
                    INNER JOIN [{streamName}].[{Funcs.GetTagsTableVariableFromTagsXml.Name}](@{InputParamName.StringIdentifiersXml}) i
                    ON r.[{Tables.Record.StringSerializedId.Name}] = [{streamName}].[{Funcs.AdjustForPutStringSerializedId.Name}](i.[{Tables.Tag.TagKey.Name}])
                    AND r.[{Tables.Record.IdentifierTypeWithoutVersionId.Name}] = i.[{Tables.Tag.TagValue.Name}]
                )
            END
            ELSE
            BEGIN
                DELETE FROM @{recordIdsToConsiderTable} WHERE [{Tables.Record.Id.Name}] NOT IN
                (
                    SELECT DISTINCT r.[{Tables.Record.Id.Name}]
                    FROM [{streamName}].[{Tables.Record.Table.Name}] r WITH (NOLOCK)
                    INNER JOIN [{streamName}].[{Funcs.GetTagsTableVariableFromTagsXml.Name}](@{InputParamName.StringIdentifiersXml}) i
                    ON r.[{Tables.Record.StringSerializedId.Name}] = [{streamName}].[{Funcs.AdjustForPutStringSerializedId.Name}](i.[{Tables.Tag.TagKey.Name}])
                    AND r.[{Tables.Record.IdentifierTypeWithVersionId.Name}] = i.[{Tables.Tag.TagValue.Name}]
                )
            END
        END
        ELSE
        BEGIN
            SET @{isRecordIdsToConsiderTableInitializedBit} = 1
            IF (@{InputParamName.VersionMatchStrategy} = '{VersionMatchStrategy.Any}')
            BEGIN
                INSERT INTO @{recordIdsToConsiderTable} ([{Tables.Record.Id.Name}])
                (
                    SELECT DISTINCT r.[{Tables.Record.Id.Name}]
                    FROM [{streamName}].[{Tables.Record.Table.Name}] r WITH (NOLOCK)
                    INNER JOIN [{streamName}].[{Funcs.GetTagsTableVariableFromTagsXml.Name}](@{InputParamName.StringIdentifiersXml}) i
                    ON r.[{Tables.Record.StringSerializedId.Name}] = [{streamName}].[{Funcs.AdjustForPutStringSerializedId.Name}](i.[{Tables.Tag.TagKey.Name}])
                    AND r.[{Tables.Record.IdentifierTypeWithoutVersionId.Name}] = i.[{Tables.Tag.TagValue.Name}]
                )
            END
            ELSE
            BEGIN
                INSERT INTO @{recordIdsToConsiderTable} ([{Tables.Record.Id.Name}])
                (
                    SELECT DISTINCT r.[{Tables.Record.Id.Name}]
                    FROM [{streamName}].[{Tables.Record.Table.Name}] r WITH (NOLOCK)
                    INNER JOIN [{streamName}].[{Funcs.GetTagsTableVariableFromTagsXml.Name}](@{InputParamName.StringIdentifiersXml}) i
                    ON r.[{Tables.Record.StringSerializedId.Name}] = [{streamName}].[{Funcs.AdjustForPutStringSerializedId.Name}](i.[{Tables.Tag.TagKey.Name}])
                    AND r.[{Tables.Record.IdentifierTypeWithVersionId.Name}] = i.[{Tables.Tag.TagValue.Name}]
                )
            END
        END
    END
    -- END APPLY STRING SERIALIZED ID FILTER
    -----------------------------------------------------------------

    -----------------------------------------------------------------
    -- BEGIN APPLY ID TYPE FILTER
    IF (@{InputParamName.IdentifierTypeIdsCsv} IS NOT NULL)
    BEGIN
        IF (@{isRecordIdsToConsiderTableInitializedBit} = 1)
        BEGIN
            IF (@{InputParamName.VersionMatchStrategy} = '{VersionMatchStrategy.Any}')
            BEGIN
                DELETE FROM @{recordIdsToConsiderTable} WHERE [{Tables.Record.Id.Name}] NOT IN
                (
                    SELECT DISTINCT r.[{Tables.Record.Id.Name}]
                    FROM [{streamName}].[{Tables.Record.Table.Name}] r WITH (NOLOCK)
                    INNER JOIN STRING_SPLIT(@{InputParamName.IdentifierTypeIdsCsv}, ',') i
                    ON r.[{Tables.Record.IdentifierTypeWithoutVersionId.Name}] = i.VALUE
                )
            END
            ELSE
            BEGIN
                DELETE FROM @{recordIdsToConsiderTable} WHERE [{Tables.Record.Id.Name}] NOT IN
                (
                    SELECT DISTINCT r.[{Tables.Record.Id.Name}]
                    FROM [{streamName}].[{Tables.Record.Table.Name}] r WITH (NOLOCK)
                    INNER JOIN STRING_SPLIT(@{InputParamName.IdentifierTypeIdsCsv}, ',') i
                    ON r.[{Tables.Record.IdentifierTypeWithVersionId.Name}] = i.VALUE
                )
            END
        END
        ELSE
        BEGIN
            SET @{isRecordIdsToConsiderTableInitializedBit} = 1
            IF (@{InputParamName.VersionMatchStrategy} = '{VersionMatchStrategy.Any}')
            BEGIN
                INSERT INTO @{recordIdsToConsiderTable} ([{Tables.Record.Id.Name}])
                (
                    SELECT DISTINCT r.[{Tables.Record.Id.Name}]
                    FROM [{streamName}].[{Tables.Record.Table.Name}] r WITH (NOLOCK)
                    INNER JOIN STRING_SPLIT(@{InputParamName.IdentifierTypeIdsCsv}, ',') i
                    ON r.[{Tables.Record.IdentifierTypeWithoutVersionId.Name}] = i.VALUE
                )
            END
            ELSE
            BEGIN
                INSERT INTO @{recordIdsToConsiderTable} ([{Tables.Record.Id.Name}])
                (
                    SELECT DISTINCT r.[{Tables.Record.Id.Name}]
                    FROM [{streamName}].[{Tables.Record.Table.Name}] r WITH (NOLOCK)
                    INNER JOIN STRING_SPLIT(@{InputParamName.IdentifierTypeIdsCsv}, ',') i
                    ON r.[{Tables.Record.IdentifierTypeWithVersionId.Name}] = i.VALUE
                )
            END
        END
    END
    -- END APPLY ID TYPE FILTER
    -----------------------------------------------------------------

    -----------------------------------------------------------------
    -- BEGIN APPLY OBJECT TYPE FILTER
    IF (@{InputParamName.ObjectTypeIdsCsv} IS NOT NULL)
    BEGIN
        IF (@{isRecordIdsToConsiderTableInitializedBit} = 1)
        BEGIN
            IF (@{InputParamName.VersionMatchStrategy} = '{VersionMatchStrategy.Any}')
            BEGIN
                DELETE FROM @{recordIdsToConsiderTable} WHERE [{Tables.Record.Id.Name}] NOT IN
                (
                    SELECT DISTINCT r.[{Tables.Record.Id.Name}]
                    FROM [{streamName}].[{Tables.Record.Table.Name}] r WITH (NOLOCK)
                    INNER JOIN STRING_SPLIT(@{InputParamName.ObjectTypeIdsCsv}, ',') i
                    ON r.[{Tables.Record.ObjectTypeWithoutVersionId.Name}] = i.VALUE   
                )
            END
            ELSE
            BEGIN
                DELETE FROM @{recordIdsToConsiderTable} WHERE [{Tables.Record.Id.Name}] NOT IN
                (
                    SELECT DISTINCT r.[{Tables.Record.Id.Name}]
                    FROM [{streamName}].[{Tables.Record.Table.Name}] r WITH (NOLOCK)
                    INNER JOIN STRING_SPLIT(@{InputParamName.ObjectTypeIdsCsv}, ',') i
                    ON r.[{Tables.Record.ObjectTypeWithVersionId.Name}] = i.VALUE
                )
            END
        END
        ELSE
        BEGIN
            SET @{isRecordIdsToConsiderTableInitializedBit} = 1
            IF (@{InputParamName.VersionMatchStrategy} = '{VersionMatchStrategy.Any}')
            BEGIN
                INSERT INTO @{recordIdsToConsiderTable} ([{Tables.Record.Id.Name}])
                (
                    SELECT DISTINCT r.[{Tables.Record.Id.Name}]
                    FROM [{streamName}].[{Tables.Record.Table.Name}] r WITH (NOLOCK)
                    INNER JOIN STRING_SPLIT(@{InputParamName.ObjectTypeIdsCsv}, ',') i
                    ON r.[{Tables.Record.ObjectTypeWithoutVersionId.Name}] = i.VALUE       
                )
            END
            ELSE
            BEGIN
                INSERT INTO @{recordIdsToConsiderTable} ([{Tables.Record.Id.Name}])
                (
                    SELECT DISTINCT r.[{Tables.Record.Id.Name}]
                    FROM [{streamName}].[{Tables.Record.Table.Name}] r WITH (NOLOCK)
                    INNER JOIN STRING_SPLIT(@{InputParamName.ObjectTypeIdsCsv}, ',') i
                    ON r.[{Tables.Record.ObjectTypeWithVersionId.Name}] = i.VALUE      
                )
            END
        END
    END
    -- END APPLY OBJECT TYPE FILTER
    -----------------------------------------------------------------
    
    -----------------------------------------------------------------
    -- BEGIN APPLY RECORD TAG FILTER
    IF (@{InputParamName.TagIdsToMatchCsv} IS NOT NULL)
    BEGIN
        DECLARE @{recordTagIdsTable} TABLE([{Tables.Tag.Id.Name}] {new BigIntSqlDataTypeRepresentation().DeclarationInSqlSyntax} NOT NULL)
        INSERT INTO @{recordTagIdsTable} ([{Tables.Tag.Id.Name}])
        SELECT VALUE FROM STRING_SPLIT(@{InputParamName.TagIdsToMatchCsv}, ',')
        DECLARE @RecordTagCount INT
        SELECT @RecordTagCount = COUNT([{Tables.Tag.Id.Name}]) FROM @{recordTagIdsTable}
        IF (@{isRecordIdsToConsiderTableInitializedBit} = 1)
        BEGIN
          DELETE FROM @{recordIdsToConsiderTable} WHERE [{Tables.Record.Id.Name}] NOT IN
          (
            SELECT rt.[RecordId] AS [{Tables.Record.Id.Name}]
            FROM [{streamName}].[{Tables.RecordTag.Table.Name}] rt WITH (NOLOCK)
            INNER JOIN @{recordTagIdsTable} tids ON tids.[{Tables.Tag.Id.Name}] = rt.[{Tables.RecordTag.TagId.Name}]
            GROUP BY rt.[{Tables.RecordTag.RecordId.Name}]
            HAVING COUNT(rt.[{Tables.RecordTag.Id.Name}]) = @RecordTagCount
          )
        END
        ELSE
        BEGIN
            SET @{isRecordIdsToConsiderTableInitializedBit} = 1
            INSERT INTO @{recordIdsToConsiderTable} ([{Tables.Record.Id.Name}])
            (
                SELECT rt.[{Tables.RecordTag.RecordId.Name}] AS [{Tables.Record.Id.Name}]
                FROM [{streamName}].[{Tables.RecordTag.Table.Name}] rt WITH (NOLOCK)
                INNER JOIN @{recordTagIdsTable} tids ON tids.[{Tables.Tag.Id.Name}] = rt.[{Tables.RecordTag.TagId.Name}]
                GROUP BY rt.[{Tables.RecordTag.RecordId.Name}]
                HAVING COUNT(rt.[{Tables.RecordTag.Id.Name}]) = @RecordTagCount
            )
        END
    END
    -- END APPLY RECORD TAG FILTER
    -----------------------------------------------------------------
    {handlingTagMixIn}
    -----------------------------------------------------------------
    -- BEGIN ADD ALL IDS IF NO FILTER SUPPLIED
    IF (@{isRecordIdsToConsiderTableInitializedBit} <> 1)
    BEGIN
        INSERT INTO @{recordIdsToConsiderTable} ([{Tables.Record.Id.Name}])
        (
            SELECT r.[{Tables.Record.Id.Name}]
            FROM [{streamName}].[{Tables.Record.Table.Name}] r WITH (NOLOCK)
        )
    END
    -- END ADD ALL IDS IF NO FILTER SUPPLIED
    -----------------------------------------------------------------

    -----------------------------------------------------------------
    -- BEGIN REMOVE DEPRECATED IDS FILTER
    IF (@{InputParamName.DeprecatedIdEventTypeIdsCsv} IS NOT NULL)
    BEGIN
        IF (@{InputParamName.VersionMatchStrategy} = '{VersionMatchStrategy.Any}')
        BEGIN
            DELETE FROM @{recordIdsToConsiderTable} WHERE [{Tables.Record.Id.Name}] IN
            (
				SELECT DISTINCT rconsider.[{Tables.Record.Id.Name}] FROM
					(SELECT [{Tables.Record.Id.Name}], [{Tables.Record.StringSerializedId.Name}], [{Tables.Record.IdentifierTypeWithoutVersionId.Name}]
					FROM [{streamName}].[{Tables.Record.Table.Name}]
					WHERE [{Tables.Record.Id.Name}] IN (SELECT [{Tables.Record.Id.Name}] From @{recordIdsToConsiderTable})
					) rconsider
				LEFT JOIN
					(SELECT [{Tables.Record.Id.Name}], [{Tables.Record.StringSerializedId.Name}], [{Tables.Record.IdentifierTypeWithoutVersionId.Name}]
					FROM [{streamName}].[{Tables.Record.Table.Name}]
					WHERE [{Tables.Record.ObjectTypeWithoutVersionId.Name}] IN (SELECT VALUE FROM STRING_SPLIT(@{InputParamName.DeprecatedIdEventTypeIdsCsv}, ','))) rdeprecated
				ON
					rconsider.[{Tables.Record.StringSerializedId.Name}] = rdeprecated.[{Tables.Record.StringSerializedId.Name}]
			    	AND rconsider.[{Tables.Record.IdentifierTypeWithoutVersionId.Name}] = rdeprecated.[{Tables.Record.IdentifierTypeWithoutVersionId.Name}]
					AND rconsider.[{Tables.Record.Id.Name}] <= rdeprecated.[{Tables.Record.Id.Name}]
			    WHERE
  			        rdeprecated.[{Tables.Record.Id.Name}] IS NOT NULL
            )
        END
        ELSE
        BEGIN
            DELETE FROM @{recordIdsToConsiderTable} WHERE [{Tables.Record.Id.Name}] IN
            (
				SELECT DISTINCT rconsider.[{Tables.Record.Id.Name}] FROM
					(SELECT [{Tables.Record.Id.Name}], [{Tables.Record.StringSerializedId.Name}], [{Tables.Record.IdentifierTypeWithVersionId.Name}]
					FROM [{streamName}].[{Tables.Record.Table.Name}]
					WHERE [{Tables.Record.Id.Name}] IN (SELECT [{Tables.Record.Id.Name}] From @{recordIdsToConsiderTable})
					) rconsider
				LEFT JOIN
					(SELECT [{Tables.Record.Id.Name}], [{Tables.Record.StringSerializedId.Name}], [{Tables.Record.IdentifierTypeWithVersionId.Name}]
					FROM [{streamName}].[{Tables.Record.Table.Name}]
					WHERE [{Tables.Record.ObjectTypeWithVersionId.Name}] IN (SELECT VALUE FROM STRING_SPLIT(@{InputParamName.DeprecatedIdEventTypeIdsCsv}, ','))) rdeprecated
				ON 
					rconsider.[{Tables.Record.StringSerializedId.Name}] = rdeprecated.[{Tables.Record.StringSerializedId.Name}]
			    	AND rconsider.[{Tables.Record.IdentifierTypeWithVersionId.Name}] = rdeprecated.[{Tables.Record.IdentifierTypeWithVersionId.Name}]
					AND rconsider.[{Tables.Record.Id.Name}] <= rdeprecated.[{Tables.Record.Id.Name}]
			    WHERE
  			        rdeprecated.[{Tables.Record.Id.Name}] IS NOT NULL
            )
        END
    END
    -- END REMOVE DEPRECATED IDS FILTER
    -----------------------------------------------------------------

    --------------------------------------------------------------------------------------
    -- END RECORD FILTER INJECTED SQL - CHANGE IN BUILDING CODE AND PATCH SPROCS        --
    --------------------------------------------------------------------------------------
");
                    return result;
                }
            }
        }
    }
}
