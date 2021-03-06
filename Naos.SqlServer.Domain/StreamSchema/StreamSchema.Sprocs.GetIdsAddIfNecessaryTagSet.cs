﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamSchema.Sprocs.GetIdsAddIfNecessaryTagSet.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static System.FormattableString;

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
            /// Container for stored procedure.
            /// </summary>
            public static class GetIdsAddIfNecessaryTagSet
            {
                /// <summary>
                /// Gets the name of the stored procedure.
                /// </summary>
                /// <value>The name of the stored procedure.</value>
                public static string Name => nameof(GetIdsAddIfNecessaryTagSet);

                /// <summary>
                /// Input parameter names.
                /// </summary>
                public enum InputParamName
                {
                    /// <summary>
                    /// The tag set as XML.
                    /// </summary>
                    TagsXml,
                }

                /// <summary>
                /// Output parameter names.
                /// </summary>
                public enum OutputParamName
                {
                    /// <summary>
                    /// The tag identifiers as XML.
                    /// </summary>
                    TagIdsXml,
                }

                /// <summary>
                /// Builds the execute stored procedure operation.
                /// </summary>
                /// <param name="streamName">Name of the stream.</param>
                /// <param name="tags">The tag set.</param>
                /// <returns>Operation to execute stored procedure.</returns>
                public static ExecuteStoredProcedureOp BuildExecuteStoredProcedureOp(
                    string streamName,
                    IReadOnlyDictionary<string, string> tags)
                {
                    var sprocName = Invariant($"[{streamName}].[{nameof(GetIdsAddIfNecessaryTagSet)}]");
                    var tagsXml = TagConversionTool.GetTagsXmlString(tags);
                    var parameters = new List<SqlParameterRepresentationBase>()
                                     {
                                         new SqlInputParameterRepresentation<string>(
                                             nameof(InputParamName.TagsXml),
                                             new XmlSqlDataTypeRepresentation(),
                                             tagsXml),
                                         new SqlOutputParameterRepresentation<string>(
                                             nameof(OutputParamName.TagIdsXml),
                                             new XmlSqlDataTypeRepresentation()),
                                     };

                    var parameterNameToDetailsMap = parameters.ToDictionary(k => k.Name, v => v);

                    var result = new ExecuteStoredProcedureOp(sprocName, parameterNameToDetailsMap);

                    return result;
                }

                /// <summary>
                /// Builds the name of the put stored procedure.
                /// </summary>
                /// <param name="streamName">Name of the stream.</param>
                /// <param name="asAlter">An optional value indicating whether or not to generate a ALTER versus CREATE; DEFAULT is false and will generate a CREATE script.</param>
                /// <returns>Name of the put stored procedure.</returns>
                public static string BuildCreationScript(
                    string streamName,
                    bool asAlter = false)
                {
                    const string tagIdsTable = "TagIdsTable";
                    const string transaction = "GetIdAddIfNecTagSetTrans";

                    var createOrModify = asAlter ? "ALTER" : "CREATE";
                    var result = Invariant(
                        $@"
{createOrModify} PROCEDURE [{streamName}].[{GetIdsAddIfNecessaryTagSet.Name}](
    @{nameof(InputParamName.TagsXml)} {new XmlSqlDataTypeRepresentation().DeclarationInSqlSyntax}
  , @{nameof(OutputParamName.TagIdsXml)} {new XmlSqlDataTypeRepresentation().DeclarationInSqlSyntax} OUTPUT
  )
AS
BEGIN
    DECLARE @{tagIdsTable} TABLE(
	[{Tables.Tag.Id.Name}] {Tables.Tag.Id.DataType.DeclarationInSqlSyntax} NULL,
	[{Tables.Tag.TagKey.Name}] {Tables.Tag.TagKey.DataType.DeclarationInSqlSyntax} NULL,
	[{Tables.Tag.TagValue.Name}] {Tables.Tag.TagValue.DataType.DeclarationInSqlSyntax} NULL)

    INSERT INTO @{tagIdsTable}
        SELECT
            e.[{Tables.Tag.Id.Name}]
		  , n.[{Tables.Tag.TagKey.Name}]
		  , n.[{Tables.Tag.TagValue.Name}]
		FROM [{streamName}].[{Funcs.GetTagsTableVariableFromTagsXml.Name}](@{InputParamName.TagsXml}) n
        LEFT JOIN [{streamName}].[{Tables.Tag.Table.Name}] e WITH (NOLOCK) ON 
            (n.[{Tables.Tag.TagKey.Name}] =  e.[{Tables.Tag.TagKey.Name}] AND (n.[{Tables.Tag.TagValue.Name}] = e.[{Tables.Tag.TagValue.Name}] OR (n.[{Tables.Tag.TagValue.Name}] is null and e.[{Tables.Tag.TagValue.Name}] is null)))
    IF EXISTS (SELECT TOP 1 * FROM @{tagIdsTable} t WHERE t.[{Tables.Tag.Id.Name}] IS NULL)
    BEGIN
        SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
        BEGIN TRANSACTION [{transaction}]
          BEGIN TRY
	        INSERT INTO [{streamName}].[{Tables.Tag.Table.Name}] WITH (TABLOCKX)
            SELECT 
		        n.[{Tables.Tag.TagKey.Name}]
		      , n.[{Tables.Tag.TagValue.Name}]
		      , GETUTCDATE()
		    FROM @{tagIdsTable} n
            LEFT JOIN [{streamName}].[{Tables.Tag.Table.Name}] e ON 
            (n.[{Tables.Tag.TagKey.Name}] =  e.[{Tables.Tag.TagKey.Name}] AND (n.[{Tables.Tag.TagValue.Name}] = e.[{Tables.Tag.TagValue.Name}] OR (n.[{Tables.Tag.TagValue.Name}] is null and e.[{Tables.Tag.TagValue.Name}] is null)))
            WHERE e.[{Tables.Tag.Id.Name}] IS NULL

            COMMIT TRANSACTION [{transaction}]
        END TRY
        BEGIN CATCH
            SET @{nameof(OutputParamName.TagIdsXml)} = NULL
            DECLARE @ErrorMessage nvarchar(max), 
                  @ErrorSeverity int, 
                  @ErrorState int

            SELECT @ErrorMessage = ERROR_MESSAGE() + ' Line ' + cast(ERROR_LINE() as nvarchar(5)), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE()

            IF (@@trancount > 0)
            BEGIN
                ROLLBACK TRANSACTION [{transaction}]
            END

            RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
        END CATCH
    END  

    IF EXISTS (SELECT TOP 1 * FROM @{tagIdsTable} WHERE [{Tables.Tag.Id.Name}] IS NULL)
    BEGIN
        SELECT @{OutputParamName.TagIdsXml} = (SELECT
	        ROW_NUMBER() OVER (ORDER BY n.[{Tables.Tag.TagKey.Name}], ISNULL(n.[{Tables.Tag.TagValue.Name}], '{TagConversionTool.NullCanaryValue}')) AS [@{TagConversionTool.TagEntryKeyAttributeName}],
	        n.{Tables.Tag.Id.Name} AS [@{TagConversionTool.TagEntryValueAttributeName}]
        FROM @{tagIdsTable} e
        INNER JOIN [{streamName}].[{Tables.Tag.Table.Name}] n ON
                (n.[{Tables.Tag.TagKey.Name}] =  e.[{Tables.Tag.TagKey.Name}] AND (n.[{Tables.Tag.TagValue.Name}] = e.[{Tables.Tag.TagValue.Name}] OR (n.[{Tables.Tag.TagValue.Name}] is null and e.[{Tables.Tag.TagValue.Name}] is null)))
        FOR XML PATH ('{TagConversionTool.TagEntryElementName}'), ROOT('{TagConversionTool.TagSetElementName}'))
    END
    ELSE
    BEGIN
        SELECT @{OutputParamName.TagIdsXml} = (SELECT
            ROW_NUMBER() OVER (ORDER BY e.[{Tables.Tag.TagKey.Name}], ISNULL(e.[{Tables.Tag.TagValue.Name}], '{TagConversionTool.NullCanaryValue}')) AS [@{TagConversionTool.TagEntryKeyAttributeName}],
	        e.{Tables.Tag.Id.Name} AS [@{TagConversionTool.TagEntryValueAttributeName}]
        FROM @{tagIdsTable} e
        FOR XML PATH ('{TagConversionTool.TagEntryElementName}'), ROOT('{TagConversionTool.TagSetElementName}'))
    END
END");

                    return result;
                }
            }
        }
    }
}