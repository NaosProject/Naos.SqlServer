// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TagConversionTool.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;
    using Naos.CodeAnalysis.Recipes;
    using OBeautifulCode.Assertion.Recipes;
    using static System.FormattableString;

    /// <summary>
    /// Sql Operation Protocol.
    /// </summary>
    public static class TagConversionTool
    {
        /// <summary>
        /// Root element name of the tag set.
        /// </summary>
        public const string TagSetElementName = "Tags";

        /// <summary>
        /// Element name of each entry in the tag set.
        /// </summary>
        public const string TagEntryElementName = "Tag";

        /// <summary>
        /// Attribute name of the 'Key' constituent of each entry.
        /// </summary>
        public const string TagEntryKeyAttributeName = "Key";

        /// <summary>
        /// Attribute name of the 'Value' constituent of each entry.
        /// </summary>
        public const string TagEntryValueAttributeName = "Value";

        /// <summary>
        /// Canary value to pass null through XML to Sql Server.
        /// </summary>
        public const string NullCanaryValue = "---NULL---";

        /// <summary>
        /// Gets the tag set as a <see cref="IReadOnlyDictionary{TKey,TValue}"/> from the provided XML as a string.
        /// </summary>
        /// <param name="tagsAsXml">The tags in XML as a string.</param>
        /// <returns><see cref="IReadOnlyDictionary{TKey,TValue}"/> from the provided XML.</returns>
        public static IReadOnlyDictionary<string, string> GetTagsFromXmlString(
            string tagsAsXml)
        {
            var result = new Dictionary<string, string>();

            if (string.IsNullOrWhiteSpace(tagsAsXml))
            {
                return result;
            }

            var doc = XDocument.Parse(tagsAsXml);
            foreach (var tag in doc.Descendants(TagEntryElementName))
            {
                var key = tag.Attribute(TagEntryKeyAttributeName)?.Value
                       ?? throw new NotSupportedException(Invariant($"Could not find the '{TagEntryKeyAttributeName}' attribute in XML node: '{tag}'."));
                var value = (tag.Attribute(TagEntryValueAttributeName)
                          ?? throw new NotSupportedException(Invariant($"Could not find the '{TagEntryValueAttributeName}' attribute in XML node: '{tag}'.")))
                   .Value; // value is allowed to be null...

                if (value == NullCanaryValue)
                {
                    value = null;
                }

                result.Add(key, value);
            }

            return result;
        }

        /// <summary>
        /// Gets the tag set converted to an XML string.
        /// </summary>
        /// <param name="tags">The tags.</param>
        /// <returns>The converted XML.</returns>
        public static string GetTagsXmlString(
            IReadOnlyDictionary<string, string> tags)
        {
            if (!tags.Any())
            {
                return null;
            }

            var tagsXmlBuilder = new StringBuilder();
            tagsXmlBuilder.Append(Invariant($"<{TagSetElementName}>"));
            foreach (var tag in tags ?? new Dictionary<string, string>())
            {
                var escapedKey = new XElement("ForEscapingOnly", tag.Key).LastNode.ToString();
                var escapedValue = tag.Value == null ? NullCanaryValue : new XElement("ForEscapingOnly", tag.Value).LastNode.ToString();
                tagsXmlBuilder.Append(Invariant($"<{TagEntryElementName} "));
                tagsXmlBuilder.Append(FormattableString.Invariant($"{TagEntryKeyAttributeName}=\"{escapedKey}\" {TagEntryValueAttributeName}=\"{escapedValue}\""));

                tagsXmlBuilder.Append("/>");
            }

            tagsXmlBuilder.Append(Invariant($"</{TagSetElementName}>"));
            var result = tagsXmlBuilder.ToString();
            return result;
        }

        /// <summary>
        /// Converts to list of items to a string/string dictionary with a numeric (specified)-based key and the element's <see cref="object.ToString"/>.
        /// </summary>
        /// <typeparam name="TElement">The type of the element.</typeparam>
        /// <param name="elements">The elements.</param>
        /// <param name="startAtOneInsteadOfZero">Optionally choose to start at 1/one instead of 0/zero; DEFAULT is 0/zero.</param>
        /// <returns>Ordinal dictionary of the elements.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "AtOne", Justification = NaosSuppressBecause.CA1702_CompoundWordsShouldBeCasedCorrectly_AnalyzerIsIncorrectlyDetectingCompoundWords)]
        public static IReadOnlyDictionary<string, string> ToOrdinalDictionary<TElement>(this IReadOnlyList<TElement> elements, bool startAtOneInsteadOfZero = false)
        {
            var result = new Dictionary<string, string>();
            if (elements != null)
            {
                for (var idx =  startAtOneInsteadOfZero ? 1 : 0;
                    idx < elements.Count;
                    idx++)
                {
                    result.Add(idx.ToString(CultureInfo.InvariantCulture), elements[idx]?.ToString());
                }
            }

            return result;
        }
    }
}