// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisallowedJoinShapesSqlScriptValidationRule.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using OBeautifulCode.Assertion.Recipes;

    /// <summary>
    /// A rule that flags suspicious join shapes selected via a flags enum.  Each flag in
    /// <see cref="DisallowedShapes"/> enables a corresponding check; unselected shapes are
    /// not flagged.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The shapes recognized by the rule are:
    /// </para>
    /// <list type="bullet">
    /// <item><description><see cref="JoinShapeIssues.SelfJoin"/> — same physical table
    /// referenced more than once in the FROM tree.</description></item>
    /// <item><description><see cref="JoinShapeIssues.ConstantOn"/> — <c>JOIN ... ON</c> with
    /// no column references in the search condition (e.g. <c>ON 1 = 1</c>).</description></item>
    /// <item><description><see cref="JoinShapeIssues.CrossJoin"/> — explicit <c>CROSS JOIN</c>.</description></item>
    /// <item><description><see cref="JoinShapeIssues.WhereBasedJoin"/> — comma-FROM with a
    /// column-on-column WHERE predicate playing the role of a join condition.</description></item>
    /// <item><description><see cref="JoinShapeIssues.LiteralInOn"/> — <c>JOIN ... ON</c>
    /// comparison with a literal on one side.</description></item>
    /// <item><description><see cref="JoinShapeIssues.NonEqualityOn"/> — <c>JOIN ... ON</c>
    /// column-on-column comparison using a non-equality operator.</description></item>
    /// <item><description><see cref="JoinShapeIssues.FunctionInOn"/> — <c>JOIN ... ON</c>
    /// comparison with a function call on one side.</description></item>
    /// <item><description><see cref="JoinShapeIssues.ImplicitCrossJoin"/> — comma-FROM with
    /// no condition (in WHERE or otherwise) tying the tables together.</description></item>
    /// </list>
    /// <para>
    /// Flag detection is independent — a single query can produce violations for multiple
    /// flags.  The rule is conservative: it only flags shapes it can identify syntactically,
    /// without schema introspection.  Sub-selects and derived tables in joins are blocked
    /// elsewhere (<c>FlatQuerySqlScriptValidationRule</c>); this rule does not attempt to
    /// look inside them.
    /// </para>
    /// <para>
    /// Composition notes:
    /// </para>
    /// <list type="bullet">
    /// <item><description>Pair with <see cref="WhereBasedJoin"/> = enabled when you also use
    /// <c>SanctionedJoinPairsSqlScriptValidationRule</c> — the latter inspects only JOIN ON
    /// predicates, so without this flag a caller could bypass join-pair sanctioning via the
    /// comma-FROM form.</description></item>
    /// <item><description>Pair with <c>ColumnScopedSimpleConjunctiveFilterSqlScriptValidationRule</c>
    /// on the joined columns so that ON predicates with <c>OR</c> / <c>NOT</c> are rejected;
    /// this rule walks each leaf BCE independently and an <c>OR</c> across two valid leaves
    /// would otherwise look like two valid joins.</description></item>
    /// </list>
    /// </remarks>
    public partial class DisallowedJoinShapesSqlScriptValidationRule : SqlScriptValidationRuleBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DisallowedJoinShapesSqlScriptValidationRule"/> class.
        /// </summary>
        /// <param name="disallowedShapes">The shapes to flag.  Bitwise-OR together the
        /// <see cref="JoinShapeIssues"/> values to enable.  Must include at least one shape
        /// (otherwise the rule does nothing).</param>
        /// <param name="id">OPTIONAL identifier.  DEFAULT is no identifier.</param>
        public DisallowedJoinShapesSqlScriptValidationRule(
            JoinShapeIssues disallowedShapes,
            string id = null)
            : base(id)
        {
            new { disallowedShapes }.AsArg().Must().NotBeEqualTo(JoinShapeIssues.None);

            this.DisallowedShapes = disallowedShapes;
        }

        /// <summary>
        /// Gets the disallowed join shapes — the flags enabled in the rule's configuration.
        /// </summary>
        public JoinShapeIssues DisallowedShapes { get; private set; }
    }
}
