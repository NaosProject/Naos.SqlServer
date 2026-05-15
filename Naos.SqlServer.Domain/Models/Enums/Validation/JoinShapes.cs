// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JoinShapes.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System;

    /// <summary>
    /// Categorical join-shape observations that
    /// <c>DisallowedJoinShapesSqlScriptValidationRule</c> can be configured to flag.  Each
    /// flag is a neutral description of a syntactic pattern; whether a given shape is
    /// problematic is a policy choice expressed by the caller's flag selection.  Setting
    /// <see cref="All"/> effectively disallows joins entirely.
    /// </summary>
    /// <remarks>
    /// Flags are independent and may be combined.  A single query may match multiple flags
    /// — for example, a CROSS APPLY would match <see cref="CrossApply"/>; a self-join via
    /// INNER JOIN would match both <see cref="SelfJoin"/> and <see cref="InnerJoin"/>.
    /// </remarks>
    [Flags]
    public enum JoinShapes
    {
        /// <summary>
        /// No shapes — used as a base value for bitwise composition.
        /// </summary>
        None = 0,

        /// <summary>
        /// An <c>INNER JOIN ... ON</c>.
        /// </summary>
        InnerJoin = 1 << 0,

        /// <summary>
        /// A <c>LEFT OUTER JOIN ... ON</c>.
        /// </summary>
        LeftOuterJoin = 1 << 1,

        /// <summary>
        /// A <c>RIGHT OUTER JOIN ... ON</c>.
        /// </summary>
        RightOuterJoin = 1 << 2,

        /// <summary>
        /// A <c>FULL OUTER JOIN ... ON</c>.
        /// </summary>
        FullOuterJoin = 1 << 3,

        /// <summary>
        /// An explicit <c>CROSS JOIN</c>.  Cartesian product by name.
        /// </summary>
        CrossJoin = 1 << 4,

        /// <summary>
        /// A <c>CROSS APPLY</c>.  T-SQL extension that invokes a table-valued expression
        /// (typically a correlated subquery or table-valued function) once per outer row.
        /// </summary>
        CrossApply = 1 << 5,

        /// <summary>
        /// An <c>OUTER APPLY</c>.  T-SQL extension; like <see cref="CrossApply"/> but
        /// preserves outer rows when the right side produces no rows.
        /// </summary>
        OuterApply = 1 << 6,

        /// <summary>
        /// Old-style comma-FROM "join" where two or more tables are listed in the FROM
        /// clause separated by commas and the join condition is expressed in the WHERE
        /// clause as a column-on-column equality.  Example: <c>FROM a, b WHERE a.x = b.x</c>.
        /// </summary>
        WhereBasedJoin = 1 << 7,

        /// <summary>
        /// An implicit Cartesian product produced by comma-separated tables in the FROM
        /// clause with no join condition tying them together — for example,
        /// <c>FROM a, b</c> with no WHERE constraint linking <c>a</c> and <c>b</c>.
        /// Distinct from <see cref="CrossJoin"/>, which is the explicit <c>CROSS JOIN</c>
        /// keyword form.
        /// </summary>
        ImplicitCrossJoin = 1 << 8,

        /// <summary>
        /// The same physical table appears more than once in the FROM tree (with or without
        /// aliases).  Example: <c>FROM dbo.users u1 JOIN dbo.users u2 ON ...</c>.
        /// </summary>
        SelfJoin = 1 << 9,

        /// <summary>
        /// A <c>JOIN ... ON</c> clause whose search condition contains no column references —
        /// for example, <c>ON 1 = 1</c> or <c>ON 'x' = 'x'</c>.
        /// </summary>
        ConstantOn = 1 << 10,

        /// <summary>
        /// A <c>JOIN ... ON</c> clause where one side of an equality comparison is a
        /// literal rather than a column reference.  Example:
        /// <c>ON v.metric_absolute_id = 'abc'</c>.
        /// </summary>
        LiteralInOn = 1 << 11,

        /// <summary>
        /// A <c>JOIN ... ON</c> clause using a non-equality operator on its predicates —
        /// for example, <c>ON a.x &lt; b.y</c>, <c>ON a.x BETWEEN b.low AND b.high</c>, or
        /// <c>ON a.x LIKE b.y</c>.
        /// </summary>
        NonEqualityOn = 1 << 12,

        /// <summary>
        /// A <c>JOIN ... ON</c> clause where one side of a comparison is a function call
        /// rather than a bare column reference — for example, <c>ON LOWER(a.col) = b.col</c>.
        /// </summary>
        FunctionInOn = 1 << 13,

        /// <summary>
        /// All recognized join shapes — set this to effectively disallow joins entirely.
        /// </summary>
        All = InnerJoin | LeftOuterJoin | RightOuterJoin | FullOuterJoin | CrossJoin
            | CrossApply | OuterApply | WhereBasedJoin | ImplicitCrossJoin | SelfJoin
            | ConstantOn | LiteralInOn | NonEqualityOn | FunctionInOn,
    }
}
