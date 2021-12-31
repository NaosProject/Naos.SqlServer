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
