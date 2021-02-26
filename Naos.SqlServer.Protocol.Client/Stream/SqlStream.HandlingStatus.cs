// --------------------------------------------------------------------------------------------------------------------
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
    using Naos.Protocol.Domain;
    using Naos.SqlServer.Domain;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Collection.Recipes;
    using OBeautifulCode.Enum.Recipes;
    using OBeautifulCode.String.Recipes;
    using OBeautifulCode.Type.Recipes;
    using static System.FormattableString;

    public partial class SqlStream
    {
        /// <inheritdoc />
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = NaosSuppressBecause.CA1506_AvoidExcessiveClassCoupling_DisagreeWithAssessment)]
        public override HandlingStatus Execute(
            GetHandlingStatusOfRecordSetByTagOp operation)
        {
            if (operation.HandlingStatusCompositionStrategy != null)
            {
                operation
                   .HandlingStatusCompositionStrategy
                   .IgnoreCancel
                   .MustForArg(Invariant($"{nameof(GetHandlingStatusOfRecordSetByTagOp)}.{nameof(HandlingStatusCompositionStrategy)}.{nameof(HandlingStatusCompositionStrategy.IgnoreCancel)}"))
                   .BeFalse(Invariant($"{nameof(HandlingStatusCompositionStrategy)}.{nameof(HandlingStatusCompositionStrategy.IgnoreCancel)} is not supported."));
            }

            if (operation.TagMatchStrategy != null)
            {
                operation.TagMatchStrategy.ScopeOfFindSet
                         .MustForArg(Invariant($"{nameof(GetHandlingStatusOfRecordSetByTagOp)}.{nameof(TagMatchStrategy)}.{nameof(TagMatchStrategy.ScopeOfFindSet)}"))
                         .BeEqualTo(Database.Domain.TagMatchScope.All);

                operation.TagMatchStrategy.ScopeOfTarget
                         .MustForArg(Invariant($"{nameof(GetHandlingStatusOfRecordSetByTagOp)}.{nameof(TagMatchStrategy)}.{nameof(TagMatchStrategy.ScopeOfTarget)}"))
                         .BeEqualTo(Database.Domain.TagMatchScope.Any);
            }

            var allLocators = this.ResourceLocatorProtocols.Execute(new GetAllResourceLocatorsOp());
            var statusPerLocator = new List<HandlingStatus>();
            foreach (var resourceLocator in allLocators)
            {
                var sqlServerLocator = resourceLocator.ConfirmAndConvert<SqlServerLocator>();
                var sqlProtocol = this.BuildSqlOperationsProtocol(sqlServerLocator);

                var tagIdsCsv =
                    operation.TagsToMatch == null
                        ? null
                        : this.GetIdsAddIfNecessaryTag(sqlServerLocator, operation.TagsToMatch).Select(_ => _.ToStringInvariantPreferred()).ToCsv();

                var op = StreamSchema.Sprocs.GetCompositeHandlingStatus.BuildExecuteStoredProcedureOp(
                    this.Name,
                    operation.Concern,
                    tagIdsCsv);

                var sprocResult = sqlProtocol.Execute(op);

                HandlingStatus status = sprocResult
                                                 .OutputParameters[StreamSchema.Sprocs.GetCompositeHandlingStatus.OutputParamName.Status.ToString()]
                                                 .GetValue<HandlingStatus>();
                statusPerLocator.Add(status);
            }

            var result = statusPerLocator.ReduceToCompositeHandlingStatus(operation.HandlingStatusCompositionStrategy);
            return result;
        }

        /// <inheritdoc />
        public override IReadOnlyList<StreamRecordHandlingEntry> Execute(
            GetHandlingHistoryOfRecordOp operation)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override HandlingStatus Execute(
            GetHandlingStatusOfRecordsByIdOp operation)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override void Execute(
            BlockRecordHandlingOp operation)
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
                    Concerns.RecordHandlingConcern,
                    operation.Details,
                    Concerns.GlobalBlockingRecordId,
                    HandlingStatus.Blocked,
                    typeof(HandlingStatus).GetAllPossibleEnumValues()
                                          .Cast<HandlingStatus>()
                                          .Except(
                                               new[]
                                               {
                                                   HandlingStatus.Blocked,
                                               })
                                          .ToList(),
                    tagIdsCsv);

                var sqlProtocol = this.BuildSqlOperationsProtocol(sqlServerLocator);
                var sprocResult = sqlProtocol.Execute(storedProcOp);
                sprocResult.MustForOp(nameof(sprocResult)).NotBeNull();
            }
        }

        /// <inheritdoc />
        public override void Execute(
            CancelBlockedRecordHandlingOp operation)
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
                    Concerns.RecordHandlingConcern,
                    operation.Details,
                    Concerns.GlobalBlockingRecordId,
                    HandlingStatus.Requested,
                    new[]
                    {
                        HandlingStatus.Blocked,
                    },
                    tagIdsCsv);

                var sqlProtocol = this.BuildSqlOperationsProtocol(sqlServerLocator);
                var sprocResult = sqlProtocol.Execute(storedProcOp);
                sprocResult.MustForOp(nameof(sprocResult)).NotBeNull();
            }
        }

        /// <inheritdoc />
        public override void Execute(
            CancelHandleRecordExecutionRequestOp operation)
        {
            var sqlServerLocator = this.TryGetLocator(operation);

            var tagIdsCsv =
                operation.Tags == null
                    ? null
                    : this.GetIdsAddIfNecessaryTag(sqlServerLocator, operation.Tags).Select(_ => _.ToStringInvariantPreferred()).ToCsv();

            var storedProcOp = StreamSchema.Sprocs.PutHandling.BuildExecuteStoredProcedureOp(
                this.Name,
                operation.Concern,
                operation.Details,
                operation.Id,
                HandlingStatus.Canceled,
                new[] { HandlingStatus.Requested, HandlingStatus.Running, HandlingStatus.Failed },
                tagIdsCsv);

            var sqlProtocol = this.BuildSqlOperationsProtocol(sqlServerLocator);
            var sprocResult = sqlProtocol.Execute(storedProcOp);
            sprocResult.MustForOp(nameof(sprocResult)).NotBeNull();
        }

        /// <inheritdoc />
        public override void Execute(
            CancelRunningHandleRecordExecutionOp operation)
        {
            var sqlServerLocator = this.TryGetLocator(operation);
            var tagIdsCsv =
                operation.Tags == null
                    ? null
                    : this.GetIdsAddIfNecessaryTag(sqlServerLocator, operation.Tags).Select(_ => _.ToStringInvariantPreferred()).ToCsv();

            var storedProcOp = StreamSchema.Sprocs.PutHandling.BuildExecuteStoredProcedureOp(
                this.Name,
                operation.Concern,
                operation.Details,
                operation.Id,
                HandlingStatus.CanceledRunning,
                new[] { HandlingStatus.Running },
                tagIdsCsv);

            var sqlProtocol = this.BuildSqlOperationsProtocol(sqlServerLocator);
            var sprocResult = sqlProtocol.Execute(storedProcOp);
            sprocResult.MustForOp(nameof(sprocResult)).NotBeNull();
        }

        /// <inheritdoc />
        public override void Execute(
            CompleteRunningHandleRecordExecutionOp operation)
        {
            var sqlServerLocator = this.TryGetLocator(operation);
            var tagIdsCsv =
                operation.Tags == null
                    ? null
                    : this.GetIdsAddIfNecessaryTag(sqlServerLocator, operation.Tags).Select(_ => _.ToStringInvariantPreferred()).ToCsv();

            var storedProcOp = StreamSchema.Sprocs.PutHandling.BuildExecuteStoredProcedureOp(
                this.Name,
                operation.Concern,
                operation.Details,
                operation.Id,
                HandlingStatus.Completed,
                new[] { HandlingStatus.Running },
                tagIdsCsv);

            var sqlProtocol = this.BuildSqlOperationsProtocol(sqlServerLocator);
            var sprocResult = sqlProtocol.Execute(storedProcOp);
            sprocResult.MustForOp(nameof(sprocResult)).NotBeNull();
        }

        /// <inheritdoc />
        public override void Execute(
            FailRunningHandleRecordExecutionOp operation)
        {
            var sqlServerLocator = this.TryGetLocator(operation);
            var tagIdsCsv =
                operation.Tags == null
                    ? null
                    : this.GetIdsAddIfNecessaryTag(sqlServerLocator, operation.Tags).Select(_ => _.ToStringInvariantPreferred()).ToCsv();

            var storedProcOp = StreamSchema.Sprocs.PutHandling.BuildExecuteStoredProcedureOp(
                this.Name,
                operation.Concern,
                operation.Details,
                operation.Id,
                HandlingStatus.Failed,
                new[] { HandlingStatus.Running },
                tagIdsCsv);

            var sqlProtocol = this.BuildSqlOperationsProtocol(sqlServerLocator);
            var sprocResult = sqlProtocol.Execute(storedProcOp);
            sprocResult.MustForOp(nameof(sprocResult)).NotBeNull();
        }

        /// <inheritdoc />
        public override void Execute(
            SelfCancelRunningHandleRecordExecutionOp operation)
        {
            var sqlServerLocator = this.TryGetLocator(operation);
            var tagIdsCsv =
                operation.Tags == null
                    ? null
                    : this.GetIdsAddIfNecessaryTag(sqlServerLocator, operation.Tags).Select(_ => _.ToStringInvariantPreferred()).ToCsv();

            var storedProcOp = StreamSchema.Sprocs.PutHandling.BuildExecuteStoredProcedureOp(
                this.Name,
                operation.Concern,
                operation.Details,
                operation.Id,
                HandlingStatus.SelfCanceledRunning,
                new[] { HandlingStatus.Running },
                tagIdsCsv);

            var sqlProtocol = this.BuildSqlOperationsProtocol(sqlServerLocator);
            var sprocResult = sqlProtocol.Execute(storedProcOp);
            sprocResult.MustForOp(nameof(sprocResult)).NotBeNull();
        }

        /// <inheritdoc />
        public override void Execute(
            RetryFailedHandleRecordExecutionOp operation)
        {
            var sqlServerLocator = this.TryGetLocator(operation);
            var tagIdsCsv =
                operation.Tags == null
                    ? null
                    : this.GetIdsAddIfNecessaryTag(sqlServerLocator, operation.Tags).Select(_ => _.ToStringInvariantPreferred()).ToCsv();

            var storedProcOp = StreamSchema.Sprocs.PutHandling.BuildExecuteStoredProcedureOp(
                this.Name,
                operation.Concern,
                operation.Details,
                operation.Id,
                HandlingStatus.RetryFailed,
                new[] { HandlingStatus.Failed },
                tagIdsCsv);

            var sqlProtocol = this.BuildSqlOperationsProtocol(sqlServerLocator);
            var sprocResult = sqlProtocol.Execute(storedProcOp);
            sprocResult.MustForOp(nameof(sprocResult)).NotBeNull();
        }
    }
}
