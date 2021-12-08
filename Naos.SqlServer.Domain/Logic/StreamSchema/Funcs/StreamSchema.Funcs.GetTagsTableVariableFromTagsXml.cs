// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamSchema.Funcs.GetTagsTableVariableFromTagsXml.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using static System.FormattableString;

    /// <summary>
    /// Container for schema.
    /// </summary>
    public static partial class StreamSchema
    {
        public static partial class Funcs
        {
            /// <summary>
            /// DESCRIPTION HERE.
            /// </summary>
            public static class GetTagsTableVariableFromTagsXml
            {
                /// <summary>
                /// Gets the name of the function.
                /// </summary>
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
                /// <returns>The creation script to create the func.</returns>
                public static string BuildCreationScript(
                    string streamName,
                    bool asAlter = false)
                {
                    var createOrModify = asAlter ? "ALTER" : "CREATE";
                    var result = Invariant(
                        $@"
{createOrModify} FUNCTION [{streamName}].[{Name}] (
      @{InputParamName.TagsXml} [xml]
)
RETURNS TABLE
AS
RETURN
		      SELECT
		        C.value('(@{TagConversionTool.TagEntryKeyAttributeName})[1]', '{Tables.Tag.TagKey.SqlDataType.DeclarationInSqlSyntax}') as [{Tables.Tag.TagKey.Name}]
		      , [{Tables.Tag.TagValue.Name}] = CASE C.value('(@{TagConversionTool.TagEntryValueAttributeName})[1]', '{Tables.Tag.TagValue.SqlDataType.DeclarationInSqlSyntax}')
			     WHEN '---NULL---' THEN NULL
				 ELSE C.value('(@{TagConversionTool.TagEntryValueAttributeName})[1]', '{Tables.Tag.TagValue.SqlDataType.DeclarationInSqlSyntax}')
				 END
		      FROM
			    @{nameof(InputParamName.TagsXml)}.nodes('/{TagConversionTool.TagSetElementName}/{TagConversionTool.TagEntryElementName}') AS T(C)
");

                    return result;
                }
            }
        }
    }
}