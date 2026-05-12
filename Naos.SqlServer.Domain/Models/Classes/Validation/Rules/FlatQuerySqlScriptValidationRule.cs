// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FlatQuerySqlScriptValidationRule.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    /// <summary>
    /// A rule that requires the SQL script to be a "flat" query — a single query scope with
    /// no CTEs, no set operators, and no subqueries of any flavor.
    /// </summary>
    /// <remarks>
    /// <para>
    /// In ScriptDom terms, "flat" means the AST contains at most one <c>QuerySpecification</c>.
    /// Every nesting construct in T-SQL introduces an additional <c>QuerySpecification</c>; the
    /// rule simply counts them.
    /// </para>
    /// <para>
    /// Flagged (each adds an additional query scope):
    /// </para>
    /// <list type="bullet">
    /// <item><description>Common table expressions — <c>WITH cte AS (SELECT …)</c>, including
    /// recursive CTEs.</description></item>
    /// <item><description>Set operators — <c>UNION</c>, <c>UNION ALL</c>, <c>INTERSECT</c>,
    /// <c>EXCEPT</c>.</description></item>
    /// <item><description>Derived tables — <c>FROM (SELECT …) t</c>.</description></item>
    /// <item><description>Scalar subqueries — <c>WHERE col = (SELECT …)</c>, scalar subqueries
    /// in the SELECT list, etc.</description></item>
    /// <item><description>Existence / membership subqueries — <c>EXISTS (…)</c>,
    /// <c>NOT EXISTS (…)</c>, <c>col IN (SELECT …)</c>, <c>col NOT IN (SELECT …)</c>.</description></item>
    /// <item><description>Quantified comparisons — <c>col = ANY (…)</c>, <c>col &gt; SOME (…)</c>,
    /// <c>col &gt; ALL (…)</c>.</description></item>
    /// <item><description>APPLY — <c>CROSS APPLY (SELECT …) x</c>, <c>OUTER APPLY (…) x</c>.</description></item>
    /// </list>
    /// <para>
    /// NOT flagged (these are flat — single <c>QuerySpecification</c>):
    /// </para>
    /// <list type="bullet">
    /// <item><description>JOINs of all kinds — INNER, LEFT/RIGHT/FULL OUTER, CROSS,
    /// comma-joins, self-joins, multi-way joins, parenthesized join groups.  Each contributes
    /// additional FROM-clause structure within the same query scope.</description></item>
    /// <item><description>Window functions — <c>… OVER (PARTITION BY … ORDER BY …)</c>.  The
    /// <c>OVER</c> clause is a modifier, not a new query scope.</description></item>
    /// <item><description>Scalar expressions — <c>CASE WHEN … THEN … ELSE … END</c>,
    /// <c>IIF(…)</c>, arithmetic, string functions, function calls.</description></item>
    /// <item><description>Inline value constructors — <c>FROM (VALUES (1), (2)) AS v(x)</c> —
    /// this is an <c>InlineDerivedTable</c>, not a nested query.</description></item>
    /// <item><description><c>GROUP BY</c>, <c>HAVING</c>, <c>ORDER BY</c>, <c>TOP N</c>,
    /// <c>OFFSET … FETCH</c> — all clauses of the single query scope.</description></item>
    /// <item><description>References to pre-defined views — the parser does not crack open the
    /// view's definition, so <c>SELECT * FROM dbo.my_view</c> stays flat regardless of how the
    /// view is internally defined.  This is a deliberate escape hatch for non-flattenable
    /// patterns such as <c>UNION</c>: define the union inside a view and select from the view.</description></item>
    /// </list>
    /// <para>
    /// The rule emits at most one violation per script, at the start offset of the second
    /// <c>QuerySpecification</c> encountered during visitor traversal.  Visit order is not
    /// always source-order: in particular, for CTEs the body is visited before the outer
    /// SELECT, so the reported offset points at the outer SELECT rather than the <c>WITH</c>
    /// clause.  The presence of any nesting is what's flagged; the exact reported location is
    /// approximate.
    /// </para>
    /// <para>
    /// Composes naturally with <see cref="ReadOnlySelectSqlScriptValidationRule"/> and
    /// <see cref="SingleStatementSqlScriptValidationRule"/> to constrain the script's shape
    /// to "exactly one read-only SELECT, no nested scopes" — the canonical shape that
    /// downstream filter-validation rules can analyze tractably.
    /// </para>
    /// </remarks>
    public partial class FlatQuerySqlScriptValidationRule : SqlScriptValidationRuleBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FlatQuerySqlScriptValidationRule"/> class.
        /// </summary>
        /// <param name="id">OPTIONAL identifier.  DEFAULT is no identifier.</param>
        public FlatQuerySqlScriptValidationRule(
            string id = null)
            : base(id)
        {
        }
    }
}
