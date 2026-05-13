// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColumnScopedSimpleConjunctiveFilterSqlScriptValidationRule.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System.Collections.Generic;
    using OBeautifulCode.Assertion.Recipes;

    /// <summary>
    /// A rule that requires every filter expression (<c>WHERE</c>, <c>HAVING</c>, and JOIN
    /// <c>ON</c> clauses) of a query to be a simple conjunction of predicates — but only when
    /// that query's filters reference one of the configured columns.  Queries whose filters do
    /// not touch any configured column are not constrained.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is a column-scoped variant of
    /// <c>SimpleConjunctiveFilterSqlScriptValidationRule</c>.  Use it when only certain
    /// columns require the conjunctive-filter shape — typically the same columns that
    /// downstream rules (e.g. <c>ConstrainedFilterOperatorsByColumnSqlScriptValidationRule</c>
    /// or <c>AuthorizedFilterValuesByColumnSqlScriptValidationRule</c>) introspect.  Queries
    /// that don't touch those columns retain the freedom to use <c>OR</c> / <c>NOT</c>.
    /// </para>
    /// <para>
    /// Specifically flagged WHEN the query's filter clauses reference at least one configured
    /// column:
    /// </para>
    /// <list type="bullet">
    /// <item><description><c>OR</c> connectors (<c>BooleanBinaryExpression</c> with
    /// <c>BooleanBinaryExpressionType.Or</c>) appearing in any filter expression of that
    /// query.</description></item>
    /// <item><description><c>NOT</c> wrappers (<c>BooleanNotExpression</c>) appearing in any
    /// filter expression of that query.</description></item>
    /// </list>
    /// <para>
    /// "Reference" means any column reference in the query's WHERE / HAVING / JOIN ON clauses
    /// resolves (through the FROM-clause alias map) to one of the configured columns.  In
    /// multi-table queries, a bare column reference whose name matches a configured column's
    /// name also counts as a reference — the rule cannot prove the bare ref ISN'T the
    /// configured column without schema introspection, so it errs on the side of enforcing
    /// the shape.
    /// </para>
    /// <para>
    /// NOT flagged (these encode "not" inline rather than as a wrapping <c>NOT</c> node, so
    /// they remain leaf predicates in a conjunction):
    /// </para>
    /// <list type="bullet">
    /// <item><description><c>a &lt;&gt; b</c> / <c>a != b</c> — inequality comparison.</description></item>
    /// <item><description><c>a NOT LIKE pattern</c> — negated pattern match.</description></item>
    /// <item><description><c>a NOT IN (...)</c> — negated set membership.</description></item>
    /// <item><description><c>a NOT BETWEEN low AND high</c> — negated range.</description></item>
    /// <item><description><c>a IS NOT NULL</c> — non-null check.</description></item>
    /// </list>
    /// <para>
    /// Why this rule exists.  Filter-validation rules that inspect specific columns (operator
    /// allow-lists, value authorization) need to reason locally about each predicate.  An
    /// <c>OR</c> at the top level invalidates that local reasoning — a predicate like
    /// <c>WHERE entity_id = 'auth' OR x = 5</c> appears to filter on <c>entity_id</c> but in
    /// fact returns rows matching either branch, defeating the authorization filter entirely.
    /// By restricting filters of queries that touch a configured column to AND-only
    /// conjunctions of leaf predicates, every individual predicate must hold for a row to
    /// match, making per-predicate validation sound.
    /// </para>
    /// <para>
    /// Composes naturally with
    /// <c>ConstrainedFilterOperatorsByColumnSqlScriptValidationRule</c> and
    /// <c>AuthorizedFilterValuesByColumnSqlScriptValidationRule</c> — configure the same
    /// column list across all three so the conjunctive-shape precondition aligns with the
    /// columns the downstream rules introspect.  When you need the conjunctive shape applied
    /// to ALL filters regardless of which columns they touch, use
    /// <c>SimpleConjunctiveFilterSqlScriptValidationRule</c> instead.
    /// </para>
    /// </remarks>
    public partial class ColumnScopedSimpleConjunctiveFilterSqlScriptValidationRule : SqlScriptValidationRuleBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnScopedSimpleConjunctiveFilterSqlScriptValidationRule"/> class.
        /// </summary>
        /// <param name="columns">The columns whose presence in a query's filter clauses
        /// triggers the conjunctive-filter requirement for that query.</param>
        /// <param name="id">OPTIONAL identifier.  DEFAULT is no identifier.</param>
        public ColumnScopedSimpleConjunctiveFilterSqlScriptValidationRule(
            IReadOnlyCollection<SchemaQualifiedColumnName> columns,
            string id = null)
            : base(id)
        {
            new { columns }.AsArg().Must().NotBeNullNorEmptyEnumerableNorContainAnyNulls();

            this.Columns = columns;
        }

        /// <summary>
        /// Gets the columns whose presence in a query's filter clauses triggers the
        /// conjunctive-filter requirement for that query.  Comparisons of schema, table, and
        /// column names against AST references are case-insensitive (SQL Server's default
        /// collation behavior).
        /// </summary>
        public IReadOnlyCollection<SchemaQualifiedColumnName> Columns { get; private set; }
    }
}
