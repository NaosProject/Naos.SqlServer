// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlServerDatabaseManager.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Protocol.Client
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Naos.CodeAnalysis.Recipes;
    using Naos.Database.Domain;
    using Naos.SqlServer.Domain;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Database.Recipes;
    using OBeautifulCode.Enum.Recipes;
    using OBeautifulCode.Execution.Recipes;
    using OBeautifulCode.Serialization.PropertyBag;
    using static System.FormattableString;

    /// <summary>
    /// Class with tools for adding, removing, and updating databases.
    /// </summary>
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = NaosSuppressBecause.CA1506_AvoidExcessiveClassCoupling_DisagreeWithAssessment)]
    public static class SqlServerDatabaseManager
    {
        /// <summary>
        /// Name of the master database.
        /// </summary>
        public const string MasterDatabaseName = "master";

        /// <summary>
        /// The default timeout.
        /// </summary>
        public static readonly TimeSpan DefaultTimeoutTimespan = TimeSpan.FromSeconds(30);

        /// <summary>
        /// Builds the connection string.
        /// </summary>
        /// <param name="connectionDefinition">The SQL locator.</param>
        /// <param name="connectionTimeout">Timeout for the connection.</param>
        /// <returns>SQL Connection string.</returns>
        public static string BuildConnectionString(
            this SqlServerConnectionDefinition connectionDefinition,
            TimeSpan connectionTimeout)
        {
            connectionDefinition.MustForArg(nameof(connectionDefinition)).NotBeNull();

            var connectionString = ConnectionStringHelper.BuildConnectionString(
                connectionDefinition.Server,
                instanceName: connectionDefinition.InstanceName,
                databaseName: connectionDefinition.DatabaseName,
                userName: connectionDefinition.UserName,
                clearTextPassword: connectionDefinition.Password,
                connectionTimeoutInSeconds: (int)connectionTimeout.TotalSeconds);

            return connectionString;
        }

        /// <summary>
        /// Runs the provided action with a new <see cref="SqlConnection" /> and then closes it.
        /// </summary>
        /// <param name="action">Action to run.</param>
        /// <param name="connectionString">Database connection string.</param>
        /// <param name="infoMessageCallback">Optional callback to capture information messages sent on connection.</param>
        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "Object is disposed correctly.")]
        public static void RunOperationOnSqlConnection(
            this Action<SqlConnection> action,
            string connectionString,
            SqlInfoMessageEventHandler
                infoMessageCallback = null)
        {
            Task AsyncOperation(SqlConnection connection)
            {
                action(connection);
                return Task.Run(() => { });
            }

            Func<Task> runOperationOnSqlConnectionAsyncFunc = () => RunOperationOnSqlConnectionAsync(AsyncOperation, connectionString, infoMessageCallback);

            runOperationOnSqlConnectionAsyncFunc.ExecuteSynchronously();
        }

        /// <summary>
        /// Runs the provided action with a new <see cref="SqlConnection" /> and then closes it.
        /// </summary>
        /// <param name="asyncAction">Asynchronous action to run.</param>
        /// <param name="connectionString">Database connection string.</param>
        /// <param name="infoMessageCallback">Optional callback to capture information messages sent on connection.</param>
        /// <returns>Task for async.</returns>
        public static async Task RunOperationOnSqlConnectionAsync(
            this Func<SqlConnection, Task> asyncAction,
            string connectionString,
            SqlInfoMessageEventHandler infoMessageCallback = null)
        {
            new { asyncAction }.AsArg().Must().NotBeNull();
            new { connectionString }.AsArg().Must().NotBeNullNorWhiteSpace();

            using (var connection = await connectionString.OpenSqlConnectionAsync(infoMessageCallback))
            {
                await asyncAction(connection);

                connection.Close();
            }
        }

        /// <summary>
        /// Puts the database into single user mode.
        /// </summary>
        /// <param name="connectionString">The connection string to the intended server.</param>
        /// <param name="databaseName">The name of the target database.</param>
        /// <param name="timeout">The command timeout (default is 30 seconds).</param>
        public static void PutDatabaseInSingleUserMode(
            string connectionString,
            string databaseName,
            TimeSpan timeout = default)
        {
            new { connectionString }.AsArg().Must().NotBeNullNorWhiteSpace();
            new { databaseName }.AsArg().Must().NotBeNullNorWhiteSpace();

            void Logic(SqlConnection connection)
            {
                PutDatabaseInSingleUserMode(connection, databaseName, timeout);
            }

            RunOperationOnSqlConnection(Logic, connectionString);
        }

        /// <summary>
        /// Put the database into multiple user mode.
        /// </summary>
        /// <param name="connectionString">The connection string to the intended server.</param>
        /// <param name="databaseName">The name of the target database.</param>
        /// <param name="timeout">The command timeout (default is 30 seconds).</param>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi", Justification = "Spelling/name is correct.")]
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "MultiUser", Justification = "Spelling/name is correct.")]
        public static void PutDatabaseIntoMultiUserMode(
            string connectionString,
            string databaseName,
            TimeSpan timeout = default)
        {
            new { connectionString }.AsArg().Must().NotBeNullNorWhiteSpace();
            new { databaseName }.AsArg().Must().NotBeNullNorWhiteSpace();

            void Logic(SqlConnection connection)
            {
                PutDatabaseIntoMultiUserMode(connection, databaseName, timeout);
            }

            RunOperationOnSqlConnection(Logic, connectionString);
        }

        /// <summary>
        /// Set the database to off line.
        /// </summary>
        /// <param name="connectionString">The connection string to the intended server.</param>
        /// <param name="databaseName">The name of the target database.</param>
        /// <param name="timeout">The command timeout (default is 30 seconds).</param>
        public static void TakeDatabaseOffline(
            string connectionString,
            string databaseName,
            TimeSpan timeout = default)
        {
            new { connectionString }.AsArg().Must().NotBeNullNorWhiteSpace();
            new { databaseName }.AsArg().Must().NotBeNullNorWhiteSpace().And().BeAlphanumeric(SqlServerDatabaseDefinition.DatabaseNameAlphanumericOtherAllowedCharacters);

            timeout = timeout == default ? DefaultTimeoutTimespan : timeout;

            var commandText = "ALTER DATABASE " + databaseName + " SET offline WITH ROLLBACK IMMEDIATE";

            void Logic(SqlConnection connection)
            {
                connection.ExecuteNonQuery(commandText, (int)timeout.TotalSeconds);
            }

            RunOperationOnSqlConnection(Logic, connectionString);
        }

        /// <summary>
        /// Set the database to on line.
        /// </summary>
        /// <param name="connectionString">The connection string to the intended server.</param>
        /// <param name="databaseName">The name of the target database.</param>
        /// <param name="timeout">The command timeout (default is 30 seconds).</param>
        public static void BringDatabaseOnline(
            string connectionString,
            string databaseName,
            TimeSpan timeout = default)
        {
            new { connectionString }.AsArg().Must().NotBeNullNorWhiteSpace();
            new { databaseName }.AsArg().Must().NotBeNullNorWhiteSpace().And().BeAlphanumeric(SqlServerDatabaseDefinition.DatabaseNameAlphanumericOtherAllowedCharacters);

            timeout = timeout == default ? DefaultTimeoutTimespan : timeout;

            var commandText = "ALTER DATABASE " + databaseName + " SET online";

            void Logic(SqlConnection connection)
            {
                connection.ExecuteNonQuery(commandText, (int)timeout.TotalSeconds);
            }

            RunOperationOnSqlConnection(Logic, connectionString);
        }

        /// <summary>
        /// Create a new database using provided definition.
        /// </summary>
        /// <param name="connectionString">Connection string to the intended database server.</param>
        /// <param name="definition">Detailed information about the database.</param>
        /// <param name="existingDatabaseStrategy">Strategy to use when encountering an existing database.</param>
        /// <param name="timeout">The command timeout (default is 30 seconds).</param>
        public static void Create(
            string connectionString,
            SqlServerDatabaseDefinition definition,
            ExistingDatabaseStrategy existingDatabaseStrategy,
            TimeSpan timeout = default)
        {
            new { connectionString }.AsArg().Must().NotBeNullNorWhiteSpace();
            new { definition }.AsArg().Must().NotBeNull();
            existingDatabaseStrategy.MustForArg(nameof(existingDatabaseStrategy))
                                    .BeElementIn(
                                         new[]
                                         {
                                             ExistingDatabaseStrategy.Skip,
                                             ExistingDatabaseStrategy.Throw,
                                         });

            timeout = timeout == default ? DefaultTimeoutTimespan : timeout;

            ThrowIfBadOnCreateOrModify(definition);
            var databaseFileMaxSize = definition.DataFileMaxSizeInKb == SqlServerDatabaseDefinition.InfinityMaxSize ? "UNLIMITED" : Invariant($"{definition.DataFileMaxSizeInKb}KB");
            var logFileMaxSize = definition.LogFileMaxSizeInKb == SqlServerDatabaseDefinition.InfinityMaxSize ? "UNLIMITED" : Invariant($"{definition.LogFileMaxSizeInKb}KB");

            var createDatabaseCommandText =
                    Invariant(
                        $@"
                        CREATE DATABASE {definition.DatabaseName}
                        ON
                        ( NAME = '{definition.DataFileLogicalName}',
                        FILENAME = '{definition.DataFilePath}',
                        SIZE = {definition.DataFileCurrentSizeInKb}KB,
                        MAXSIZE = {databaseFileMaxSize},
                        FILEGROWTH = {definition.DataFileGrowthSizeInKb}KB )
                        LOG ON
                        ( NAME = '{definition.LogFileLogicalName}',
                        FILENAME = '{definition.LogFilePath}',
                        SIZE = {definition.LogFileCurrentSizeInKb}KB,
                        MAXSIZE = {logFileMaxSize},
                        FILEGROWTH = {definition.LogFileGrowthSizeInKb}KB )");

            void Logic(SqlConnection connection)
            {
                var checkExistsText = $"SELECT DB_ID('{definition.DatabaseName}')";
                var existingDatabaseNameId = connection.ReadSingleValue(checkExistsText);
                if (existingDatabaseNameId != null)
                {
                    switch (existingDatabaseStrategy)
                    {
                        case ExistingDatabaseStrategy.Throw:
                            throw new InvalidOperationException(Invariant($"Server '{connectionString.ObfuscateCredentialsInConnectionString()}' already has a database '{definition.DatabaseName}', {nameof(existingDatabaseStrategy)} was set to {ExistingStreamStrategy.Throw}."));
                        case ExistingDatabaseStrategy.Skip:
                            return;
                        default:
                            throw new NotSupportedException(
                                Invariant($"{nameof(existingDatabaseStrategy)} '{existingDatabaseStrategy}' is not supported."));
                    }
                }

                connection.ExecuteNonQuery(createDatabaseCommandText, (int)timeout.TotalSeconds);

                if (definition.RecoveryMode != RecoveryMode.Unspecified)
                {
                    var setRecoveryModeCommandText = BuildSetRecoveryModeSqlCommandText(definition.DatabaseName, definition.RecoveryMode);

                    connection.ExecuteNonQuery(setRecoveryModeCommandText, (int)timeout.TotalSeconds);
                }
            }

            RunOperationOnSqlConnection(Logic, connectionString);
        }

        /// <summary>
        /// List databases from server.
        /// </summary>
        /// <param name="connectionString">Connection string to the intended database server.</param>
        /// <param name="timeout">The command timeout (default is 30 seconds).</param>
        /// <returns>All databases from server.</returns>
        public static SqlServerDatabaseDefinition[] Retrieve(
            string connectionString,
            TimeSpan timeout = default)
        {
            new { connectionString }.AsArg().Must().NotBeNullNorWhiteSpace();

            timeout = timeout == default ? DefaultTimeoutTimespan : timeout;

            // using the database name is the only good way to differentiate System from User databases
            // see: http://stackoverflow.com/a/9682659/356790.  This is how the SMO does it.
            const string query = @"
                SELECT
                    d.name as DatabaseName,
                    df.name as DataFileLogicalName,
                    df.physical_name as DataFilePath,
                    lf.name as LogFileLogicalName,
                    lf.physical_name as LogFilePath,
                    Replace(d.recovery_model_desc, '_', '') as RecoveryMode,
                    Case When d.name in ('master','model','msdb','tempdb') Then 'System' Else 'User' End as DatabaseType,
                    Round(Cast(df.size as bigint) * 125 / 16, 0) as DataFileCurrentSizeInKb,
	                Case When df.max_size = -1 Then -1 Else Round(Cast(df.max_size as bigint) * 125 / 16, 0) End as DataFileMaxSizeInKb,
	                Case When df.is_percent_growth = 1 Then 0 Else Round(Cast(df.growth as bigint) * 125 / 16, 0) End as DataFileGrowthSizeInKb, 
	                Round(Cast(lf.size as bigint) * 125 / 16, 0) as LogFileCurrentSizeInKb,
	                Case When lf.max_size = -1 Then -1 Else Round(Cast(lf.max_size as bigint) * 125 / 16, 0) End as LogFileMaxSizeInKb,
	                Case When lf.is_percent_growth = 1 Then 0 Else Round(Cast(lf.growth as bigint) * 125 / 16, 0) End as LogFileGrowthSizeInKb
                FROM sys.databases d
                INNER JOIN sys.master_files df
                    ON d.database_id = df.database_id AND df.type = 0
                INNER JOIN sys.master_files lf
                    ON d.database_id = lf.database_id AND lf.type = 1";

            var propertyBagSerializer = new ObcPropertyBagSerializer();

            var result = new SqlServerDatabaseDefinition[0];

            void Logic(SqlConnection connection)
            {
                result = connection.ReadAllRowsWithNamedColumns(query, (int)timeout.TotalSeconds)
                                .Select(_ =>
                                        {
                                            var enumConvertedDictionary = _
                                               .ToDictionary(
                                                    k => k.Key,
                                                    v =>
                                                    {
                                                        switch (v.Key)
                                                        {
                                                            case nameof(DatabaseType):
                                                                return v.Value?.ToString().ToEnum<DatabaseType>(true);
                                                            case nameof(RecoveryMode):
                                                                return v.Value?.ToString().ToEnum<RecoveryMode>(true);
                                                            default:
                                                                return v.Value;
                                                        }
                                                    });

                                            return propertyBagSerializer.Deserialize<SqlServerDatabaseDefinition>(enumConvertedDictionary);
                                        })
                                .ToArray();
            }

            RunOperationOnSqlConnection(Logic, connectionString);

            return result;
        }

        /// <summary>
        /// Update a database to the new definition.
        /// </summary>
        /// <param name="connectionString">Connection string to the intended database server.</param>
        /// <param name="currentDefinition">Detailed information about how the database is.</param>
        /// <param name="newDefinition">Detailed information about how the database should look after the update.</param>
        /// <param name="timeout">The command timeout (default is 30 seconds).</param>
        public static void Update(
            string connectionString,
            SqlServerDatabaseDefinition currentDefinition,
            SqlServerDatabaseDefinition newDefinition,
            TimeSpan timeout = default)
        {
            new { connectionString }.AsArg().Must().NotBeNullNorWhiteSpace();
            new { currentDefinition }.AsArg().Must().NotBeNull();
            new { newDefinition }.AsArg().Must().NotBeNull();

            timeout = timeout == default ? DefaultTimeoutTimespan : timeout;

            ThrowIfBadOnCreateOrModify(currentDefinition);
            ThrowIfBadOnCreateOrModify(newDefinition);

            void Logic(SqlConnection connection)
            {
                PutDatabaseInSingleUserMode(connection, currentDefinition.DatabaseName, timeout);

                try
                {
                    if (currentDefinition.DatabaseName != newDefinition.DatabaseName)
                    {
                        var renameDatabaseText = @"ALTER DATABASE " + currentDefinition.DatabaseName + " MODIFY NAME = "
                                                 + newDefinition.DatabaseName;
                        connection.ExecuteNonQuery(renameDatabaseText, (int)timeout.TotalSeconds);
                    }

                    if ((currentDefinition.DataFileLogicalName != newDefinition.DataFileLogicalName)
                        && (currentDefinition.DataFilePath != newDefinition.DataFilePath))
                    {
                        var updateDataFileText =
                            Invariant($@"ALTER DATABASE {newDefinition.DatabaseName} MODIFY FILE (
                        NAME = '{currentDefinition.DataFileLogicalName}',
                        NEWNAME = '{newDefinition.DataFileLogicalName}',
                        FILENAME = '{newDefinition.DataFilePath}')");
                        connection.ExecuteNonQuery(updateDataFileText, (int)timeout.TotalSeconds);
                    }

                    if ((currentDefinition.LogFileLogicalName != newDefinition.LogFileLogicalName)
                        && (currentDefinition.LogFilePath != newDefinition.LogFilePath))
                    {
                        var updateLogFileText =
                            Invariant($@"ALTER DATABASE {newDefinition.DatabaseName} MODIFY FILE (
                        NAME = '{currentDefinition.LogFileLogicalName}',
                        NEWNAME = '{newDefinition.LogFileLogicalName}',
                        FILENAME = '{newDefinition.LogFilePath}')");
                        connection.ExecuteNonQuery(updateLogFileText, (int)timeout.TotalSeconds);
                    }

                    if ((newDefinition.RecoveryMode != RecoveryMode.Unspecified) && (currentDefinition.RecoveryMode != newDefinition.RecoveryMode))
                    {
                        var setRecoveryModeCommandText = BuildSetRecoveryModeSqlCommandText(newDefinition.DatabaseName, newDefinition.RecoveryMode);

                        connection.ExecuteNonQuery(setRecoveryModeCommandText, (int)timeout.TotalSeconds);
                    }

                    if (newDefinition.DataFileMaxSizeInKb != currentDefinition.DataFileMaxSizeInKb)
                    {
                        var maxSize = newDefinition.DataFileMaxSizeInKb == SqlServerDatabaseDefinition.InfinityMaxSize ? "UNLIMITED" : Invariant($"{newDefinition.DataFileMaxSizeInKb}KB");
                        var updateDataFileMaxSizeText = Invariant($@"ALTER DATABASE {newDefinition.DatabaseName} MODIFY FILE (NAME = '{newDefinition.DataFileLogicalName}', MAXSIZE = {maxSize})");
                        connection.ExecuteNonQuery(updateDataFileMaxSizeText, (int)timeout.TotalSeconds);
                    }

                    if (newDefinition.DataFileCurrentSizeInKb != currentDefinition.DataFileCurrentSizeInKb)
                    {
                        var updateDataFileCurrentSizeText = Invariant($@"ALTER DATABASE {newDefinition.DatabaseName} MODIFY FILE (NAME = '{newDefinition.DataFileLogicalName}', SIZE = {newDefinition.DataFileCurrentSizeInKb}KB)");
                        connection.ExecuteNonQuery(updateDataFileCurrentSizeText, (int)timeout.TotalSeconds);
                    }

                    if (newDefinition.DataFileGrowthSizeInKb != currentDefinition.DataFileGrowthSizeInKb)
                    {
                        var updateDataFileGrowthText = Invariant($@"ALTER DATABASE {newDefinition.DatabaseName} MODIFY FILE (NAME = '{newDefinition.DataFileLogicalName}', FILEGROWTH = {newDefinition.DataFileGrowthSizeInKb}KB)");
                        connection.ExecuteNonQuery(updateDataFileGrowthText, (int)timeout.TotalSeconds);
                    }

                    if (newDefinition.LogFileMaxSizeInKb != currentDefinition.LogFileMaxSizeInKb)
                    {
                        var maxSize = newDefinition.LogFileMaxSizeInKb == SqlServerDatabaseDefinition.InfinityMaxSize ? "UNLIMITED" : Invariant($"{newDefinition.LogFileMaxSizeInKb}KB");
                        var updateLogFileMaxSizeText = Invariant($@"ALTER DATABASE {newDefinition.DatabaseName} MODIFY FILE (NAME = '{newDefinition.LogFileLogicalName}', MAXSIZE = {maxSize})");
                        connection.ExecuteNonQuery(updateLogFileMaxSizeText, (int)timeout.TotalSeconds);
                    }

                    if (newDefinition.LogFileCurrentSizeInKb != currentDefinition.LogFileCurrentSizeInKb)
                    {
                        var updateLogFileCurrentSizeText = Invariant($@"ALTER DATABASE {newDefinition.DatabaseName} MODIFY FILE (NAME = '{newDefinition.LogFileLogicalName}', SIZE = {newDefinition.LogFileCurrentSizeInKb}KB)");
                        connection.ExecuteNonQuery(updateLogFileCurrentSizeText, (int)timeout.TotalSeconds);
                    }

                    if (newDefinition.LogFileGrowthSizeInKb != currentDefinition.LogFileGrowthSizeInKb)
                    {
                        var updateLogFileGrowthText = Invariant($@"ALTER DATABASE {newDefinition.DatabaseName} MODIFY FILE (NAME = '{newDefinition.LogFileLogicalName}', FILEGROWTH = {newDefinition.LogFileGrowthSizeInKb}KB)");
                        connection.ExecuteNonQuery(updateLogFileGrowthText, (int)timeout.TotalSeconds);
                    }
                }
                finally
                {
                    PutDatabaseIntoMultiUserMode(connection, newDefinition.DatabaseName, timeout);
                }
            }

            var realConnectionString = connectionString.AddOrUpdateInitialCatalogInConnectionString(currentDefinition.DatabaseName); // make sure it's going to take the only connection when it goes in single user

            RunOperationOnSqlConnection(Logic, realConnectionString);
        }

        /// <summary>
        /// Delete a database.
        /// </summary>
        /// <param name="connectionString">Connection string to the intended database server.</param>
        /// <param name="databaseName">Name of the database to delete.</param>
        /// <param name="timeout">The command timeout (default is 30 seconds).</param>
        public static void Delete(
            string connectionString,
            string databaseName,
            TimeSpan timeout = default)
        {
            new { connectionString }.AsArg().Must().NotBeNullNorWhiteSpace();
            new { databaseName }.AsArg().Must().NotBeNullNorWhiteSpace().And().BeAlphanumeric(SqlServerDatabaseDefinition.DatabaseNameAlphanumericOtherAllowedCharacters);

            timeout = timeout == default ? DefaultTimeoutTimespan : timeout;

            var realConnectionString = connectionString.AddOrUpdateInitialCatalogInConnectionString(databaseName); // make sure it's going to take the only connection when it goes in single user

            var commandText = "USE master; DROP DATABASE " + databaseName;

            void Logic(SqlConnection connection)
            {
                PutDatabaseInSingleUserMode(connection, databaseName, timeout);
                connection.ExecuteNonQuery(commandText, (int)timeout.TotalSeconds);
            }

            RunOperationOnSqlConnection(Logic, realConnectionString);
        }

        /// <summary>
        /// Retrieves the default location that the server will save data files when creating a new database (Only works on MS SQL Server 2012 and higher, otherwise null).
        /// </summary>
        /// <param name="connectionString">Connection string to the intended database server.</param>
        /// <param name="timeout">The command timeout (default is 30 seconds).</param>
        /// <returns>Default location that the server will save data files to.</returns>
        public static string GetInstanceDefaultDataPath(
            string connectionString,
            TimeSpan timeout = default)
        {
            new { connectionString }.AsArg().Must().NotBeNullNorWhiteSpace();

            timeout = timeout == default ? DefaultTimeoutTimespan : timeout;

            string ret = null;
            void Logic(SqlConnection connection)
            {
                var commandText = "SELECT CONVERT(sysname, SERVERPROPERTY('InstanceDefaultDataPath'))";
                ret = connection
                     .ReadSingleValue(commandText, (int)timeout.TotalSeconds)
                    ?.ToString();
            }

            RunOperationOnSqlConnection(Logic, connectionString);

            return ret;
        }

        /// <summary>
        /// Retrieves the default location that the server will save log files when creating a new database (Only works on MS SQL Server 2012 and higher, otherwise null).
        /// </summary>
        /// <param name="connectionString">Connection string to the intended database server.</param>
        /// <param name="timeout">The command timeout (default is 30 seconds).</param>
        /// <returns>Default location that the server will save log files to.</returns>
        public static string GetInstanceDefaultLogPath(
            string connectionString,
            TimeSpan timeout = default)
        {
            new { connectionString }.AsArg().Must().NotBeNullNorWhiteSpace();

            timeout = timeout == default ? DefaultTimeoutTimespan : timeout;

            string ret = null;
            void Logic(SqlConnection connection)
            {
                var commandText = "SELECT CONVERT(sysname, SERVERPROPERTY('InstanceDefaultLogPath'))";
                ret = connection
                     .ReadSingleValue(commandText, (int)timeout.TotalSeconds)
                    ?.ToString();
            }

            RunOperationOnSqlConnection(Logic, connectionString);

            return ret;
        }

        /// <summary>
        /// Perform a full (non-differential) database backup.
        /// </summary>
        /// <remarks>
        /// During a full or differential database backup, SQL Server backs up enough of the transaction log to produce a consistent database when the backup is restored.
        /// When you restore a backup created by BACKUP DATABASE (a data backup), the entire backup is restored. Only a log backup can be restored to a specific time or transaction within the backup
        /// This method does not support appending a backup to an existing file nor any of the methods to age/overwrite backups in an existing file.
        /// This method will always overwrite an existing file.  It's more difficult to get SQL Server to emit an error if a file already exists.
        /// See: <a href="http://dba.stackexchange.com/questions/98536/how-to-generate-an-error-when-backing-up-to-an-existing-file"/>.
        /// </remarks>
        /// <param name="connectionString">Connection string to the intended database server.</param>
        /// <param name="databaseName">Name of the database to backup.</param>
        /// <param name="backupDetails">The details of how to perform the backup.</param>
        /// <param name="announcer">Optional announcer to log messages about what is happening.</param>
        /// <param name="timeout">The command timeout (default is 30 seconds).</param>
        public static void BackupFull(
            string connectionString,
            string databaseName,
            BackupSqlServerDatabaseDetails backupDetails,
            Action<Func<object>> announcer = null,
            TimeSpan timeout = default)
        {
            new { connectionString }.AsArg().Must().NotBeNullNorWhiteSpace();
            new { databaseName }.AsArg().Must().NotBeNullNorWhiteSpace();
            new { backupDetails }.AsArg().Must().NotBeNull();

            void NullAnnounce(Func<object> announcement)
            {
                /* no-op */
            }

            var localAnnouncer = announcer ?? NullAnnounce;

            localAnnouncer(() => new { Database = databaseName, BackupDetails = backupDetails });

            // execute the backup
            var commandText = BuildBackupFullSqlCommandText(databaseName, backupDetails);

            localAnnouncer(() => "Running command: " + commandText);

            void Logic(SqlConnection connection)
            {
                connection.ExecuteNonQuery(commandText, (int)timeout.TotalSeconds);
            }

            RunOperationOnSqlConnection(Logic, connectionString);

            localAnnouncer(() => "Completed successfully.");
        }

        /// <summary>
        /// Perform a full (non-differential) database backup.
        /// </summary>
        /// <remarks>
        /// During a full or differential database backup, SQL Server backs up enough of the transaction log to produce a consistent database when the backup is restored.
        /// When you restore a backup created by BACKUP DATABASE (a data backup), the entire backup is restored. Only a log backup can be restored to a specific time or transaction within the backup
        /// This method does not support appending a backup to an existing file nor any of the methods to age/overwrite backups in an existing file.
        /// This method will always overwrite an existing file.  It's more difficult to get SQL Server to emit an error if a file already exists.
        /// See: <a href="http://dba.stackexchange.com/questions/98536/how-to-generate-an-error-when-backing-up-to-an-existing-file"/>.
        /// </remarks>
        /// <param name="connectionString">Connection string to the intended database server.</param>
        /// <param name="databaseName">Name of the database to backup.</param>
        /// <param name="backupDetails">The details of how to perform the backup.</param>
        /// <param name="announcer">Optional announcer to log messages about what is happening.</param>
        /// <param name="timeout">The command timeout (default is 30 seconds).</param>
        /// <returns>Task to support async await calling.</returns>
        public static async Task BackupFullAsync(
            string connectionString,
            string databaseName,
            BackupSqlServerDatabaseDetails backupDetails,
            Action<Func<object>> announcer = null,
            TimeSpan timeout = default)
        {
            new { connectionString }.AsArg().Must().NotBeNullNorWhiteSpace();
            new { databaseName }.AsArg().Must().NotBeNullNorWhiteSpace();
            new { backupDetails }.AsArg().Must().NotBeNull();

            void NullAnnounce(Func<object> announcement)
            {
                /* no-op */
            }

            var localAnnouncer = announcer ?? NullAnnounce;

            localAnnouncer(() => new { Database = databaseName, BackupDetails = backupDetails });

            // execute the backup
            var commandText = BuildBackupFullSqlCommandText(databaseName, backupDetails);

            localAnnouncer(() => "Running command: " + commandText);

            async Task Logic(SqlConnection connection)
            {
                await connection.ExecuteNonQueryAsync(commandText, (int)timeout.TotalSeconds);
            }

            await RunOperationOnSqlConnectionAsync(Logic, connectionString);

            localAnnouncer(() => "Completed successfully.");
        }

        /// <summary>
        /// Restores an entire database from a full database backup.
        /// </summary>
        /// <param name="connectionString">Connection string to the intended database server.</param>
        /// <param name="databaseName">Name of the database to restore.</param>
        /// <param name="restoreDetails">The details of how to perform the restore.</param>
        /// <param name="announcer">Optional announcer to log messages about what is happening.</param>
        /// <param name="timeout">The command timeout (default is 30 seconds).</param>
        public static void RestoreFull(
            string connectionString,
            string databaseName,
            RestoreSqlServerDatabaseDetails restoreDetails,
            Action<Func<object>> announcer = null,
            TimeSpan timeout = default)
        {
            new { connectionString }.AsArg().Must().NotBeNullNorWhiteSpace();
            new { databaseName }.AsArg().Must().NotBeNullNorWhiteSpace();
            new { restoreDetails }.AsArg().Must().NotBeNull();

            void NullAnnounce(Func<object> announcement)
            {
                /* no-op */
            }

            var localAnnouncer = announcer ?? NullAnnounce;

            localAnnouncer(() => new { Database = databaseName, RestoreDetails = restoreDetails });

            // execute the restore
            var commandText = BuildRestoreFullSqlCommandText(databaseName, restoreDetails, localAnnouncer, connectionString, timeout);

            localAnnouncer(() => "Running command: " + commandText);

            void RestoreLogic(SqlConnection connection)
            {
                connection.ExecuteNonQuery(commandText, (int)timeout.TotalSeconds);
            }

            RunOperationOnSqlConnection(RestoreLogic, connectionString);

            // make sure the database isn't stuck in "Restoring..."
            void ConfirmAvailable(SqlConnection connection)
            {
                connection.ExecuteNonQuery("Select * from sys.tables", (int)timeout.TotalSeconds);
            }

            var databaseSpecificConnectionString = connectionString.AddOrUpdateInitialCatalogInConnectionString(databaseName);
            RunOperationOnSqlConnection(ConfirmAvailable, databaseSpecificConnectionString);

            localAnnouncer(() => "Completed successfully.");
        }

        /// <summary>
        /// Restores an entire database from a full database backup.
        /// </summary>
        /// <param name="connectionString">Connection string to the intended database server.</param>
        /// <param name="databaseName">Name of the database to restore.</param>
        /// <param name="restoreDetails">The details of how to perform the restore.</param>
        /// <param name="announcer">Optional announcer to log messages about what is happening.</param>
        /// <param name="timeout">The command timeout (default is 30 seconds).</param>
        /// <returns>Task to support async await calling.</returns>
        public static async Task RestoreFullAsync(
            string connectionString,
            string databaseName,
            RestoreSqlServerDatabaseDetails restoreDetails,
            Action<Func<object>> announcer = null,
            TimeSpan timeout = default)
        {
            new { connectionString }.AsArg().Must().NotBeNullNorWhiteSpace();
            new { databaseName }.AsArg().Must().NotBeNullNorWhiteSpace();
            new { restoreDetails }.AsArg().Must().NotBeNull();

            void NullAnnounce(Func<object> announcement)
            {
                /* no-op */
            }

            var localAnnouncer = announcer ?? NullAnnounce;

            localAnnouncer(() => new { Database = databaseName, RestoreDetails = restoreDetails });

            // execute the restore
            var commandText = BuildRestoreFullSqlCommandText(databaseName, restoreDetails, localAnnouncer, connectionString, timeout);

            localAnnouncer(() => "Running command: " + commandText);

            async Task RestoreLogic(SqlConnection connection)
            {
                await connection.ExecuteNonQueryAsync(commandText, (int)timeout.TotalSeconds);
            }

            await RunOperationOnSqlConnectionAsync(RestoreLogic, connectionString);

            // make sure the database isn't stuck in "Restoring..."
            async Task ConfirmAvailable(SqlConnection connection)
            {
                await connection.ExecuteNonQueryAsync("Select * from sys.tables", (int)timeout.TotalSeconds);
            }

            var databaseSpecificConnectionString = connectionString.AddOrUpdateInitialCatalogInConnectionString(databaseName);
            await RunOperationOnSqlConnectionAsync(ConfirmAvailable, databaseSpecificConnectionString);

            localAnnouncer(() => "Completed successfully.");
        }

        /// <summary>
        /// Sets the recovery mode of the database.
        /// </summary>
        /// <param name="connectionString">Connection string to the intended database server.</param>
        /// <param name="databaseName">Name of the database to restore.</param>
        /// <param name="recoveryMode">Recovery mode to set database to.</param>
        /// <param name="timeout">The command timeout (default is 30 seconds).</param>
        /// <returns>Task for async.</returns>
        public static async Task SetRecoveryModeAsync(
            string connectionString,
            string databaseName,
            RecoveryMode recoveryMode,
            TimeSpan timeout = default)
        {
            new { connectionString }.AsArg().Must().NotBeNullNorWhiteSpace();
            new { databaseName }.AsArg().Must().NotBeNullNorWhiteSpace();
            new { recoveryMode }.AsArg().Must().NotBeEqualTo(RecoveryMode.Unspecified);

            timeout = timeout == default ? DefaultTimeoutTimespan : timeout;

            async Task Logic(SqlConnection connection)
            {
                var commandText = BuildSetRecoveryModeSqlCommandText(databaseName, recoveryMode);

                await connection.ExecuteNonQueryAsync(commandText, (int)timeout.TotalSeconds);
            }

            await RunOperationOnSqlConnectionAsync(Logic, connectionString);
        }

        /// <summary>
        /// Gets the recovery mode of the database.
        /// </summary>
        /// <param name="connectionString">Connection string to the intended database server.</param>
        /// <param name="databaseName">Name of the database to restore.</param>
        /// <param name="timeout">The command timeout (default is 30 seconds).</param>
        /// <returns>Recovery mode.</returns>
        public static async Task<RecoveryMode> GetRecoveryModeAsync(
            string connectionString,
            string databaseName,
            TimeSpan timeout = default)
        {
            new { connectionString }.AsArg().Must().NotBeNullNorWhiteSpace();
            new { databaseName }.AsArg().Must().NotBeNullNorWhiteSpace().And().BeAlphanumeric(SqlServerDatabaseDefinition.DatabaseNameAlphanumericOtherAllowedCharacters);

            var commandText = $"SELECT recovery_model_desc FROM sys.databases WHERE name = '{databaseName}'";

            var recoveryMode = RecoveryMode.Unspecified;

            async Task Logic(SqlConnection connection)
            {
                var recoveryModeRaw = (await connection.ReadSingleValueAsync(commandText, (int)timeout.TotalSeconds))?.ToString();

                if (string.IsNullOrWhiteSpace(recoveryModeRaw))
                {
                    throw new InvalidOperationException(Invariant($"'{commandText}' should have returned a value but was null or whitespace."));
                }

                recoveryMode = (RecoveryMode)Enum.Parse(typeof(RecoveryMode), recoveryModeRaw.ToUpperInvariant().Replace("_", string.Empty), true);
            }

            await RunOperationOnSqlConnectionAsync(Logic, connectionString);

            return recoveryMode;
        }

        /// <summary>
        /// Get detailed information about a table's makeup.
        /// </summary>
        /// <param name="connectionString">The connection string to the intended server.</param>
        /// <param name="databaseName">The name of the target database.</param>
        /// <param name="tableName">The name of the target table to describe.</param>
        /// <param name="tableSchema">The schema of the table to query details for.</param>
        /// <param name="timeout">The command timeout (default is 30 seconds).</param>
        /// <returns>Detailed information about a table's makeup.</returns>
        public static TableDescription GetTableDescription(
            string connectionString,
            string databaseName,
            string tableName,
            string tableSchema = "dbo",
            TimeSpan timeout = default)
        {
            new { databaseName }.AsArg().Must().NotBeNullNorWhiteSpace().And().BeAlphanumeric(SqlServerDatabaseDefinition.DatabaseNameAlphanumericOtherAllowedCharacters);
            new { tableName }.AsArg().Must().NotBeNullNorWhiteSpace().And().BeAlphanumeric(TableDefinition.TableNameAlphanumericOtherAllowedCharacters);
            new { tableSchema }.AsArg().Must().NotBeNullNorWhiteSpace().And().BeAlphanumeric(TableDefinition.TableSchemaNameAlphanumericOtherAllowedCharacters);

            timeout = timeout == default ? DefaultTimeoutTimespan : timeout;

            var sqlParams = new[]
            {
                databaseName.CreateInputSqlParameter("DatabaseName"),
                tableSchema.CreateInputSqlParameter("Schema"),
                tableName.CreateInputSqlParameter("TableName"),
            };

            var commandText =
                @"SELECT
                        COLUMN_NAME as ColumnName,
                        ORDINAL_POSITION as OrdinalPosition,
                        COLUMN_DEFAULT as ColumnDefault,
                        (CASE UPPER(IS_NULLABLE)
						WHEN 'YES' THEN (cast(1 as bit)) ELSE (cast(0 as bit))  END) as IsNullable,
                        DATA_TYPE as SqlDataType
                FROM INFORMATION_SCHEMA.COLUMNS
                WHERE
                    TABLE_CATALOG   = @DatabaseName
                    AND TABLE_SCHEMA= @Schema
                    AND TABLE_NAME  = @TableName
                ORDER BY ORDINAL_POSITION";

            var columns = new ColumnDescription[0];

            var targetedDatabaseConnectionString = connectionString.AddOrUpdateInitialCatalogInConnectionString(databaseName);

            var propertyBagSerializer = new ObcPropertyBagSerializer();

            void QueryColumnsFilesLogic(SqlConnection connection)
            {
                columns = connection.ReadAllRowsWithNamedColumns(commandText, (int)timeout.TotalSeconds, sqlParams)
                                    .Select(_ => propertyBagSerializer.Deserialize<ColumnDescription>(_))
                                    .ToArray();
            }

            SqlServerDatabaseManager.RunOperationOnSqlConnection(
                QueryColumnsFilesLogic,
                targetedDatabaseConnectionString);

            var ret = new TableDescription(databaseName, tableSchema, tableName, columns);
            return ret;
        }

        private static string BuildSetRecoveryModeSqlCommandText(
            string databaseName,
            RecoveryMode recoveryMode)
        {
            new { databaseName }.AsArg().Must().NotBeNullNorWhiteSpace().And().BeAlphanumeric(SqlServerDatabaseDefinition.DatabaseNameAlphanumericOtherAllowedCharacters);

            string modeMixIn;

            switch (recoveryMode)
            {
                case RecoveryMode.Simple:
                    modeMixIn = "SIMPLE";
                    break;
                case RecoveryMode.Full:
                    modeMixIn = "FULL";
                    break;
                case RecoveryMode.BulkLogged:
                    modeMixIn = "BULK_LOGGED";
                    break;
                default:
                    throw new NotSupportedException(Invariant($"Unsupported recovery mode to set: {recoveryMode}"));
            }

            var result = "ALTER DATABASE " + databaseName + " SET RECOVERY " + modeMixIn;

            return result;
        }

        private static void ThrowIfBad(
            SqlServerDatabaseDefinition definition)
        {
            new { definition.DatabaseName }.AsArg().Must().NotBeNullNorWhiteSpace().And().BeAlphanumeric(SqlServerDatabaseDefinition.DatabaseNameAlphanumericOtherAllowedCharacters);
            new { definition.DataFileLogicalName }.AsArg().Must().NotBeNullNorWhiteSpace().And().BeAlphanumeric(SqlServerDatabaseDefinition.DatabaseLogicalFileNameAlphanumericOtherAllowedCharacters);
            new { definition.LogFileLogicalName }.AsArg().Must().NotBeNullNorWhiteSpace().And().BeAlphanumeric(SqlServerDatabaseDefinition.DatabaseLogicalFileNameAlphanumericOtherAllowedCharacters);

            SqlInjectorChecker.ThrowIfNotValidPath(definition.DataFilePath);
            SqlInjectorChecker.ThrowIfNotValidPath(definition.LogFilePath);
        }

        private static void ThrowIfBadOnCreateOrModify(
            SqlServerDatabaseDefinition definition)
        {
            ThrowIfBad(definition);
            if (definition.DatabaseType == DatabaseType.System)
            {
                throw new InvalidOperationException("Cannot create nor modify system databases.");
            }
        }

        private static void PutDatabaseInSingleUserMode(
            SqlConnection connection,
            string databaseName,
            TimeSpan timeout = default)
        {
            new { databaseName }.AsArg().Must().NotBeNullNorWhiteSpace().And().BeAlphanumeric(SqlServerDatabaseDefinition.DatabaseNameAlphanumericOtherAllowedCharacters);

            timeout = timeout == default ? DefaultTimeoutTimespan : timeout;

            var commandText = "ALTER DATABASE " + databaseName + " SET SINGLE_USER WITH ROLLBACK IMMEDIATE";

            connection.ExecuteNonQuery(commandText, (int)timeout.TotalSeconds);
        }

        private static void PutDatabaseIntoMultiUserMode(
            SqlConnection connection,
            string databaseName,
            TimeSpan timeout = default)
        {
            new { databaseName }.AsArg().Must().NotBeNullNorWhiteSpace().And().BeAlphanumeric(SqlServerDatabaseDefinition.DatabaseNameAlphanumericOtherAllowedCharacters);

            timeout = timeout == default ? DefaultTimeoutTimespan : timeout;

            var commandText = "ALTER DATABASE " + databaseName + " SET MULTI_USER WITH ROLLBACK IMMEDIATE";

            connection.ExecuteNonQuery(commandText, (int)timeout.TotalSeconds);
        }

        private static string BuildBackupFullSqlCommandText(
            string databaseName,
            BackupSqlServerDatabaseDetails backupDetails)
        {
            // construct the non-options portion of the backup command
            var commandBuilder = new StringBuilder();
            var backupDatabase = $"BACKUP DATABASE [{databaseName}]";
            commandBuilder.AppendLine(backupDatabase);

            string deviceName;
            string backupLocation;
            if (backupDetails.Device == Device.Disk)
            {
                deviceName = "DISK";
                backupLocation = backupDetails.BackupTo.LocalPath;
            }
            else if (backupDetails.Device == Device.Url)
            {
                deviceName = "URL";
                backupLocation = backupDetails.BackupTo.ToString();
            }
            else
            {
                throw new NotSupportedException("This device is not supported: " + backupDetails.Device);
            }

            var backupTo = $"TO {deviceName} = '{backupLocation}'";
            commandBuilder.AppendLine(backupTo);

            // construct the WITH options
            commandBuilder.AppendLine("WITH");
            var withOptions = new List<string>();

            if (!string.IsNullOrWhiteSpace(backupDetails.Credential))
            {
                var credential = $"CREDENTIAL = '{backupDetails.Credential}'";
                withOptions.Add(credential);
            }

            string checksumOption;
            if (backupDetails.ChecksumOption == ChecksumOption.Checksum)
            {
                checksumOption = "CHECKSUM";
                withOptions.Add(checksumOption);

                string errorHandling;
                if (backupDetails.ErrorHandling == ErrorHandling.ContinueAfterError)
                {
                    errorHandling = "CONTINUE_AFTER_ERROR";
                }
                else if (backupDetails.ErrorHandling == ErrorHandling.StopOnError)
                {
                    errorHandling = "STOP_ON_ERROR";
                }
                else
                {
                    throw new NotSupportedException(
                        "This error handling option is not supported: " + backupDetails.ErrorHandling);
                }

                withOptions.Add(errorHandling);
            }
            else if (backupDetails.ChecksumOption == ChecksumOption.NoChecksum)
            {
                checksumOption = "NO_CHECKSUM";
                withOptions.Add(checksumOption);
            }
            else
            {
                throw new NotSupportedException(
                    "This checksum option is not supported: " + backupDetails.ChecksumOption);
            }

            string compressionOption;
            if (backupDetails.CompressionOption == CompressionOption.Compression)
            {
                compressionOption = "COMPRESSION";
            }
            else if (backupDetails.CompressionOption == CompressionOption.NoCompression)
            {
                compressionOption = "NO_COMPRESSION";
            }
            else
            {
                throw new NotSupportedException(
                    "This compression option is not supported: " + backupDetails.CompressionOption);
            }

            withOptions.Add(compressionOption);

            if (!string.IsNullOrWhiteSpace(backupDetails.Name))
            {
                var name = $"NAME = '{backupDetails.Name}'";
                withOptions.Add(name);
            }

            if (!string.IsNullOrWhiteSpace(backupDetails.Description))
            {
                var description = $"DESCRIPTION = '{backupDetails.Description}'";
                withOptions.Add(description);
            }

            if (backupDetails.Cipher != Cipher.NoEncryption)
            {
                string cipher;
                if (backupDetails.Cipher == Cipher.Aes128)
                {
                    cipher = "AES_128";
                }
                else if (backupDetails.Cipher == Cipher.Aes192)
                {
                    cipher = "AES_192";
                }
                else if (backupDetails.Cipher == Cipher.Aes256)
                {
                    cipher = "AES_256";
                }
                else if (backupDetails.Cipher == Cipher.TripleDes3Key)
                {
                    cipher = "TRIPLE_DES_3KEY";
                }
                else
                {
                    throw new NotSupportedException("This cipher is not supported: " + backupDetails.Cipher);
                }

                string encryptor;
                if (backupDetails.Encryptor == Encryptor.ServerCertificate)
                {
                    encryptor = "SERVER CERTIFICATE";
                }
                else if (backupDetails.Encryptor == Encryptor.ServerAsymmetricKey)
                {
                    encryptor = "SERVER ASYMMETRIC KEY";
                }
                else
                {
                    throw new NotSupportedException(Invariant($"This {nameof(Encryptor)} is not supported: {backupDetails.Encryptor}"));
                }

                var encryption = $"ENCRYPTION ( ALGORITHM = {cipher}, {encryptor} = {backupDetails.EncryptorName})";
                withOptions.Add(encryption);
            }

            withOptions.Add("FORMAT");

            // append the WITH options
            var withOptionsCsv = withOptions.Aggregate((current, next) => current + "," + Environment.NewLine + next);
            commandBuilder.AppendLine(withOptionsCsv);

            var result = commandBuilder.ToString();

            return result;
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = NaosSuppressBecause.CA1502_AvoidExcessiveComplexity_DisagreeWithAssessment)]
        private static string BuildRestoreFullSqlCommandText(
            string databaseName,
            RestoreSqlServerDatabaseDetails restoreDetails,
            Action<Func<object>> localAnnouncer,
            string connectionString,
            TimeSpan timeout)
        {
            // construct the non-options portion of the backup command
            var commandBuilder = new StringBuilder();
            var backupDatabase = $"RESTORE DATABASE [{databaseName}]";
            commandBuilder.AppendLine(backupDatabase);

            string deviceName;
            string backupLocation;
            if (restoreDetails.Device == Device.Disk)
            {
                deviceName = "DISK";
                backupLocation = restoreDetails.RestoreFrom.LocalPath;
            }
            else if (restoreDetails.Device == Device.Url)
            {
                deviceName = "URL";
                backupLocation = restoreDetails.RestoreFrom.ToString();
            }
            else
            {
                throw new NotSupportedException("This device is not supported: " + restoreDetails.Device);
            }

            var restoreFrom = $"FROM {deviceName} = '{backupLocation}'";
            commandBuilder.AppendLine(restoreFrom);

            // construct the WITH options
            commandBuilder.AppendLine("WITH");
            var withOptions = new List<string>();

            switch (restoreDetails.RecoveryOption)
            {
                case RecoveryOption.Recovery:
                    withOptions.Add("RECOVERY");
                    break;
                case RecoveryOption.NoRecovery:
                    withOptions.Add("NORECOVERY");
                    break;
                default:
                    throw new ArgumentException("Unsupported recovery option: " + restoreDetails.RecoveryOption);
            }

            if (!string.IsNullOrWhiteSpace(restoreDetails.Credential))
            {
                var credential = $"CREDENTIAL = '{restoreDetails.Credential}'";
                withOptions.Add(credential);
            }

            // should the backup be restored to a specific path?
            var useSpecifiedDataFilePath = !string.IsNullOrWhiteSpace(restoreDetails.DataFilePath);
            var useSpecifiedLogFilePath = !string.IsNullOrWhiteSpace(restoreDetails.LogFilePath);
            if (useSpecifiedDataFilePath || useSpecifiedLogFilePath)
            {
                localAnnouncer(() => "Confirming that the specific file is in the server file list.");
                var fileListCommand = "RESTORE FILELISTONLY " + restoreFrom;
                var restoreFiles = new List<RestoreFile>();

                var propertyBagSerializer = new ObcPropertyBagSerializer();
                void QueryRestoreFilesLogic(SqlConnection connection)
                {
                    restoreFiles = connection.ReadAllRowsWithNamedColumns(fileListCommand, (int)timeout.TotalSeconds)
                                             .Select(_ => propertyBagSerializer.Deserialize<RestoreFile>(_))
                                             .ToList();
                }

                RunOperationOnSqlConnection(QueryRestoreFilesLogic, connectionString);

                if (useSpecifiedDataFilePath)
                {
                    var dataFiles = restoreFiles.Where(_ => _.Type == "D").ToList();
                    if (dataFiles.Count != 1)
                    {
                        throw new InvalidOperationException(
                            "Cannot restore from a backup with multiple data files when the file path to restore the data to is specified in the restore details.");
                    }

                    var moveTo = $"MOVE '{dataFiles.First().LogicalName}' TO '{restoreDetails.DataFilePath}'";
                    withOptions.Add(moveTo);
                }

                if (useSpecifiedLogFilePath)
                {
                    var logFiles = restoreFiles.Where(_ => _.Type == "L").ToList();
                    if (logFiles.Count != 1)
                    {
                        throw new InvalidOperationException(
                            "Cannot restore from a backup with multiple log files when the file path to restore the log to is specified in the restore details.");
                    }

                    var moveTo = $"MOVE '{logFiles.First().LogicalName}' TO '{restoreDetails.LogFilePath}'";
                    withOptions.Add(moveTo);
                }
            }

            string checksumOption;
            if (restoreDetails.ChecksumOption == ChecksumOption.Checksum)
            {
                checksumOption = "CHECKSUM";
                withOptions.Add(checksumOption);

                string errorHandling;
                if (restoreDetails.ErrorHandling == ErrorHandling.ContinueAfterError)
                {
                    errorHandling = "CONTINUE_AFTER_ERROR";
                }
                else if (restoreDetails.ErrorHandling == ErrorHandling.StopOnError)
                {
                    errorHandling = "STOP_ON_ERROR";
                }
                else
                {
                    throw new NotSupportedException(
                        "This error handling option is not supported: " + restoreDetails.ErrorHandling);
                }

                withOptions.Add(errorHandling);
            }
            else if (restoreDetails.ChecksumOption == ChecksumOption.NoChecksum)
            {
                checksumOption = "NO_CHECKSUM";
                withOptions.Add(checksumOption);
            }
            else
            {
                throw new NotSupportedException(
                    "This checksum option is not supported: " + restoreDetails.ChecksumOption);
            }

            if (restoreDetails.RestrictedUserOption == RestrictedUserOption.Normal)
            {
            }
            else if (restoreDetails.RestrictedUserOption == RestrictedUserOption.RestrictedUser)
            {
                withOptions.Add("RESTRICTED_USER");
            }
            else
            {
                throw new NotSupportedException(
                    "This restricted user option is not supported: " + restoreDetails.RestrictedUserOption);
            }

            if (restoreDetails.ReplaceOption == ReplaceOption.DoNotReplaceExistingDatabaseAndThrow)
            {
            }
            else if (restoreDetails.ReplaceOption == ReplaceOption.ReplaceExistingDatabase)
            {
                withOptions.Add("REPLACE");
            }
            else
            {
                throw new NotSupportedException(
                    "This replace option is not supported: " + restoreDetails.ReplaceOption);
            }

            // append the WITH options
            var withOptionsCsv = withOptions.Aggregate((current, next) => current + "," + Environment.NewLine + next);
            commandBuilder.AppendLine(withOptionsCsv);

            var result = commandBuilder.ToString();

            return result;
        }
    }
}
