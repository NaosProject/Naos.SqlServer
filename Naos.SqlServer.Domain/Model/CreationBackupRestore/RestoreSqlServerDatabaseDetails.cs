// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RestoreSqlServerDatabaseDetails.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System;
    using Naos.Database.Domain;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Type;

    /// <summary>
    /// Captures the details of a restore operation.
    /// </summary>
    public partial class RestoreSqlServerDatabaseDetails : IModelViaCodeGen, IForsakeDeepCloneWithVariantsViaCodeGen
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RestoreSqlServerDatabaseDetails"/> class.
        /// </summary>
        /// <param name="checksumOption">The checksum option.</param>
        /// <param name="credential">The credential.</param>
        /// <param name="dataFilePath">The data file path.</param>
        /// <param name="device">The device.</param>
        /// <param name="errorHandling">The error handling.</param>
        /// <param name="logFilePath">The log file path.</param>
        /// <param name="recoveryOption">The recovery option.</param>
        /// <param name="replaceOption">The replace option.</param>
        /// <param name="restoreFrom">The restore from.</param>
        /// <param name="restrictedUserOption">The restricted user option.</param>
        /// <exception cref="System.ArgumentException">
        /// Credential cannot be null or whitespace when Device is URL
        /// or
        /// ErrorHandling cannot be None when using checksum.
        /// </exception>
        public RestoreSqlServerDatabaseDetails(
            ChecksumOption checksumOption,
            string credential,
            string dataFilePath,
            Device device,
            ErrorHandling errorHandling,
            string logFilePath,
            RecoveryOption recoveryOption,
            ReplaceOption replaceOption,
            Uri restoreFrom,
            RestrictedUserOption restrictedUserOption)
        {
            new { restoreFrom }.AsArg().Must().NotBeNull();

            if (device == Device.Url)
            {
                if (string.IsNullOrWhiteSpace(credential))
                {
                    throw new ArgumentException("Credential cannot be null or whitespace when Device is URL");
                }

                SqlInjectorChecker.ThrowIfNotAlphanumericOrSpaceOrUnderscore(credential);
            }

            if (!string.IsNullOrWhiteSpace(dataFilePath))
            {
                SqlInjectorChecker.ThrowIfNotValidPath(dataFilePath);
            }

            if (!string.IsNullOrWhiteSpace(logFilePath))
            {
                SqlInjectorChecker.ThrowIfNotValidPath(logFilePath);
            }

            if (checksumOption == ChecksumOption.Checksum)
            {
                if (errorHandling == ErrorHandling.None)
                {
                    throw new ArgumentException("ErrorHandling cannot be None when using checksum.");
                }
            }

            this.ChecksumOption = checksumOption;
            this.Credential = credential;
            this.DataFilePath = dataFilePath;
            this.Device = device;
            this.ErrorHandling = errorHandling;
            this.LogFilePath = logFilePath;
            this.RecoveryOption = recoveryOption;
            this.ReplaceOption = replaceOption;
            this.RestoreFrom = restoreFrom;
            this.RestrictedUserOption = restrictedUserOption;
        }

        /// <summary>
        /// Gets an enum value indicating whether to enable checksums.
        /// </summary>
        public ChecksumOption ChecksumOption { get; private set; }

        /// <summary>
        /// Gets the name of the credential to use when restoring from a URL.
        /// </summary>
        public string Credential { get; private set; }

        /// <summary>
        /// Gets the path to write the data file to.
        /// Must be a file and NOT a directory (i.e. c:\SqlServer\Data\MyDatabase.mdf, not c:\SqlServer\Data\).
        /// </summary>
        /// <remarks>
        /// If null then to use the path where the data file was backed up from.
        /// </remarks>
        public string DataFilePath { get; private set; }

        /// <summary>
        /// Gets an enum value indicating the device to restore from.
        /// </summary>
        public Device Device { get; private set; }

        /// <summary>
        /// Gets an enum value for the error handling method to use.
        /// </summary>
        public ErrorHandling ErrorHandling { get; private set; }

        /// <summary>
        /// Gets the full path to the log file.
        /// Must be a file and NOT a directory (i.e. c:\SqlServer\Data\MyDatabase.mdf, not c:\SqlServer\Data\).
        /// </summary>
        /// <remarks>
        /// If null then to use the path where the log file was backed up from.
        /// </remarks>
        public string LogFilePath { get; private set; }

        /// <summary>
        /// Gets an enum value for the recovery option.
        /// </summary>
        public RecoveryOption RecoveryOption { get; private set; }

        /// <summary>
        /// Gets an enum value with instructions on what to do when restoring to a database that already exists.
        /// </summary>
        public ReplaceOption ReplaceOption { get; private set; }

        /// <summary>
        /// Gets the location at which to pull the backup for restoration (i.e. file path or URL)
        /// For local/network storage, must be a file and NOT a directory (i.e. c:\MyBackups\TodayBackup.bak, not c:\MyBackups).
        /// </summary>
        public Uri RestoreFrom { get; private set; }

        /// <summary>
        /// Gets an enum value that indicates whether or not to put the database into restricted user mode after restoring.
        /// </summary>
        public RestrictedUserOption RestrictedUserOption { get; private set; }
    }
}
