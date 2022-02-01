﻿// --------------------------------------------------------------------------------------------------------------------
// <auto-generated>
//   Generated using OBeautifulCode.CodeGen.ModelObject (1.0.174.0)
// </auto-generated>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using global::System;
    using global::System.CodeDom.Compiler;
    using global::System.Collections.Concurrent;
    using global::System.Collections.Generic;
    using global::System.Collections.ObjectModel;
    using global::System.Diagnostics.CodeAnalysis;
    using global::System.Globalization;
    using global::System.Linq;

    using global::OBeautifulCode.Cloning.Recipes;
    using global::OBeautifulCode.Equality.Recipes;
    using global::OBeautifulCode.Type;
    using global::OBeautifulCode.Type.Recipes;

    using static global::System.FormattableString;

    [Serializable]
    public partial class BackupSqlServerDatabaseDetails : IModel<BackupSqlServerDatabaseDetails>
    {
        /// <summary>
        /// Determines whether two objects of type <see cref="BackupSqlServerDatabaseDetails"/> are equal.
        /// </summary>
        /// <param name="left">The object to the left of the equality operator.</param>
        /// <param name="right">The object to the right of the equality operator.</param>
        /// <returns>true if the two items are equal; otherwise false.</returns>
        public static bool operator ==(BackupSqlServerDatabaseDetails left, BackupSqlServerDatabaseDetails right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
            {
                return false;
            }

            var result = left.Equals(right);

            return result;
        }

        /// <summary>
        /// Determines whether two objects of type <see cref="BackupSqlServerDatabaseDetails"/> are not equal.
        /// </summary>
        /// <param name="left">The object to the left of the equality operator.</param>
        /// <param name="right">The object to the right of the equality operator.</param>
        /// <returns>true if the two items are not equal; otherwise false.</returns>
        public static bool operator !=(BackupSqlServerDatabaseDetails left, BackupSqlServerDatabaseDetails right) => !(left == right);

        /// <inheritdoc />
        public bool Equals(BackupSqlServerDatabaseDetails other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (ReferenceEquals(other, null))
            {
                return false;
            }

            var result = this.BackupTo.IsEqualTo(other.BackupTo)
                      && this.ChecksumOption.IsEqualTo(other.ChecksumOption)
                      && this.Cipher.IsEqualTo(other.Cipher)
                      && this.CompressionOption.IsEqualTo(other.CompressionOption)
                      && this.Credential.IsEqualTo(other.Credential, StringComparer.Ordinal)
                      && this.Description.IsEqualTo(other.Description, StringComparer.Ordinal)
                      && this.Device.IsEqualTo(other.Device)
                      && this.Encryptor.IsEqualTo(other.Encryptor)
                      && this.EncryptorName.IsEqualTo(other.EncryptorName, StringComparer.Ordinal)
                      && this.ErrorHandling.IsEqualTo(other.ErrorHandling)
                      && this.Name.IsEqualTo(other.Name, StringComparer.Ordinal);

            return result;
        }

        /// <inheritdoc />
        public override bool Equals(object obj) => this == (obj as BackupSqlServerDatabaseDetails);

        /// <inheritdoc />
        public override int GetHashCode() => HashCodeHelper.Initialize()
            .Hash(this.BackupTo)
            .Hash(this.ChecksumOption)
            .Hash(this.Cipher)
            .Hash(this.CompressionOption)
            .Hash(this.Credential)
            .Hash(this.Description)
            .Hash(this.Device)
            .Hash(this.Encryptor)
            .Hash(this.EncryptorName)
            .Hash(this.ErrorHandling)
            .Hash(this.Name)
            .Value;

        /// <inheritdoc />
        public object Clone() => this.DeepClone();

        /// <inheritdoc />
        public BackupSqlServerDatabaseDetails DeepClone()
        {
            var result = new BackupSqlServerDatabaseDetails(
                                 this.Name?.DeepClone(),
                                 this.Description?.DeepClone(),
                                 this.Device.DeepClone(),
                                 this.BackupTo?.DeepClone(),
                                 this.Credential?.DeepClone(),
                                 this.CompressionOption.DeepClone(),
                                 this.ChecksumOption.DeepClone(),
                                 this.ErrorHandling.DeepClone(),
                                 this.Cipher.DeepClone(),
                                 this.Encryptor.DeepClone(),
                                 this.EncryptorName?.DeepClone());

            return result;
        }

        /// <inheritdoc />
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        public override string ToString()
        {
            var result = Invariant($"Naos.SqlServer.Domain.BackupSqlServerDatabaseDetails: BackupTo = {this.BackupTo?.ToString() ?? "<null>"}, ChecksumOption = {this.ChecksumOption.ToString() ?? "<null>"}, Cipher = {this.Cipher.ToString() ?? "<null>"}, CompressionOption = {this.CompressionOption.ToString() ?? "<null>"}, Credential = {this.Credential?.ToString(CultureInfo.InvariantCulture) ?? "<null>"}, Description = {this.Description?.ToString(CultureInfo.InvariantCulture) ?? "<null>"}, Device = {this.Device.ToString() ?? "<null>"}, Encryptor = {this.Encryptor.ToString() ?? "<null>"}, EncryptorName = {this.EncryptorName?.ToString(CultureInfo.InvariantCulture) ?? "<null>"}, ErrorHandling = {this.ErrorHandling.ToString() ?? "<null>"}, Name = {this.Name?.ToString(CultureInfo.InvariantCulture) ?? "<null>"}.");

            return result;
        }
    }
}