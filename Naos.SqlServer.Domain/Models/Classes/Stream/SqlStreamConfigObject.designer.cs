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
    using global::OBeautifulCode.Serialization;
    using global::OBeautifulCode.Type;
    using global::OBeautifulCode.Type.Recipes;

    using static global::System.FormattableString;

    [Serializable]
    public partial class SqlStreamConfigObject : IModel<SqlStreamConfigObject>
    {
        /// <summary>
        /// Determines whether two objects of type <see cref="SqlStreamConfigObject"/> are equal.
        /// </summary>
        /// <param name="left">The object to the left of the equality operator.</param>
        /// <param name="right">The object to the right of the equality operator.</param>
        /// <returns>true if the two items are equal; otherwise false.</returns>
        public static bool operator ==(SqlStreamConfigObject left, SqlStreamConfigObject right)
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
        /// Determines whether two objects of type <see cref="SqlStreamConfigObject"/> are not equal.
        /// </summary>
        /// <param name="left">The object to the left of the equality operator.</param>
        /// <param name="right">The object to the right of the equality operator.</param>
        /// <returns>true if the two items are not equal; otherwise false.</returns>
        public static bool operator !=(SqlStreamConfigObject left, SqlStreamConfigObject right) => !(left == right);

        /// <inheritdoc />
        public bool Equals(SqlStreamConfigObject other)
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
                      && this.SerializerFactoryTypeRepresentation.IsEqualTo(other.SerializerFactoryTypeRepresentation)
                      && this.DefaultConnectionTimeout.IsEqualTo(other.DefaultConnectionTimeout)
                      && this.DefaultCommandTimeout.IsEqualTo(other.DefaultCommandTimeout)
                      && this.DefaultSerializerRepresentation.IsEqualTo(other.DefaultSerializerRepresentation)
                      && this.DefaultSerializationFormat.IsEqualTo(other.DefaultSerializationFormat)
                      && this.AllLocators.IsEqualTo(other.AllLocators);

            return result;
        }

        /// <inheritdoc />
        public override bool Equals(object obj) => this == (obj as SqlStreamConfigObject);

        /// <inheritdoc />
        public override int GetHashCode() => HashCodeHelper.Initialize()
            .Hash(this.Name)
            .Hash(this.SerializerFactoryTypeRepresentation)
            .Hash(this.DefaultConnectionTimeout)
            .Hash(this.DefaultCommandTimeout)
            .Hash(this.DefaultSerializerRepresentation)
            .Hash(this.DefaultSerializationFormat)
            .Hash(this.AllLocators)
            .Value;

        /// <inheritdoc />
        public object Clone() => this.DeepClone();

        /// <inheritdoc />
        public SqlStreamConfigObject DeepClone()
        {
            var result = new SqlStreamConfigObject(
                                 this.Name?.DeepClone(),
                                 this.SerializerFactoryTypeRepresentation?.DeepClone(),
                                 this.DefaultConnectionTimeout.DeepClone(),
                                 this.DefaultCommandTimeout.DeepClone(),
                                 this.DefaultSerializerRepresentation?.DeepClone(),
                                 this.DefaultSerializationFormat.DeepClone(),
                                 this.AllLocators?.DeepClone());

            return result;
        }

        /// <summary>
        /// Deep clones this object with a new <see cref="Name" />.
        /// </summary>
        /// <param name="name">The new <see cref="Name" />.  This object will NOT be deep cloned; it is used as-is.</param>
        /// <returns>New <see cref="SqlStreamConfigObject" /> using the specified <paramref name="name" /> for <see cref="Name" /> and a deep clone of every other property.</returns>
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
        public SqlStreamConfigObject DeepCloneWithName(string name)
        {
            var result = new SqlStreamConfigObject(
                                 name,
                                 this.SerializerFactoryTypeRepresentation?.DeepClone(),
                                 this.DefaultConnectionTimeout.DeepClone(),
                                 this.DefaultCommandTimeout.DeepClone(),
                                 this.DefaultSerializerRepresentation?.DeepClone(),
                                 this.DefaultSerializationFormat.DeepClone(),
                                 this.AllLocators?.DeepClone());

            return result;
        }

        /// <summary>
        /// Deep clones this object with a new <see cref="SerializerFactoryTypeRepresentation" />.
        /// </summary>
        /// <param name="serializerFactoryTypeRepresentation">The new <see cref="SerializerFactoryTypeRepresentation" />.  This object will NOT be deep cloned; it is used as-is.</param>
        /// <returns>New <see cref="SqlStreamConfigObject" /> using the specified <paramref name="serializerFactoryTypeRepresentation" /> for <see cref="SerializerFactoryTypeRepresentation" /> and a deep clone of every other property.</returns>
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
        public SqlStreamConfigObject DeepCloneWithSerializerFactoryTypeRepresentation(TypeRepresentation serializerFactoryTypeRepresentation)
        {
            var result = new SqlStreamConfigObject(
                                 this.Name?.DeepClone(),
                                 serializerFactoryTypeRepresentation,
                                 this.DefaultConnectionTimeout.DeepClone(),
                                 this.DefaultCommandTimeout.DeepClone(),
                                 this.DefaultSerializerRepresentation?.DeepClone(),
                                 this.DefaultSerializationFormat.DeepClone(),
                                 this.AllLocators?.DeepClone());

            return result;
        }

        /// <summary>
        /// Deep clones this object with a new <see cref="DefaultConnectionTimeout" />.
        /// </summary>
        /// <param name="defaultConnectionTimeout">The new <see cref="DefaultConnectionTimeout" />.  This object will NOT be deep cloned; it is used as-is.</param>
        /// <returns>New <see cref="SqlStreamConfigObject" /> using the specified <paramref name="defaultConnectionTimeout" /> for <see cref="DefaultConnectionTimeout" /> and a deep clone of every other property.</returns>
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
        public SqlStreamConfigObject DeepCloneWithDefaultConnectionTimeout(TimeSpan defaultConnectionTimeout)
        {
            var result = new SqlStreamConfigObject(
                                 this.Name?.DeepClone(),
                                 this.SerializerFactoryTypeRepresentation?.DeepClone(),
                                 defaultConnectionTimeout,
                                 this.DefaultCommandTimeout.DeepClone(),
                                 this.DefaultSerializerRepresentation?.DeepClone(),
                                 this.DefaultSerializationFormat.DeepClone(),
                                 this.AllLocators?.DeepClone());

            return result;
        }

        /// <summary>
        /// Deep clones this object with a new <see cref="DefaultCommandTimeout" />.
        /// </summary>
        /// <param name="defaultCommandTimeout">The new <see cref="DefaultCommandTimeout" />.  This object will NOT be deep cloned; it is used as-is.</param>
        /// <returns>New <see cref="SqlStreamConfigObject" /> using the specified <paramref name="defaultCommandTimeout" /> for <see cref="DefaultCommandTimeout" /> and a deep clone of every other property.</returns>
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
        public SqlStreamConfigObject DeepCloneWithDefaultCommandTimeout(TimeSpan defaultCommandTimeout)
        {
            var result = new SqlStreamConfigObject(
                                 this.Name?.DeepClone(),
                                 this.SerializerFactoryTypeRepresentation?.DeepClone(),
                                 this.DefaultConnectionTimeout.DeepClone(),
                                 defaultCommandTimeout,
                                 this.DefaultSerializerRepresentation?.DeepClone(),
                                 this.DefaultSerializationFormat.DeepClone(),
                                 this.AllLocators?.DeepClone());

            return result;
        }

        /// <summary>
        /// Deep clones this object with a new <see cref="DefaultSerializerRepresentation" />.
        /// </summary>
        /// <param name="defaultSerializerRepresentation">The new <see cref="DefaultSerializerRepresentation" />.  This object will NOT be deep cloned; it is used as-is.</param>
        /// <returns>New <see cref="SqlStreamConfigObject" /> using the specified <paramref name="defaultSerializerRepresentation" /> for <see cref="DefaultSerializerRepresentation" /> and a deep clone of every other property.</returns>
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
        public SqlStreamConfigObject DeepCloneWithDefaultSerializerRepresentation(SerializerRepresentation defaultSerializerRepresentation)
        {
            var result = new SqlStreamConfigObject(
                                 this.Name?.DeepClone(),
                                 this.SerializerFactoryTypeRepresentation?.DeepClone(),
                                 this.DefaultConnectionTimeout.DeepClone(),
                                 this.DefaultCommandTimeout.DeepClone(),
                                 defaultSerializerRepresentation,
                                 this.DefaultSerializationFormat.DeepClone(),
                                 this.AllLocators?.DeepClone());

            return result;
        }

        /// <summary>
        /// Deep clones this object with a new <see cref="DefaultSerializationFormat" />.
        /// </summary>
        /// <param name="defaultSerializationFormat">The new <see cref="DefaultSerializationFormat" />.  This object will NOT be deep cloned; it is used as-is.</param>
        /// <returns>New <see cref="SqlStreamConfigObject" /> using the specified <paramref name="defaultSerializationFormat" /> for <see cref="DefaultSerializationFormat" /> and a deep clone of every other property.</returns>
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
        public SqlStreamConfigObject DeepCloneWithDefaultSerializationFormat(SerializationFormat defaultSerializationFormat)
        {
            var result = new SqlStreamConfigObject(
                                 this.Name?.DeepClone(),
                                 this.SerializerFactoryTypeRepresentation?.DeepClone(),
                                 this.DefaultConnectionTimeout.DeepClone(),
                                 this.DefaultCommandTimeout.DeepClone(),
                                 this.DefaultSerializerRepresentation?.DeepClone(),
                                 defaultSerializationFormat,
                                 this.AllLocators?.DeepClone());

            return result;
        }

        /// <summary>
        /// Deep clones this object with a new <see cref="AllLocators" />.
        /// </summary>
        /// <param name="allLocators">The new <see cref="AllLocators" />.  This object will NOT be deep cloned; it is used as-is.</param>
        /// <returns>New <see cref="SqlStreamConfigObject" /> using the specified <paramref name="allLocators" /> for <see cref="AllLocators" /> and a deep clone of every other property.</returns>
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
        public SqlStreamConfigObject DeepCloneWithAllLocators(IReadOnlyCollection<ISqlServerLocator> allLocators)
        {
            var result = new SqlStreamConfigObject(
                                 this.Name?.DeepClone(),
                                 this.SerializerFactoryTypeRepresentation?.DeepClone(),
                                 this.DefaultConnectionTimeout.DeepClone(),
                                 this.DefaultCommandTimeout.DeepClone(),
                                 this.DefaultSerializerRepresentation?.DeepClone(),
                                 this.DefaultSerializationFormat.DeepClone(),
                                 allLocators);

            return result;
        }

        /// <inheritdoc />
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        public override string ToString()
        {
            var result = Invariant($"Naos.SqlServer.Domain.SqlStreamConfigObject: Name = {this.Name?.ToString(CultureInfo.InvariantCulture) ?? "<null>"}, SerializerFactoryTypeRepresentation = {this.SerializerFactoryTypeRepresentation?.ToString() ?? "<null>"}, DefaultConnectionTimeout = {this.DefaultConnectionTimeout.ToString() ?? "<null>"}, DefaultCommandTimeout = {this.DefaultCommandTimeout.ToString() ?? "<null>"}, DefaultSerializerRepresentation = {this.DefaultSerializerRepresentation?.ToString() ?? "<null>"}, DefaultSerializationFormat = {this.DefaultSerializationFormat.ToString() ?? "<null>"}, AllLocators = {this.AllLocators?.ToString() ?? "<null>"}.");

            return result;
        }
    }
}