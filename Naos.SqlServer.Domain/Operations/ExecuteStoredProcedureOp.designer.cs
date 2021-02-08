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

    using global::Naos.Protocol.Domain;

    using global::OBeautifulCode.Equality.Recipes;
    using global::OBeautifulCode.Type;
    using global::OBeautifulCode.Type.Recipes;

    using static global::System.FormattableString;

    [Serializable]
    public partial class ExecuteStoredProcedureOp : IModel<ExecuteStoredProcedureOp>
    {
        /// <summary>
        /// Determines whether two objects of type <see cref="ExecuteStoredProcedureOp"/> are equal.
        /// </summary>
        /// <param name="left">The object to the left of the equality operator.</param>
        /// <param name="right">The object to the right of the equality operator.</param>
        /// <returns>true if the two items are equal; otherwise false.</returns>
        public static bool operator ==(ExecuteStoredProcedureOp left, ExecuteStoredProcedureOp right)
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
        /// Determines whether two objects of type <see cref="ExecuteStoredProcedureOp"/> are not equal.
        /// </summary>
        /// <param name="left">The object to the left of the equality operator.</param>
        /// <param name="right">The object to the right of the equality operator.</param>
        /// <returns>true if the two items are not equal; otherwise false.</returns>
        public static bool operator !=(ExecuteStoredProcedureOp left, ExecuteStoredProcedureOp right) => !(left == right);

        /// <inheritdoc />
        public bool Equals(ExecuteStoredProcedureOp other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (ReferenceEquals(other, null))
            {
                return false;
            }

            var result = this.Name.IsEqualTo(other.Name, StringComparer.Ordinal)
                      && this.ParameterNameToDetailsMap.IsEqualTo(other.ParameterNameToDetailsMap);

            return result;
        }

        /// <inheritdoc />
        public override bool Equals(object obj) => this == (obj as ExecuteStoredProcedureOp);

        /// <inheritdoc />
        public override int GetHashCode() => HashCodeHelper.Initialize()
            .Hash(this.Name)
            .Hash(this.ParameterNameToDetailsMap)
            .Value;

        /// <inheritdoc />
        public new ExecuteStoredProcedureOp DeepClone() => (ExecuteStoredProcedureOp)this.DeepCloneInternal();

        /// <summary>
        /// Deep clones this object with a new <see cref="Name" />.
        /// </summary>
        /// <param name="name">The new <see cref="Name" />.  This object will NOT be deep cloned; it is used as-is.</param>
        /// <returns>New <see cref="ExecuteStoredProcedureOp" /> using the specified <paramref name="name" /> for <see cref="Name" /> and a deep clone of every other property.</returns>
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
        public ExecuteStoredProcedureOp DeepCloneWithName(string name)
        {
            var result = new ExecuteStoredProcedureOp(
                                 name,
                                 this.ParameterNameToDetailsMap?.ToDictionary(k => k.Key?.DeepClone(), v => v.Value?.DeepClone()));

            return result;
        }

        /// <summary>
        /// Deep clones this object with a new <see cref="ParameterNameToDetailsMap" />.
        /// </summary>
        /// <param name="parameterNameToDetailsMap">The new <see cref="ParameterNameToDetailsMap" />.  This object will NOT be deep cloned; it is used as-is.</param>
        /// <returns>New <see cref="ExecuteStoredProcedureOp" /> using the specified <paramref name="parameterNameToDetailsMap" /> for <see cref="ParameterNameToDetailsMap" /> and a deep clone of every other property.</returns>
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
        public ExecuteStoredProcedureOp DeepCloneWithParameterNameToDetailsMap(IReadOnlyDictionary<string, SqlParameterRepresentationBase> parameterNameToDetailsMap)
        {
            var result = new ExecuteStoredProcedureOp(
                                 this.Name?.DeepClone(),
                                 parameterNameToDetailsMap);

            return result;
        }

        /// <inheritdoc />
        protected override OperationBase DeepCloneInternal()
        {
            var result = new ExecuteStoredProcedureOp(
                                 this.Name?.DeepClone(),
                                 this.ParameterNameToDetailsMap?.ToDictionary(k => k.Key?.DeepClone(), v => v.Value?.DeepClone()));

            return result;
        }

        /// <inheritdoc />
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        public override string ToString()
        {
            var result = Invariant($"Naos.SqlServer.Domain.ExecuteStoredProcedureOp: Name = {this.Name?.ToString(CultureInfo.InvariantCulture) ?? "<null>"}, ParameterNameToDetailsMap = {this.ParameterNameToDetailsMap?.ToString() ?? "<null>"}.");

            return result;
        }
    }
}