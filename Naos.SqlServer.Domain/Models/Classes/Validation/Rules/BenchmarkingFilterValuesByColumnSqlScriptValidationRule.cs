// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BenchmarkingFilterValuesByColumnSqlScriptValidationRule.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System.Collections.Generic;
    using OBeautifulCode.Assertion.Recipes;

    /// <summary>
    /// A rule that constrains how a partition column (e.g. <c>entity_id</c>) may be filtered
    /// so that queries fit one of three benchmarking-safe shapes: direct access to owned
    /// rows, aggregation across a chosen peer cohort, or exploration of non-owned entities.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The rule is designed for systems where each row of a table belongs to an "entity"
    /// (a company, a tenant, a user, etc.).  The caller owns one or more of those entities
    /// — their <see cref="OwnedValues"/>; every other entity is a "peer."  The rule enforces
    /// the invariant that any filter on the constrained column constrains the result set to
    /// a known partition of rows, in one of the three shapes described below.  Operators and
    /// value mixes that do not constrain the result set (or that would leak owned rows into
    /// a peer query, or vice versa) are rejected.
    /// </para>
    /// <para>
    /// Three pass conditions — pick exactly one shape per query:
    /// </para>
    /// <list type="bullet">
    /// <item><description><b>(a) Own data.</b>  <c>column = ownedValue</c> or
    /// <c>column IN (ownedValues...)</c>.  Every listed value must be in
    /// <see cref="OwnedValues"/>.  Caller retrieves rows for their own entities — direct
    /// access.</description></item>
    /// <item><description><b>(b) Benchmark cohort.</b>  <c>column IN (peer_a, peer_b, ...)</c>
    /// where every listed value is a peer (NOT in <see cref="OwnedValues"/>) AND at least
    /// <see cref="MinimumDistinctPeerValues"/> distinct values are present.  Caller has
    /// identified a specific group of peers and wants aggregate statistics across them.
    /// The minimum-distinct threshold prevents naming a single peer (or two) as a way to
    /// effectively single out their data via aggregation.</description></item>
    /// <item><description><b>(c) Peer exploration.</b>  <c>column &lt;&gt; ownedValue</c> or
    /// <c>column NOT IN (ownedValues...)</c>.  Every listed value must be in
    /// <see cref="OwnedValues"/>.  Caller is browsing the universe of non-owned entities to
    /// discover candidates for a benchmark cohort.  Listing only owned values keeps this
    /// safe: the caller cannot use the exclusion form to bypass authorization on any peer
    /// (excluding a peer from the result set would still leak everyone else, but the rule
    /// disallows naming peers in this shape).</description></item>
    /// </list>
    /// <para>
    /// Mixed-value filters and unsupported operators are rejected:
    /// </para>
    /// <list type="bullet">
    /// <item><description><c>IN</c> with a mix of owned and peer values fails — could not be
    /// classified as either (a) or (b).</description></item>
    /// <item><description><c>NOT IN</c> or <c>&lt;&gt;</c> with any peer value fails — would
    /// return the listed peer's rows EXCLUDED but include everyone else (all owned rows + all
    /// other peer rows), which is the opposite of a peer-exclusion shape and indistinguishable
    /// from authorization-bypass probing.  There is no NOT-IN equivalent of (b): "<c>NOT IN
    /// (many peers)</c>" cannot constrain the result set (it returns everything EXCEPT the
    /// listed peers, which leaks owned rows and many other peer rows).</description></item>
    /// <item><description>Operators other than <c>=</c>, <c>IN</c>, <c>&lt;&gt;</c>, <c>NOT IN</c>
    /// are rejected — <c>&lt;</c>, <c>&gt;</c>, <c>BETWEEN</c>, <c>LIKE</c>, <c>IS NULL</c>,
    /// etc. do not constrain the result set to a caller-listed partition in either direction.</description></item>
    /// <item><description>Filter values must be literal constants — no parameters, function
    /// calls, expressions, or <c>NULL</c>.  The rule needs to enumerate the values and
    /// compare them against <see cref="OwnedValues"/>.</description></item>
    /// </list>
    /// <para>
    /// Other constraints:
    /// </para>
    /// <list type="bullet">
    /// <item><description>Only one filter on the constrained column is allowed per query.
    /// Combining patterns (e.g. <c>column IN (peer_pool) AND column &lt;&gt; owned</c>)
    /// would require intersection-of-filters analysis that the rule does not attempt.</description></item>
    /// <item><description>If <see cref="RequireFilterOnConstrainedColumn"/> is true (the
    /// default) and the constrained column's table appears in the query's FROM clause but no
    /// filter on the constrained column is present, the rule flags the missing filter.</description></item>
    /// <item><description>Comparisons of schema, table, and column names against AST
    /// references are case-insensitive.  Comparisons of values against
    /// <see cref="OwnedValues"/> are case-sensitive — entity ids are typically opaque
    /// external identifiers (UUIDs, GUIDs, ...) where case matters.</description></item>
    /// </list>
    /// <para>
    /// Composes naturally with
    /// <c>ColumnScopedSimpleConjunctiveFilterSqlScriptValidationRule</c> (configure the same
    /// constrained column there so the conjunctive-shape precondition holds — without it,
    /// <c>OR</c> would let the caller name half the filter under one shape and half under
    /// another) and with <c>ConstrainedFilterOperatorsByColumnSqlScriptValidationRule</c>
    /// (configure the same column there with operator allow-list
    /// <c>{ Equal, In, NotEqual, NotIn }</c> so the operator policy aligns).
    /// </para>
    /// <para>
    /// Note: this rule constrains the SHAPE of the filter on the partition column.  It does
    /// not enforce that cross-entity queries return aggregates only (case (b) typically uses
    /// <c>SUM</c>/<c>AVG</c>/<c>COUNT</c>, but the rule does not check this).  If
    /// row-level cross-entity exposure is a concern in your domain, pair this rule with a
    /// separate aggregation-required policy at the application layer.
    /// </para>
    /// </remarks>
    public partial class BenchmarkingFilterValuesByColumnSqlScriptValidationRule : SqlScriptValidationRuleBase
    {
        /// <summary>
        /// The default value of <see cref="MinimumDistinctPeerValues"/>.
        /// </summary>
        public const int DefaultMinimumDistinctPeerValues = 3;

        /// <summary>
        /// Initializes a new instance of the <see cref="BenchmarkingFilterValuesByColumnSqlScriptValidationRule"/> class.
        /// </summary>
        /// <param name="column">The constrained (partition) column — typically <c>entity_id</c>
        /// or similar.</param>
        /// <param name="ownedValues">The values the caller owns: their own entity ids.
        /// Anything not in this set is a "peer."  Comparisons against AST literal values
        /// are case-sensitive.</param>
        /// <param name="minimumDistinctPeerValues">OPTIONAL minimum number of distinct peer
        /// values required for the (b) benchmark-cohort shape.  DEFAULT is
        /// <see cref="DefaultMinimumDistinctPeerValues"/>.</param>
        /// <param name="requireFilterOnConstrainedColumn">OPTIONAL flag indicating whether
        /// a query whose FROM clause references the constrained column's table MUST include
        /// a filter on the constrained column.  DEFAULT is <c>true</c>.</param>
        /// <param name="id">OPTIONAL identifier.  DEFAULT is no identifier.</param>
        public BenchmarkingFilterValuesByColumnSqlScriptValidationRule(
            SchemaQualifiedColumnName column,
            IReadOnlyCollection<string> ownedValues,
            int minimumDistinctPeerValues = DefaultMinimumDistinctPeerValues,
            bool requireFilterOnConstrainedColumn = true,
            string id = null)
            : base(id)
        {
            new { column }.AsArg().Must().NotBeNull();
            new { ownedValues }.AsArg().Must().NotBeNullNorEmptyEnumerableNorContainAnyNulls().And().Each().NotBeNullNorWhiteSpace();
            new { minimumDistinctPeerValues }.AsArg().Must().BeGreaterThanOrEqualTo(1);

            this.Column = column;
            this.OwnedValues = ownedValues;
            this.MinimumDistinctPeerValues = minimumDistinctPeerValues;
            this.RequireFilterOnConstrainedColumn = requireFilterOnConstrainedColumn;
        }

        /// <summary>
        /// Gets the constrained (partition) column.
        /// </summary>
        public SchemaQualifiedColumnName Column { get; private set; }

        /// <summary>
        /// Gets the set of values the caller owns.  Anything not in this set is treated as
        /// a peer.  Comparisons are case-sensitive.
        /// </summary>
        public IReadOnlyCollection<string> OwnedValues { get; private set; }

        /// <summary>
        /// Gets the minimum number of distinct peer values required for the (b)
        /// benchmark-cohort shape.
        /// </summary>
        public int MinimumDistinctPeerValues { get; private set; }

        /// <summary>
        /// Gets a value indicating whether a query whose FROM clause references the
        /// constrained column's table MUST include a filter on the constrained column.
        /// </summary>
        public bool RequireFilterOnConstrainedColumn { get; private set; }
    }
}
