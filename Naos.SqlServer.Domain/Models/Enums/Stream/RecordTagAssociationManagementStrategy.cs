// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RecordTagAssociationManagementStrategy.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System.Diagnostics.CodeAnalysis;
    using Naos.CodeAnalysis.Recipes;

    /// <summary>
    /// Strategy of managing record and tag association.
    /// </summary>
    public enum RecordTagAssociationManagementStrategy
    {
        /// <summary>
        /// The tags can be or not be associated by an external process, nothing is done during insert.
        /// </summary>
        ExternallyManaged,

        /// <summary>
        /// The tags are associated with the record during execution of the <see cref="StreamSchema.Sprocs.PutRecord"/> in the same transaction as the insert.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sproc", Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "PutIn", Justification = NaosSuppressBecause.CA_ALL_SeeOtherSuppressionMessages)]
        AssociatedDuringPutInSprocInTransaction,

        /// <summary>
        /// The tags are associated with the record during execution of the <see cref="StreamSchema.Sprocs.PutRecord"/> NOT in the same transaction as the insert.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sproc", Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "PutIn", Justification = NaosSuppressBecause.CA_ALL_SeeOtherSuppressionMessages)]
        AssociatedDuringPutInSprocOutOfTransaction,
    }
}