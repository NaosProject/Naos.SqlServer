// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlStream.StandardUpdateHandlingStatusForStreamOp.cs" company="Naos Project">
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
        private static readonly IReadOnlyCollection<HandlingStatus> AllHandlingStatusesExceptDisabledForStream =
            typeof(HandlingStatus)
               .GetAllPossibleEnumValues()
               .Cast<HandlingStatus>()
               .Except(
                    new[]
                    {
                        HandlingStatus.DisabledForStream,
                    })
               .ToList();

        /// <inheritdoc />
        public override void Execute(
            StandardUpdateHandlingStatusForStreamOp operation)
        {
            operation.MustForArg(nameof(operation)).NotBeNull();

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
                    tagIdsCsv,
                    false);

                var sqlProtocol = this.BuildSqlOperationsProtocol(sqlServerLocator);
                var sprocResult = sqlProtocol.Execute(storedProcOp);
                sprocResult.MustForOp(nameof(sprocResult)).NotBeNull();
            }
        }
    }
}
