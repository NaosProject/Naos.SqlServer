// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlStream.Create.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Protocol.Client
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Naos.Database.Domain;
    using Naos.Protocol.Domain;
    using Naos.SqlServer.Domain;
    using OBeautifulCode.Collection.Recipes;
    using OBeautifulCode.Database.Recipes;
    using static System.FormattableString;

    public partial class SqlStream : IReturningProtocol<UpdateStreamStoredProceduresOp, UpdateStreamStoredProceduresResult>
    {
        /// <inheritdoc />
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Acceptable given it creates the streams.")]
        public override CreateStreamResult Execute(
            CreateStreamOp operation)
        {
            var allLocators = this.ResourceLocatorProtocols.Execute(new GetAllResourceLocatorsOp());
            var alreadyExisted = false;
            var wasCreated = true;
            foreach (var locator in allLocators)
            {
                if (locator is SqlServerLocator sqlLocator)
                {
                    using (var connection = sqlLocator.OpenSqlConnection(this.DefaultConnectionTimeout))
                    {
                        var streamAlreadyExists = connection.HasAtLeastOneRowWhenReading(
                            Invariant($"select * from sys.schemas where name = '{this.Name}'"));

                        alreadyExisted = alreadyExisted || streamAlreadyExists;
                        if (streamAlreadyExists)
                        {
                            switch (operation.ExistingStreamEncounteredStrategy)
                            {
                                case ExistingStreamEncounteredStrategy.Overwrite:
                                    throw new NotSupportedException(FormattableString.Invariant(
                                        $"Overwriting streams is not currently supported; stream '{this.Name}' already exists, {nameof(operation)}.{nameof(operation.ExistingStreamEncounteredStrategy)} was set to {ExistingStreamEncounteredStrategy.Overwrite}."));
                                case ExistingStreamEncounteredStrategy.Throw:
                                    throw new InvalidDataException(FormattableString.Invariant($"Stream '{this.Name}' already exists, {nameof(operation)}.{nameof(operation.ExistingStreamEncounteredStrategy)} was set to {ExistingStreamEncounteredStrategy.Throw}."));
                                case ExistingStreamEncounteredStrategy.Skip:
                                    wasCreated = false;
                                    break;
                            }
                        }
                        else
                        {
                            var creationScripts = new[]
                                                  {
                                                      StreamSchema.BuildCreationScriptForSchema(this.Name),
                                                      StreamSchema.Tables.NextUniqueLong.BuildCreationScript(this.Name),
                                                      StreamSchema.Tables.TypeWithoutVersion.BuildCreationScript(this.Name),
                                                      StreamSchema.Tables.TypeWithVersion.BuildCreationScript(this.Name),
                                                      StreamSchema.Tables.SerializerRepresentation.BuildCreationScript(this.Name),
                                                      StreamSchema.Tables.Tag.BuildCreationScript(this.Name),
                                                      StreamSchema.Tables.Record.BuildCreationScript(this.Name),
                                                      StreamSchema.Tables.RecordTag.BuildCreationScript(this.Name),
                                                      StreamSchema.Tables.Handling.BuildCreationScript(this.Name),
                                                      StreamSchema.Tables.HandlingTag.BuildCreationScript(this.Name),
                                                      StreamSchema.Tables.CompositeHandlingStatusSortOrder.BuildCreationScript(this.Name),
                                                      StreamSchema.Funcs.GetTagsTableVariableFromTagsXml.BuildCreationScript(this.Name),
                                                      StreamSchema.Funcs.GetTagsTableVariableFromTagIdsXml.BuildCreationScript(this.Name),
                                                      StreamSchema.Sprocs.GetIdAddIfNecessaryTypeWithoutVersion.BuildCreationScript(this.Name),
                                                      StreamSchema.Sprocs.GetIdAddIfNecessaryTypeWithVersion.BuildCreationScript(this.Name),
                                                      StreamSchema.Sprocs.GetTypeFromId.BuildCreationScript(this.Name),
                                                      StreamSchema.Sprocs.GetIdAddIfNecessarySerializerRepresentation.BuildCreationScript(this.Name),
                                                      StreamSchema.Sprocs.GetSerializerRepresentationFromId.BuildCreationScript(this.Name),
                                                      StreamSchema.Sprocs.GetIdsAddIfNecessaryTagSet.BuildCreationScript(this.Name),
                                                      StreamSchema.Sprocs.GetTagSetFromIds.BuildCreationScript(this.Name),
                                                      StreamSchema.Sprocs.GetLatestRecordMetadataById.BuildCreationScript(this.Name),
                                                      StreamSchema.Sprocs.GetLatestRecordById.BuildCreationScript(this.Name),
                                                      StreamSchema.Sprocs.GetNextUniqueLong.BuildCreationScript(this.Name),
                                                      StreamSchema.Sprocs.PutRecord.BuildCreationScript(this.Name),
                                                      StreamSchema.Sprocs.PutHandling.BuildCreationScript(this.Name),
                                                      StreamSchema.Sprocs.GetCompositeHandlingStatus.BuildCreationScript(this.Name),
                                                      StreamSchema.Sprocs.TryHandleRecord.BuildCreationScript(this.Name),
                                                      StreamSchema.Sprocs.CreateStreamUser.BuildCreationScript(this.Name),
                                                      StreamSchema.Sprocs.GetStreamDetails.BuildCreationScript(this.Name),
                                                      StreamSchema.BuildCreationScriptForRoles(this.Name), // must be at end to reference the items.
                                                  };

                            foreach (var script in creationScripts)
                            {
                                connection.ExecuteNonQuery(script);
                            }
                        }
                    }
                }
                else
                {
                    throw SqlServerLocator.BuildInvalidLocatorException(locator.GetType());
                }
            }

            var createResult = new CreateStreamResult(alreadyExisted, wasCreated);
            return createResult;
        }

        /// <inheritdoc />
        public UpdateStreamStoredProceduresResult Execute(
            UpdateStreamStoredProceduresOp operation)
        {
            var allLocators = this.ResourceLocatorProtocols.Execute(new GetAllResourceLocatorsOp());
            var priorVersions = new List<string>();
            foreach (var locator in allLocators)
            {
                if (locator is SqlServerLocator sqlLocator)
                {
                    using (var connection = sqlLocator.OpenSqlConnection(this.DefaultConnectionTimeout))
                    {
                        var streamAlreadyExists = connection.HasAtLeastOneRowWhenReading(
                            Invariant($"select * from sys.schemas where name = '{this.Name}'"));

                        if (!streamAlreadyExists)
                        {
                            throw new InvalidDataException(
                                FormattableString.Invariant(
                                    $"Stream '{this.Name}' did exist, this is unexpected and not possible to ALTER stored procedures as they are not present."));
                        }

                        var alterScripts = new[]
                                           {
                                               StreamSchema.Funcs.GetTagsTableVariableFromTagsXml.BuildCreationScript(this.Name, true),
                                               StreamSchema.Funcs.GetTagsTableVariableFromTagIdsXml.BuildCreationScript(this.Name, true),
                                               StreamSchema.Sprocs.GetIdAddIfNecessaryTypeWithoutVersion.BuildCreationScript(this.Name, true),
                                               StreamSchema.Sprocs.GetIdAddIfNecessaryTypeWithVersion.BuildCreationScript(this.Name, true),
                                               StreamSchema.Sprocs.GetTypeFromId.BuildCreationScript(this.Name, true),
                                               StreamSchema.Sprocs.GetIdAddIfNecessarySerializerRepresentation.BuildCreationScript(this.Name, true),
                                               StreamSchema.Sprocs.GetSerializerRepresentationFromId.BuildCreationScript(this.Name, true),
                                               StreamSchema.Sprocs.GetIdsAddIfNecessaryTagSet.BuildCreationScript(this.Name, true),
                                               StreamSchema.Sprocs.GetTagSetFromIds.BuildCreationScript(this.Name, true),
                                               StreamSchema.Sprocs.GetLatestRecordMetadataById.BuildCreationScript(this.Name, true),
                                               StreamSchema.Sprocs.GetLatestRecordById.BuildCreationScript(this.Name, true),
                                               StreamSchema.Sprocs.GetNextUniqueLong.BuildCreationScript(this.Name, true),
                                               StreamSchema.Sprocs.PutRecord.BuildCreationScript(this.Name, true),
                                               StreamSchema.Sprocs.PutHandling.BuildCreationScript(this.Name, true),
                                               StreamSchema.Sprocs.GetCompositeHandlingStatus.BuildCreationScript(this.Name, true),
                                               StreamSchema.Sprocs.TryHandleRecord.BuildCreationScript(this.Name, true),
                                               StreamSchema.Sprocs.CreateStreamUser.BuildCreationScript(this.Name, true),
                                               StreamSchema.Sprocs.GetStreamDetails.BuildCreationScript(this.Name, true),
                                           };

                        var detailsOperation = StreamSchema.Sprocs.GetStreamDetails.BuildExecuteStoredProcedureOp(this.Name);
                        var protocol = this.BuildSqlOperationsProtocol(sqlLocator);
                        var sprocResult = protocol.Execute(detailsOperation);
                        var detailsXml = sprocResult.OutputParameters[StreamSchema.Sprocs.GetStreamDetails.OutputParamName.DetailsXml.ToString()].GetValue<string>();
                        var detailsMap = TagConversionTool.GetTagsFromXmlString(detailsXml);
                        var version = detailsMap?.FirstOrDefault(_ => _.Key == StreamSchema.Sprocs.GetStreamDetails.VersionKey).Value;
                        priorVersions.Add(version);

                        foreach (var script in alterScripts)
                        {
                            connection.ExecuteNonQuery(script);
                        }
                    }
                }
                else
                {
                    throw SqlServerLocator.BuildInvalidLocatorException(locator.GetType());
                }
            }

            var distinctNonNullVersions = priorVersions.Distinct().Where(_ => _ != null).ToList();
            var priorVersion = distinctNonNullVersions.Any() ? distinctNonNullVersions.ToCsv() : null;
            var updateResult = new UpdateStreamStoredProceduresResult(priorVersion);
            return updateResult;
        }
    }
}
