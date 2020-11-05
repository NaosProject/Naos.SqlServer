// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BackupSqlServerDatabaseDetails.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Naos.CodeAnalysis.Recipes;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Type;

    /// <summary>
    /// Captures the details of a backup operation.
    /// </summary>
    public class BackupSqlServerDatabaseDetails : IModelViaCodeGen
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BackupSqlServerDatabaseDetails"/> class.
        /// </summary>
        /// <param name="backupTo">The backup to.</param>
        /// <param name="checksumOption">The checksum option.</param>
        /// <param name="cipher">The cipher.</param>
        /// <param name="compressionOption">The compression option.</param>
        /// <param name="credential">The credential.</param>
        /// <param name="description">The description.</param>
        /// <param name="device">The device.</param>
        /// <param name="encryptor">The encryptor.</param>
        /// <param name="encryptorName">Name of the encryptor.</param>
        /// <param name="errorHandling">The error handling.</param>
        /// <param name="name">The name.</param>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "encryptor", Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
        public BackupSqlServerDatabaseDetails(
            Uri backupTo,
            ChecksumOption checksumOption,
            Cipher cipher,
            CompressionOption compressionOption,
            string credential,
            string description,
            Device device,
            Encryptor encryptor,
            string encryptorName,
            ErrorHandling errorHandling,
            string name)
        {
            new { backupTo }.AsArg().Must().NotBeNull();

            if (device == Device.Url)
            {
                if (string.IsNullOrWhiteSpace(credential))
                {
                    throw new ArgumentException("Credential cannot be null or whitespace when Device is URL");
                }

                SqlInjectorChecker.ThrowIfNotAlphanumericOrSpaceOrUnderscore(credential);
            }

            if (!string.IsNullOrWhiteSpace(name))
            {
                if (name.Length > 128)
                {
                    throw new ArgumentException("Name cannot be more than 128 characters in length.");
                }
            }

            if (!string.IsNullOrWhiteSpace(description))
            {
                if (description.Length > 255)
                {
                    throw new ArgumentException("Description cannot be more than 255 characters in length.");
                }
            }

            if (cipher != Cipher.NoEncryption)
            {
                if (encryptor == Encryptor.None)
                {
                    throw new ArgumentException("Encryptor is required when any Cipher != NoEncryption");
                }

                if (string.IsNullOrWhiteSpace(encryptorName))
                {
                    throw new ArgumentException("EncryptorName is required when any Cipher != NoEncryption.");
                }

                SqlInjectorChecker.ThrowIfNotAlphanumericOrSpaceOrUnderscore(encryptorName);
            }

            if (checksumOption == ChecksumOption.Checksum)
            {
                if (errorHandling == ErrorHandling.None)
                {
                    throw new ArgumentException("ErrorHandling cannot be None when using checksum.");
                }
            }

            if (!string.IsNullOrWhiteSpace(name))
            {
                SqlInjectorChecker.ThrowIfNotAlphanumericOrSpaceOrUnderscore(name);
            }

            if (!string.IsNullOrWhiteSpace(description))
            {
                SqlInjectorChecker.ThrowIfNotAlphanumericOrSpaceOrUnderscore(description);
            }

            this.BackupTo = backupTo;
            this.ChecksumOption = checksumOption;
            this.Cipher = cipher;
            this.CompressionOption = compressionOption;
            this.Credential = credential;
            this.Description = description;
            this.Device = device;
            this.Encryptor = encryptor;
            this.EncryptorName = encryptorName;
            this.ErrorHandling = errorHandling;
            this.Name = name;
        }

        /// <summary>
        /// Gets the location at which to save the backup (i.e. file path or URL).
        /// For local/network storage, must be a file and NOT a directory (i.e. c:\MyBackups\TodayBackup.bak, not c:\MyBackups).
        /// </summary>
        public Uri BackupTo { get; private set; }

        /// <summary>
        /// Gets an enum value indicating whether to enable checksums.
        /// </summary>
        public ChecksumOption ChecksumOption { get; private set; }

        /// <summary>
        /// Gets an enum value for the cipher to use when encrypting the backup.
        /// </summary>
        public Cipher Cipher { get; private set; }

        /// <summary>
        /// Gets an enum value indicating whether to compress the backup.
        /// </summary>
        public CompressionOption CompressionOption { get; private set; }

        /// <summary>
        /// Gets the name of the credential to use when backing up to a URL.
        /// </summary>
        public string Credential { get; private set; }

        /// <summary>
        /// Gets a description of the backup.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Gets an enum value indicating the device to backup to.
        /// </summary>
        public Device Device { get; private set; }

        /// <summary>
        /// Gets an enum value for the encryptor to use when encrypting the backup.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Encryptor", Justification = "Spelling/name is correct.")]
        public Encryptor Encryptor { get; private set; }

        /// <summary>
        /// Gets the name of the encryptor (i.e. server certificate name or asymmetric key name).
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Encryptor", Justification = "Spelling/name is correct.")]
        public string EncryptorName { get; private set; }

        /// <summary>
        /// Gets an enum value indicating the error handling method to use.
        /// </summary>
        public ErrorHandling ErrorHandling { get; private set; }

        /// <summary>
        /// Gets the name of the backup (not the name of the backup file,
        /// but rather the name of the backup set identifying the backup within the file).
        /// If not specified, it is blank.
        /// </summary>
        public string Name { get; private set; }
    }
}
