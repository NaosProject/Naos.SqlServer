// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BackupSqlServerDatabaseDetails.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using OBeautifulCode.Assertion.Recipes;

    /// <summary>
    /// Captures the details of a backup operation.
    /// </summary>
    public class BackupSqlServerDatabaseDetails
    {
        /// <summary>
        /// Gets or sets the location at which to save the backup (i.e. file path or URL).
        /// For local/network storage, must be a file and NOT a directory (i.e. c:\MyBackups\TodayBackup.bak, not c:\MyBackups).
        /// </summary>
        public Uri BackupTo { get; set; }

        /// <summary>
        /// Gets or sets an enum value indicating whether to enable checksums.
        /// </summary>
        public ChecksumOption ChecksumOption { get; set; }

        /// <summary>
        /// Gets or sets an enum value for the cipher to use when encrypting the backup.
        /// </summary>
        public Cipher Cipher { get; set; }

        /// <summary>
        /// Gets or sets an enum value indicating whether to compress the backup.
        /// </summary>
        public CompressionOption CompressionOption { get; set; }

        /// <summary>
        /// Gets or sets the name of the credential to use when backing up to a URL.
        /// </summary>
        public string Credential { get; set; }

        /// <summary>
        /// Gets or sets a description of the backup.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets an enum value indicating the device to backup to.
        /// </summary>
        public Device Device { get; set; }

        /// <summary>
        /// Gets or sets an enum value for the encryptor to use when encrypting the backup.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Encryptor", Justification = "Spelling/name is correct.")]
        public Encryptor Encryptor { get; set; }

        /// <summary>
        /// Gets or sets the name of the encryptor (i.e. server certificate name or asymmetric key name).
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Encryptor", Justification = "Spelling/name is correct.")]
        public string EncryptorName { get; set; }

        /// <summary>
        /// Gets or sets an enum value indicating the error handling method to use.
        /// </summary>
        public ErrorHandling ErrorHandling { get; set; }

        /// <summary>
        /// Gets or sets the name of the backup (not the name of the backup file,
        /// but rather the name of the backup set identifying the backup within the file).
        /// If not specified, it is blank.
        /// </summary>
        public string Name { get; set; }
    }

    /// <summary>
    /// Extension methods for <see cref="BackupSqlServerDatabaseDetails"/>.
    /// </summary>
    public static class BackupSqlServerDatabaseDetailsExtensions
    {
        /// <summary>
        /// Throws an exception if the <see cref="BackupSqlServerDatabaseDetails"/> is invalid.
        /// </summary>
        /// <param name="backupDetails">The backup details to validate.</param>
        public static void ThrowIfInvalid(this BackupSqlServerDatabaseDetails backupDetails)
        {
            new { backupDetails }.AsArg().Must().NotBeNull();
            new { backupDetails.BackupTo }.AsArg().Must().NotBeNull();

            if (backupDetails.Device == Device.Url)
            {
                if (string.IsNullOrWhiteSpace(backupDetails.Credential))
                {
                    throw new ArgumentException("Credential cannot be null or whitespace when Device is URL");
                }

                SqlInjectorChecker.ThrowIfNotAlphanumericOrSpaceOrUnderscore(backupDetails.Credential);
            }

            if (!string.IsNullOrWhiteSpace(backupDetails.Name))
            {
                if (backupDetails.Name.Length > 128)
                {
                    throw new ArgumentException("Name cannot be more than 128 characters in length.");
                }
            }

            if (!string.IsNullOrWhiteSpace(backupDetails.Description))
            {
                if (backupDetails.Description.Length > 255)
                {
                    throw new ArgumentException("Description cannot be more than 255 characters in length.");
                }
            }

            if (backupDetails.Cipher != Cipher.NoEncryption)
            {
                if (backupDetails.Encryptor == Encryptor.None)
                {
                    throw new ArgumentException("Encryptor is required when any Cipher != NoEncryption");
                }

                if (string.IsNullOrWhiteSpace(backupDetails.EncryptorName))
                {
                    throw new ArgumentException("EncryptorName is required when any Cipher != NoEncryption.");
                }

                SqlInjectorChecker.ThrowIfNotAlphanumericOrSpaceOrUnderscore(backupDetails.EncryptorName);
            }

            if (backupDetails.ChecksumOption == ChecksumOption.Checksum)
            {
                if (backupDetails.ErrorHandling == ErrorHandling.None)
                {
                    throw new ArgumentException("ErrorHandling cannot be None when using checksum.");
                }
            }

            if (!string.IsNullOrWhiteSpace(backupDetails.Name))
            {
                SqlInjectorChecker.ThrowIfNotAlphanumericOrSpaceOrUnderscore(backupDetails.Name);
            }

            if (!string.IsNullOrWhiteSpace(backupDetails.Description))
            {
                SqlInjectorChecker.ThrowIfNotAlphanumericOrSpaceOrUnderscore(backupDetails.Description);
            }
        }
    }
}
