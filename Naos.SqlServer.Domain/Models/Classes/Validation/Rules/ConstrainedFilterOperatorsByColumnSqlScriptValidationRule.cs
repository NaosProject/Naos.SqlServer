// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConstrainedFilterOperatorsByColumnSqlScriptValidationRule.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System.Collections.Generic;
    using OBeautifulCode.Assertion.Recipes;

    /// <summary>
    /// A rule that constrains which filter operators are allowed on specific columns.  For
    /// each configured column, only the operators in its allow-list may appear in filter
    /// predicates (<c>WHERE</c>, <c>HAVING</c>, <c>JOIN ... ON</c>).
    /// </summary>
    /// <remarks>
    /// <para>
    /// Typical use: restrict <c>entity_id</c>-style columns to whole-string matching only,
    /// rejecting <c>LIKE</c> / <c>&gt;</c> / <c>BETWEEN</c> / etc.  Example configuration:
    /// </para>
    /// <code>
    /// new ConstrainedFilterOperatorsByColumnSqlScriptValidationRule(new[]
    /// {
    ///     new ColumnFilterOperators(
    ///         new SchemaQualifiedColumnName("dbo", "users", "entity_id"),
    ///         new[] { FilterOperator.Equal, FilterOperator.In }),
    /// });
    /// </code>
    /// <para>
    /// With this configuration, <c>WHERE entity_id = 'abc'</c> and
    /// <c>WHERE entity_id IN ('a', 'b')</c> pass; <c>WHERE entity_id LIKE 'a%'</c>,
    /// <c>WHERE entity_id &gt; 'm'</c>, <c>WHERE entity_id BETWEEN 'a' AND 'z'</c>, and
    /// <c>WHERE entity_id IS NULL</c> are flagged.
    /// </para>
    /// <para>
    /// Columns NOT listed in the configuration are unaffected — any operator is allowed on
    /// them.  This rule restricts; it does not grant.  Pair with other rules to constrain the
    /// rest of the script.
    /// </para>
    /// <para>
    /// Column resolution is handled by
    /// <c>FilterPredicateSqlScriptValidationRuleEvaluatorBase</c>: bare and alias-qualified
    /// column references are resolved to fully-qualified <c>SchemaQualifiedColumnName</c> via
    /// the FROM-clause alias map.  In single-table queries, a bare reference unambiguously
    /// resolves to that one table.  In multi-table queries (JOINs), a bare reference cannot
    /// be resolved without schema introspection; the rule treats this as a violation:
    /// <c>"column reference must be table-qualified in multi-table queries: {name}"</c> — but
    /// only when the bare column's name matches one of the configured columns by name.  Bare
    /// references whose name does not match any configured column are skipped.
    /// </para>
    /// <para>
    /// Predicate types handled (one rule check per predicate, with column-to-column
    /// comparisons producing one check per side with the operator viewed from each side):
    /// </para>
    /// <list type="bullet">
    /// <item><description>Binary comparisons: <c>=</c>, <c>&lt;&gt;</c>, <c>!=</c>, <c>&lt;</c>, <c>&gt;</c>,
    /// <c>&lt;=</c>, <c>&gt;=</c>, <c>!&lt;</c>, <c>!&gt;</c>.</description></item>
    /// <item><description><c>LIKE</c> / <c>NOT LIKE</c>.</description></item>
    /// <item><description><c>IN (...)</c> / <c>NOT IN (...)</c>.</description></item>
    /// <item><description><c>BETWEEN low AND high</c> / <c>NOT BETWEEN low AND high</c>.</description></item>
    /// <item><description><c>IS NULL</c> / <c>IS NOT NULL</c>.</description></item>
    /// </list>
    /// <para>
    /// Composes naturally with <c>SimpleConjunctiveFilterSqlScriptValidationRule</c>
    /// (so each predicate is analyzed standalone) and with
    /// <c>FlatQuerySqlScriptValidationRule</c> (so there is only one query scope to analyze).
    /// </para>
    /// </remarks>
    public partial class ConstrainedFilterOperatorsByColumnSqlScriptValidationRule : SqlScriptValidationRuleBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConstrainedFilterOperatorsByColumnSqlScriptValidationRule"/> class.
        /// </summary>
        /// <param name="columnFilterOperators">The per-column operator allow-lists.</param>
        /// <param name="id">OPTIONAL identifier.  DEFAULT is no identifier.</param>
        public ConstrainedFilterOperatorsByColumnSqlScriptValidationRule(
            IReadOnlyCollection<ColumnFilterOperators> columnFilterOperators,
            string id = null)
            : base(id)
        {
            new { columnFilterOperators }.AsArg().Must().NotBeNullNorEmptyEnumerableNorContainAnyNulls();

            this.ColumnFilterOperators = columnFilterOperators;
        }

        /// <summary>
        /// Gets the per-column operator allow-lists.  Comparisons of schema, table, and
        /// column names against AST references are case-insensitive (SQL Server's default
        /// collation behavior).
        /// </summary>
        public IReadOnlyCollection<ColumnFilterOperators> ColumnFilterOperators { get; private set; }
    }
}
