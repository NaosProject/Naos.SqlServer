// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilterOperator.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    /// <summary>
    /// The kind of comparison operators used in a filter predicate (WHERE / HAVING / ON
    /// clause).  These map normalized AST predicate types to a single enum so that
    /// filter-validation rules can reason about operators without dealing with ScriptDom's
    /// many predicate node types directly.
    /// </summary>
    public enum FilterOperator
    {
        /// <summary>
        /// Unknown (default).
        /// </summary>
        Unknown,

        /// <summary>
        /// Equality comparison: <c>a = b</c>.
        /// </summary>
        Equal,

        /// <summary>
        /// Inequality comparison: <c>a &lt;&gt; b</c> or <c>a != b</c>.
        /// </summary>
        NotEqual,

        /// <summary>
        /// Strictly less than: <c>a &lt; b</c>.
        /// </summary>
        LessThan,

        /// <summary>
        /// Strictly greater than: <c>a &gt; b</c>.
        /// </summary>
        GreaterThan,

        /// <summary>
        /// Less than or equal: <c>a &lt;= b</c> (also <c>a !&gt; b</c>).
        /// </summary>
        LessThanOrEqual,

        /// <summary>
        /// Greater than or equal: <c>a &gt;= b</c> (also <c>a !&lt; b</c>).
        /// </summary>
        GreaterThanOrEqual,

        /// <summary>
        /// Pattern match: <c>a LIKE pattern</c>.
        /// </summary>
        Like,

        /// <summary>
        /// Negated pattern match: <c>a NOT LIKE pattern</c>.
        /// </summary>
        NotLike,

        /// <summary>
        /// Set membership: <c>a IN (v1, v2, ...)</c>.
        /// </summary>
        In,

        /// <summary>
        /// Negated set membership: <c>a NOT IN (v1, v2, ...)</c>.
        /// </summary>
        NotIn,

        /// <summary>
        /// Range membership: <c>a BETWEEN low AND high</c>.
        /// </summary>
        Between,

        /// <summary>
        /// Negated range membership: <c>a NOT BETWEEN low AND high</c>.
        /// </summary>
        NotBetween,

        /// <summary>
        /// Null check: <c>a IS NULL</c>.
        /// </summary>
        IsNull,

        /// <summary>
        /// Non-null check: <c>a IS NOT NULL</c>.
        /// </summary>
        IsNotNull,
    }
}
