// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamSchema.Sprocs.GetTagSetFromIds.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Naos.CodeAnalysis.Recipes;
    using OBeautifulCode.Collection.Recipes;
    using OBeautifulCode.String.Recipes;
    using static System.FormattableString;

    public static partial class StreamSchema
    {
        public static partial class Sprocs
        {
            /// <summary>
            /// Stored procedure: GetTagSetFromIds.
            /// </summary>
            public static class GetTagSetFromIds
            {
                /// <summary>
                /// Gets the name of the stored procedure.
                /// </summary>
                public static string Name => nameof(GetTagSetFromIds);

                /// <summary>
                /// Input parameter names.
                /// </summary>
                [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Param", Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
                public enum InputParamName
                {
                    /// <summary>
                    /// The tag identifiers as CSV.
                    /// </summary>
                    TagIdsCsv,
                }

                /// <summary>
                /// Output parameter names.
                /// </summary>
                [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Param", Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
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
                    var sprocName = Invariant($"[{streamName}].{Name}");
                    var tagIdsCsv = tagIds?.Select(_ => _.ToStringInvariantPreferred()).ToCsv();
                    var parameters = new List<SqlParameterRepresentationBase>()
                                     {
                                         new SqlInputParameterRepresentation<string>(
                                             nameof(InputParamName.TagIdsCsv),
                                             Tables.Record.TagIdsCsv.SqlDataType,
                                             tagIdsCsv),
                                         new SqlOutputParameterRepresentation<string>(
                                             nameof(OutputParamName.TagsXml),
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
                /// <returns>Creation script for creating the stored procedure.</returns>
                public static string BuildCreationScript(
                    string streamName,
                    bool asAlter = false)
                {
                    var createOrModify = asAlter ? "ALTER" : "CREATE";
                    var result = Invariant(
                        $@"
{createOrModify} PROCEDURE [{streamName}].[{GetTagSetFromIds.Name}](
  @{nameof(InputParamName.TagIdsCsv)} {Tables.Record.TagIdsCsv.SqlDataType.DeclarationInSqlSyntax},
  @{nameof(OutputParamName.TagsXml)} {new XmlSqlDataTypeRepresentation().DeclarationInSqlSyntax} OUTPUT
  )
AS
BEGIN
    SELECT @{OutputParamName.TagsXml} = (SELECT
	    {Tables.Tag.TagKey.Name} AS [@{TagConversionTool.TagEntryKeyAttributeName}],
	    ISNULL({Tables.Tag.TagValue.Name},'{TagConversionTool.NullCanaryValue}') AS [@{TagConversionTool.TagEntryValueAttributeName}]
    FROM [{streamName}].[{Tables.Tag.Table.Name}]
    WHERE [{Tables.Tag.Id.Name}] IN (SELECT value AS [{Tables.Tag.Id.Name}] FROM STRING_SPLIT(@{InputParamName.TagIdsCsv}, ','))
    ORDER BY [{Tables.Tag.Id.Name}]
    FOR XML PATH ('{TagConversionTool.TagEntryElementName}'), ROOT('{TagConversionTool.TagSetElementName}'))
END");

                    return result;
                }
            }
        }
    }
}