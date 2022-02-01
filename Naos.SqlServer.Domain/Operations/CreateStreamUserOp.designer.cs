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
    using global::OBeautifulCode.Representation.System;
    using global::OBeautifulCode.Type;
    using global::OBeautifulCode.Type.Recipes;

    using static global::System.FormattableString;

    [Serializable]
    public partial class CreateStreamUserOp : IModel<CreateStreamUserOp>
    {
        /// <summary>
        /// Determines whether two objects of type <see cref="CreateStreamUserOp"/> are equal.
        /// </summary>
        /// <param name="left">The object to the left of the equality operator.</param>
        /// <param name="right">The object to the right of the equality operator.</param>
        /// <returns>true if the two items are equal; otherwise false.</returns>
        public static bool operator ==(CreateStreamUserOp left, CreateStreamUserOp right)
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
        /// Determines whether two objects of type <see cref="CreateStreamUserOp"/> are not equal.
        /// </summary>
        /// <param name="left">The object to the left of the equality operator.</param>
        /// <param name="right">The object to the right of the equality operator.</param>
        /// <returns>true if the two items are not equal; otherwise false.</returns>
        public static bool operator !=(CreateStreamUserOp left, CreateStreamUserOp right) => !(left == right);

        /// <inheritdoc />
        public bool Equals(CreateStreamUserOp other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (ReferenceEquals(other, null))
            {
                return false;
            }

            var result = this.UserName.IsEqualTo(other.UserName, StringComparer.Ordinal)
                      && this.ClearTextPassword.IsEqualTo(other.ClearTextPassword, StringComparer.Ordinal)
                      && this.ProtocolsToGrantAccessFor.IsEqualTo(other.ProtocolsToGrantAccessFor);

            return result;
        }

        /// <inheritdoc />
        public override bool Equals(object obj) => this == (obj as CreateStreamUserOp);

        /// <inheritdoc />
        public override int GetHashCode() => HashCodeHelper.Initialize()
            .Hash(this.UserName)
            .Hash(this.ClearTextPassword)
            .Hash(this.ProtocolsToGrantAccessFor)
            .Value;

        /// <inheritdoc />
        public new CreateStreamUserOp DeepClone() => (CreateStreamUserOp)this.DeepCloneInternal();

        /// <summary>
        /// Deep clones this object with a new <see cref="UserName" />.
        /// </summary>
        /// <param name="userName">The new <see cref="UserName" />.  This object will NOT be deep cloned; it is used as-is.</param>
        /// <returns>New <see cref="CreateStreamUserOp" /> using the specified <paramref name="userName" /> for <see cref="UserName" /> and a deep clone of every other property.</returns>
        [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings")]
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
        public CreateStreamUserOp DeepCloneWithUserName(string userName)
        {
            var result = new CreateStreamUserOp(
                                 userName,
                                 this.ClearTextPassword?.DeepClone(),
                                 this.ProtocolsToGrantAccessFor?.DeepClone());

            return result;
        }

        /// <summary>
        /// Deep clones this object with a new <see cref="ClearTextPassword" />.
        /// </summary>
        /// <param name="clearTextPassword">The new <see cref="ClearTextPassword" />.  This object will NOT be deep cloned; it is used as-is.</param>
        /// <returns>New <see cref="CreateStreamUserOp" /> using the specified <paramref name="clearTextPassword" /> for <see cref="ClearTextPassword" /> and a deep clone of every other property.</returns>
        [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings")]
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
        public CreateStreamUserOp DeepCloneWithClearTextPassword(string clearTextPassword)
        {
            var result = new CreateStreamUserOp(
                                 this.UserName?.DeepClone(),
                                 clearTextPassword,
                                 this.ProtocolsToGrantAccessFor?.DeepClone());

            return result;
        }

        /// <summary>
        /// Deep clones this object with a new <see cref="ProtocolsToGrantAccessFor" />.
        /// </summary>
        /// <param name="protocolsToGrantAccessFor">The new <see cref="ProtocolsToGrantAccessFor" />.  This object will NOT be deep cloned; it is used as-is.</param>
        /// <returns>New <see cref="CreateStreamUserOp" /> using the specified <paramref name="protocolsToGrantAccessFor" /> for <see cref="ProtocolsToGrantAccessFor" /> and a deep clone of every other property.</returns>
        [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings")]
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
        public CreateStreamUserOp DeepCloneWithProtocolsToGrantAccessFor(IReadOnlyCollection<TypeRepresentation> protocolsToGrantAccessFor)
        {
            var result = new CreateStreamUserOp(
                                 this.UserName?.DeepClone(),
                                 this.ClearTextPassword?.DeepClone(),
                                 protocolsToGrantAccessFor);

            return result;
        }

        /// <inheritdoc />
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        protected override OperationBase DeepCloneInternal()
        {
            var result = new CreateStreamUserOp(
                                 this.UserName?.DeepClone(),
                                 this.ClearTextPassword?.DeepClone(),
                                 this.ProtocolsToGrantAccessFor?.DeepClone());

            return result;
        }

        /// <inheritdoc />
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        public override string ToString()
        {
            var result = Invariant($"Naos.SqlServer.Domain.CreateStreamUserOp: UserName = {this.UserName?.ToString(CultureInfo.InvariantCulture) ?? "<null>"}, ClearTextPassword = {this.ClearTextPassword?.ToString(CultureInfo.InvariantCulture) ?? "<null>"}, ProtocolsToGrantAccessFor = {this.ProtocolsToGrantAccessFor?.ToString() ?? "<null>"}.");

            return result;
        }
    }
}