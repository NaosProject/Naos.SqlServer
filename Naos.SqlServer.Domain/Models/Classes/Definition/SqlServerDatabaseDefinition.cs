﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlServerDatabaseDefinition.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using Naos.CodeAnalysis.Recipes;
    using Naos.Database.Domain;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Type;
    using static System.FormattableString;

    /// <summary>
    /// Detailed information about the database's configuration (file size and name type stuff).
    /// </summary>
    public partial class SqlServerDatabaseDefinition : IDatabaseDefinition, IModelViaCodeGen
    {
        /// <summary>
        /// The extension (WITHOUT the PERIOD) of an MS SQL Server Data File.
        /// </summary>
        public const string MicrosoftSqlDataFileExtension = "mdf";

        /// <summary>
        /// The extension (WITHOUT the PERIOD) of an MS SQL Server Log File.
        /// </summary>
        public const string MicrosoftSqlLogFileExtension = "ldf";

        /// <summary>
        /// The default size of the data and log files for MS SQL Server.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Kb", Justification = NaosSuppressBecause.CA1709_IdentifiersShouldBeCasedCorrectly_CasingIsAsPreferred)]
        public const long MicrosoftSqlDefaultCurrentFileSizeInKb = 8000L;

        /// <summary>
        /// The default size of growth for the data and log files for MS SQL Server.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Kb", Justification = NaosSuppressBecause.CA1709_IdentifiersShouldBeCasedCorrectly_CasingIsAsPreferred)]
        public const long MicrosoftSqlDefaultFileGrowthSizeInKb = 64000L;

        /// <summary>
        /// The value to be specified for unlimited file growth in an MS SQL Server file.
        /// </summary>
        public const int InfinityMaxSize = -1;

        /// <summary>
        /// The characters that are allowed in a database name, in addition to alphanumeric characters.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = NaosSuppressBecause.CA2104_DoNotDeclareReadOnlyMutableReferenceTypes_TypeIsImmutable)]
        public static readonly IReadOnlyCollection<char> DatabaseNameAlphanumericOtherAllowedCharacters = new[] { ' ', '_' };

        /// <summary>
        /// The characters that are allowed in a database logical file name, in addition to alphanumeric characters.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = NaosSuppressBecause.CA2104_DoNotDeclareReadOnlyMutableReferenceTypes_TypeIsImmutable)]
        public static readonly IReadOnlyCollection<char> DatabaseLogicalFileNameAlphanumericOtherAllowedCharacters = new[] { ' ', '_' };

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerDatabaseDefinition"/> class.
        /// </summary>
        /// <param name="databaseName">Name of the database.</param>
        /// <param name="databaseType">Type of the database.</param>
        /// <param name="recoveryMode">The recovery mode.</param>
        /// <param name="dataFileLogicalName">Name of the data file logical.</param>
        /// <param name="dataFilePath">The data file path.</param>
        /// <param name="dataFileCurrentSizeInKb">The data file current size in kilobytes.</param>
        /// <param name="dataFileMaxSizeInKb">The data file maximum size in kilobytes.</param>
        /// <param name="dataFileGrowthSizeInKb">The data file growth size in kilobytes.</param>
        /// <param name="logFileLogicalName">Name of the log file logical.</param>
        /// <param name="logFilePath">The log file path.</param>
        /// <param name="logFileCurrentSizeInKb">The log file current size in kilobytes.</param>
        /// <param name="logFileMaxSizeInKb">The log file maximum size in kilobytes.</param>
        /// <param name="logFileGrowthSizeInKb">The log file growth size in kilobytes.</param>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Kb", Justification = NaosSuppressBecause.CA1709_IdentifiersShouldBeCasedCorrectly_CasingIsAsPreferred)]
        public SqlServerDatabaseDefinition(
            string databaseName,
            DatabaseType databaseType,
            RecoveryMode recoveryMode,
            string dataFileLogicalName,
            string dataFilePath,
            long dataFileCurrentSizeInKb,
            long dataFileMaxSizeInKb,
            long dataFileGrowthSizeInKb,
            string logFileLogicalName,
            string logFilePath,
            long logFileCurrentSizeInKb,
            long logFileMaxSizeInKb,
            long logFileGrowthSizeInKb)
        {
            databaseName.MustForArg(nameof(databaseName)).NotBeNullNorWhiteSpace().And().BeAlphanumeric(DatabaseNameAlphanumericOtherAllowedCharacters);

            if (!string.IsNullOrWhiteSpace(dataFileLogicalName))
            {
                dataFileLogicalName.MustForArg(nameof(dataFileLogicalName)).BeAlphanumeric(DatabaseLogicalFileNameAlphanumericOtherAllowedCharacters);
            }

            if (!string.IsNullOrWhiteSpace(dataFilePath))
            {
                SqlInjectorChecker.ThrowIfNotValidPath(dataFilePath, nameof(dataFilePath));
            }

            if (!string.IsNullOrWhiteSpace(logFileLogicalName))
            {
                logFileLogicalName.MustForArg(nameof(logFileLogicalName)).BeAlphanumeric(DatabaseLogicalFileNameAlphanumericOtherAllowedCharacters);
            }

            if (!string.IsNullOrWhiteSpace(logFilePath))
            {
                SqlInjectorChecker.ThrowIfNotValidPath(logFilePath, nameof(logFilePath));
            }

            this.DatabaseName = databaseName;
            this.DatabaseType = databaseType;
            this.RecoveryMode = recoveryMode;
            this.DataFileLogicalName = dataFileLogicalName;
            this.DataFilePath = dataFilePath;
            this.LogFilePath = logFilePath;
            this.DataFileCurrentSizeInKb = dataFileCurrentSizeInKb;
            this.DataFileMaxSizeInKb = dataFileMaxSizeInKb;
            this.DataFileGrowthSizeInKb = dataFileGrowthSizeInKb;
            this.LogFileLogicalName = logFileLogicalName;
            this.LogFileCurrentSizeInKb = logFileCurrentSizeInKb;
            this.LogFileMaxSizeInKb = logFileMaxSizeInKb;
            this.LogFileGrowthSizeInKb = logFileGrowthSizeInKb;
        }

        /// <inheritdoc />
        public string DatabaseName { get; private set; }

        /// <summary>
        /// Gets the type of database.
        /// </summary>
        public DatabaseType DatabaseType { get; private set; }

        /// <summary>
        /// Gets the recovery mode.
        /// </summary>
        public RecoveryMode RecoveryMode { get; private set; }

        /// <summary>
        /// Gets the metadata name of the data file.
        /// </summary>
        public string DataFileLogicalName { get; private set; }

        /// <summary>
        /// Gets the full path to the data file.
        /// </summary>
        public string DataFilePath { get; private set; }

        /// <summary>
        /// Gets the full path to the log file.
        /// </summary>
        public string LogFilePath { get; private set; }

        /// <summary>
        /// Gets the current size of data file in kilobytes.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Kb", Justification = NaosSuppressBecause.CA1709_IdentifiersShouldBeCasedCorrectly_CasingIsAsPreferred)]
        public long DataFileCurrentSizeInKb { get; private set; }

        /// <summary>
        /// Gets the max size of data file in kilobytes.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Kb", Justification = NaosSuppressBecause.CA1709_IdentifiersShouldBeCasedCorrectly_CasingIsAsPreferred)]
        public long DataFileMaxSizeInKb { get; private set; }

        /// <summary>
        /// Gets the size of growth interval of data file in kilobytes.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Kb", Justification = NaosSuppressBecause.CA1709_IdentifiersShouldBeCasedCorrectly_CasingIsAsPreferred)]
        public long DataFileGrowthSizeInKb { get; private set; }

        /// <summary>
        /// Gets the metadata name of the log file.
        /// </summary>
        public string LogFileLogicalName { get; private set; }

        /// <summary>
        /// Gets the current size of log file in kilobytes.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Kb", Justification = NaosSuppressBecause.CA1709_IdentifiersShouldBeCasedCorrectly_CasingIsAsPreferred)]
        public long LogFileCurrentSizeInKb { get; private set; }

        /// <summary>
        /// Gets the max size of data file in kilobytes.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Kb", Justification = NaosSuppressBecause.CA1709_IdentifiersShouldBeCasedCorrectly_CasingIsAsPreferred)]
        public long LogFileMaxSizeInKb { get; private set; }

        /// <summary>
        /// Gets the size of growth interval of log file in kilobytes.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Kb", Justification = NaosSuppressBecause.CA1709_IdentifiersShouldBeCasedCorrectly_CasingIsAsPreferred)]
        public long LogFileGrowthSizeInKb { get; private set; }

        /// <summary>
        /// Builds the database configuration using defaults in substitute of any direct specification not provided.
        /// </summary>
        /// <param name="databaseName">Name of the database.</param>
        /// <param name="dataDirectory">The data directory.</param>
        /// <param name="databaseType">The database type.</param>
        /// <param name="logDirectory">The log directory.</param>
        /// <param name="recoveryMode">The recovery mode.</param>
        /// <param name="dataFileLogicalName">Name of the data file logical.</param>
        /// <param name="dataFileNameOnDisk">The data file name on disk.</param>
        /// <param name="dataFileCurrentSizeInKb">The data file current size in kb.</param>
        /// <param name="dataFileMaxSizeInKb">The data file maximum size in kb.</param>
        /// <param name="dataFileGrowthSizeInKb">The data file growth size in kb.</param>
        /// <param name="logFileLogicalName">Name of the log file logical.</param>
        /// <param name="logFileNameOnDisk">The log file name on disk.</param>
        /// <param name="logFileCurrentSizeInKb">The log file current size in kb.</param>
        /// <param name="logFileMaxSizeInKb">The log file maximum size in kb.</param>
        /// <param name="logFileGrowthSizeInKb">The log file growth size in kb.</param>
        /// <returns>DatabaseConfiguration.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Kb", Justification = NaosSuppressBecause.CA1709_IdentifiersShouldBeCasedCorrectly_CasingIsAsPreferred)]
        public static SqlServerDatabaseDefinition BuildDatabaseConfigurationUsingDefaultsAsNecessary(
            string databaseName,
            string dataDirectory,
            DatabaseType databaseType = DatabaseType.User,
            string logDirectory = null,
            RecoveryMode recoveryMode = RecoveryMode.Simple,
            string dataFileLogicalName = null,
            string dataFileNameOnDisk = null,
            long? dataFileCurrentSizeInKb = null,
            long? dataFileMaxSizeInKb = null,
            long? dataFileGrowthSizeInKb = null,
            string logFileLogicalName = null,
            string logFileNameOnDisk = null,
            long? logFileCurrentSizeInKb = null,
            long? logFileMaxSizeInKb = null,
            long? logFileGrowthSizeInKb = null)
        {
            databaseName.MustForArg(nameof(databaseName)).NotBeNullNorWhiteSpace();

            var databaseConfiguration = new SqlServerDatabaseDefinition(
                databaseName,
                databaseType,
                recoveryMode,
                dataFileLogicalName ?? Invariant($"{databaseName}Data"),
                Path.Combine(
                    dataDirectory,
                    dataFileNameOnDisk ?? Invariant($"{databaseName}Data.{MicrosoftSqlDataFileExtension}")),
                dataFileCurrentSizeInKb ?? MicrosoftSqlDefaultCurrentFileSizeInKb,
                dataFileMaxSizeInKb     ?? InfinityMaxSize,
                dataFileGrowthSizeInKb  ?? MicrosoftSqlDefaultFileGrowthSizeInKb,
                logFileLogicalName      ?? Invariant($"{databaseName}Log"),
                Path.Combine(
                    logDirectory      ?? dataDirectory,
                    logFileNameOnDisk ?? Invariant($"{databaseName}Log.{MicrosoftSqlLogFileExtension}")),
                logFileCurrentSizeInKb ?? MicrosoftSqlDefaultCurrentFileSizeInKb,
                logFileMaxSizeInKb     ?? InfinityMaxSize,
                logFileGrowthSizeInKb  ?? MicrosoftSqlDefaultFileGrowthSizeInKb);

            return databaseConfiguration;
        }
    }
}