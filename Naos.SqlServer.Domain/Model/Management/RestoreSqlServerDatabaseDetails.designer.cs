﻿// --------------------------------------------------------------------------------------------------------------------
// <auto-generated>
//   Generated using OBeautifulCode.CodeGen.ModelObject (1.0.145.0)
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

    using global::Naos.Database.Domain;

    using global::OBeautifulCode.Equality.Recipes;
    using global::OBeautifulCode.Type;
    using global::OBeautifulCode.Type.Recipes;

    using static global::System.FormattableString;

    [Serializable]
    public partial class RestoreSqlServerDatabaseDetails : IModel<RestoreSqlServerDatabaseDetails>
    {
        /// <summary>
        /// Determines whether two objects of type <see cref="RestoreSqlServerDatabaseDetails"/> are equal.
        /// </summary>
        /// <param name="left">The object to the left of the equality operator.</param>
        /// <param name="right">The object to the right of the equality operator.</param>
        /// <returns>true if the two items are equal; otherwise false.</returns>
        public static bool operator ==(RestoreSqlServerDatabaseDetails left, RestoreSqlServerDatabaseDetails right)
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
        /// Determines whether two objects of type <see cref="RestoreSqlServerDatabaseDetails"/> are not equal.
        /// </summary>
        /// <param name="left">The object to the left of the equality operator.</param>
        /// <param name="right">The object to the right of the equality operator.</param>
        /// <returns>true if the two items are not equal; otherwise false.</returns>
        public static bool operator !=(RestoreSqlServerDatabaseDetails left, RestoreSqlServerDatabaseDetails right) => !(left == right);

        /// <inheritdoc />
        public bool Equals(RestoreSqlServerDatabaseDetails other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (ReferenceEquals(other, null))
            {
                return false;
            }

            var result = this.ChecksumOption.IsEqualTo(other.ChecksumOption)
                      && this.Credential.IsEqualTo(other.Credential, StringComparer.Ordinal)
                      && this.DataFilePath.IsEqualTo(other.DataFilePath, StringComparer.Ordinal)
                      && this.Device.IsEqualTo(other.Device)
                      && this.ErrorHandling.IsEqualTo(other.ErrorHandling)
                      && this.LogFilePath.IsEqualTo(other.LogFilePath, StringComparer.Ordinal)
                      && this.RecoveryOption.IsEqualTo(other.RecoveryOption)
                      && this.ReplaceOption.IsEqualTo(other.ReplaceOption)
                      && this.RestoreFrom.IsEqualTo(other.RestoreFrom)
                      && this.RestrictedUserOption.IsEqualTo(other.RestrictedUserOption);

            return result;
        }

        /// <inheritdoc />
        public override bool Equals(object obj) => this == (obj as RestoreSqlServerDatabaseDetails);

        /// <inheritdoc />
        public override int GetHashCode() => HashCodeHelper.Initialize()
            .Hash(this.ChecksumOption)
            .Hash(this.Credential)
            .Hash(this.DataFilePath)
            .Hash(this.Device)
            .Hash(this.ErrorHandling)
            .Hash(this.LogFilePath)
            .Hash(this.RecoveryOption)
            .Hash(this.ReplaceOption)
            .Hash(this.RestoreFrom)
            .Hash(this.RestrictedUserOption)
            .Value;

        /// <inheritdoc />
        public object Clone() => this.DeepClone();

        /// <inheritdoc />
        public RestoreSqlServerDatabaseDetails DeepClone()
        {
            var result = new RestoreSqlServerDatabaseDetails(
                                 this.DataFilePath?.DeepClone(),
                                 this.LogFilePath?.DeepClone(),
                                 this.Device,
                                 this.RestoreFrom?.DeepClone(),
                                 this.Credential?.DeepClone(),
                                 this.ChecksumOption,
                                 this.ErrorHandling,
                                 this.RecoveryOption,
                                 this.ReplaceOption,
                                 this.RestrictedUserOption);

            return result;
        }

        /// <inheritdoc />
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        public override string ToString()
        {
            var result = Invariant($"Naos.SqlServer.Domain.RestoreSqlServerDatabaseDetails: ChecksumOption = {this.ChecksumOption.ToString() ?? "<null>"}, Credential = {this.Credential?.ToString(CultureInfo.InvariantCulture) ?? "<null>"}, DataFilePath = {this.DataFilePath?.ToString(CultureInfo.InvariantCulture) ?? "<null>"}, Device = {this.Device.ToString() ?? "<null>"}, ErrorHandling = {this.ErrorHandling.ToString() ?? "<null>"}, LogFilePath = {this.LogFilePath?.ToString(CultureInfo.InvariantCulture) ?? "<null>"}, RecoveryOption = {this.RecoveryOption.ToString() ?? "<null>"}, ReplaceOption = {this.ReplaceOption.ToString() ?? "<null>"}, RestoreFrom = {this.RestoreFrom?.ToString() ?? "<null>"}, RestrictedUserOption = {this.RestrictedUserOption.ToString() ?? "<null>"}.");

            return result;
        }
    }
}