// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlStream.RecordFilter.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Protocol.Client
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Naos.CodeAnalysis.Recipes;
    using Naos.Database.Domain;
    using Naos.SqlServer.Domain;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Collection.Recipes;
    using OBeautifulCode.Representation.System;
    using OBeautifulCode.String.Recipes;
    using OBeautifulCode.Type;
    using static System.FormattableString;

    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Acceptable given it creates the stream.")]
    [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = NaosSuppressBecause.CA1711_IdentifiersShouldNotHaveIncorrectSuffix_TypeNameAddedAsSuffixForTestsWhereTypeIsPrimaryConcern)]
    public partial class SqlStream
    {
        private static readonly VersionMatchStrategy[] SupportedVersionMatchStrategies = new[]
        {
            VersionMatchStrategy.Any,
            VersionMatchStrategy.SpecifiedVersion,
        };

        private static readonly RecordsToFilterSelectionStrategy[] SupportedRecordsToFilterSelectionStrategy = new[]
        {
            RecordsToFilterSelectionStrategy.All,
            RecordsToFilterSelectionStrategy.LatestById,
            RecordsToFilterSelectionStrategy.LatestByIdAndObjectType,
        };

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = NaosSuppressBecause.CA1502_AvoidExcessiveComplexity_DisagreeWithAssessment)]
        private RecordFilterConvertedForStoredProcedure ConvertRecordFilter(
            RecordFilter recordFilter,
            RecordsToFilterCriteria recordsToFilterCriteria,
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

            // Note that this method is not the ideal place to validate RecordsToFilterCriteria
            // (that's all we're doing here with that object)
            // but this is the easiest way to guarantee it is validated anytime a RecordFilter is used.
            if (recordsToFilterCriteria != null)
            {
                recordsToFilterCriteria.RecordsToFilterSelectionStrategy
                    .MustForArg(Invariant($"{nameof(recordsToFilterCriteria)}.{nameof(RecordsToFilterCriteria.RecordsToFilterSelectionStrategy)}"))
                    .BeElementIn(SupportedRecordsToFilterSelectionStrategy);

                recordsToFilterCriteria.VersionMatchStrategy
                    .MustForArg(Invariant($"{nameof(recordsToFilterCriteria)}.{nameof(RecordsToFilterCriteria.VersionMatchStrategy)}"))
                    .BeElementIn(SupportedVersionMatchStrategies);
            }

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
