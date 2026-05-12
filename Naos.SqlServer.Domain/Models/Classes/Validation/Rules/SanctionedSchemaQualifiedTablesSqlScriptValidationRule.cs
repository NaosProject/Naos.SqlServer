// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SanctionedSchemaQualifiedTablesSqlScriptValidationRule.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System.Collections.Generic;
    using OBeautifulCode.Assertion.Recipes;

    /// <summary>
    /// A rule that requires every schema-qualified table reference in the SQL script to be in
    /// a sanctioned allow-list of <c>schema.table</c> tuples.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Each <c>NamedTableReference</c> with an explicit <c>SchemaIdentifier</c> is checked.
    /// The <c>(schema, table)</c> tuple must appear in the
    /// <see cref="SanctionedSchemaQualifiedTables"/> collection (compared case-insensitively
    /// on both parts).  Each non-matching reference is flagged.
    /// </para>
    /// <para>
    /// This rule fires only on schema-qualified references.  Bare references such as
    /// <c>SELECT * FROM my_table</c> are silently ignored — they neither match nor violate.
    /// To force every table to be schema-qualified (so this rule can evaluate it), compose
    /// with <c>SchemaQualifiedTableReferencesSqlScriptValidationRule</c>.  Together they
    /// guarantee that every table in the script is both qualified AND in the sanctioned set —
    /// the airtight version of "only query the tables explicitly declared as allowed".
    /// </para>
    /// <para>
    /// Constructs in scope (all <c>NamedTableReference</c> in ScriptDom):
    /// </para>
    /// <list type="bullet">
    /// <item><description>Tables in <c>SELECT … FROM</c> and any kind of <c>JOIN</c>.</description></item>
    /// <item><description>Targets of <c>INSERT INTO</c>, <c>UPDATE</c>, <c>DELETE FROM</c>,
    /// <c>MERGE INTO</c>, plus <c>MERGE … USING</c> sources.</description></item>
    /// <item><description>Tables referenced inside subquery / derived-table / CTE bodies.</description></item>
    /// </list>
    /// <para>
    /// Constructs NOT in scope (different AST nodes, would each need their own rule):
    /// </para>
    /// <list type="bullet">
    /// <item><description>Stored procedure invocations — <c>EXEC dbo.sp_foo</c> — modeled as
    /// <c>ProcedureReference</c>.</description></item>
    /// <item><description>DDL targets — <c>CREATE TABLE dbo.t</c>, <c>DROP TABLE dbo.t</c>,
    /// <c>ALTER TABLE</c>, <c>CREATE INDEX … ON</c> — modeled as a bare <c>SchemaObjectName</c>
    /// on the statement.</description></item>
    /// <item><description>Table-valued function calls in <c>FROM</c> — <c>SELECT * FROM dbo.fn()</c>
    /// — modeled as <c>SchemaObjectFunctionTableReference</c>.</description></item>
    /// <item><description><c>OPENROWSET</c>, <c>OPENQUERY</c>, <c>OPENDATASOURCE</c> — already
    /// handled by <c>DisallowAdHocDistributedQueriesSqlScriptValidationRule</c>.</description></item>
    /// </list>
    /// <para>
    /// Exemptions:
    /// </para>
    /// <list type="bullet">
    /// <item><description>Temp tables — names starting with <c>#</c> or <c>##</c> are exempt;
    /// they don't have a user-addressable schema.</description></item>
    /// <item><description>Table variables (<c>@var</c>) — parsed as a different AST node
    /// (<c>VariableTableReference</c>), not visited by this rule.</description></item>
    /// </list>
    /// <para>
    /// Views and tables are syntactically indistinguishable in the AST — both are
    /// <c>NamedTableReference</c>.  If you want a view to pass, include it in the sanctioned
    /// list using the same <c>schema.view_name</c> form.
    /// </para>
    /// <para>
    /// Known limitation — CTE name references.  Inside a script like
    /// <c>WITH cte AS (…) SELECT … FROM cte</c>, the <c>FROM cte</c> part is a bare
    /// <c>NamedTableReference</c>, which this rule skips (no schema to check against the
    /// allowlist).  No false positives, but also no enforcement on CTE references.  Pair with
    /// <c>FlatQuerySqlScriptValidationRule</c> if you want to disallow CTEs entirely.
    /// </para>
    /// </remarks>
    public partial class SanctionedSchemaQualifiedTablesSqlScriptValidationRule : SqlScriptValidationRuleBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SanctionedSchemaQualifiedTablesSqlScriptValidationRule"/> class.
        /// </summary>
        /// <param name="sanctionedSchemaQualifiedTables">The sanctioned schema-qualified tables.</param>
        /// <param name="id">OPTIONAL identifier.  DEFAULT is no identifier.</param>
        public SanctionedSchemaQualifiedTablesSqlScriptValidationRule(
            IReadOnlyCollection<SchemaQualifiedTableName> sanctionedSchemaQualifiedTables,
            string id = null)
            : base(id)
        {
            new { sanctionedSchemaQualifiedTables }.AsArg().Must().NotBeNullNorEmptyEnumerableNorContainAnyNulls();

            this.SanctionedSchemaQualifiedTables = sanctionedSchemaQualifiedTables;
        }

        /// <summary>
        /// Gets the sanctioned schema-qualified tables.  Each schema-qualified table reference
        /// encountered in the script is compared case-insensitively (on both schema and table
        /// parts) against this collection.
        /// </summary>
        public IReadOnlyCollection<SchemaQualifiedTableName> SanctionedSchemaQualifiedTables { get; private set; }
    }
}
