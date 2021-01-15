// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlStream.Create.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Protocol.Client
{
    using System;
    using System.IO;
    using Naos.Database.Domain;
    using Naos.Protocol.Domain;
    using Naos.SqlServer.Domain;
    using OBeautifulCode.Database.Recipes;
    using static System.FormattableString;

    public partial class SqlStream
    {
        /// <inheritdoc />
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Acceptable given it creates the streams.")]
        public override void Execute(
            CreateStreamOp operation)
        {
            var allLocators = this.ResourceLocatorProtocols.Execute(new GetAllResourceLocatorsOp());
            foreach (var locator in allLocators)
            {
                if (locator is SqlServerLocator sqlLocator)
                {
                    using (var connection = sqlLocator.OpenSqlConnection(this.DefaultConnectionTimeout))
                    {
                        // TODO: should we use a transaction here?
                        var streamAlreadyExists = connection.HasAtLeastOneRowWhenReading(
                            Invariant($"select * from sys.schemas where name = '{this.Name}'"));

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
                                                      StreamSchema.Funcs.GetStatusSortOrderTableVariable.BuildCreationScript(this.Name),
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
        }
    }
}
