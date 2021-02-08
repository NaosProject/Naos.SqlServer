// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamSchema.Funcs.GetTagsTableVariableFromTagsXml.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System;
    using static System.FormattableString;

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
                }

                /// <summary>
                /// Builds the creation script.
                /// </summary>
                /// <param name="streamName">Name of the stream.</param>
                /// <param name="asAlter">An optional value indicating whether or not to generate a ALTER versus CREATE; DEFAULT is false and will generate a CREATE script.</param>
                /// <returns>The creation script.</returns>
                public static string BuildCreationScript(
                    string streamName,
                    bool asAlter = false)
                {
                    var createOrModify = asAlter ? "ALTER" : "CREATE";
                    var result = Invariant(
                        $@"
{createOrModify} FUNCTION [{streamName}].[{GetTagsTableVariableFromTagsXml.Name}] (
      @{InputParamName.TagsXml} [xml]
)
RETURNS TABLE
AS
RETURN 
		      SELECT
		        C.value('(@{TagConversionTool.TagEntryKeyAttributeName})[1]', '{Tables.Tag.TagKey.DataType.DeclarationInSqlSyntax}') as [{Tables.Tag.TagKey.Name}]
		      , [{Tables.Tag.TagValue.Name}] = CASE C.value('(@{TagConversionTool.TagEntryValueAttributeName})[1]', '{Tables.Tag.TagValue.DataType.DeclarationInSqlSyntax}')
			     WHEN '---NULL---' THEN NULL
				 ELSE C.value('(@{TagConversionTool.TagEntryValueAttributeName})[1]', '{Tables.Tag.TagValue.DataType.DeclarationInSqlSyntax}')
				 END
		      FROM
			    @{nameof(InputParamName.TagsXml)}.nodes('/{TagConversionTool.TagSetElementName}/{TagConversionTool.TagEntryElementName}') AS T(C)
");

                    return result;
                }

                /// <summary>
                /// Builds the declaration and execution SQL syntax.
                /// </summary>
                /// <param name="tagsTableVariableName">Name of the tags table variable.</param>
                /// <returns>System.String.</returns>
                public static string BuildTagsTableDeclarationSyntax(
                    string tagsTableVariableName)
                {
                    var result = FormattableString.Invariant(
                        $@"
            DECLARE @{tagsTableVariableName} TABLE(
			[{Tables.Tag.TagKey.Name}] {Tables.Tag.TagKey.DataType.DeclarationInSqlSyntax} NOT NULL,
			[{Tables.Tag.TagValue.Name}] {Tables.Tag.TagValue.DataType.DeclarationInSqlSyntax} NULL)
");

                    return result;
                }

                /// <summary>
                /// Builds the declaration and execution SQL syntax.
                /// </summary>
                /// <param name="tagsTableVariableName">Name of the tags table variable.</param>
                /// <returns>System.String.</returns>
                public static string BuildTagsTableWithIdDeclarationSyntax(
                    string tagsTableVariableName)
                {
                    var result = FormattableString.Invariant(
                        $@"
            DECLARE @{tagsTableVariableName} TABLE(
			[{Tables.Tag.Id.Name}] {Tables.Tag.Id.DataType.DeclarationInSqlSyntax} NULL,
			[{Tables.Tag.TagKey.Name}] {Tables.Tag.TagKey.DataType.DeclarationInSqlSyntax} NULL,
			[{Tables.Tag.TagValue.Name}] {Tables.Tag.TagValue.DataType.DeclarationInSqlSyntax} NULL)
");

                    return result;
                }

                /// <summary>
                /// Builds the declaration and execution SQL syntax.
                /// </summary>
                /// <param name="tagsTableVariableName">Name of the tags table variable.</param>
                /// <returns>System.String.</returns>
                public static string BuildTagsTableWithOnlyIdDeclarationSyntax(
                    string tagsTableVariableName)
                {
                    var result = FormattableString.Invariant(
                        $@"
            DECLARE @{tagsTableVariableName} TABLE(
			[{Tables.Tag.Id.Name}] {Tables.Tag.Id.DataType.DeclarationInSqlSyntax} NOT NULL)
");

                    return result;
                }
            }
        }
    }
}