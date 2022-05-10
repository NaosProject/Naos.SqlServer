// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlStream.StandardGetHandlingStatusOp.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Protocol.Client
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Maintainability",
            "CA1506:AvoidExcessiveClassCoupling",
            Justification = NaosSuppressBecause.CA1506_AvoidExcessiveClassCoupling_DisagreeWithAssessment)]
        public override IReadOnlyDictionary<long, HandlingStatus> Execute(
            StandardGetHandlingStatusOp operation)
        {
            operation.MustForArg(nameof(operation)).NotBeNull();

            var sqlServerLocator = this.TryGetLocator(operation);

            var sqlProtocol = this.BuildSqlOperationsProtocol(sqlServerLocator);
            var convertedRecordFilter = this.ConvertRecordFilter(operation.RecordFilter, sqlServerLocator);

            var handlingTagsCsv = !operation.HandlingFilter.Tags?.Any() ?? true
                ? null
                : this.GetIdsAddIfNecessaryTag(
                           sqlServerLocator,
                           operation.HandlingFilter.Tags)
                      .Select(_ => _.ToStringInvariantPreferred())
                      .Distinct()
                      .ToCsv();
            var op = StreamSchema.Sprocs.GetHandlingStatuses.BuildExecuteStoredProcedureOp(
                this.Name,
                operation.Concern,
                convertedRecordFilter,
                operation.HandlingFilter.CurrentHandlingStatuses,
                handlingTagsCsv);

            var sprocResult = sqlProtocol.Execute(op);

            var outputParameter = sprocResult
               .OutputParameters[StreamSchema.Sprocs.GetHandlingStatuses.OutputParamName.RecordIdHandlingStatusXml.ToString()];
            var recordIdStatusXml = outputParameter.GetValueOfType<string>();

            Dictionary<long, HandlingStatus> result;
            if (string.IsNullOrEmpty(recordIdStatusXml))
            {
                result = new Dictionary<long, HandlingStatus>();
            }
            else
            {
                var tags = recordIdStatusXml.GetTagsFromXmlString();
                result = tags.ToDictionary(
                    k => long.Parse(k.Name, CultureInfo.InvariantCulture),
                    v => v.Value?.ToEnum<HandlingStatus>() ?? HandlingStatus.AvailableByDefault);
            }

            return result;
        }
    }
}
