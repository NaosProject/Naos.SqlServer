// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamSchema.Sprocs.GetTagSetFromIds.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Naos.CodeAnalysis.Recipes;

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
            public static class GetTagSetFromIds
            {
                /// <summary>
                /// Gets the name of the stored procedure.
                /// </summary>
                /// <value>The name of the stored procedure.</value>
                public static string Name => nameof(GetTagSetFromIds);

                /// <summary>
                /// Input parameter names.
                /// </summary>
                [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Param", Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
                public enum InputParamName
                {
                    /// <summary>
                    /// The tag identifiers as XML.
                    /// </summary>
                    TagIdsXml,
                }

                /// <summary>
                /// Output parameter names.
                /// </summary>
                [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Param", Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
                public enum OutputParamName
                {
                    /// <summary>
                    /// The tag set as XML.
                    /// </summary>
                    TagsXml,
                }

                /// <summary>
                /// Builds the execute stored procedure operation.
                /// </summary>
                /// <param name="streamName">Name of the stream.</param>
                /// <param name="tagIds">The tag identifier set.</param>
                /// <returns>Operation to execute stored procedure.</returns>
                public static ExecuteStoredProcedureOp BuildExecuteStoredProcedureOp(
                    string streamName,
                    IReadOnlyList<long> tagIds)
                {
                    var sprocName = FormattableString.Invariant($"[{streamName}].{Name}");
                    var tagIdsDictionary = tagIds.ToOrdinalDictionary(true);
                    var tagsXml = TagConversionTool.GetTagsXmlString(tagIdsDictionary);
                    var parameters = new List<SqlParameterRepresentationBase>()
                                     {
                                         new SqlInputParameterRepresentation<string>(
                                             nameof(InputParamName.TagIdsXml),
                                             new StringSqlDataTypeRepresentation(true, StringSqlDataTypeRepresentation.MaxLengthConstant),
                                             tagsXml),
                                         new SqlOutputParameterRepresentation<string>(
                                             nameof(OutputParamName.TagsXml),
                                             new StringSqlDataTypeRepresentation(true, StringSqlDataTypeRepresentation.MaxLengthConstant)),
                                     };

                    var parameterNameToDetailsMap = parameters.ToDictionary(k => k.Name, v => v);

                    var result = new ExecuteStoredProcedureOp(sprocName, parameterNameToDetailsMap);

                    return result;
                }

                /// <summary>
                /// Builds the name of the put stored procedure.
                /// </summary>
                /// <param name="streamName">Name of the stream.</param>
                /// <returns>Name of the put stored procedure.</returns>
                public static string BuildCreationScript(
                    string streamName)
                {
                    return FormattableString.Invariant(
                        $@"
CREATE PROCEDURE [{streamName}].[{GetTagSetFromIds.Name}](
  @{nameof(InputParamName.TagIdsXml)} {new XmlSqlDataTypeRepresentation().DeclarationInSqlSyntax},
  @{nameof(OutputParamName.TagsXml)} {new XmlSqlDataTypeRepresentation().DeclarationInSqlSyntax} OUTPUT
  )
AS
BEGIN      
    SELECT @{OutputParamName.TagsXml} = (SELECT
	    {Tables.Tag.TagKey.Name} AS [@{TagConversionTool.TagEntryKeyAttributeName}],
	    ISNULL({Tables.Tag.TagValue.Name},'{TagConversionTool.NullCanaryValue}') AS [@{TagConversionTool.TagEntryValueAttributeName}]
    FROM [{streamName}].[{Tables.Tag.Table.Name}]    
    WHERE [{Tables.Tag.Id.Name}] IN (SELECT {Tables.Tag.TagValue.Name} FROM [{streamName}].[{Funcs.GetTagsTableVariableFromTagIdsXml.Name}](@{InputParamName.TagIdsXml}))
    FOR XML PATH ('{TagConversionTool.TagEntryElementName}'), ROOT('{TagConversionTool.TagSetElementName}'))

END");
                }
            }
        }
    }
}