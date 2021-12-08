﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlStream.HandlingStatus.cs" company="Naos Project">
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
    using OBeautifulCode.String.Recipes;
    using OBeautifulCode.Type.Recipes;
    using static System.FormattableString;

    public partial class SqlStream
    {
        private static readonly IReadOnlyCollection<HandlingStatus> AllHandlingStatusesExceptDisabledForStream = typeof(HandlingStatus)
            .GetAllPossibleEnumValues()
            .Cast<HandlingStatus>()
            .Except(
                new[]
                {
                    HandlingStatus.DisabledForStream,
                })
            .ToList();

        /// <inheritdoc />
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = NaosSuppressBecause.CA1506_AvoidExcessiveClassCoupling_DisagreeWithAssessment)]
        public override IReadOnlyCollection<HandlingStatus> Execute(
            StandardGetHandlingStatusOp operation)
        {
            throw new NotImplementedException("Not yet implemented");

            ////if (operation.HandlingStatusCompositionStrategy != null)
            ////{
            ////    operation
            ////       .HandlingStatusCompositionStrategy
            ////       .IgnoreCancel
            ////       .MustForArg(Invariant($"{nameof(StandardGetHandlingStatusOp)}.{nameof(HandlingStatusCompositionStrategy)}.{nameof(HandlingStatusCompositionStrategy.IgnoreCancel)}"))
            ////       .BeFalse(Invariant($"{nameof(HandlingStatusCompositionStrategy)}.{nameof(HandlingStatusCompositionStrategy.IgnoreCancel)} is not supported."));
            ////}

            ////if (operation.TagMatchStrategy != null)
            ////{
            ////    operation.TagMatchStrategy.ScopeOfFindSet
            ////             .MustForArg(Invariant($"{nameof(StandardGetHandlingStatusOp)}.{nameof(TagMatchStrategy)}.{nameof(TagMatchStrategy.ScopeOfFindSet)}"))
            ////             .BeEqualTo(Database.Domain.TagMatchScope.All);

            ////    operation.TagMatchStrategy.ScopeOfTarget
            ////             .MustForArg(Invariant($"{nameof(StandardGetHandlingStatusOp)}.{nameof(TagMatchStrategy)}.{nameof(TagMatchStrategy.ScopeOfTarget)}"))
            ////             .BeEqualTo(Database.Domain.TagMatchScope.Any);
            ////}

            ////var allLocators = this.ResourceLocatorProtocols.Execute(new GetAllResourceLocatorsOp());
            ////var statusPerLocator = new List<HandlingStatus>();
            ////foreach (var resourceLocator in allLocators)
            ////{
            ////    var sqlServerLocator = resourceLocator.ConfirmAndConvert<SqlServerLocator>();
            ////    var sqlProtocol = this.BuildSqlOperationsProtocol(sqlServerLocator);

            ////    var tagIdsCsv =
            ////        operation.TagsToMatch == null
            ////            ? null
            ////            : this.GetIdsAddIfNecessaryTag(sqlServerLocator, operation.TagsToMatch).Select(_ => _.ToStringInvariantPreferred()).ToCsv();

            ////    var op = StreamSchema.Sprocs.GetCompositeHandlingStatus.BuildExecuteStoredProcedureOp(
            ////        this.Name,
            ////        operation.Concern,
            ////        tagIdsCsv);

            ////    var sprocResult = sqlProtocol.Execute(op);

            ////    HandlingStatus status = sprocResult
            ////                                     .OutputParameters[StreamSchema.Sprocs.GetCompositeHandlingStatus.OutputParamName.Status.ToString()]
            ////                                     .GetValue<HandlingStatus>();
            ////    statusPerLocator.Add(status);
            ////}

            ////var result = statusPerLocator.ReduceToCompositeHandlingStatus(operation.HandlingStatusCompositionStrategy);

            ////return result;
        }

        /// <inheritdoc />
        public override IReadOnlyList<StreamRecordHandlingEntry> Execute(
            StandardGetHandlingHistoryOp operation)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override void Execute(
            StandardUpdateHandlingStatusForStreamOp operation)
        {
            var locators = this.ResourceLocatorProtocols.Execute(new GetAllResourceLocatorsOp());

            foreach (var locator in locators)
            {
                var sqlServerLocator = locator as SqlServerLocator
                                    ?? throw new NotSupportedException(
                                           Invariant(
                                               $"{nameof(GetResourceLocatorForUniqueIdentifierOp)} should return a {nameof(SqlServerLocator)} and returned {locator?.GetType().ToStringReadable()}."));

                var tagIdsCsv =
                    operation.Tags == null
                        ? null
                        : this.GetIdsAddIfNecessaryTag(sqlServerLocator, operation.Tags).Select(_ => _.ToStringInvariantPreferred()).ToCsv();

                var storedProcOp = StreamSchema.Sprocs.PutHandling.BuildExecuteStoredProcedureOp(
                    this.Name,
                    Concerns.StreamHandlingDisabledConcern,
                    operation.Details,
                    Concerns.GlobalBlockingRecordId,
                    operation.NewStatus,
                    operation.NewStatus == HandlingStatus.DisabledForStream
                        ? AllHandlingStatusesExceptDisabledForStream
                        : new[] { HandlingStatus.DisabledForStream },
                    tagIdsCsv);

                var sqlProtocol = this.BuildSqlOperationsProtocol(sqlServerLocator);
                var sprocResult = sqlProtocol.Execute(storedProcOp);
                sprocResult.MustForOp(nameof(sprocResult)).NotBeNull();
            }
        }

        /// <inheritdoc />
        public override void Execute(
            StandardUpdateHandlingStatusForRecordOp operation)
        {
            var sqlServerLocator = this.TryGetLocator(operation);

            var tagIdsCsv = operation.Tags == null
                ? null
                : this.GetIdsAddIfNecessaryTag(sqlServerLocator, operation.Tags).Select(_ => _.ToStringInvariantPreferred()).ToCsv();

            var storedProcOp = StreamSchema.Sprocs.PutHandling.BuildExecuteStoredProcedureOp(
                this.Name,
                operation.Concern,
                operation.Details,
                operation.InternalRecordId,
                operation.NewStatus,
                operation.AcceptableCurrentStatuses,
                tagIdsCsv);

            var sqlProtocol = this.BuildSqlOperationsProtocol(sqlServerLocator);

            var sprocResult = sqlProtocol.Execute(storedProcOp);

            sprocResult.MustForOp(nameof(sprocResult)).NotBeNull();
        }
    }
}
