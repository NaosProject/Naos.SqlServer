// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateStreamStoredProceduresOp.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using OBeautifulCode.Type;

    /// <summary>
    /// Alters the stored procedures of the stream to the schema version being executed.
    /// </summary>
    public partial class UpdateStreamStoredProceduresOp : ReturningOperationBase<UpdateStreamStoredProceduresResult>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateStreamStoredProceduresOp"/> class.
        /// </summary>
        /// <param name="recordTagAssociationManagementStrategy">OPTIONAL record tag association management strategy.  DEFAULT is AssociatedDuringPutInSprocInTransaction."/>.</param>
        /// <param name="maxConcurrentHandlingCount">OPTIONAL maximum concurrent handling count.  DEFAULT is no limit.</param>
        public UpdateStreamStoredProceduresOp(
            RecordTagAssociationManagementStrategy recordTagAssociationManagementStrategy = RecordTagAssociationManagementStrategy.AssociatedDuringPutInSprocInTransaction,
            int? maxConcurrentHandlingCount = null)
        {
            // maxConcurrentHandlingCount.MustForArg(nameof(maxConcurrentHandlingCount)).greater

            this.RecordTagAssociationManagementStrategy = recordTagAssociationManagementStrategy;
            this.MaxConcurrentHandlingCount = maxConcurrentHandlingCount;
        }

        /// <summary>
        /// Gets the record tag association management strategy.
        /// </summary>
        public RecordTagAssociationManagementStrategy RecordTagAssociationManagementStrategy { get; private set; }

        /// <summary>
        /// Gets the maximum concurrent handling count.
        /// </summary>
        public int? MaxConcurrentHandlingCount { get; private set; }
    }
}