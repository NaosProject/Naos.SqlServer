// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JoinShapeIssues.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System;

    /// <summary>
    /// Categories of "weird" join shapes that <c>DisallowedJoinShapesSqlScriptValidationRule</c>
    /// can be configured to flag.  Flags are independent — set as many as your policy needs.
    /// </summary>
    [Flags]
    public enum JoinShapeIssues
    {
        /// <summary>
        /// No issues — used as a base value for bitwise composition.
        /// </summary>
        None = 0,

        /// <summary>
        /// The same physical table appears more than once in the FROM tree (with or without
        /// aliases).  Example: <c>FROM dbo.users u1 JOIN dbo.users u2 ON ...</c>.  Self-joins
        /// are legitimate in some contexts (hierarchies, row-to-row comparisons) but for
        /// LLM-generated queries usually mean the bot duplicated a table by accident.
        /// </summary>
        SelfJoin = 1,

        /// <summary>
        /// A <c>JOIN ... ON</c> clause whose search condition contains no column references —
        /// for example, <c>ON 1 = 1</c> or <c>ON 'x' = 'x'</c>.  Functionally a Cartesian
        /// product disguised as a join; usually means the LLM "faked" a join because it
        /// could not figure out how the two tables relate.
        /// </summary>
        ConstantOn = 2,

        /// <summary>
        /// An explicit <c>CROSS JOIN</c>.  Cartesian product by name.  Sometimes intentional
        /// (numbers tables, calendar tables) but usually a code smell.
        /// </summary>
        CrossJoin = 4,

        /// <summary>
        /// Old-style comma-FROM "join" where two or more tables are listed in the FROM
        /// clause separated by commas and the join condition is expressed in the WHERE
        /// clause as a column-on-column equality.  Example: <c>FROM a, b WHERE a.x = b.x</c>.
        /// Functionally identical to <c>INNER JOIN ON</c> but bypasses the JOIN-ON-targeted
        /// rules (<c>SanctionedJoinPairsSqlScriptValidationRule</c> in particular).  Pair
        /// with this flag to force the canonical JOIN ON syntax.
        /// </summary>
        WhereBasedJoin = 8,

        /// <summary>
        /// A <c>JOIN ... ON</c> clause where one side of an equality comparison is a literal
        /// rather than a column reference.  Example: <c>ON v.metric_absolute_id = 'abc'</c>.
        /// Semantically a filter, not a join condition — wrong clause for that predicate.
        /// </summary>
        LiteralInOn = 16,

        /// <summary>
        /// A <c>JOIN ... ON</c> clause using a non-equality operator on column references —
        /// for example, <c>ON a.x &lt; b.y</c>, <c>ON a.x BETWEEN b.low AND b.high</c>, or
        /// <c>ON a.x LIKE b.y</c>.  Range/pattern joins are legitimate in some contexts but
        /// rare in benchmarking and often a code smell from LLM-generated queries.
        /// </summary>
        NonEqualityOn = 32,

        /// <summary>
        /// A <c>JOIN ... ON</c> clause where one side of a comparison is a function call
        /// rather than a bare column reference — for example, <c>ON LOWER(a.col) = b.col</c>.
        /// Disables index usage, and usually masks a type or casing mismatch.
        /// </summary>
        FunctionInOn = 64,

        /// <summary>
        /// An implicit Cartesian product produced by comma-separated tables in the FROM
        /// clause with no join condition tying them together — for example,
        /// <c>FROM a, b</c> with no WHERE constraint linking <c>a</c> and <c>b</c>.  Almost
        /// always unintended.  Distinct from <see cref="CrossJoin"/>, which is the explicit
        /// <c>CROSS JOIN</c> keyword form.
        /// </summary>
        ImplicitCrossJoin = 128,

        /// <summary>
        /// All problematic join shapes.
        /// </summary>
        All = SelfJoin | ConstantOn | CrossJoin | WhereBasedJoin | LiteralInOn | NonEqualityOn | FunctionInOn | ImplicitCrossJoin,
    }
}
