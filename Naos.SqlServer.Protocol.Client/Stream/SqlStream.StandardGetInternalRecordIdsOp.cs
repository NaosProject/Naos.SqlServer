// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlStream.StandardGetInternalRecordIdsOp.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Protocol.Client
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using Naos.CodeAnalysis.Recipes;
    using Naos.Database.Domain;
    using Naos.SqlServer.Domain;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.DateTime.Recipes;
    using OBeautifulCode.Serialization;
    using OBeautifulCode.String.Recipes;
    using static System.FormattableString;

    public partial class SqlStream
    {
        /// <inheritdoc />
        public override IReadOnlyCollection<long> Execute(
            StandardGetInternalRecordIdsOp operation)
        {
            operation.MustForArg(nameof(operation)).NotBeNull();

            var sqlServerLocator = this.TryGetLocator(operation);

            var convertedRecordFilter = this.ConvertRecordFilter(operation.RecordFilter, sqlServerLocator);

            var storedProcOp = StreamSchema.Sprocs.GetInternalRecordIds.BuildExecuteStoredProcedureOp(
                this.Name,
                convertedRecordFilter);

            var sqlProtocol = this.BuildSqlOperationsProtocol(sqlServerLocator);
            var sprocResult = sqlProtocol.Execute(storedProcOp);

            var resultString = sprocResult
                              .OutputParameters[nameof(StreamSchema.Sprocs.GetInternalRecordIds.OutputParamName.InternalRecordIdsCsvOutput)]
                              .GetValueOfType<string>();

            var resultList = resultString.FromCsv();
            var result = resultList
                        .Select(long.Parse)
                        .ToList();

            return result;
        }
    }
}
