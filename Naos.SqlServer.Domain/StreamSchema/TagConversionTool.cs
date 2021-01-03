// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TagConversionTool.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;
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
                var escapedValue = tag.Value == null ? null : new XElement("ForEscapingOnly", tag.Value).LastNode.ToString();
                tagsXmlBuilder.Append(Invariant($"<{TagEntryElementName} "));
                if (escapedValue == null)
                {
                    tagsXmlBuilder.Append(FormattableString.Invariant($"{TagEntryKeyAttributeName}=\"{escapedKey}\" {TagEntryValueAttributeName}=null"));
                }
                else
                {
                    tagsXmlBuilder.Append(FormattableString.Invariant($"{TagEntryKeyAttributeName}=\"{escapedKey}\" {TagEntryValueAttributeName}=\"{escapedValue}\""));
                }

                tagsXmlBuilder.Append("/>");
            }

            tagsXmlBuilder.Append(Invariant($"</{TagSetElementName}>"));
            var result = tagsXmlBuilder.ToString();
            return result;
        }
    }
}