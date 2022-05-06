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
    public partial class SqlServerConnectionDefinition : IModel<SqlServerConnectionDefinition>
    {
        /// <summary>
        /// Determines whether two objects of type <see cref="SqlServerConnectionDefinition"/> are equal.
        /// </summary>
        /// <param name="left">The object to the left of the equality operator.</param>
        /// <param name="right">The object to the right of the equality operator.</param>
        /// <returns>true if the two items are equal; otherwise false.</returns>
        public static bool operator ==(SqlServerConnectionDefinition left, SqlServerConnectionDefinition right)
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
        /// Determines whether two objects of type <see cref="SqlServerConnectionDefinition"/> are not equal.
        /// </summary>
        /// <param name="left">The object to the left of the equality operator.</param>
        /// <param name="right">The object to the right of the equality operator.</param>
        /// <returns>true if the two items are not equal; otherwise false.</returns>
        public static bool operator !=(SqlServerConnectionDefinition left, SqlServerConnectionDefinition right) => !(left == right);

        /// <inheritdoc />
        public bool Equals(SqlServerConnectionDefinition other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (ReferenceEquals(other, null))
            {
                return false;
            }

            var result = this.Server.IsEqualTo(other.Server, StringComparer.Ordinal)
                      && this.InstanceName.IsEqualTo(other.InstanceName, StringComparer.Ordinal)
                      && this.DatabaseName.IsEqualTo(other.DatabaseName, StringComparer.Ordinal)
                      && this.UserName.IsEqualTo(other.UserName, StringComparer.Ordinal)
                      && this.Password.IsEqualTo(other.Password, StringComparer.Ordinal);

            return result;
        }

        /// <inheritdoc />
        public override bool Equals(object obj) => this == (obj as SqlServerConnectionDefinition);

        /// <inheritdoc />
        public override int GetHashCode() => HashCodeHelper.Initialize()
            .Hash(this.Server)
            .Hash(this.InstanceName)
            .Hash(this.DatabaseName)
            .Hash(this.UserName)
            .Hash(this.Password)
            .Value;

        /// <inheritdoc />
        public object Clone() => this.DeepClone();

        /// <inheritdoc />
        public SqlServerConnectionDefinition DeepClone()
        {
            var result = new SqlServerConnectionDefinition
                             {
                                 Server       = this.Server?.DeepClone(),
                                 InstanceName = this.InstanceName?.DeepClone(),
                                 DatabaseName = this.DatabaseName?.DeepClone(),
                                 UserName     = this.UserName?.DeepClone(),
                                 Password     = this.Password?.DeepClone(),
                             };

            return result;
        }

        /// <summary>
        /// Deep clones this object with a new <see cref="Server" />.
        /// </summary>
        /// <param name="server">The new <see cref="Server" />.  This object will NOT be deep cloned; it is used as-is.</param>
        /// <returns>New <see cref="SqlServerConnectionDefinition" /> using the specified <paramref name="server" /> for <see cref="Server" /> and a deep clone of every other property.</returns>
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
        public SqlServerConnectionDefinition DeepCloneWithServer(string server)
        {
            var result = new SqlServerConnectionDefinition
                             {
                                 Server       = server,
                                 InstanceName = this.InstanceName?.DeepClone(),
                                 DatabaseName = this.DatabaseName?.DeepClone(),
                                 UserName     = this.UserName?.DeepClone(),
                                 Password     = this.Password?.DeepClone(),
                             };

            return result;
        }

        /// <summary>
        /// Deep clones this object with a new <see cref="InstanceName" />.
        /// </summary>
        /// <param name="instanceName">The new <see cref="InstanceName" />.  This object will NOT be deep cloned; it is used as-is.</param>
        /// <returns>New <see cref="SqlServerConnectionDefinition" /> using the specified <paramref name="instanceName" /> for <see cref="InstanceName" /> and a deep clone of every other property.</returns>
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
        public SqlServerConnectionDefinition DeepCloneWithInstanceName(string instanceName)
        {
            var result = new SqlServerConnectionDefinition
                             {
                                 Server       = this.Server?.DeepClone(),
                                 InstanceName = instanceName,
                                 DatabaseName = this.DatabaseName?.DeepClone(),
                                 UserName     = this.UserName?.DeepClone(),
                                 Password     = this.Password?.DeepClone(),
                             };

            return result;
        }

        /// <summary>
        /// Deep clones this object with a new <see cref="DatabaseName" />.
        /// </summary>
        /// <param name="databaseName">The new <see cref="DatabaseName" />.  This object will NOT be deep cloned; it is used as-is.</param>
        /// <returns>New <see cref="SqlServerConnectionDefinition" /> using the specified <paramref name="databaseName" /> for <see cref="DatabaseName" /> and a deep clone of every other property.</returns>
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
        public SqlServerConnectionDefinition DeepCloneWithDatabaseName(string databaseName)
        {
            var result = new SqlServerConnectionDefinition
                             {
                                 Server       = this.Server?.DeepClone(),
                                 InstanceName = this.InstanceName?.DeepClone(),
                                 DatabaseName = databaseName,
                                 UserName     = this.UserName?.DeepClone(),
                                 Password     = this.Password?.DeepClone(),
                             };

            return result;
        }

        /// <summary>
        /// Deep clones this object with a new <see cref="UserName" />.
        /// </summary>
        /// <param name="userName">The new <see cref="UserName" />.  This object will NOT be deep cloned; it is used as-is.</param>
        /// <returns>New <see cref="SqlServerConnectionDefinition" /> using the specified <paramref name="userName" /> for <see cref="UserName" /> and a deep clone of every other property.</returns>
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
        public SqlServerConnectionDefinition DeepCloneWithUserName(string userName)
        {
            var result = new SqlServerConnectionDefinition
                             {
                                 Server       = this.Server?.DeepClone(),
                                 InstanceName = this.InstanceName?.DeepClone(),
                                 DatabaseName = this.DatabaseName?.DeepClone(),
                                 UserName     = userName,
                                 Password     = this.Password?.DeepClone(),
                             };

            return result;
        }

        /// <summary>
        /// Deep clones this object with a new <see cref="Password" />.
        /// </summary>
        /// <param name="password">The new <see cref="Password" />.  This object will NOT be deep cloned; it is used as-is.</param>
        /// <returns>New <see cref="SqlServerConnectionDefinition" /> using the specified <paramref name="password" /> for <see cref="Password" /> and a deep clone of every other property.</returns>
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
        public SqlServerConnectionDefinition DeepCloneWithPassword(string password)
        {
            var result = new SqlServerConnectionDefinition
                             {
                                 Server       = this.Server?.DeepClone(),
                                 InstanceName = this.InstanceName?.DeepClone(),
                                 DatabaseName = this.DatabaseName?.DeepClone(),
                                 UserName     = this.UserName?.DeepClone(),
                                 Password     = password,
                             };

            return result;
        }

        /// <inheritdoc />
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        public override string ToString()
        {
            var result = Invariant($"Naos.SqlServer.Domain.SqlServerConnectionDefinition: Server = {this.Server?.ToString(CultureInfo.InvariantCulture) ?? "<null>"}, InstanceName = {this.InstanceName?.ToString(CultureInfo.InvariantCulture) ?? "<null>"}, DatabaseName = {this.DatabaseName?.ToString(CultureInfo.InvariantCulture) ?? "<null>"}, UserName = {this.UserName?.ToString(CultureInfo.InvariantCulture) ?? "<null>"}, Password = {this.Password?.ToString(CultureInfo.InvariantCulture) ?? "<null>"}.");

            return result;
        }
    }
}