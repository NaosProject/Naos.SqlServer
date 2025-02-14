// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlStream.TagCaching.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Protocol.Client
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Naos.SqlServer.Domain;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Type;
    using static System.FormattableString;

    public partial class SqlStream
    {
        private readonly ConcurrentDictionary<long, NamedValue<string>> tagIdToKeyValueMap = new ConcurrentDictionary<long, NamedValue<string>>();
        private readonly ConcurrentDictionary<NamedValue<string>, long> tagKeyValueToIdMap = new ConcurrentDictionary<NamedValue<string>, long>();

        /// <summary>
        /// Gets the ids add if necessary tag.
        /// </summary>
        /// <param name="locator">The locator.</param>
        /// <param name="tags">The tags.</param>
        /// <returns>IReadOnlyList&lt;System.Int64&gt;.</returns>
        public IReadOnlyList<long> GetIdsAddIfNecessaryTag(
            SqlServerLocator locator,
            IReadOnlyCollection<NamedValue<string>> tags)
        {
            if (tags == null)
            {
                return null;
            }

            var sqlProtocol = this.BuildSqlOperationsProtocol(locator);
            var notOrderedResult = new List<long>();
            var remaining = new List<NamedValue<string>>();

            foreach (var keyValuePair in tags)
            {
                var found = this.tagKeyValueToIdMap.TryGetValue(keyValuePair, out var id);
                if (found)
                {
                    notOrderedResult.Add(id);
                }
                else
                {
                    remaining.Add(keyValuePair);
                }
            }

            if (remaining.Any())
            {
                var storedProcWithVersionOp = StreamSchema.Sprocs.GetIdsAddIfNecessaryTagSet.BuildExecuteStoredProcedureOp(
                    this.Name,
                    remaining);
                var sprocResultWithVersion = sqlProtocol.Execute(storedProcWithVersionOp);
                var tagIdsXml = sprocResultWithVersion
                               .OutputParameters[nameof(StreamSchema.Sprocs.GetIdsAddIfNecessaryTagSet.OutputParamName.TagIdsXml)]
                               .GetValueOfType<string>();
                var tagIds = tagIdsXml.GetTagsFromXmlString();
                var additional = tagIds.Select(_ => long.Parse(_.Value, CultureInfo.InvariantCulture)).ToList();
                additional.Count.MustForOp(Invariant($"{nameof(additional)}-comparedTo-{nameof(remaining)}-Counts")).BeEqualTo(remaining.Count);

                // this is the sort order of the output of the sproc.
                var orderedRemaining = remaining.OrderBy(_ => _.Name).ThenBy(_ => _.Value ?? XmlConversionTool.NullCanaryValue).ToList();
                for (int idx = 0;
                    idx < orderedRemaining.Count;
                    idx++)
                {
                    this.tagKeyValueToIdMap.TryAdd(orderedRemaining[idx], additional[idx]);
                    this.tagIdToKeyValueMap.TryAdd(additional[idx], orderedRemaining[idx]);
                }

                notOrderedResult.AddRange(additional);
            }

            var result = notOrderedResult.OrderBy(_ => _).ToList();

            return result;
        }

        /// <summary>
        /// Gets the tags by identifiers.
        /// </summary>
        /// <param name="locator">The locator.</param>
        /// <param name="tagIds">The tag identifiers.</param>
        /// <returns>IReadOnlyDictionary&lt;System.String, System.String&gt;.</returns>
        public IReadOnlyCollection<NamedValue<string>> GetTagsByIds(
            SqlServerLocator locator,
            IReadOnlyCollection<long> tagIds)
        {
            if (tagIds == null)
            {
                return null;
            }

            if (!tagIds.Any())
            {
                return new List<NamedValue<string>>();
            }

            var result = new List<NamedValue<string>>();
            var remaining = new List<long>();

            foreach (var id in tagIds)
            {
                var found = this.tagIdToKeyValueMap.TryGetValue(id, out var keyValuePair);
                if (found)
                {
                    result.Add(new NamedValue<string>(keyValuePair.Name, keyValuePair.Value));
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
                var tagsXml = sprocResultWithVersion.OutputParameters[nameof(StreamSchema.Sprocs.GetTagSetFromIds.OutputParamName.TagsXml)].GetValueOfType<string>();
                var additional = XmlConversionTool.GetTagsFromXmlString(tagsXml).ToList() ?? new List<NamedValue<string>>();

                additional.Count.MustForOp(Invariant($"{nameof(additional)}-comparedTo-{nameof(remaining)}-Counts")).BeEqualTo(remaining.Count);

                // this is the sort order of sproc return.
                var remainingSortedOnId = remaining.OrderBy(_ => _).ToList();
                for (int idx = 0;
                    idx < remaining.Count;
                    idx++)
                {
                    this.tagKeyValueToIdMap.TryAdd(additional[idx], remainingSortedOnId[idx]);
                    this.tagIdToKeyValueMap.TryAdd(remainingSortedOnId[idx], additional[idx]);
                    result.Add(new NamedValue<string>(additional[idx].Name, additional[idx].Value));
                }
            }

            return result;
        }
    }
}
