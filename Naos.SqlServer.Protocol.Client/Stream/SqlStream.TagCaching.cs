// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlStream.TagCaching.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Protocol.Client
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Naos.CodeAnalysis.Recipes;
    using Naos.Database.Domain;
    using Naos.Protocol.Domain;
    using Naos.Recipes.RunWithRetry;
    using Naos.SqlServer.Domain;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Compression;
    using OBeautifulCode.Database.Recipes;
    using OBeautifulCode.Enum.Recipes;
    using OBeautifulCode.Representation.System;
    using OBeautifulCode.Serialization;
    using static System.FormattableString;
    using SerializationFormat = OBeautifulCode.Serialization.SerializationFormat;

    public partial class SqlStream
    {
        /// <summary>
        /// Gets the ids add if necessary tag.
        /// </summary>
        /// <param name="locator">The locator.</param>
        /// <param name="tags">The tags.</param>
        /// <returns>IReadOnlyList&lt;System.Int64&gt;.</returns>
        public IReadOnlyList<long> GetIdsAddIfNecessaryTag(SqlServerLocator locator, IReadOnlyDictionary<string, string> tags)
        {
            var sqlProtocol = this.BuildSqlOperationsProtocol(locator);

            var storedProcWithVersionOp = StreamSchema.Sprocs.GetIdsAddIfNecessaryTagSet.BuildExecuteStoredProcedureOp(
                this.Name,
                tags);
            var sprocResultWithVersion = sqlProtocol.Execute(storedProcWithVersionOp);
            var tagIdsXml = sprocResultWithVersion.OutputParameters[nameof(StreamSchema.Sprocs.GetIdsAddIfNecessaryTagSet.OutputParamName.TagIdsXml)].GetValue<string>();
            var tagIds = TagConversionTool.GetTagsFromXmlString(tagIdsXml);
            var result = tagIds.Values.Select(long.Parse).ToList();

            return result;
        }

        /// <summary>
        /// Gets the tags by identifiers.
        /// </summary>
        /// <param name="locator">The locator.</param>
        /// <param name="tagIds">The tag identifiers.</param>
        /// <returns>IReadOnlyDictionary&lt;System.String, System.String&gt;.</returns>
        public IReadOnlyDictionary<string, string> GetTagsByIds(SqlServerLocator locator, IReadOnlyCollection<string> tagIds)
        {
            if (tagIds == null || !tagIds.Any())
            {
                return new Dictionary<string, string>();
            }

            var sqlProtocol = this.BuildSqlOperationsProtocol(locator);

            var storedProcWithVersionOp = StreamSchema.Sprocs.GetTagSetFromIds.BuildExecuteStoredProcedureOp(
                this.Name,
                tagIds.Select(long.Parse).ToList());
            var sprocResultWithVersion = sqlProtocol.Execute(storedProcWithVersionOp);
            var tagsXml = sprocResultWithVersion.OutputParameters[nameof(StreamSchema.Sprocs.GetTagSetFromIds.OutputParamName.TagsXml)].GetValue<string>();
            var result = TagConversionTool.GetTagsFromXmlString(tagsXml);
            return result;
        }
    }
}
