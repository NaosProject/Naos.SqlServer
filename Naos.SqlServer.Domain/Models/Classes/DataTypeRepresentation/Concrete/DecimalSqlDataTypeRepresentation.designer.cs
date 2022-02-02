﻿// --------------------------------------------------------------------------------------------------------------------
// <auto-generated>
//   Generated using OBeautifulCode.CodeGen.ModelObject (1.0.176.0)
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
    public partial class DecimalSqlDataTypeRepresentation : IModel<DecimalSqlDataTypeRepresentation>
    {
        /// <summary>
        /// Determines whether two objects of type <see cref="DecimalSqlDataTypeRepresentation"/> are equal.
        /// </summary>
        /// <param name="left">The object to the left of the equality operator.</param>
        /// <param name="right">The object to the right of the equality operator.</param>
        /// <returns>true if the two items are equal; otherwise false.</returns>
        public static bool operator ==(DecimalSqlDataTypeRepresentation left, DecimalSqlDataTypeRepresentation right)
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
        /// Determines whether two objects of type <see cref="DecimalSqlDataTypeRepresentation"/> are not equal.
        /// </summary>
        /// <param name="left">The object to the left of the equality operator.</param>
        /// <param name="right">The object to the right of the equality operator.</param>
        /// <returns>true if the two items are not equal; otherwise false.</returns>
        public static bool operator !=(DecimalSqlDataTypeRepresentation left, DecimalSqlDataTypeRepresentation right) => !(left == right);

        /// <inheritdoc />
        public bool Equals(DecimalSqlDataTypeRepresentation other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (ReferenceEquals(other, null))
            {
                return false;
            }

            var result = this.Precision.IsEqualTo(other.Precision)
                      && this.Scale.IsEqualTo(other.Scale);

            return result;
        }

        /// <inheritdoc />
        public override bool Equals(object obj) => this == (obj as DecimalSqlDataTypeRepresentation);

        /// <inheritdoc />
        public override int GetHashCode() => HashCodeHelper.Initialize()
            .Hash(this.Precision)
            .Hash(this.Scale)
            .Value;

        /// <inheritdoc />
        public new DecimalSqlDataTypeRepresentation DeepClone() => (DecimalSqlDataTypeRepresentation)this.DeepCloneInternal();

        /// <inheritdoc />
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        protected override SqlDataTypeRepresentationBase DeepCloneInternal()
        {
            var result = new DecimalSqlDataTypeRepresentation(
                                 this.Precision.DeepClone(),
                                 this.Scale.DeepClone());

            return result;
        }

        /// <inheritdoc />
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        public override string ToString()
        {
            var result = Invariant($"Naos.SqlServer.Domain.DecimalSqlDataTypeRepresentation: Precision = {this.Precision.ToString(CultureInfo.InvariantCulture) ?? "<null>"}, Scale = {this.Scale.ToString(CultureInfo.InvariantCulture) ?? "<null>"}.");

            return result;
        }
    }
}