// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlStream.StandardGetDistinctStringSerializedIdsOp.cs" company="Naos Project">
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
    using OBeautifulCode.Collection.Recipes;
    using OBeautifulCode.DateTime.Recipes;
    using OBeautifulCode.Serialization;
    using OBeautifulCode.String.Recipes;
    using OBeautifulCode.Type.Recipes;
    using static System.FormattableString;

    public partial class SqlStream
    {
        /// <inheritdoc />
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "internalRecordId", Justification = "Leaving for debugging and future use.")]
        public override IReadOnlyCollection<StringSerializedIdentifier> Execute(
            StandardGetDistinctStringSerializedIdsOp operation)
        {
            operation.MustForArg(nameof(operation)).NotBeNull();

            var sqlServerLocator = this.TryGetLocator(operation);

            var convertedRecordFilter = this.ConvertRecordFilter(operation.RecordFilter, sqlServerLocator);

            var storedProcOp = StreamSchema.Sprocs.GetDistinctStringSerializedIds.BuildExecuteStoredProcedureOp(
                this.Name,
                convertedRecordFilter);

            var sqlProtocol = this.BuildSqlOperationsProtocol(sqlServerLocator);
            var sprocResult = sqlProtocol.Execute(storedProcOp);

            var resultXml = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.GetDistinctStringSerializedIds.OutputParamName.StringIdentifiersOutputXml)].GetValueOfType<string>();
            var resultList = resultXml.GetTagsFromXmlString();
            var typeIdToTypeRepMap = resultList
               .ToDictionary(
                    k => k.Name,
                    v => this.GetTypeById(
                                  sqlServerLocator,
                                  int.Parse(v.Name, CultureInfo.InvariantCulture),
                                  true)
                             .WithVersion);

            var result = resultList
                        .Select(
                             _ => new StringSerializedIdentifier(
                                 _.Value,
                                 typeIdToTypeRepMap[_.Name]))
                        .ToList();

            return result;
        }
    }
}
