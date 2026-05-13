// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SimpleConjunctiveFilterSqlScriptValidationRule.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    /// <summary>
    /// A rule that requires every filter expression (<c>WHERE</c>, <c>HAVING</c>, and JOIN
    /// <c>ON</c> clauses) to be a simple conjunction of predicates — only <c>AND</c> is
    /// permitted as a boolean connector; <c>OR</c> and <c>NOT</c> are disallowed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Specifically flagged:
    /// </para>
    /// <list type="bullet">
    /// <item><description><c>OR</c> connectors (<c>BooleanBinaryExpression</c> with
    /// <c>BooleanBinaryExpressionType.Or</c>) appearing in any filter expression.</description></item>
    /// <item><description><c>NOT</c> wrappers (<c>BooleanNotExpression</c>) appearing in any
    /// filter expression.</description></item>
    /// </list>
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
    /// Why this rule exists.  Filter-validation rules that inspect specific columns
    /// (operator allow-lists, value authorization) need to reason locally about each
    /// predicate.  An <c>OR</c> at the top level invalidates that local reasoning — a
    /// predicate like <c>WHERE entity_id = 'auth' OR x = 5</c> appears to filter on
    /// <c>entity_id</c> but in fact returns rows matching either branch, defeating the
    /// authorization filter entirely.  By restricting filters to AND-only conjunctions of
    /// leaf predicates, every individual predicate must hold for a row to match, making
    /// per-predicate validation sound.
    /// </para>
    /// <para>
    /// The same reasoning rules out explicit <c>NOT (…)</c> wrappers: <c>WHERE NOT (entity_id = 'auth')</c>
    /// inverts the meaning of a predicate that looks like an authorization filter.
    /// </para>
    /// <para>
    /// Designed as a precondition for the filter-introspection rules
    /// <c>ConstrainedFilterOperatorsByColumnSqlScriptValidationRule</c> and
    /// <c>AuthorizedFilterValuesByColumnSqlScriptValidationRule</c>.  Apply this rule
    /// alongside them when the script's filter shape must be statically analyzable.
    /// </para>
    /// </remarks>
    public partial class SimpleConjunctiveFilterSqlScriptValidationRule : SqlScriptValidationRuleBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleConjunctiveFilterSqlScriptValidationRule"/> class.
        /// </summary>
        /// <param name="id">OPTIONAL identifier.  DEFAULT is no identifier.</param>
        public SimpleConjunctiveFilterSqlScriptValidationRule(
            string id = null)
            : base(id)
        {
        }
    }
}
