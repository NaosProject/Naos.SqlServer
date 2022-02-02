// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlStream.CreateOrUpdate.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Protocol.Client
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using Naos.CodeAnalysis.Recipes;
    using Naos.Database.Domain;
    using Naos.SqlServer.Domain;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Collection.Recipes;
    using OBeautifulCode.Database.Recipes;
    using OBeautifulCode.Type;
    using static System.FormattableString;

    public partial class SqlStream : ISyncReturningProtocol<UpdateStreamStoredProceduresOp, UpdateStreamStoredProceduresResult>
    {
        /// <inheritdoc />
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Acceptable given it creates the streams.")]
        public override CreateStreamResult Execute(
            StandardCreateStreamOp operation)
        {
            operation.MustForArg(nameof(operation)).NotBeNull();

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
                            switch (operation.ExistingStreamStrategy)
                            {
                                case ExistingStreamStrategy.Overwrite:
                                    throw new NotSupportedException(FormattableString.Invariant(
                                        $"Overwriting streams is not currently supported; stream '{this.Name}' already exists, {nameof(operation)}.{nameof(operation.ExistingStreamStrategy)} was set to {ExistingStreamStrategy.Overwrite}."));
                                case ExistingStreamStrategy.Throw:
                                    throw new InvalidOperationException(FormattableString.Invariant($"Stream '{this.Name}' already exists, {nameof(operation)}.{nameof(operation.ExistingStreamStrategy)} was set to {ExistingStreamStrategy.Throw}."));
                                case ExistingStreamStrategy.Skip:
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
                                                      StreamSchema.Funcs.GetTagsTableVariableFromTagsXml.BuildCreationScript(this.Name),
                                                      StreamSchema.Sprocs.CreateStreamUser.BuildCreationScript(this.Name),
                                                      StreamSchema.Sprocs.GetDistinctStringSerializedIds.BuildCreationScript(this.Name),
                                                      StreamSchema.Sprocs.GetHandlingStatuses.BuildCreationScript(this.Name),
                                                      StreamSchema.Sprocs.GetIdAddIfNecessarySerializerRepresentation.BuildCreationScript(this.Name),
                                                      StreamSchema.Sprocs.GetIdsAddIfNecessaryTagSet.BuildCreationScript(this.Name),
                                                      StreamSchema.Sprocs.GetIdAddIfNecessaryTypeWithoutVersion.BuildCreationScript(this.Name),
                                                      StreamSchema.Sprocs.GetIdAddIfNecessaryTypeWithVersion.BuildCreationScript(this.Name),
                                                      StreamSchema.Sprocs.GetInternalRecordIds.BuildCreationScript(this.Name),
                                                      StreamSchema.Sprocs.GetLatestRecord.BuildCreationScript(this.Name),
                                                      StreamSchema.Sprocs.GetLatestStringSerializedObject.BuildCreationScript(this.Name),
                                                      StreamSchema.Sprocs.GetNextUniqueLong.BuildCreationScript(this.Name),
                                                      StreamSchema.Sprocs.GetSerializerRepresentationFromId.BuildCreationScript(this.Name),
                                                      StreamSchema.Sprocs.GetStreamDetails.BuildCreationScript(this.Name, RecordTagAssociationManagementStrategy.AssociatedDuringPutInSprocInTransaction, null),
                                                      StreamSchema.Sprocs.GetTagSetFromIds.BuildCreationScript(this.Name),
                                                      StreamSchema.Sprocs.GetTypeFromId.BuildCreationScript(this.Name),
                                                      StreamSchema.Sprocs.PutRecord.BuildCreationScript(this.Name, RecordTagAssociationManagementStrategy.AssociatedDuringPutInSprocInTransaction),
                                                      StreamSchema.Sprocs.PutHandling.BuildCreationScript(this.Name),
                                                      StreamSchema.Sprocs.TryHandleRecord.BuildCreationScript(this.Name, null),
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
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = NaosSuppressBecause.CA1506_AvoidExcessiveClassCoupling_DisagreeWithAssessment)]
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
                            throw new InvalidDataException(Invariant($"Stream '{this.Name}' did exist, this is unexpected and not possible to ALTER stored procedures as they are not present."));
                        }

                        var alterScripts = new[]
                                           {
                                               StreamSchema.Funcs.GetTagsTableVariableFromTagsXml.BuildCreationScript(this.Name, true),
                                               StreamSchema.Sprocs.CreateStreamUser.BuildCreationScript(this.Name, true),
                                               StreamSchema.Sprocs.GetDistinctStringSerializedIds.BuildCreationScript(this.Name, true),
                                               StreamSchema.Sprocs.GetHandlingStatuses.BuildCreationScript(this.Name, true),
                                               StreamSchema.Sprocs.GetIdAddIfNecessarySerializerRepresentation.BuildCreationScript(this.Name, true),
                                               StreamSchema.Sprocs.GetIdsAddIfNecessaryTagSet.BuildCreationScript(this.Name, true),
                                               StreamSchema.Sprocs.GetIdAddIfNecessaryTypeWithoutVersion.BuildCreationScript(this.Name, true),
                                               StreamSchema.Sprocs.GetIdAddIfNecessaryTypeWithVersion.BuildCreationScript(this.Name, true),
                                               StreamSchema.Sprocs.GetInternalRecordIds.BuildCreationScript(this.Name, true),
                                               StreamSchema.Sprocs.GetLatestRecord.BuildCreationScript(this.Name, true),
                                               StreamSchema.Sprocs.GetLatestStringSerializedObject.BuildCreationScript(this.Name, true),
                                               StreamSchema.Sprocs.GetNextUniqueLong.BuildCreationScript(this.Name, true),
                                               StreamSchema.Sprocs.GetSerializerRepresentationFromId.BuildCreationScript(this.Name, true),
                                               StreamSchema.Sprocs.GetStreamDetails.BuildCreationScript(
                                                   this.Name,
                                                   RecordTagAssociationManagementStrategy.AssociatedDuringPutInSprocInTransaction,
                                                   null,
                                                   true),
                                               StreamSchema.Sprocs.GetTagSetFromIds.BuildCreationScript(this.Name, true),
                                               StreamSchema.Sprocs.GetTypeFromId.BuildCreationScript(this.Name, true),
                                               StreamSchema.Sprocs.PutRecord.BuildCreationScript(
                                                   this.Name,
                                                   RecordTagAssociationManagementStrategy.AssociatedDuringPutInSprocInTransaction,
                                                   true),
                                               StreamSchema.Sprocs.PutHandling.BuildCreationScript(this.Name, true),
                                               StreamSchema.Sprocs.TryHandleRecord.BuildCreationScript(this.Name, null, true),
                                           };

                        var detailsOperation = StreamSchema.Sprocs.GetStreamDetails.BuildExecuteStoredProcedureOp(this.Name);
                        var protocol = this.BuildSqlOperationsProtocol(sqlLocator);
                        var sprocResult = protocol.Execute(detailsOperation);
                        var detailsXml = sprocResult.OutputParameters[StreamSchema.Sprocs.GetStreamDetails.OutputParamName.DetailsXml.ToString()].GetValueOfType<string>();
                        var detailsMap = detailsXml.GetTagsFromXmlString();
                        var version = detailsMap?.FirstOrDefault(_ => _.Name == StreamSchema.Sprocs.GetStreamDetails.VersionKey)?.Value;
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
