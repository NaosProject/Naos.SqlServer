// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SchemaQualifiedTableReferencesSqlScriptValidationRule.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    /// <summary>
    /// A rule that requires every table reference in the SQL script to be schema-qualified.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Each <c>NamedTableReference</c> in the AST must carry a non-null <c>SchemaIdentifier</c>
    /// — that is, every reference must be written as <c>schema.table</c> (or
    /// <c>database.schema.table</c> / <c>server.database.schema.table</c>), not as a bare
    /// <c>table</c>.  The rule fires once per offending reference, at the reference's
    /// start offset.
    /// </para>
    /// <para>
    /// Constructs in scope (all of these are <c>NamedTableReference</c> in ScriptDom):
    /// </para>
    /// <list type="bullet">
    /// <item><description>Tables in <c>SELECT … FROM</c> and any kind of <c>JOIN</c>.</description></item>
    /// <item><description>Targets of <c>INSERT INTO</c>, <c>UPDATE</c>, <c>DELETE FROM</c>,
    /// <c>MERGE INTO</c>, plus <c>MERGE … USING</c> sources.</description></item>
    /// <item><description>Tables referenced inside subquery / derived-table bodies and inside
    /// CTE bodies.</description></item>
    /// </list>
    /// <para>
    /// Constructs NOT in scope (these are different AST nodes and would each need their own
    /// rule):
    /// </para>
    /// <list type="bullet">
    /// <item><description>Stored procedure invocations — <c>EXEC sp_help</c> — modeled as
    /// <c>ProcedureReference</c>, not <c>NamedTableReference</c>.</description></item>
    /// <item><description>DDL targets — <c>CREATE TABLE x</c>, <c>DROP TABLE x</c>,
    /// <c>ALTER TABLE x</c>, <c>CREATE INDEX … ON x</c>, etc. — modeled as a bare
    /// <c>SchemaObjectName</c> on the statement, not as a <c>NamedTableReference</c>.</description></item>
    /// <item><description>Table-valued function calls in <c>FROM</c> — <c>SELECT * FROM dbo.fn()</c>
    /// — modeled as <c>SchemaObjectFunctionTableReference</c>.</description></item>
    /// <item><description><c>OPENROWSET</c>, <c>OPENQUERY</c>, <c>OPENDATASOURCE</c> — already
    /// handled by <c>DisallowAdHocDistributedQueriesSqlScriptValidationRule</c>.</description></item>
    /// </list>
    /// <para>
    /// Exemptions:
    /// </para>
    /// <list type="bullet">
    /// <item><description>Temp tables — names starting with <c>#</c> (local) or <c>##</c> (global)
    /// are tempdb-resident, do not have a user-addressable schema, and T-SQL convention is to
    /// write them without any schema prefix.  Bare references to such names are exempt.</description></item>
    /// <item><description>Table variables (<c>@var</c>) — parsed as a different AST node
    /// (<c>VariableTableReference</c>), not visited by this rule.</description></item>
    /// </list>
    /// <para>
    /// Known limitation — CTE name references.  Inside a script like
    /// <c>WITH cte AS (…) SELECT … FROM cte</c>, the <c>FROM cte</c> part is parsed as a
    /// <c>NamedTableReference</c> with no schema, identical in shape to a real bare table
    /// reference.  The parser does not know "cte" is a CTE alias; only semantic analysis does.
    /// This rule will therefore flag <c>cte</c> as unqualified.  The recommended composition
    /// is to apply <c>FlatQuerySqlScriptValidationRule</c> alongside this one — which blocks
    /// CTEs entirely — so the limitation never bites in practice.
    /// </para>
    /// <para>
    /// Designed to compose with <c>SanctionedSchemasSqlScriptValidationRule</c>: together they
    /// guarantee that every table reference in the script is both schema-qualified AND
    /// references a sanctioned schema, closing the "ignored unspecified schema" loophole that
    /// <c>SanctionedSchemas</c> alone leaves open.
    /// </para>
    /// </remarks>
    public partial class SchemaQualifiedTableReferencesSqlScriptValidationRule : SqlScriptValidationRuleBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SchemaQualifiedTableReferencesSqlScriptValidationRule"/> class.
        /// </summary>
        /// <param name="id">OPTIONAL identifier.  DEFAULT is no identifier.</param>
        public SchemaQualifiedTableReferencesSqlScriptValidationRule(
            string id = null)
            : base(id)
        {
        }
    }
}
