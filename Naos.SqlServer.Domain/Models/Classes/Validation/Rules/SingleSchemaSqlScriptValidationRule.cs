// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SingleSchemaSqlScriptValidationRule.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    /// <summary>
    /// A rule that requires every explicitly named schema in a SQL script to be the same
    /// schema (compared case-insensitively).
    /// </summary>
    /// <remarks>
    /// <para>
    /// The first explicit schema reference encountered during AST traversal sets the
    /// "canonical" schema for the script.  Every subsequent reference to a different schema
    /// is flagged.  Schemas are compared case-insensitively, so <c>dbo</c>, <c>Dbo</c>, and
    /// <c>DBO</c> are treated as the same schema.
    /// </para>
    /// <para>
    /// Like the other schema-policy rules, this rule only inspects schemas that are
    /// <em>explicitly named</em> in the script.  Bare references such as
    /// <c>SELECT * FROM my_table</c> are silently ignored — they neither set the canonical
    /// schema nor count as violations.  Mixing schema-qualified and unqualified references in
    /// a single-schema script is therefore permitted (the unqualified ones still resolve at
    /// runtime against the executing principal's default schema, which is a separate concern).
    /// </para>
    /// <para>
    /// The rule emits one violation per non-canonical schema reference, at the start offset of
    /// the schema-qualified name.  Visit order is not always source-order: in particular, the
    /// outer SELECT's FROM clause is visited before a CTE body, so for a script like
    /// <c>WITH cte AS (SELECT … FROM dbo.t1) SELECT … FROM my_schema.t2</c> the canonical
    /// schema is <c>my_schema</c> (visited first via the outer FROM) and <c>dbo</c> (visited
    /// later inside the CTE body) is the one that's flagged.  The presence of multiple
    /// schemas is what's flagged; which one ends up "canonical" depends on visit order.
    /// </para>
    /// <para>
    /// Schema-level statements (<c>CREATE SCHEMA</c>, <c>DROP SCHEMA</c>, <c>ALTER SCHEMA</c>)
    /// and security statements (<c>GRANT ON SCHEMA::…</c>) contribute schema references too —
    /// a script that <c>CREATE SCHEMA my_schema; SELECT * FROM dbo.t</c> uses two schemas and
    /// is flagged.
    /// </para>
    /// <para>
    /// Composes naturally with the other schema-policy rules — apply
    /// <c>DisallowedSchemas</c>, <c>DisallowSystemSchemas</c>, or <c>SanctionedSchemas</c>
    /// alongside this rule to constrain <em>which</em> single schema is allowed, in addition
    /// to enforcing that it's only one.
    /// </para>
    /// </remarks>
    public partial class SingleSchemaSqlScriptValidationRule : SqlScriptValidationRuleBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SingleSchemaSqlScriptValidationRule"/> class.
        /// </summary>
        /// <param name="id">OPTIONAL identifier.  DEFAULT is no identifier.</param>
        public SingleSchemaSqlScriptValidationRule(
            string id = null)
            : base(id)
        {
        }
    }
}
