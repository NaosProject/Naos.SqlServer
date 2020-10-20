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

    /// <summary>
    /// Captures the details of a restore operation.
    /// </summary>
    public class RestoreSqlServerDatabaseDetails
    {
        /// <summary>
        /// Gets or sets an enum value indicating whether to enable checksums.
        /// </summary>
        public ChecksumOption ChecksumOption { get; set; }

        /// <summary>
        /// Gets or sets the name of the credential to use when restoring from a URL.
        /// </summary>
        public string Credential { get; set; }

        /// <summary>
        /// Gets or sets the path to write the data file to.
        /// Must be a file and NOT a directory (i.e. c:\SqlServer\Data\MyDatabase.mdf, not c:\SqlServer\Data\).
        /// </summary>
        /// <remarks>
        /// If null then to use the path where the data file was backed up from.
        /// </remarks>
        public string DataFilePath { get; set; }

        /// <summary>
        /// Gets or sets an enum value indicating the device to restore from.
        /// </summary>
        public Device Device { get; set; }

        /// <summary>
        /// Gets or sets an enum value for the error handling method to use.
        /// </summary>
        public ErrorHandling ErrorHandling { get; set; }

        /// <summary>
        /// Gets or sets the full path to the log file.
        /// Must be a file and NOT a directory (i.e. c:\SqlServer\Data\MyDatabase.mdf, not c:\SqlServer\Data\).
        /// </summary>
        /// <remarks>
        /// If null then to use the path where the log file was backed up from.
        /// </remarks>
        public string LogFilePath { get; set; }

        /// <summary>
        /// Gets or sets an enum value for the recovery option.
        /// </summary>
        public RecoveryOption RecoveryOption { get; set; }

        /// <summary>
        /// Gets or sets an enum value with instructions on what to do when restoring to a database that already exists.
        /// </summary>
        public ReplaceOption ReplaceOption { get; set; }

        /// <summary>
        /// Gets or sets the location at which to pull the backup for restoration (i.e. file path or URL)
        /// For local/network storage, must be a file and NOT a directory (i.e. c:\MyBackups\TodayBackup.bak, not c:\MyBackups).
        /// </summary>
        public Uri RestoreFrom { get; set; }

        /// <summary>
        /// Gets or sets an enum value that indicates whether or not to put the database into restricted user mode after restoring.
        /// </summary>
        public RestrictedUserOption RestrictedUserOption { get; set; }
    }

    /// <summary>
    /// Extension methods for <see cref="RestoreSqlServerDatabaseDetails"/>.
    /// </summary>
    public static class RestoreSqlServerDatabaseDetailsExtensions
    {
        /// <summary>
        /// Throws an exception if the <see cref="RestoreSqlServerDatabaseDetails"/> is invalid.
        /// </summary>
        /// <param name="restoreDetails">The restore details to validate.</param>
        public static void ThrowIfInvalid(this RestoreSqlServerDatabaseDetails restoreDetails)
        {
            new { restoreDetails }.AsArg().Must().NotBeNull();
            new { restoreDetails.RestoreFrom }.AsArg().Must().NotBeNull();

            if (restoreDetails.Device == Device.Url)
            {
                if (string.IsNullOrWhiteSpace(restoreDetails.Credential))
                {
                    throw new ArgumentException("Credential cannot be null or whitespace when Device is URL");
                }

                SqlInjectorChecker.ThrowIfNotAlphanumericOrSpaceOrUnderscore(restoreDetails.Credential);
            }

            if (!string.IsNullOrWhiteSpace(restoreDetails.DataFilePath))
            {
                SqlInjectorChecker.ThrowIfNotValidPath(restoreDetails.DataFilePath);
            }

            if (!string.IsNullOrWhiteSpace(restoreDetails.LogFilePath))
            {
                SqlInjectorChecker.ThrowIfNotValidPath(restoreDetails.LogFilePath);
            }

            if (restoreDetails.ChecksumOption == ChecksumOption.Checksum)
            {
                if (restoreDetails.ErrorHandling == ErrorHandling.None)
                {
                    throw new ArgumentException("ErrorHandling cannot be None when using checksum.");
                }
            }
        }
    }
}
