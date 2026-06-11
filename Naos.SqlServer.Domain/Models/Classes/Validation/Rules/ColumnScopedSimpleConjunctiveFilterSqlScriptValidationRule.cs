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
    /// A rule that disallows <c>OR</c> connectors and explicit <c>NOT</c> wrappers in any
    /// filter sub-expression (within <c>WHERE</c>, <c>HAVING</c>, and JOIN <c>ON</c> clauses)
    /// that references one of the configured columns.  <c>OR</c> / <c>NOT</c> over other
    /// columns is permitted, as long as the configured columns are filtered outside it.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is a column-scoped variant of
    /// <c>SimpleConjunctiveFilterSqlScriptValidationRule</c>.  Use it when only certain
    /// columns require the conjunctive-filter shape — typically the same columns that
    /// downstream rules (e.g. <c>ConstrainedFilterOperatorsByColumnSqlScriptValidationRule</c>
    /// or <c>BenchmarkingFilterValuesByColumnSqlScriptValidationRule</c>) introspect.
    /// </para>
    /// <para>
    /// The invariant being protected: every result row must satisfy the predicates on the
    /// configured columns.  An <c>OR</c> / <c>NOT</c> threatens that invariant only when a
    /// configured column is referenced WITHIN the <c>OR</c> / <c>NOT</c> subtree:
    /// </para>
    /// <list type="bullet">
    /// <item><description><c>WHERE entity_id = 'x' OR name = 'y'</c> — flagged: the
    /// <c>OR</c>'s subtree references <c>entity_id</c>, so rows can match without satisfying
    /// the <c>entity_id</c> predicate.</description></item>
    /// <item><description><c>WHERE NOT (entity_id = 'x')</c> — flagged: the <c>NOT</c>
    /// inverts a predicate on <c>entity_id</c>.</description></item>
    /// <item><description><c>WHERE entity_id = 'x' AND ((year = 2026 AND quarter = 1) OR
    /// (year = 2025 AND quarter IN (1, 4)))</c> — NOT flagged: the <c>OR</c>'s subtree does
    /// not reference <c>entity_id</c>; the <c>entity_id</c> predicate is AND-ed outside the
    /// <c>OR</c> and holds for every result row.</description></item>
    /// </list>
    /// <para>
    /// "Reference" means a column reference anywhere within the <c>OR</c> / <c>NOT</c>
    /// subtree resolves (through the FROM-clause alias map) to one of the configured columns.
    /// In multi-table queries, a bare column reference whose name matches a configured
    /// column's name also counts as a reference — the rule cannot prove the bare ref ISN'T
    /// the configured column without schema introspection, so it errs on the side of
    /// enforcing the shape.
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
    /// <c>OR</c> over a configured column invalidates that local reasoning — a predicate like
    /// <c>WHERE entity_id = 'auth' OR x = 5</c> appears to filter on <c>entity_id</c> but in
    /// fact returns rows matching either branch, defeating the authorization filter entirely.
    /// By requiring configured-column predicates to sit in AND-only positions (never inside
    /// an <c>OR</c> branch or <c>NOT</c> wrapper), every configured-column predicate must
    /// hold for a row to match, making per-predicate validation sound.
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
        /// <param name="columns">The columns that may not be referenced within an <c>OR</c>
        /// or <c>NOT</c> filter sub-expression.</param>
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
        /// Gets the columns that may not be referenced within an <c>OR</c> or <c>NOT</c>
        /// filter sub-expression.  Comparisons of schema, table, and column names against
        /// AST references are case-insensitive (SQL Server's default collation behavior).
        /// </summary>
        public IReadOnlyCollection<SchemaQualifiedColumnName> Columns { get; private set; }
    }
}
