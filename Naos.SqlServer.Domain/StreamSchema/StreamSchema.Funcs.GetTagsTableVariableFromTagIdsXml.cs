// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamSchema.Funcs.GetTagsTableVariableFromTagIdsXml.cs" company="Naos Project">
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
            public static class GetTagsTableVariableFromTagIdsXml
            {
                /// <summary>
                /// Gets the name of the function.
                /// </summary>
                /// <value>The name of the function.</value>
                public static string Name => nameof(GetTagsTableVariableFromTagIdsXml);

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
                /// <returns>The creation script.</returns>
                public static string BuildCreationScript(
                    string streamName)
                {
                    return FormattableString.Invariant(
                        $@"
CREATE FUNCTION [{streamName}].[{Name}] (
      @{InputParamName.TagsXml} [xml]
)
RETURNS TABLE
AS
RETURN 
		      SELECT
		        C.value('(@{TagConversionTool.TagEntryKeyAttributeName})[1]', '{Tables.Tag.TagKey.DataType.DeclarationInSqlSyntax}') as [{Tables.Tag.TagKey.Name}]
		      , [{Tables.Tag.TagValue.Name}] = CASE C.value('(@{TagConversionTool.TagEntryValueAttributeName})[1]', '{Tables.Tag.TagValue.DataType.DeclarationInSqlSyntax}')
			     WHEN '---NULL---' THEN NULL
				 ELSE C.value('(@{TagConversionTool.TagEntryValueAttributeName})[1]', '{Tables.Tag.Id.DataType.DeclarationInSqlSyntax}')
				 END
		      FROM
			    @{nameof(InputParamName.TagsXml)}.nodes('/{TagConversionTool.TagSetElementName}/{TagConversionTool.TagEntryElementName}') AS T(C)
");
                }
            }
        }
    }
}