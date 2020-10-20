﻿// --------------------------------------------------------------------------------------------------------------------
// <auto-generated>
//   Generated using OBeautifulCode.CodeGen.ModelObject (1.0.104.0)
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

    using global::OBeautifulCode.Equality.Recipes;
    using global::OBeautifulCode.Type;
    using global::OBeautifulCode.Type.Recipes;

    using static global::System.FormattableString;

    [Serializable]
    public partial class BinarySqlDataTypeRepresentation : IModel<BinarySqlDataTypeRepresentation>
    {
        /// <summary>
        /// Determines whether two objects of type <see cref="BinarySqlDataTypeRepresentation"/> are equal.
        /// </summary>
        /// <param name="left">The object to the left of the equality operator.</param>
        /// <param name="right">The object to the right of the equality operator.</param>
        /// <returns>true if the two items are equal; otherwise false.</returns>
        public static bool operator ==(BinarySqlDataTypeRepresentation left, BinarySqlDataTypeRepresentation right)
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
        /// Determines whether two objects of type <see cref="BinarySqlDataTypeRepresentation"/> are not equal.
        /// </summary>
        /// <param name="left">The object to the left of the equality operator.</param>
        /// <param name="right">The object to the right of the equality operator.</param>
        /// <returns>true if the two items are not equal; otherwise false.</returns>
        public static bool operator !=(BinarySqlDataTypeRepresentation left, BinarySqlDataTypeRepresentation right) => !(left == right);

        /// <inheritdoc />
        public bool Equals(BinarySqlDataTypeRepresentation other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (ReferenceEquals(other, null))
            {
                return false;
            }

            var result = this.SupportedLength.IsEqualTo(other.SupportedLength);

            return result;
        }

        /// <inheritdoc />
        public override bool Equals(object obj) => this == (obj as BinarySqlDataTypeRepresentation);

        /// <inheritdoc />
        public override int GetHashCode() => HashCodeHelper.Initialize()
            .Hash(this.SupportedLength)
            .Value;

        /// <inheritdoc />
        public new BinarySqlDataTypeRepresentation DeepClone() => (BinarySqlDataTypeRepresentation)this.DeepCloneInternal();

        /// <summary>
        /// Deep clones this object with a new <see cref="SupportedLength" />.
        /// </summary>
        /// <param name="supportedLength">The new <see cref="SupportedLength" />.  This object will NOT be deep cloned; it is used as-is.</param>
        /// <returns>New <see cref="BinarySqlDataTypeRepresentation" /> using the specified <paramref name="supportedLength" /> for <see cref="SupportedLength" /> and a deep clone of every other property.</returns>
        [SuppressMessage("Microsoft.Design", "CA1002: DoNotExposeGenericLists")]
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly")]
        [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
        [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")]
        [SuppressMessage("Microsoft.Naming", "CA1715:IdentifiersShouldHaveCorrectPrefix")]
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords")]
        [SuppressMessage("Microsoft.Naming", "CA1719:ParameterNamesShouldNotMatchMemberNames")]
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames")]
        [SuppressMessage("Microsoft.Naming", "CA1722:IdentifiersShouldNotHaveIncorrectPrefix")]
        [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration")]
        [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms")]
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly")]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public BinarySqlDataTypeRepresentation DeepCloneWithSupportedLength(int supportedLength)
        {
            var result = new BinarySqlDataTypeRepresentation(
                                 supportedLength);

            return result;
        }

        /// <inheritdoc />
        protected override SqlDataTypeRepresentationBase DeepCloneInternal()
        {
            var result = new BinarySqlDataTypeRepresentation(
                                 this.SupportedLength);

            return result;
        }

        /// <inheritdoc />
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        public override string ToString()
        {
            var result = Invariant($"Naos.SqlServer.Domain.BinarySqlDataTypeRepresentation: SupportedLength = {this.SupportedLength.ToString(CultureInfo.InvariantCulture) ?? "<null>"}.");

            return result;
        }
    }
}