// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlStream.StandardGetHandlingHistoryOp.cs" company="Naos Project">
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
    using OBeautifulCode.DateTime.Recipes;
    using OBeautifulCode.Enum.Recipes;
    using OBeautifulCode.Representation.System;
    using OBeautifulCode.Serialization;
    using OBeautifulCode.String.Recipes;
    using OBeautifulCode.Type.Recipes;
    using static System.FormattableString;

    public partial class SqlStream
    {
        /// <inheritdoc />
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = NaosSuppressBecause.CA1506_AvoidExcessiveClassCoupling_DisagreeWithAssessment)]
        public override IReadOnlyList<StreamRecordHandlingEntry> Execute(
            StandardGetHandlingHistoryOp operation)
        {
            operation.MustForArg(nameof(operation)).NotBeNull();

            var sqlServerLocator = this.TryGetLocator(operation);

            var sqlProtocol = this.BuildSqlOperationsProtocol(sqlServerLocator);

            var storedProcWithVersionOp = StreamSchema.Sprocs.GetHandlingHistory.BuildExecuteStoredProcedureOp(
                this.Name,
                operation.Concern,
                operation.InternalRecordId);

            var sprocResultWithVersion = sqlProtocol.Execute(storedProcWithVersionOp);
            var handlingEntriesAsXml = sprocResultWithVersion
                           .OutputParameters[nameof(StreamSchema.Sprocs.GetHandlingHistory.OutputParamName.EntriesXml)]
                           .GetValueOfType<string>();
            var handlingEntries = handlingEntriesAsXml.GetHandlingEntriesFromXmlString();
            var minRecordCreatedUtc = handlingEntries.Entries.Min(_ => _.RecordCreatedUtc);
            var entriesWithAvailableInitial = new[]
                                              {
                                                  new XmlConversionTool.SerializableEntrySetItem
                                                  {
                                                      Concern = operation.Concern,
                                                      Details = "Available by existence of record.",
                                                      Id = 0,
                                                      RecordCreatedUtc = minRecordCreatedUtc,
                                                      RecordId = operation.InternalRecordId,
                                                      Status = HandlingStatus.AvailableByDefault.ToString(),
                                                      TagIdsCsv = null,
                                                  },
                                              }.Concat(handlingEntries.Entries)
                                               .ToList();
            var handlingEntryIdToTagsMap = entriesWithAvailableInitial
                                          .ToDictionary(
                                               k => k.Id,
                                               v =>
                                                   this.GetTagsByIds(
                                                       sqlServerLocator,
                                                       v.TagIdsCsv.FromCsv().Select(long.Parse).ToList()));

            var result = entriesWithAvailableInitial.Select(
                _ =>
                {
                    var tags = handlingEntryIdToTagsMap[_.Id];
                    var handlingStatus = _.Status.ToEnum<HandlingStatus>();
                    var utcTime = _.RecordCreatedUtc.ToUtc();
                    var entry = new StreamRecordHandlingEntry(
                        _.Id,
                        _.RecordId,
                        _.Concern,
                        handlingStatus,
                        tags,
                        _.Details,
                        utcTime);
                    return entry;
                }).ToList();

            return result;
        }
    }
}
