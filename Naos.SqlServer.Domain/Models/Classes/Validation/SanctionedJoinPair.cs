// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SanctionedJoinPair.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Type;
    using static System.FormattableString;

    /// <summary>
    /// A sanctioned join pair — two columns that may legally appear on the two sides of an
    /// equality join predicate.  Pairs are bidirectional: <c>(A, B)</c> sanctions both
    /// <c>ON A = B</c> and <c>ON B = A</c>.
    /// </summary>
    /// <remarks>
    /// Used by <c>SanctionedJoinPairsSqlScriptValidationRule</c> to express the legal join
    /// edges between columns.  Typically these correspond to foreign-key relationships in
    /// the schema (e.g., a parent's primary key paired with each child's foreign key column).
    /// </remarks>
    public partial class SanctionedJoinPair : IModelViaCodeGen, IDeclareToStringMethod
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SanctionedJoinPair"/> class.
        /// </summary>
        /// <param name="leftColumn">One side of the pair.</param>
        /// <param name="rightColumn">The other side of the pair.</param>
        public SanctionedJoinPair(
            SchemaQualifiedColumnName leftColumn,
            SchemaQualifiedColumnName rightColumn)
        {
            new { leftColumn }.AsArg().Must().NotBeNull();
            new { rightColumn }.AsArg().Must().NotBeNull();

            this.LeftColumn = leftColumn;
            this.RightColumn = rightColumn;
        }

        /// <summary>
        /// Gets one side of the pair.  Order is not significant — the rule matches both
        /// orderings.
        /// </summary>
        public SchemaQualifiedColumnName LeftColumn { get; private set; }

        /// <summary>
        /// Gets the other side of the pair.  Order is not significant — the rule matches
        /// both orderings.
        /// </summary>
        public SchemaQualifiedColumnName RightColumn { get; private set; }

        /// <inheritdoc cref="IDeclareToStringMethod" />
        public override string ToString()
        {
            var result = Invariant($"{this.LeftColumn} <-> {this.RightColumn}");

            return result;
        }
    }
}
