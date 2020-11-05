// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DatabaseConfiguration.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System.Diagnostics.CodeAnalysis;
    using Naos.CodeAnalysis.Recipes;
    using OBeautifulCode.Type;

    /// <summary>
    /// Detailed information about the database's configuration (file size and name type stuff).
    /// </summary>
    public class DatabaseConfiguration : IModelViaCodeGen
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseConfiguration"/> class.
        /// </summary>
        /// <param name="databaseName">Name of the database.</param>
        /// <param name="databaseType">Type of the database.</param>
        /// <param name="recoveryMode">The recovery mode.</param>
        /// <param name="dataFileLogicalName">Name of the data file logical.</param>
        /// <param name="dataFilePath">The data file path.</param>
        /// <param name="logFilePath">The log file path.</param>
        /// <param name="dataFileCurrentSizeInKb">The data file current size in kilobytes.</param>
        /// <param name="dataFileMaxSizeInKb">The data file maximum size in kilobytes.</param>
        /// <param name="dataFileGrowthSizeInKb">The data file growth size in kilobytes.</param>
        /// <param name="logFileLogicalName">Name of the log file logical.</param>
        /// <param name="logFileCurrentSizeInKb">The log file current size in kilobytes.</param>
        /// <param name="logFileMaxSizeInKb">The log file maximum size in kilobytes.</param>
        /// <param name="logFileGrowthSizeInKb">The log file growth size in kilobytes.</param>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Kb", Justification = "Name is correct in context.")]
        public DatabaseConfiguration(
            string databaseName,
            DatabaseType databaseType,
            RecoveryMode recoveryMode,
            string dataFileLogicalName,
            string dataFilePath,
            string logFilePath,
            long dataFileCurrentSizeInKb,
            long dataFileMaxSizeInKb,
            long dataFileGrowthSizeInKb,
            string logFileLogicalName,
            long logFileCurrentSizeInKb,
            long logFileMaxSizeInKb,
            long logFileGrowthSizeInKb)
        {
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

        /// <summary>
        /// Gets the name of database.
        /// </summary>
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
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Kb", Justification = "Spelling/name is correct.")]
        public long DataFileCurrentSizeInKb { get; private set; }

        /// <summary>
        /// Gets the max size of data file in kilobytes.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Kb", Justification = "Spelling/name is correct.")]
        public long DataFileMaxSizeInKb { get; private set; }

        /// <summary>
        /// Gets the size of growth interval of data file in kilobytes.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Kb", Justification = "Spelling/name is correct.")]
        public long DataFileGrowthSizeInKb { get; private set; }

        /// <summary>
        /// Gets the metadata name of the log file.
        /// </summary>
        public string LogFileLogicalName { get; private set; }

        /// <summary>
        /// Gets the current size of log file in kilobytes.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Kb", Justification = "Spelling/name is correct.")]
        public long LogFileCurrentSizeInKb { get; private set; }

        /// <summary>
        /// Gets the max size of data file in kilobytes.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Kb", Justification = "Spelling/name is correct.")]
        public long LogFileMaxSizeInKb { get; private set; }

        /// <summary>
        /// Gets the size of growth interval of log file in kilobytes.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Kb", Justification = "Spelling/name is correct.")]
        public long LogFileGrowthSizeInKb { get; private set; }
    }
}
