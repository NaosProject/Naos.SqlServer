// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AllowOnlySchemasSqlScriptValidationRule.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System.Collections.Generic;
    using OBeautifulCode.Assertion.Recipes;

    /// <summary>
    /// A rule that requires every explicitly named schema in a SQL script to appear in a
    /// sanctioned allow-list.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This rule fires only on schemas that are <em>explicitly named</em> in the script (e.g.
    /// <c>dbo</c> in <c>SELECT * FROM dbo.t</c>).  Bare references such as
    /// <c>SELECT * FROM my_table</c> are NOT flagged, even though at runtime they resolve
    /// against the executing principal's default schema — which could be a schema outside the
    /// sanctioned list.
    /// </para>
    /// <para>
    /// Enforcing schema qualification is a separate concern from enforcing an allow-list and
    /// lives in a dedicated rule (e.g. a <c>DisallowUnqualifiedObjectReferences…</c> rule).
    /// Apply both when both policies are needed: this rule closes the "wrong schema was named"
    /// hole; the other closes the "no schema was named" hole.
    /// </para>
    /// <para>
    /// An empty <see cref="SanctionedSchemas"/> collection is not supported — the constructor
    /// rejects it.  If you want to forbid every named schema, use
    /// <see cref="DisallowSchemasSqlScriptValidationRule"/> with the specific schemas you want
    /// to block, or write a custom rule.
    /// </para>
    /// </remarks>
    public partial class AllowOnlySchemasSqlScriptValidationRule : SqlScriptValidationRuleBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AllowOnlySchemasSqlScriptValidationRule"/> class.
        /// </summary>
        /// <param name="sanctionedSchemas">The sanctioned schemas.</param>
        /// <param name="id">OPTIONAL identifier.  DEFAULT is no identifier.</param>
        public AllowOnlySchemasSqlScriptValidationRule(
            IReadOnlyCollection<string> sanctionedSchemas,
            string id = null)
            : base(id)
        {
            new { sanctionedSchemas }.AsArg().Must().NotBeNullNorEmptyEnumerableNorContainAnyNulls().And().Each().NotBeNullNorWhiteSpace();

            this.SanctionedSchemas = sanctionedSchemas;
        }

        /// <summary>
        /// Gets the sanctioned schemas.
        /// </summary>
        /// <remarks>
        /// Each schema reference encountered in the script is compared case-insensitively against this collection.
        /// </remarks>
        public IReadOnlyCollection<string> SanctionedSchemas { get; private set; }
    }
}
