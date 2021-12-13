﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DatabaseObjectCopier.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Protocol.Management
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.SqlServer.Management.Smo;
    using Naos.SqlServer.Domain;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Database.Recipes;
    using static System.FormattableString;

    /// <summary>
    /// Logic to copy objects from one database to another.
    /// </summary>
    public static class DatabaseObjectCopier
    {
        /// <summary>
        /// Copies objects from one database to another.
        /// </summary>
        /// <param name="orderedObjectNamesToCopy">Names of objects to copy in order of expected execution.</param>
        /// <param name="sourceDatabaseConnectionString">Connection string to the source database.</param>
        /// <param name="targetDatabaseConnectionString">Connection string to the target database.</param>
        /// <param name="announcer">Optional announcer to log messages about what is happening.</param>
        /// <returns>Task for async.</returns>
        public static async Task CopyObjects(
            IReadOnlyList<string> orderedObjectNamesToCopy,
            string sourceDatabaseConnectionString,
            string targetDatabaseConnectionString,
            Action<Func<object>> announcer = null)
        {
            new { orderedObjectNamesToCopy }.AsArg().Must().NotBeNullNorEmptyEnumerableNorContainAnyNulls();
            new { sourceDatabaseConnectionString }.AsArg().Must().NotBeNullNorWhiteSpace();
            new { targetDatabaseConnectionString }.AsArg().Must().NotBeNullNorWhiteSpace();

            void NullAnnounce(Func<object> announcement)
            {
                /* no-op */
            }

            var localAnnouncer = announcer ?? NullAnnounce;

            void SqlMessagesToAnnouncerAdapter(object sender, SqlInfoMessageEventArgs args)
            {
                localAnnouncer(() => args);
            }

            var scriptedObjects = Scripter.ScriptObjectsFromDatabase(sourceDatabaseConnectionString, orderedObjectNamesToCopy);

            using (var targetConnection = await targetDatabaseConnectionString.OpenSqlConnectionAsync(SqlMessagesToAnnouncerAdapter))
            {
                async Task RunScriptOnServer(ScriptedObject scriptedObject, string scriptToRun)
                {
                    localAnnouncer(() => Invariant($"Applying create script for '{scriptedObject.Name}' of type '{scriptedObject.DatabaseObjectType}'"));
                    try
                    {
                        async Task ServerAction(Server server)
                        {
                            // because it might contain "GO" statements most likely this needs to be executed via the SMO connection.
                            server.ConnectionContext.ExecuteNonQuery(scriptToRun);

                            await Task.Run(() => { });
                        }

                        await SqlServerSmoDatabaseManager.RunOperationOnSmoServerAsync(ServerAction, targetConnection);
                    }
                    catch (Exception ex)
                    {
                        throw new FailedOperationException(
                            Invariant($"Failed to run script on database {targetDatabaseConnectionString.ObfuscateCredentialsInConnectionString()}; {scriptToRun}"),
                            ex);
                    }
                }

                foreach (var scriptedObject in scriptedObjects.Reverse())
                {
                    await RunScriptOnServer(scriptedObject, scriptedObject.DropScript);
                }

                foreach (var scriptedObject in scriptedObjects)
                {
                    await RunScriptOnServer(scriptedObject, scriptedObject.CreateScript);
                }

                var tables = scriptedObjects.Where(_ => _.DatabaseObjectType == ScriptableObjectType.Table).ToList();
                if (tables.Any())
                {
                    var copyOptions = SqlBulkCopyOptions.CheckConstraints
                                    | SqlBulkCopyOptions.FireTriggers
                                    | SqlBulkCopyOptions.KeepIdentity
                                    | SqlBulkCopyOptions.KeepNulls
                                    | SqlBulkCopyOptions.TableLock;

                    using (var sourceConnection = await sourceDatabaseConnectionString.OpenSqlConnectionAsync(SqlMessagesToAnnouncerAdapter))
                    {
                        foreach (var table in tables)
                        {
                            table.Name.MustForArg(Invariant($"{nameof(table)}.{nameof(ScriptedObject.Name)}")).NotBeNullNorWhiteSpace().And().BeAlphanumeric(TableDefinition.TableNameAlphanumericOtherAllowedCharacters);

                            using (var transaction = targetConnection.BeginTransaction("BcpTable-" + table.Name))
                            {
                                using (var bcp = new SqlBulkCopy(targetConnection, copyOptions, transaction) { DestinationTableName = table.Name })
                                {
                                    using (var command = sourceConnection.CreateCommand())
                                    {
                                        command.CommandType = CommandType.Text;
                                        command.CommandText = "SELECT * FROM " + table.Name;
                                        using (var reader = await command.ExecuteReaderAsync())
                                        {
                                            await bcp.WriteToServerAsync(reader);
                                        }
                                    }
                                }

                                transaction.Commit();
                            }
                        }
                    }
                }
            }
        }
    }
}
