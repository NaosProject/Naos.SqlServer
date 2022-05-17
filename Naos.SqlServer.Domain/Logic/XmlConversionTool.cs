// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlConversionTool.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Serialization;
    using Naos.CodeAnalysis.Recipes;
    using OBeautifulCode.String.Recipes;
    using OBeautifulCode.Type;
    using static System.FormattableString;

    /// <summary>
    /// Tools for dealing with XML in SQL.
    /// </summary>
    public static class XmlConversionTool
    {
        private static readonly XmlSerializer EntrySerializer = new XmlSerializer(typeof(SerializableEntrySet));

        private static readonly XmlSerializer TagXmlSerializer = new XmlSerializer(typeof(SerializableTagSet));

        private static readonly XmlSerializerNamespaces XmlSerializerNamespaces = new XmlSerializerNamespaces(
            new[]
            {
                new XmlQualifiedName(string.Empty, string.Empty),
            });

        private static readonly XmlReaderSettings XmlReaderSettings = new XmlReaderSettings
                                                                      {
                                                                          IgnoreWhitespace = true,
                                                                      };

        private static readonly XmlWriterSettings XmlWriterSettings = new XmlWriterSettings
                                                                      {
                                                                          Indent = true,
                                                                          OmitXmlDeclaration = true,
                                                                      };

        /// <summary>
        /// Root element name of the tag set.
        /// </summary>
        public const string TagSetElementName = "Tags";

        /// <summary>
        /// Element name of each entry in the tag set.
        /// </summary>
        public const string TagEntryElementName = "Tag";

        /// <summary>
        /// Root element name of the entry set.
        /// </summary>
        public const string EntrySetElementName = "Entries";

        /// <summary>
        /// Element name of each entry in the entry set.
        /// </summary>
        public const string EntryElementName = "Entry";

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
        /// The empty tag set as XML.
        /// </summary>
        public static readonly string EmptyTagSetXml = Invariant($"<{TagSetElementName}></{TagSetElementName}>");

        /// <summary>
        /// Gets the handling entries from XML string.
        /// </summary>
        /// <param name="handlingEntriesAsXml">The handling entries as XML.</param>
        /// <returns>SerializableEntrySet.</returns>
        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = NaosSuppressBecause.CA2202_DoNotDisposeObjectsMultipleTimes_AnalyzerIsIncorrectlyFlaggingObjectAsBeingDisposedMultipleTimes)]
        public static SerializableEntrySet GetHandlingEntriesFromXmlString(
            this string handlingEntriesAsXml)
        {
            using (var stringReader = new StringReader(handlingEntriesAsXml))
            using (var reader = XmlReader.Create(stringReader, XmlReaderSettings))
            {
                var result = (SerializableEntrySet)EntrySerializer.Deserialize(reader);
                return result;
            }
        }

        /// <summary>
        /// Gets the tag set as a <see cref="IReadOnlyDictionary{TKey,TValue}"/> from the provided XML as a string.
        /// </summary>
        /// <param name="tagsAsXml">The tags in XML as a string.</param>
        /// <returns><see cref="IReadOnlyDictionary{TKey,TValue}"/> from the provided XML.</returns>
        public static IReadOnlyList<NamedValue<string>> GetTagsFromXmlString(
            this string tagsAsXml)
        {
            if (tagsAsXml == null)
            {
                return new List<NamedValue<string>>();
            }

            var result = new List<NamedValue<string>>();

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

                result.Add(new NamedValue<string>(key, value));
            }

            return result;
        }

        /// <summary>
        /// Gets the tag set converted to an XML string.
        /// </summary>
        /// <param name="tags">The tags.</param>
        /// <returns>The converted XML.</returns>
        public static string GetTagsXmlString(
            this IReadOnlyCollection<NamedValue<string>> tags)
        {
            if (tags == null)
            {
                return null;
            }

            if (!tags.Any())
            {
                return EmptyTagSetXml;
            }

            var tagsXmlBuilder = new StringBuilder();
            tagsXmlBuilder.Append(Invariant($"<{TagSetElementName}>"));
            foreach (var tag in tags ?? new List<NamedValue<string>>())
            {
                var escapedKey = new XElement("ForEscapingOnly", tag.Name).LastNode.ToString();
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
        /// Gets the tag set converted to an XML string.
        /// </summary>
        /// <param name="tags">The tags.</param>
        /// <returns>The converted XML.</returns>
        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = NaosSuppressBecause.CA2202_DoNotDisposeObjectsMultipleTimes_AnalyzerIsIncorrectlyFlaggingObjectAsBeingDisposedMultipleTimes)]
        public static string GetTagsXmlString(
            this IReadOnlyCollection<Tuple<string, int>> tags)
        {
            if (tags == null)
            {
                return null;
            }

            if (!tags.Any())
            {
                return EmptyTagSetXml;
            }

            string result = null;
            var stringBuilder = new StringBuilder();
            using (var stringWriter = new StringWriter(stringBuilder, CultureInfo.InvariantCulture))
            using (var writer = XmlWriter.Create(stringWriter, XmlWriterSettings))
            {
                var serializableTagSetItems = new SerializableTagSet
                                              {
                                                  Tags = tags.Select(
                                                                  _ => new SerializableTagSetItem
                                                                       {
                                                                           Key = _.Item1,
                                                                           Value = _.Item2.ToStringInvariantPreferred(),
                                                                       })
                                                             .Distinct()
                                                             .ToArray(),
                                              };

                TagXmlSerializer.Serialize(
                    writer,
                    serializableTagSetItems,
                    XmlSerializerNamespaces);

                result = stringBuilder.ToString();
            }

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
            if (elements == null)
            {
                return null;
            }

            var result = new Dictionary<string, string>();
            for (var idx =  startAtOneInsteadOfZero ? 1 : 0;
                idx < elements.Count + (startAtOneInsteadOfZero ? 1 : 0);
                idx++)
            {
                result.Add(idx.ToString(CultureInfo.InvariantCulture), elements[idx - (startAtOneInsteadOfZero ? 1 : 0)]?.ToString());
            }

            return result;
        }

        /// <summary>
        /// Model object that is only used for the <see cref="XmlSerializer"/> to store a set of tags to send in XML to SQL Server.
        /// </summary>
        [Serializable]
        [XmlRoot(TagSetElementName)]
        public class SerializableTagSet
        {
            /// <summary>
            /// Gets or sets the tags.
            /// </summary>
            [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "It's what the XML serializer plays nice with.")]
            [XmlElement(TagEntryElementName)] // Do NOT use XmlArray/XmlArrayItem attributes as it double nests...
            public SerializableTagSetItem[] Tags { get; set; }
        }

        /// <summary>
        /// Model object that is only used for the <see cref="XmlSerializer"/> to store a single tag entry in <see cref="SerializableTagSet"/>.
        /// </summary>
        [Serializable]
        [XmlRoot(TagEntryElementName)]
        public class SerializableTagSetItem
        {
            /// <summary>
            /// Gets or sets the key.
            /// </summary>
            [XmlAttribute(TagEntryKeyAttributeName)]
            public string Key { get; set; }

            /// <summary>
            /// Gets or sets the value.
            /// </summary>
            [XmlAttribute(TagEntryValueAttributeName)]
            public string Value { get; set; }
        }

        /// <summary>
        /// Model object that is only used for the <see cref="XmlSerializer"/> to store a set of entries to send in XML to SQL Server.
        /// </summary>
        [Serializable]
        [XmlRoot(EntrySetElementName)]
        public class SerializableEntrySet
        {
            /// <summary>
            /// Gets or sets the Entrys.
            /// </summary>
            [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "It's what the XML serializer plays nice with.")]
            [XmlElement(EntryElementName)] // Do NOT use XmlArray/XmlArrayItem attributes as it double nests...
            public SerializableEntrySetItem[] Entries { get; set; }
        }

        /// <summary>
        /// Model object that is only used for the <see cref="XmlSerializer"/> to store a single entry entry in <see cref="SerializableEntrySet"/>.
        /// </summary>
        [Serializable]
        [XmlRoot(EntryElementName)]
        public class SerializableEntrySetItem
        {
            /// <summary>
            /// Gets or sets the identifier.
            /// </summary>
            [XmlElement(nameof(Id))]
            public long Id { get; set; }

            /// <summary>
            /// Gets or sets the internal record identifier.
            /// </summary>
            [XmlElement(nameof(RecordId))]
            public long RecordId { get; set; }

            /// <summary>
            /// Gets or sets the record handling concern.
            /// </summary>
            [XmlElement(nameof(Concern))]
            public string Concern { get; set; }

            /// <summary>
            /// Gets or sets the record handling status.
            /// </summary>
            [XmlElement(nameof(Status))]
            public string Status { get; set; }

            /// <summary>
            /// Gets or sets the record handling details.
            /// </summary>
            [XmlElement(nameof(Details))]
            public string Details { get; set; }

            /// <summary>
            /// Gets or sets the record handling tag identifiers as CSV.
            /// </summary>
            [XmlElement(nameof(TagIdsCsv))]
            public string TagIdsCsv { get; set; }

            /// <summary>
            /// Gets or sets the record handling created <see cref="DateTime"/> in UTC format.
            /// </summary>
            [XmlElement(nameof(RecordCreatedUtc))]
            public DateTime RecordCreatedUtc { get; set; }
        }
    }
}