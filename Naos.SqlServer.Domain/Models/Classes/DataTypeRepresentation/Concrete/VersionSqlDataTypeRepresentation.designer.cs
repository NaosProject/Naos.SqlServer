﻿// --------------------------------------------------------------------------------------------------------------------
// <auto-generated>
//   Generated using OBeautifulCode.CodeGen.ModelObject (1.0.178.0)
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
    public partial class VersionSqlDataTypeRepresentation : IModel<VersionSqlDataTypeRepresentation>
    {
        /// <summary>
        /// Determines whether two objects of type <see cref="VersionSqlDataTypeRepresentation"/> are equal.
        /// </summary>
        /// <param name="left">The object to the left of the equality operator.</param>
        /// <param name="right">The object to the right of the equality operator.</param>
        /// <returns>true if the two items are equal; otherwise false.</returns>
        public static bool operator ==(VersionSqlDataTypeRepresentation left, VersionSqlDataTypeRepresentation right)
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
        /// Determines whether two objects of type <see cref="VersionSqlDataTypeRepresentation"/> are not equal.
        /// </summary>
        /// <param name="left">The object to the left of the equality operator.</param>
        /// <param name="right">The object to the right of the equality operator.</param>
        /// <returns>true if the two items are not equal; otherwise false.</returns>
        public static bool operator !=(VersionSqlDataTypeRepresentation left, VersionSqlDataTypeRepresentation right) => !(left == right);

        /// <inheritdoc />
        public bool Equals(VersionSqlDataTypeRepresentation other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (ReferenceEquals(other, null))
            {
                return false;
            }

            var result = true;

            return result;
        }

        /// <inheritdoc />
        public override bool Equals(object obj) => this == (obj as VersionSqlDataTypeRepresentation);

        /// <inheritdoc />
        public override int GetHashCode() => HashCodeHelper.Initialize()
            .Hash("VersionSqlDataTypeRepresentation")
            .Value;

        /// <inheritdoc />
        public new VersionSqlDataTypeRepresentation DeepClone() => (VersionSqlDataTypeRepresentation)this.DeepCloneInternal();

        /// <inheritdoc />
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        protected override SqlDataTypeRepresentationBase DeepCloneInternal()
        {
            var result = new VersionSqlDataTypeRepresentation();

            return result;
        }

        /// <inheritdoc />
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        public override string ToString()
        {
            var result = Invariant($"Naos.SqlServer.Domain.VersionSqlDataTypeRepresentation: <no properties>.");

            return result;
        }
    }
}