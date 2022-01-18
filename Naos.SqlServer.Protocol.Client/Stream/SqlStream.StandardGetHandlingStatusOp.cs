// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlStream.StandardGetHandlingStatusOp.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Protocol.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Naos.CodeAnalysis.Recipes;
    using Naos.Database.Domain;
    using Naos.SqlServer.Domain;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Collection.Recipes;
    using OBeautifulCode.Enum.Recipes;
    using OBeautifulCode.Representation.System;
    using OBeautifulCode.String.Recipes;
    using OBeautifulCode.Type;
    using OBeautifulCode.Type.Recipes;
    using static System.FormattableString;

    public partial class SqlStream
    {
        /// <inheritdoc />
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = NaosSuppressBecause.CA1506_AvoidExcessiveClassCoupling_DisagreeWithAssessment)]
        public override IReadOnlyDictionary<long, HandlingStatus> Execute(
            StandardGetHandlingStatusOp operation)
        {
            operation.MustForArg(nameof(operation)).NotBeNull();

            var allLocators = this.ResourceLocatorProtocols.Execute(new GetAllResourceLocatorsOp());
            var statusPerLocator = new List<HandlingStatus>();
            foreach (var resourceLocator in allLocators)
            {
                var sqlServerLocator = resourceLocator.ConfirmAndConvert<SqlServerLocator>();
                var sqlProtocol = this.BuildSqlOperationsProtocol(sqlServerLocator);

                var internalRecordIdsCsv = operation.RecordFilter.InternalRecordIds.Select(_ => _.ToStringInvariantPreferred()).ToCsv();

                var tagIdsCsv =
                    operation.RecordFilter.Tags == null
                        ? null
                        : this.GetIdsAddIfNecessaryTag(sqlServerLocator, operation.RecordFilter.Tags).Select(_ => _.ToStringInvariantPreferred()).ToCsv();

                var distinctIdentifierTypes = operation.RecordFilter.Ids?.Select(_ => _.IdentifierType).Distinct().ToList() ?? new List<TypeRepresentation>();
                var typeToIdMap = distinctIdentifierTypes.ToDictionary(
                    k => k,
                    v => this.GetIdsAddIfNecessaryType(sqlServerLocator, v.ToWithAndWithoutVersion()));

                var identifierTypes = operation
                             .RecordFilter
                             .IdTypes
                             .Select(_ => this.GetIdsAddIfNecessaryType(sqlServerLocator, _.ToWithAndWithoutVersion()))
                             .ToList();
                var identifierTypeIdsCsv = identifierTypes
                                      .Select(
                                           _ =>
                                               operation.RecordFilter.VersionMatchStrategy == VersionMatchStrategy.Any
                                                   ? _.IdWithoutVersion.ToStringInvariantPreferred()
                                                   : _.IdWithVersion.ToStringInvariantPreferred())
                                      .ToCsv();

                var objectTypes = operation
                             .RecordFilter
                             .ObjectTypes
                             .Select(_ => this.GetIdsAddIfNecessaryType(sqlServerLocator, _.ToWithAndWithoutVersion()))
                             .ToList();
                var objectTypeIdsCsv = objectTypes
                                      .Select(
                                           _ =>
                                               operation.RecordFilter.VersionMatchStrategy == VersionMatchStrategy.Any
                                                   ? _.IdWithoutVersion.ToStringInvariantPreferred()
                                                   : _.IdWithVersion.ToStringInvariantPreferred())
                                      .ToCsv();

                var stringIdsToMatchXml =
                    operation.RecordFilter.Ids?
                             .Select(
                                  _ => new NamedValue<int>(
                                      _.StringSerializedId,
                                      operation.RecordFilter.VersionMatchStrategy == VersionMatchStrategy.Any
                                          ? typeToIdMap[_.IdentifierType].IdWithoutVersion
                                          : typeToIdMap[_.IdentifierType].IdWithVersion))
                             .ToList()
                             .GetTagsXmlString()
                 ?? TagConversionTool.EmptyTagSetXml;

                var op = StreamSchema.Sprocs.GetHandlingStatuses.BuildExecuteStoredProcedureOp(
                    this.Name,
                    operation.Concern,
                    internalRecordIdsCsv,
                    identifierTypeIdsCsv,
                    objectTypeIdsCsv,
                    stringIdsToMatchXml,
                    tagIdsCsv,
                    operation.RecordFilter.TagMatchStrategy);

                var sprocResult = sqlProtocol.Execute(op);

                HandlingStatus status = sprocResult
                                                 .OutputParameters[StreamSchema.Sprocs.GetCompositeHandlingStatus.OutputParamName.Status.ToString()]
                                                 .GetValue<HandlingStatus>();
                statusPerLocator.Add(status);
            }

            var result = statusPerLocator.ReduceToCompositeHandlingStatus(operation.HandlingStatusCompositionStrategy);

            return result;
        }
    }
}
