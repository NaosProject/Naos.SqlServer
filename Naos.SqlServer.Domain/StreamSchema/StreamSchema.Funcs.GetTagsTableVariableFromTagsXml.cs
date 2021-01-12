// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamSchema.Funcs.GetTagsTableVariableFromTagsXml.cs" company="Naos Project">
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
        /// User defined functions.
        /// </summary>
        public static partial class Funcs
        {
            /// <summary>
            /// Class GetTagsTableVariableFromTagsXml.
            /// </summary>
            public static class GetTagsTableVariableFromTagsXml
            {
                /// <summary>
                /// Gets the name of the function.
                /// </summary>
                /// <value>The name of the function.</value>
                public static string Name => nameof(GetTagsTableVariableFromTagsXml);

                /// <summary>
                /// Input parameter names.
                /// </summary>
                public enum InputParamName
                {
                    /// <summary>
                    /// The tag set as XML.
                    /// </summary>
                    TagsXml,

                    /// <summary>
                    /// Include the identifier.
                    /// </summary>
                    IncludeId,
                }

                /// <summary>
                /// Builds the creation script.
                /// </summary>
                /// <param name="streamName">Name of the stream.</param>
                /// <returns>The creation script.</returns>
                public static string BuildCreationScript(
                    string streamName)
                {
                    const string tagsTable = "TagsTable";

                    return FormattableString.Invariant(
                        $@"
CREATE FUNCTION [{streamName}].[{GetTagsTableVariableFromTagsXml.Name}] (
      @{InputParamName.TagsXml} [xml]
    , @{InputParamName.IncludeId} [bit]
)
RETURNS TABLE
AS
        IF (@{InputParamName.IncludeId} = 1)
            BEGIN
             {BuildTagsTableDeclarationSyntax(tagsTable, true)}
             
		      INSERT INTO @{tagsTable}
		      SELECT
                null
		      , C.value('(@{TagConversionTool.TagEntryKeyAttributeName})[1]', '{Tables.Tag.TagKey.DataType.DeclarationInSqlSyntax}') as [{Tables.Tag.TagKey.Name}]
		      , C.value('(@{TagConversionTool.TagEntryValueAttributeName})[1]', '{Tables.Tag.TagValue.DataType.DeclarationInSqlSyntax}') as [{Tables.Tag.TagValue.Name}]
		      FROM
			    @{nameof(InputParamName.TagsXml)}.nodes('/{TagConversionTool.TagSetElementName}/{TagConversionTool.TagEntryElementName}') AS T(C)

		      UPDATE @{tagsTable} SET [{Tables.Tag.TagValue.Name}] = null WHERE [{Tables.Tag.TagValue.Name}] = '{TagConversionTool.NullCanaryValue}'
              RETURN @{tagsTable}
            END
        ELSE
            BEGIN
             {BuildTagsTableDeclarationSyntax(tagsTable, false)}
		      INSERT INTO @{tagsTable}
		      SELECT
		        C.value('(@{TagConversionTool.TagEntryKeyAttributeName})[1]', '{Tables.Tag.TagKey.DataType.DeclarationInSqlSyntax}') as [{Tables.Tag.TagKey.Name}]
		      , C.value('(@{TagConversionTool.TagEntryValueAttributeName})[1]', '{Tables.Tag.TagValue.DataType.DeclarationInSqlSyntax}') as [{Tables.Tag.TagValue.Name}]
		      FROM
			    @{nameof(InputParamName.TagsXml)}.nodes('/{TagConversionTool.TagSetElementName}/{TagConversionTool.TagEntryElementName}') AS T(C)

		      UPDATE @{tagsTable} SET [{Tables.Tag.TagValue.Name}] = null WHERE [{Tables.Tag.TagValue.Name}] = '{TagConversionTool.NullCanaryValue}'
              RETURN @{tagsTable}
            END
");
                }

                /// <summary>
                /// Builds the declaration and execution SQL syntax.
                /// </summary>
                /// <param name="tagsTableVariableName">Name of the tags table variable.</param>
                /// <param name="includeId">Include the identifier column.</param>
                /// <returns>System.String.</returns>
                public static string BuildTagsTableDeclarationSyntax(
                    string tagsTableVariableName,
                    bool includeId)
                {
                    var result = FormattableString.Invariant(
                        $@"
            DECLARE @{tagsTableVariableName} TABLE(
{(includeId ? FormattableString.Invariant($"[{Tables.Tag.Id.Name}] {Tables.Tag.Id.DataType.DeclarationInSqlSyntax},") : string.Empty)}
			[{Tables.Tag.TagKey.Name}] {Tables.Tag.TagKey.DataType.DeclarationInSqlSyntax} NOT NULL,
			[{Tables.Tag.TagValue.Name}] {Tables.Tag.TagValue.DataType.DeclarationInSqlSyntax} NULL)
");

                    return result;
                }
            }
        }
    }
}