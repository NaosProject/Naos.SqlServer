// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisallowedJoinShapesSqlScriptValidationRule.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using OBeautifulCode.Assertion.Recipes;

    /// <summary>
    /// A rule that flags join shapes selected via a flags enum.  Each flag in
    /// <see cref="DisallowedShapes"/> enables a corresponding check; unselected shapes
    /// are not flagged.  Whether a given shape is "problematic" is a policy choice
    /// expressed by which flags the caller enables — set <see cref="JoinShapes.All"/>
    /// to effectively disallow joins entirely.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The shapes recognized by the rule are categorical observations about a query's
    /// FROM tree and any associated ON / WHERE clauses.  See <see cref="JoinShapes"/>
    /// for the complete list and per-flag descriptions.
    /// </para>
    /// <para>
    /// Flag detection is independent — a single query may produce violations for
    /// multiple flags (e.g. a self-join via INNER JOIN matches both
    /// <see cref="JoinShapes.SelfJoin"/> and <see cref="JoinShapes.InnerJoin"/>).  The
    /// rule is conservative: it only flags shapes it can identify syntactically,
    /// without schema introspection.  Sub-selects and derived tables in joins are
    /// blocked elsewhere (<c>FlatQuerySqlScriptValidationRule</c>); this rule does not
    /// attempt to look inside them.
    /// </para>
    /// <para>
    /// Composition notes:
    /// </para>
    /// <list type="bullet">
    /// <item><description>Enable <see cref="JoinShapes.WhereBasedJoin"/> when you also use
    /// <c>SanctionedJoinPairsSqlScriptValidationRule</c> — the latter inspects only
    /// JOIN ON predicates, so without this flag a caller could bypass join-pair
    /// sanctioning via the comma-FROM form.</description></item>
    /// <item><description>Pair with <c>ColumnScopedSimpleConjunctiveFilterSqlScriptValidationRule</c>
    /// on the joined columns so that ON predicates with <c>OR</c> / <c>NOT</c> are
    /// rejected; this rule walks each leaf predicate independently, and an <c>OR</c>
    /// across two otherwise-valid leaves would otherwise look like two valid joins.</description></item>
    /// </list>
    /// </remarks>
    public partial class DisallowedJoinShapesSqlScriptValidationRule : SqlScriptValidationRuleBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DisallowedJoinShapesSqlScriptValidationRule"/> class.
        /// </summary>
        /// <param name="disallowedShapes">The shapes to flag.  Bitwise-OR together the
        /// <see cref="JoinShapes"/> values to enable.  Must include at least one shape
        /// (otherwise the rule does nothing).</param>
        /// <param name="id">OPTIONAL identifier.  DEFAULT is no identifier.</param>
        public DisallowedJoinShapesSqlScriptValidationRule(
            JoinShapes disallowedShapes,
            string id = null)
            : base(id)
        {
            new { disallowedShapes }.AsArg().Must().NotBeEqualTo(JoinShapes.None);

            this.DisallowedShapes = disallowedShapes;
        }

        /// <summary>
        /// Gets the disallowed join shapes — the flags enabled in the rule's configuration.
        /// </summary>
        public JoinShapes DisallowedShapes { get; private set; }
    }
}
