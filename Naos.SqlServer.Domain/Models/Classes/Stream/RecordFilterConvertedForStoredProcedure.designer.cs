﻿// --------------------------------------------------------------------------------------------------------------------
// <auto-generated>
//   Generated using OBeautifulCode.CodeGen.ModelObject (1.0.181.0)
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

    using global::OBeautifulCode.Cloning.Recipes;
    using global::OBeautifulCode.Equality.Recipes;
    using global::OBeautifulCode.Type;
    using global::OBeautifulCode.Type.Recipes;

    using static global::System.FormattableString;

    [Serializable]
    public partial class RecordFilterConvertedForStoredProcedure : IModel<RecordFilterConvertedForStoredProcedure>
    {
        /// <summary>
        /// Determines whether two objects of type <see cref="RecordFilterConvertedForStoredProcedure"/> are equal.
        /// </summary>
        /// <param name="left">The object to the left of the equality operator.</param>
        /// <param name="right">The object to the right of the equality operator.</param>
        /// <returns>true if the two items are equal; otherwise false.</returns>
        public static bool operator ==(RecordFilterConvertedForStoredProcedure left, RecordFilterConvertedForStoredProcedure right)
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
        /// Determines whether two objects of type <see cref="RecordFilterConvertedForStoredProcedure"/> are not equal.
        /// </summary>
        /// <param name="left">The object to the left of the equality operator.</param>
        /// <param name="right">The object to the right of the equality operator.</param>
        /// <returns>true if the two items are not equal; otherwise false.</returns>
        public static bool operator !=(RecordFilterConvertedForStoredProcedure left, RecordFilterConvertedForStoredProcedure right) => !(left == right);

        /// <inheritdoc />
        public bool Equals(RecordFilterConvertedForStoredProcedure other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (ReferenceEquals(other, null))
            {
                return false;
            }

            var result = this.InternalRecordIdsCsv.IsEqualTo(other.InternalRecordIdsCsv, StringComparer.Ordinal)
                      && this.IdentifierTypeIdsCsv.IsEqualTo(other.IdentifierTypeIdsCsv, StringComparer.Ordinal)
                      && this.ObjectTypeIdsCsv.IsEqualTo(other.ObjectTypeIdsCsv, StringComparer.Ordinal)
                      && this.StringIdsToMatchXml.IsEqualTo(other.StringIdsToMatchXml, StringComparer.Ordinal)
                      && this.TagIdsCsv.IsEqualTo(other.TagIdsCsv, StringComparer.Ordinal)
                      && this.TagMatchStrategy.IsEqualTo(other.TagMatchStrategy)
                      && this.VersionMatchStrategy.IsEqualTo(other.VersionMatchStrategy)
                      && this.DeprecatedIdEventTypeIdsCsv.IsEqualTo(other.DeprecatedIdEventTypeIdsCsv, StringComparer.Ordinal);

            return result;
        }

        /// <inheritdoc />
        public override bool Equals(object obj) => this == (obj as RecordFilterConvertedForStoredProcedure);

        /// <inheritdoc />
        public override int GetHashCode() => HashCodeHelper.Initialize()
            .Hash(this.InternalRecordIdsCsv)
            .Hash(this.IdentifierTypeIdsCsv)
            .Hash(this.ObjectTypeIdsCsv)
            .Hash(this.StringIdsToMatchXml)
            .Hash(this.TagIdsCsv)
            .Hash(this.TagMatchStrategy)
            .Hash(this.VersionMatchStrategy)
            .Hash(this.DeprecatedIdEventTypeIdsCsv)
            .Value;

        /// <inheritdoc />
        public object Clone() => this.DeepClone();

        /// <inheritdoc />
        public RecordFilterConvertedForStoredProcedure DeepClone()
        {
            var result = new RecordFilterConvertedForStoredProcedure(
                                 this.InternalRecordIdsCsv?.DeepClone(),
                                 this.IdentifierTypeIdsCsv?.DeepClone(),
                                 this.ObjectTypeIdsCsv?.DeepClone(),
                                 this.StringIdsToMatchXml?.DeepClone(),
                                 this.TagIdsCsv?.DeepClone(),
                                 this.TagMatchStrategy.DeepClone(),
                                 this.VersionMatchStrategy.DeepClone(),
                                 this.DeprecatedIdEventTypeIdsCsv?.DeepClone());

            return result;
        }

        /// <summary>
        /// Deep clones this object with a new <see cref="InternalRecordIdsCsv" />.
        /// </summary>
        /// <param name="internalRecordIdsCsv">The new <see cref="InternalRecordIdsCsv" />.  This object will NOT be deep cloned; it is used as-is.</param>
        /// <returns>New <see cref="RecordFilterConvertedForStoredProcedure" /> using the specified <paramref name="internalRecordIdsCsv" /> for <see cref="InternalRecordIdsCsv" /> and a deep clone of every other property.</returns>
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
        public RecordFilterConvertedForStoredProcedure DeepCloneWithInternalRecordIdsCsv(string internalRecordIdsCsv)
        {
            var result = new RecordFilterConvertedForStoredProcedure(
                                 internalRecordIdsCsv,
                                 this.IdentifierTypeIdsCsv?.DeepClone(),
                                 this.ObjectTypeIdsCsv?.DeepClone(),
                                 this.StringIdsToMatchXml?.DeepClone(),
                                 this.TagIdsCsv?.DeepClone(),
                                 this.TagMatchStrategy.DeepClone(),
                                 this.VersionMatchStrategy.DeepClone(),
                                 this.DeprecatedIdEventTypeIdsCsv?.DeepClone());

            return result;
        }

        /// <summary>
        /// Deep clones this object with a new <see cref="IdentifierTypeIdsCsv" />.
        /// </summary>
        /// <param name="identifierTypeIdsCsv">The new <see cref="IdentifierTypeIdsCsv" />.  This object will NOT be deep cloned; it is used as-is.</param>
        /// <returns>New <see cref="RecordFilterConvertedForStoredProcedure" /> using the specified <paramref name="identifierTypeIdsCsv" /> for <see cref="IdentifierTypeIdsCsv" /> and a deep clone of every other property.</returns>
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
        public RecordFilterConvertedForStoredProcedure DeepCloneWithIdentifierTypeIdsCsv(string identifierTypeIdsCsv)
        {
            var result = new RecordFilterConvertedForStoredProcedure(
                                 this.InternalRecordIdsCsv?.DeepClone(),
                                 identifierTypeIdsCsv,
                                 this.ObjectTypeIdsCsv?.DeepClone(),
                                 this.StringIdsToMatchXml?.DeepClone(),
                                 this.TagIdsCsv?.DeepClone(),
                                 this.TagMatchStrategy.DeepClone(),
                                 this.VersionMatchStrategy.DeepClone(),
                                 this.DeprecatedIdEventTypeIdsCsv?.DeepClone());

            return result;
        }

        /// <summary>
        /// Deep clones this object with a new <see cref="ObjectTypeIdsCsv" />.
        /// </summary>
        /// <param name="objectTypeIdsCsv">The new <see cref="ObjectTypeIdsCsv" />.  This object will NOT be deep cloned; it is used as-is.</param>
        /// <returns>New <see cref="RecordFilterConvertedForStoredProcedure" /> using the specified <paramref name="objectTypeIdsCsv" /> for <see cref="ObjectTypeIdsCsv" /> and a deep clone of every other property.</returns>
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
        public RecordFilterConvertedForStoredProcedure DeepCloneWithObjectTypeIdsCsv(string objectTypeIdsCsv)
        {
            var result = new RecordFilterConvertedForStoredProcedure(
                                 this.InternalRecordIdsCsv?.DeepClone(),
                                 this.IdentifierTypeIdsCsv?.DeepClone(),
                                 objectTypeIdsCsv,
                                 this.StringIdsToMatchXml?.DeepClone(),
                                 this.TagIdsCsv?.DeepClone(),
                                 this.TagMatchStrategy.DeepClone(),
                                 this.VersionMatchStrategy.DeepClone(),
                                 this.DeprecatedIdEventTypeIdsCsv?.DeepClone());

            return result;
        }

        /// <summary>
        /// Deep clones this object with a new <see cref="StringIdsToMatchXml" />.
        /// </summary>
        /// <param name="stringIdsToMatchXml">The new <see cref="StringIdsToMatchXml" />.  This object will NOT be deep cloned; it is used as-is.</param>
        /// <returns>New <see cref="RecordFilterConvertedForStoredProcedure" /> using the specified <paramref name="stringIdsToMatchXml" /> for <see cref="StringIdsToMatchXml" /> and a deep clone of every other property.</returns>
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
        public RecordFilterConvertedForStoredProcedure DeepCloneWithStringIdsToMatchXml(string stringIdsToMatchXml)
        {
            var result = new RecordFilterConvertedForStoredProcedure(
                                 this.InternalRecordIdsCsv?.DeepClone(),
                                 this.IdentifierTypeIdsCsv?.DeepClone(),
                                 this.ObjectTypeIdsCsv?.DeepClone(),
                                 stringIdsToMatchXml,
                                 this.TagIdsCsv?.DeepClone(),
                                 this.TagMatchStrategy.DeepClone(),
                                 this.VersionMatchStrategy.DeepClone(),
                                 this.DeprecatedIdEventTypeIdsCsv?.DeepClone());

            return result;
        }

        /// <summary>
        /// Deep clones this object with a new <see cref="TagIdsCsv" />.
        /// </summary>
        /// <param name="tagIdsCsv">The new <see cref="TagIdsCsv" />.  This object will NOT be deep cloned; it is used as-is.</param>
        /// <returns>New <see cref="RecordFilterConvertedForStoredProcedure" /> using the specified <paramref name="tagIdsCsv" /> for <see cref="TagIdsCsv" /> and a deep clone of every other property.</returns>
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
        public RecordFilterConvertedForStoredProcedure DeepCloneWithTagIdsCsv(string tagIdsCsv)
        {
            var result = new RecordFilterConvertedForStoredProcedure(
                                 this.InternalRecordIdsCsv?.DeepClone(),
                                 this.IdentifierTypeIdsCsv?.DeepClone(),
                                 this.ObjectTypeIdsCsv?.DeepClone(),
                                 this.StringIdsToMatchXml?.DeepClone(),
                                 tagIdsCsv,
                                 this.TagMatchStrategy.DeepClone(),
                                 this.VersionMatchStrategy.DeepClone(),
                                 this.DeprecatedIdEventTypeIdsCsv?.DeepClone());

            return result;
        }

        /// <summary>
        /// Deep clones this object with a new <see cref="TagMatchStrategy" />.
        /// </summary>
        /// <param name="tagMatchStrategy">The new <see cref="TagMatchStrategy" />.  This object will NOT be deep cloned; it is used as-is.</param>
        /// <returns>New <see cref="RecordFilterConvertedForStoredProcedure" /> using the specified <paramref name="tagMatchStrategy" /> for <see cref="TagMatchStrategy" /> and a deep clone of every other property.</returns>
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
        public RecordFilterConvertedForStoredProcedure DeepCloneWithTagMatchStrategy(TagMatchStrategy tagMatchStrategy)
        {
            var result = new RecordFilterConvertedForStoredProcedure(
                                 this.InternalRecordIdsCsv?.DeepClone(),
                                 this.IdentifierTypeIdsCsv?.DeepClone(),
                                 this.ObjectTypeIdsCsv?.DeepClone(),
                                 this.StringIdsToMatchXml?.DeepClone(),
                                 this.TagIdsCsv?.DeepClone(),
                                 tagMatchStrategy,
                                 this.VersionMatchStrategy.DeepClone(),
                                 this.DeprecatedIdEventTypeIdsCsv?.DeepClone());

            return result;
        }

        /// <summary>
        /// Deep clones this object with a new <see cref="VersionMatchStrategy" />.
        /// </summary>
        /// <param name="versionMatchStrategy">The new <see cref="VersionMatchStrategy" />.  This object will NOT be deep cloned; it is used as-is.</param>
        /// <returns>New <see cref="RecordFilterConvertedForStoredProcedure" /> using the specified <paramref name="versionMatchStrategy" /> for <see cref="VersionMatchStrategy" /> and a deep clone of every other property.</returns>
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
        public RecordFilterConvertedForStoredProcedure DeepCloneWithVersionMatchStrategy(VersionMatchStrategy versionMatchStrategy)
        {
            var result = new RecordFilterConvertedForStoredProcedure(
                                 this.InternalRecordIdsCsv?.DeepClone(),
                                 this.IdentifierTypeIdsCsv?.DeepClone(),
                                 this.ObjectTypeIdsCsv?.DeepClone(),
                                 this.StringIdsToMatchXml?.DeepClone(),
                                 this.TagIdsCsv?.DeepClone(),
                                 this.TagMatchStrategy.DeepClone(),
                                 versionMatchStrategy,
                                 this.DeprecatedIdEventTypeIdsCsv?.DeepClone());

            return result;
        }

        /// <summary>
        /// Deep clones this object with a new <see cref="DeprecatedIdEventTypeIdsCsv" />.
        /// </summary>
        /// <param name="deprecatedIdEventTypeIdsCsv">The new <see cref="DeprecatedIdEventTypeIdsCsv" />.  This object will NOT be deep cloned; it is used as-is.</param>
        /// <returns>New <see cref="RecordFilterConvertedForStoredProcedure" /> using the specified <paramref name="deprecatedIdEventTypeIdsCsv" /> for <see cref="DeprecatedIdEventTypeIdsCsv" /> and a deep clone of every other property.</returns>
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
        public RecordFilterConvertedForStoredProcedure DeepCloneWithDeprecatedIdEventTypeIdsCsv(string deprecatedIdEventTypeIdsCsv)
        {
            var result = new RecordFilterConvertedForStoredProcedure(
                                 this.InternalRecordIdsCsv?.DeepClone(),
                                 this.IdentifierTypeIdsCsv?.DeepClone(),
                                 this.ObjectTypeIdsCsv?.DeepClone(),
                                 this.StringIdsToMatchXml?.DeepClone(),
                                 this.TagIdsCsv?.DeepClone(),
                                 this.TagMatchStrategy.DeepClone(),
                                 this.VersionMatchStrategy.DeepClone(),
                                 deprecatedIdEventTypeIdsCsv);

            return result;
        }

        /// <inheritdoc />
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        public override string ToString()
        {
            var result = Invariant($"Naos.SqlServer.Domain.RecordFilterConvertedForStoredProcedure: InternalRecordIdsCsv = {this.InternalRecordIdsCsv?.ToString(CultureInfo.InvariantCulture) ?? "<null>"}, IdentifierTypeIdsCsv = {this.IdentifierTypeIdsCsv?.ToString(CultureInfo.InvariantCulture) ?? "<null>"}, ObjectTypeIdsCsv = {this.ObjectTypeIdsCsv?.ToString(CultureInfo.InvariantCulture) ?? "<null>"}, StringIdsToMatchXml = {this.StringIdsToMatchXml?.ToString(CultureInfo.InvariantCulture) ?? "<null>"}, TagIdsCsv = {this.TagIdsCsv?.ToString(CultureInfo.InvariantCulture) ?? "<null>"}, TagMatchStrategy = {this.TagMatchStrategy.ToString() ?? "<null>"}, VersionMatchStrategy = {this.VersionMatchStrategy.ToString() ?? "<null>"}, DeprecatedIdEventTypeIdsCsv = {this.DeprecatedIdEventTypeIdsCsv?.ToString(CultureInfo.InvariantCulture) ?? "<null>"}.");

            return result;
        }
    }
}