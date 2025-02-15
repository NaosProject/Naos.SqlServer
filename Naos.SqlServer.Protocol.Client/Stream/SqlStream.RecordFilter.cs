// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlStream.RecordFilter.cs" company="Naos Project">
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
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices.ComTypes;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Naos.CodeAnalysis.Recipes;
    using Naos.Database.Domain;
    using Naos.Recipes.RunWithRetry;
    using Naos.SqlServer.Domain;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Collection.Recipes;
    using OBeautifulCode.Compression;
    using OBeautifulCode.Database.Recipes;
    using OBeautifulCode.Enum.Recipes;
    using OBeautifulCode.Representation.System;
    using OBeautifulCode.Serialization;
    using OBeautifulCode.String.Recipes;
    using OBeautifulCode.Type;
    using OBeautifulCode.Type.Recipes;
    using static System.FormattableString;
    using SerializationFormat = OBeautifulCode.Serialization.SerializationFormat;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Acceptable given it creates the stream.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = NaosSuppressBecause.CA1711_IdentifiersShouldNotHaveIncorrectSuffix_TypeNameAddedAsSuffixForTestsWhereTypeIsPrimaryConcern)]
    public partial class SqlStream
    {
        private static readonly VersionMatchStrategy[] SupportedVersionMatchStrategies = new[]
        {
            VersionMatchStrategy.Any,
            VersionMatchStrategy.SpecifiedVersion,
        };

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = NaosSuppressBecause.CA1502_AvoidExcessiveComplexity_DisagreeWithAssessment)]
        private RecordFilterConvertedForStoredProcedure ConvertRecordFilter(
            RecordFilter recordFilter,
            SqlServerLocator sqlServerLocator)
        {
            recordFilter.VersionMatchStrategy
                .MustForArg(nameof(recordFilter))
                .BeElementIn(SupportedVersionMatchStrategies);

            recordFilter.TagMatchStrategy
                .MustForArg(Invariant($"{nameof(recordFilter)}.{nameof(recordFilter.TagMatchStrategy)}"))
                .BeEqualTo(TagMatchStrategy.RecordContainsAllQueryTags, "Currently only supported tag strategy.");

            var internalRecordIdsCsv = !recordFilter.InternalRecordIds?.Any() ?? true
                ? null
                : recordFilter.InternalRecordIds
                    .Select(_ => _.ToStringInvariantPreferred())
                    .Distinct()
                    .ToCsv();

            var tagIdsCsv = !recordFilter.Tags?.Any() ?? true
                ? null
                : this.GetIdsAddIfNecessaryTag(
                        sqlServerLocator,
                        recordFilter.Tags)
                    .Select(_ => _.ToStringInvariantPreferred())
                    .Distinct()
                    .ToCsv();

            var distinctIdentifierTypes = (recordFilter.Ids ?? new List<StringSerializedIdentifier>())
                .Select(_ => _.IdentifierType)
                .Distinct()
                .ToList();

            IdentifiedType GetIdentifiedType(
                TypeRepresentation typeToConvert)
            {
                return recordFilter.VersionMatchStrategy == VersionMatchStrategy.Any
                    ? this.GetIdsAddIfNecessaryTypeVersionless(sqlServerLocator, typeToConvert)
                    : this.GetIdsAddIfNecessaryType(sqlServerLocator, typeToConvert.ToWithAndWithoutVersion());
            }

            var typeToIdMap = distinctIdentifierTypes.ToDictionary(
                k => k,
                GetIdentifiedType);

            var identifierTypes = (recordFilter.IdTypes ?? new List<TypeRepresentation>())
                .Select(GetIdentifiedType)
                .ToList();
            var identifierTypeIdsCsv = !identifierTypes.Any()
                ? null
                : identifierTypes
                    .Select(
                        _ =>
                            recordFilter.VersionMatchStrategy == VersionMatchStrategy.Any
                                ? _.IdWithoutVersion.ToStringInvariantPreferred()
                                : _.IdWithVersion.ToStringInvariantPreferred())
                    .Distinct()
                    .ToCsv();

            var objectTypes = (recordFilter.ObjectTypes ?? new List<TypeRepresentation>())
                .Select(GetIdentifiedType)
                .ToList();
            var objectTypeIdsCsv = !objectTypes.Any()
                ? null
                : objectTypes
                    .Select(
                        _ =>
                            recordFilter.VersionMatchStrategy == VersionMatchStrategy.Any
                                ? _.IdWithoutVersion.ToStringInvariantPreferred()
                                : _.IdWithVersion.ToStringInvariantPreferred())
                    .Distinct()
                    .ToCsv();

            var deprecatedIdTypes = (recordFilter.DeprecatedIdTypes ?? new List<TypeRepresentation>())
                .Select(GetIdentifiedType)
                .ToList();
            var deprecatedIdTypeIdsCsv = !deprecatedIdTypes.Any()
                ? null
                : deprecatedIdTypes
                    .Select(
                        _ =>
                            recordFilter.VersionMatchStrategy == VersionMatchStrategy.Any
                                ? _.IdWithoutVersion.ToStringInvariantPreferred()
                                : _.IdWithVersion.ToStringInvariantPreferred())
                    .Distinct()
                    .ToCsv();

            var stringIdsToMatchXml = !recordFilter.Ids?.Any() ?? true
                ? null
                : recordFilter.Ids
                    .Select(
                        _ => new Tuple<string, int>(
                            _.StringSerializedId,
                            recordFilter.VersionMatchStrategy == VersionMatchStrategy.Any
                                ? typeToIdMap[_.IdentifierType].IdWithoutVersion
                                : typeToIdMap[_.IdentifierType].IdWithVersion))
                    .ToList()
                    .GetTagsXmlString();

            var result = new RecordFilterConvertedForStoredProcedure(
                internalRecordIdsCsv,
                identifierTypeIdsCsv,
                objectTypeIdsCsv,
                stringIdsToMatchXml,
                tagIdsCsv,
                recordFilter.TagMatchStrategy,
                recordFilter.VersionMatchStrategy,
                deprecatedIdTypeIdsCsv);
            return result;
        }
    }
}
