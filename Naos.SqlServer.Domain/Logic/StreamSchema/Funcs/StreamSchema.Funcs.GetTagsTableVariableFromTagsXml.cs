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
            /// Function: GetTagsTableVariableFromTagsXml.
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
                    var createOrModify = asAlter ? "CREATE OR ALTER" : "CREATE";
                    var result = Invariant(
                        $@"
{createOrModify} FUNCTION [{streamName}].[{Name}] (
      @{InputParamName.TagsXml} [xml]
)
RETURNS TABLE
AS
RETURN
		      SELECT
                -- NOTE: Besides for Record and Handling Tags, we also use this function to parse XML containing String Serialized Ids.  
                -- The .NET client code uses the same XML template format for Tags as it does for String Serialized Ids (it treats them as tags).
                -- The Tag Key is the String Serialized Id and the Tag Value is the Identifier Type Id (integer in string format).
                -- In the past, both Tags Keys and String Serialized Ids were defined as NVARCHAR(450) but now these support different maximum character values.
                -- So here we need to use the greater of the two.  That's String Serialized Id from the Record table for the Tag Key column 
                -- and Tag Value value from the Tag table for Tag Value column (the Identifier Type Id, as an integer, will only have a dozen or so characters max).  
                -- In the future, we should not use this function for String Serialized Ids.  That should get it's own XML template and there should
                -- be a function dedicated to parsing that XML into a table.
		        C.value('(@{XmlConversionTool.TagEntryKeyAttributeName})[1]', '{Tables.Record.StringSerializedId.SqlDataType.DeclarationInSqlSyntax}') as [{Tables.Tag.TagKey.Name}]
		      , [{Tables.Tag.TagValue.Name}] = CASE C.value('(@{XmlConversionTool.TagEntryValueAttributeName})[1]', '{Tables.Tag.TagValue.SqlDataType.DeclarationInSqlSyntax}')
			     WHEN '{XmlConversionTool.NullCanaryValue}' THEN NULL
				 ELSE C.value('(@{XmlConversionTool.TagEntryValueAttributeName})[1]', '{Tables.Tag.TagValue.SqlDataType.DeclarationInSqlSyntax}')
				 END
		      FROM
			    @{nameof(InputParamName.TagsXml)}.nodes('/{XmlConversionTool.TagSetElementName}/{XmlConversionTool.TagEntryElementName}') AS T(C)
");

                    return result;
                }
            }
        }
    }
}