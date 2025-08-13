// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlServerStandardStream.StandardGetInternalRecordIdsOp.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Protocol.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Naos.Database.Domain;
    using Naos.SqlServer.Domain;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.String.Recipes;
    using static System.FormattableString;

    public partial class SqlServerStandardStream
    {
        /// <inheritdoc />
        public override IReadOnlyCollection<long> Execute(
            StandardGetInternalRecordIdsOp operation)
        {
            operation.MustForArg(nameof(operation)).NotBeNull();

            var sqlServerLocator = this.TryGetLocator(operation);

            var convertedRecordFilter = this.ConvertRecordFilter(
                operation.RecordFilter,
                operation.RecordsToFilterCriteria,
                sqlServerLocator);

            var storedProcOp = StreamSchema.Sprocs.GetInternalRecordIds.BuildExecuteStoredProcedureOp(
                this.Name,
                convertedRecordFilter,
                operation.RecordsToFilterCriteria);

            var sqlProtocol = this.BuildSqlOperationsProtocol(sqlServerLocator);
            var sprocResult = sqlProtocol.Execute(storedProcOp);

            var resultString = sprocResult
                              .OutputParameters[nameof(StreamSchema.Sprocs.GetInternalRecordIds.OutputParamName.InternalRecordIdsCsvOutput)]
                              .GetValueOfType<string>();

            var resultList = resultString.FromCsv();
            var result = resultList
                        .Select(long.Parse)
                        .ToList();

            if (!result.Any())
            {
                switch (operation.RecordNotFoundStrategy)
                {
                    case RecordNotFoundStrategy.ReturnDefault:
                        return Array.Empty<long>();
                    case RecordNotFoundStrategy.Throw:
                        throw new InvalidOperationException(
                            Invariant(
                                $"Expected stream {this.StreamRepresentation} to contain a matching record for {operation}, none was found and {nameof(operation.RecordNotFoundStrategy)} is '{operation.RecordNotFoundStrategy}'."));
                    default:
                        throw new NotSupportedException(
                            Invariant($"{nameof(RecordNotFoundStrategy)} {operation.RecordNotFoundStrategy} is not supported."));
                }
            }

            return result;
        }
    }
}
