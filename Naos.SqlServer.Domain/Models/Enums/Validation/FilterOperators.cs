// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilterOperators.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System;

    /// <summary>
    /// Flags enum corresponding to <see cref="FilterOperator"/>.
    /// </summary>
    [Flags]
    public enum FilterOperators
    {
        /// <summary>
        /// No operators.
        /// </summary>
        None = 0,

        /// <summary>
        /// Equality comparison: <c>a = b</c>.
        /// </summary>
        Equal = 1,

        /// <summary>
        /// Inequality comparison: <c>a &lt;&gt; b</c> or <c>a != b</c>.
        /// </summary>
        NotEqual = 2,

        /// <summary>
        /// Strictly less than: <c>a &lt; b</c>.
        /// </summary>
        LessThan = 4,

        /// <summary>
        /// Strictly greater than: <c>a &gt; b</c>.
        /// </summary>
        GreaterThan = 8,

        /// <summary>
        /// Less than or equal: <c>a &lt;= b</c> (also <c>a !&gt; b</c>).
        /// </summary>
        LessThanOrEqual = 16,

        /// <summary>
        /// Greater than or equal: <c>a &gt;= b</c> (also <c>a !&lt; b</c>).
        /// </summary>
        GreaterThanOrEqual = 32,

        /// <summary>
        /// Pattern match: <c>a LIKE pattern</c>.
        /// </summary>
        Like = 64,

        /// <summary>
        /// Negated pattern match: <c>a NOT LIKE pattern</c>.
        /// </summary>
        NotLike = 128,

        /// <summary>
        /// Set membership: <c>a IN (v1, v2, ...)</c>.
        /// </summary>
        In = 256,

        /// <summary>
        /// Negated set membership: <c>a NOT IN (v1, v2, ...)</c>.
        /// </summary>
        NotIn = 512,

        /// <summary>
        /// Range membership: <c>a BETWEEN low AND high</c>.
        /// </summary>
        Between = 1024,

        /// <summary>
        /// Negated range membership: <c>a NOT BETWEEN low AND high</c>.
        /// </summary>
        NotBetween = 2048,

        /// <summary>
        /// Null check: <c>a IS NULL</c>.
        /// </summary>
        IsNull = 4096,

        /// <summary>
        /// Non-null check: <c>a IS NOT NULL</c>.
        /// </summary>
        IsNotNull = 8192,
    }
}
