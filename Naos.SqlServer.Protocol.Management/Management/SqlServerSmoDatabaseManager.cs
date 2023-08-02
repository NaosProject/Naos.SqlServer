// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlServerSmoDatabaseManager.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Protocol.Management
{
    using System;
    using System.Data.SqlClient;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Microsoft.SqlServer.Management.Common;
    using Microsoft.SqlServer.Management.Smo;
    using Naos.CodeAnalysis.Recipes;
    using Naos.SqlServer.Protocol.Client;
    using OBeautifulCode.Execution.Recipes;
    using static System.FormattableString;

    /// <summary>
    /// Logic to copy objects from one database to another.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Smo", Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
    public static class SqlServerSmoDatabaseManager
    {
        /// <summary>
        /// Runs the specified operation against the <see cref="Database" /> object.
        /// </summary>
        /// <param name="action">Operation to run against the SMO database, <see cref="Database" />.</param>
        /// <param name="connectionString">Database connection string.</param>
        /// <param name="infoMessageCallback">Optional callback to capture information messages sent on connection.</param>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Smo", Justification = "Spelling/name is correct.")]
        public static void RunOperationOnSmoDatabase(Action<Database> action, string connectionString, SqlInfoMessageEventHandler infoMessageCallback = null)
        {
            Task AsyncOperation(Database database)
            {
                action(database);
                return Task.Run(() => { });
            }

            Func<Task> runOperationOnSmoDatabaseAsyncFunc = () => RunOperationOnSmoDatabaseAsync(AsyncOperation, connectionString, infoMessageCallback);

            runOperationOnSmoDatabaseAsyncFunc.ExecuteSynchronously();
        }

        /// <summary>
        /// Runs the specified operation against the <see cref="Database" /> object.
        /// </summary>
        /// <param name="asyncAction">Operation to run against the SMO database, <see cref="Database" />.</param>
        /// <param name="connectionString">Database connection string.</param>
        /// <param name="infoMessageCallback">Optional callback to capture information messages sent on connection.</param>
        /// <returns>Task for async.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Smo", Justification = "Spelling/name is correct.")]
        public static async Task RunOperationOnSmoDatabaseAsync(Func<Database, Task> asyncAction, string connectionString, SqlInfoMessageEventHandler infoMessageCallback = null)
        {
            async Task ConnectionAction(SqlConnection connection)
            {
                await RunOperationOnSmoDatabaseAsync(asyncAction, connection);
            }

            await SqlServerDatabaseManager.RunOperationOnSqlConnectionAsync(ConnectionAction, connectionString, infoMessageCallback);
        }

        /// <summary>
        /// Runs the specified operation against the <see cref="Database" /> object.
        /// </summary>
        /// <param name="asyncAction">Operation to run against the SMO database, <see cref="Database" />.</param>
        /// <param name="connection">Database connection.</param>
        /// <returns>Task for async.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Smo", Justification = "Spelling/name is correct.")]
        public static async Task RunOperationOnSmoDatabaseAsync(Func<Database, Task> asyncAction, SqlConnection connection)
        {
            async Task ServerAction(Server server)
            {
                var databaseName = connection.Database;
                var database = server.Databases[databaseName];
                if (database == null)
                {
                    throw new MissingObjectException(Invariant($"Database: {databaseName} on DataSource {connection.DataSource} was not found or is not accessible."));
                }

                await asyncAction(database);
            }

            await RunOperationOnSmoServerAsync(ServerAction, connection);
        }

        /// <summary>
        /// Runs the specified operation against the <see cref="Server" /> object.
        /// </summary>
        /// <param name="asyncAction">Operation to run against the SMO server, <see cref="Server" />.</param>
        /// <param name="connection">Database connection.</param>
        /// <returns>Task for async.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Smo", Justification = "Spelling/name is correct.")]
        public static async Task RunOperationOnSmoServerAsync(Func<Server, Task> asyncAction, SqlConnection connection)
        {
            var server = new Server(new ServerConnection(connection));

            await asyncAction(server);
        }
    }
}
