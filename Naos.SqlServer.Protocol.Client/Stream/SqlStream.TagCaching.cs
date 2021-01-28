// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlStream.TagCaching.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Protocol.Client
{
    using System;
    using System.Collections.Concurrent;
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
        private readonly ConcurrentDictionary<long, KeyValuePair<string, string>> tagIdToKeyValueMap = new ConcurrentDictionary<long, KeyValuePair<string, string>>();
        private readonly ConcurrentDictionary<KeyValuePair<string, string>, long> tagKeyValueToIdMap = new ConcurrentDictionary<KeyValuePair<string, string>, long>();

        /// <summary>
        /// Gets the ids add if necessary tag.
        /// </summary>
        /// <param name="locator">The locator.</param>
        /// <param name="tags">The tags.</param>
        /// <returns>IReadOnlyList&lt;System.Int64&gt;.</returns>
        public IReadOnlyList<long> GetIdsAddIfNecessaryTag(SqlServerLocator locator, IReadOnlyDictionary<string, string> tags)
        {
            var sqlProtocol = this.BuildSqlOperationsProtocol(locator);
            var result = new List<long>();
            var remaining = new List<KeyValuePair<string, string>>();

            foreach (var keyValuePair in tags)
            {
                var found = this.tagKeyValueToIdMap.TryGetValue(keyValuePair, out var id);
                if (found)
                {
                    result.Add(id);
                }
                else
                {
                    remaining.Add(keyValuePair);
                }
            }

            if (remaining.Any())
            {
                var remainingTags = remaining.ToDictionary(k => k.Key, v => v.Value);
                var storedProcWithVersionOp = StreamSchema.Sprocs.GetIdsAddIfNecessaryTagSet.BuildExecuteStoredProcedureOp(
                    this.Name,
                    remainingTags);
                var sprocResultWithVersion = sqlProtocol.Execute(storedProcWithVersionOp);
                var tagIdsXml = sprocResultWithVersion
                               .OutputParameters[nameof(StreamSchema.Sprocs.GetIdsAddIfNecessaryTagSet.OutputParamName.TagIdsXml)]
                               .GetValue<string>();
                var tagIds = TagConversionTool.GetTagsFromXmlString(tagIdsXml);
                var additional = tagIds.Select(_ => long.Parse(_.Value)).ToList();
                additional.Count.MustForOp(Invariant($"{nameof(additional)}-comparedTo-{nameof(remaining)}-Counts")).BeEqualTo(remaining.Count);

                // this is the sort order of the output of the sproc.
                var orderedRemaining = remaining.OrderBy(_ => _.Key).ThenBy(_ => _.Value ?? TagConversionTool.NullCanaryValue).ToList();
                for (int idx = 0;
                    idx < orderedRemaining.Count;
                    idx++)
                {
                    this.tagKeyValueToIdMap.TryAdd(orderedRemaining[idx], additional[idx]);
                    this.tagIdToKeyValueMap.TryAdd(additional[idx], orderedRemaining[idx]);
                }

                result.AddRange(additional);
            }

            return result;
        }

        /// <summary>
        /// Gets the tags by identifiers.
        /// </summary>
        /// <param name="locator">The locator.</param>
        /// <param name="tagIds">The tag identifiers.</param>
        /// <returns>IReadOnlyDictionary&lt;System.String, System.String&gt;.</returns>
        public IReadOnlyDictionary<string, string> GetTagsByIds(SqlServerLocator locator, IReadOnlyCollection<long> tagIds)
        {
            if (tagIds == null || !tagIds.Any())
            {
                return new Dictionary<string, string>();
            }

            var result = new Dictionary<string, string>();
            var remaining = new List<long>();

            foreach (var id in tagIds)
            {
                var found = this.tagIdToKeyValueMap.TryGetValue(id, out var keyValuePair);
                if (found)
                {
                    result.Add(keyValuePair.Key, keyValuePair.Value);
                }
                else
                {
                    remaining.Add(id);
                }
            }

            if (remaining.Any())
            {
                var sqlProtocol = this.BuildSqlOperationsProtocol(locator);

                var storedProcWithVersionOp = StreamSchema.Sprocs.GetTagSetFromIds.BuildExecuteStoredProcedureOp(
                    this.Name,
                    remaining.ToList());
                var sprocResultWithVersion = sqlProtocol.Execute(storedProcWithVersionOp);
                var tagsXml = sprocResultWithVersion.OutputParameters[nameof(StreamSchema.Sprocs.GetTagSetFromIds.OutputParamName.TagsXml)].GetValue<string>();
                var additional = TagConversionTool.GetTagsFromXmlString(tagsXml).ToList();

                additional.Count.MustForOp(Invariant($"{nameof(additional)}-comparedTo-{nameof(remaining)}-Counts")).BeEqualTo(remaining.Count);

                // this is the sort order of sproc return.
                var remainingSortedOnId = remaining.OrderBy(_ => _).ToList();
                for (int idx = 0;
                    idx < remaining.Count;
                    idx++)
                {
                    this.tagKeyValueToIdMap.TryAdd(additional[idx], remainingSortedOnId[idx]);
                    this.tagIdToKeyValueMap.TryAdd(remainingSortedOnId[idx], additional[idx]);
                    result.Add(additional[idx].Key, additional[idx].Value);
                }
            }

            return result;
        }
    }
}
