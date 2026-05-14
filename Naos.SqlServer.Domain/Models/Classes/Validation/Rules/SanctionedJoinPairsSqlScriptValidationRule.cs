// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SanctionedJoinPairsSqlScriptValidationRule.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System.Collections.Generic;
    using OBeautifulCode.Assertion.Recipes;

    /// <summary>
    /// A rule that constrains which column pairs may appear on the two sides of an equality
    /// JOIN ON predicate.  The configured pairs typically correspond to foreign-key
    /// relationships in the schema.
    /// </summary>
    /// <remarks>
    /// <para>
    /// For each ON predicate of the form <c>L.col = R.col</c> in a <c>JOIN ... ON</c>, the rule
    /// resolves both sides through the alias map and checks whether the resulting
    /// <c>(left, right)</c> pair appears in the sanctioned list (in either order).
    /// </para>
    /// <para>
    /// Behavior per predicate:
    /// </para>
    /// <list type="bullet">
    /// <item><description><b>Sanctioned pair</b> — passes.</description></item>
    /// <item><description><b>Neither side is a column that appears in any sanctioned pair</b>
    /// — uncovered, skipped silently.  This rule constrains only the columns it has been told
    /// about; joins on other columns are not its concern.</description></item>
    /// <item><description><b>At least one side IS a column that appears in some sanctioned
    /// pair, but this particular pair is not sanctioned</b> — violation: the caller used a
    /// constrained column in a join that the FK graph does not allow.</description></item>
    /// </list>
    /// <para>
    /// Example.  Configure the FK edges from <c>dbo.metric_absolute.metric_absolute_id</c> to
    /// each child table's <c>metric_absolute_id</c>:
    /// </para>
    /// <code>
    /// new SanctionedJoinPairsSqlScriptValidationRule(new[]
    /// {
    ///     new SanctionedJoinPair(
    ///         new SchemaQualifiedColumnName("dbo", "metric_absolute", "metric_absolute_id"),
    ///         new SchemaQualifiedColumnName("dbo", "value_absolute_calendar_quarter", "metric_absolute_id")),
    ///     new SanctionedJoinPair(
    ///         new SchemaQualifiedColumnName("dbo", "metric_absolute", "metric_absolute_id"),
    ///         new SchemaQualifiedColumnName("dbo", "value_absolute_fiscal_year", "metric_absolute_id")),
    /// });
    /// </code>
    /// <para>
    /// With this configuration:
    /// </para>
    /// <list type="bullet">
    /// <item><description><c>metric_absolute m JOIN value_absolute_calendar_quarter v ON m.metric_absolute_id = v.metric_absolute_id</c>
    /// — passes (pair sanctioned).</description></item>
    /// <item><description><c>value_absolute_calendar_quarter v JOIN value_absolute_fiscal_year f ON v.metric_absolute_id = f.metric_absolute_id</c>
    /// — violation: both columns are constrained (each appears in a sanctioned pair) but
    /// the pair <c>(v, f)</c> is not sanctioned, so the caller is bypassing the FK graph.</description></item>
    /// </list>
    /// <para>
    /// Scope and limitations:
    /// </para>
    /// <list type="bullet">
    /// <item><description>Only <c>JOIN ... ON</c> predicates are inspected.  Old-style
    /// comma-FROM joins (joining via WHERE) are not covered here.  Pair with
    /// <c>DisallowedJoinShapesSqlScriptValidationRule</c> (with the
    /// <c>WhereBasedJoin</c> flag) to force the canonical JOIN ON form.</description></item>
    /// <item><description>Only equality comparisons (<c>=</c>) are inspected.  Non-equality
    /// join conditions (<c>&lt;</c>, <c>BETWEEN</c>, <c>LIKE</c>, ...) are not join "pairs";
    /// flag those with <c>DisallowedJoinShapesSqlScriptValidationRule</c> using
    /// <c>NonEqualityOn</c> if you don't want them.</description></item>
    /// <item><description>Only column-on-column comparisons are inspected.  ON predicates
    /// where one side is a literal, parameter, function call, or other expression are
    /// skipped — they're not joins through a pair.  If those shapes concern you, use
    /// <c>DisallowedJoinShapesSqlScriptValidationRule</c> with <c>LiteralInOn</c> or
    /// <c>FunctionInOn</c>.</description></item>
    /// <item><description>Joins where one side is a bare (unqualified) column reference in a
    /// multi-table FROM are silently skipped: the rule cannot resolve the column without
    /// schema introspection.  Other rules (filter-predicate rules; a future "all references
    /// must be table-qualified" rule) can enforce qualification.</description></item>
    /// <item><description>Sub-selects and derived tables in joins are blocked by
    /// <c>FlatQuerySqlScriptValidationRule</c>; this rule does not attempt to resolve
    /// columns through them.</description></item>
    /// <item><description>Multiple ON predicates joined with <c>AND</c> are walked
    /// independently — each equality BCE is checked on its own.  Composite-key
    /// relationships (requiring BOTH columns to match for a single logical join) are not
    /// modeled.</description></item>
    /// <item><description>Compose with
    /// <c>ColumnScopedSimpleConjunctiveFilterSqlScriptValidationRule</c> on the same
    /// constrained columns so that ON predicates with <c>OR</c> / <c>NOT</c> are rejected
    /// before this rule walks them — otherwise the rule walks through OR/NOT and may emit
    /// confusing violations.</description></item>
    /// </list>
    /// <para>
    /// Comparisons of schema, table, and column names against AST references are
    /// case-insensitive (SQL Server's default collation behavior).
    /// </para>
    /// </remarks>
    public partial class SanctionedJoinPairsSqlScriptValidationRule : SqlScriptValidationRuleBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SanctionedJoinPairsSqlScriptValidationRule"/> class.
        /// </summary>
        /// <param name="sanctionedJoinPairs">The sanctioned join pairs.</param>
        /// <param name="id">OPTIONAL identifier.  DEFAULT is no identifier.</param>
        public SanctionedJoinPairsSqlScriptValidationRule(
            IReadOnlyCollection<SanctionedJoinPair> sanctionedJoinPairs,
            string id = null)
            : base(id)
        {
            new { sanctionedJoinPairs }.AsArg().Must().NotBeNullNorEmptyEnumerableNorContainAnyNulls();

            this.SanctionedJoinPairs = sanctionedJoinPairs;
        }

        /// <summary>
        /// Gets the sanctioned join pairs.
        /// </summary>
        public IReadOnlyCollection<SanctionedJoinPair> SanctionedJoinPairs { get; private set; }
    }
}
