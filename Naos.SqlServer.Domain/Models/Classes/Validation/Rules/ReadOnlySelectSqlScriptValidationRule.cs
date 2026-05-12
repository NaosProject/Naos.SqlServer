// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReadOnlySelectSqlScriptValidationRule.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    /// <summary>
    /// A rule that requires every top-level statement in the SQL script to be a read-only
    /// <c>SELECT</c> statement.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Each top-level statement must satisfy both conditions:
    /// </para>
    /// <list type="bullet">
    /// <item><description>It is a <c>SelectStatement</c> in the AST.  Non-SELECT statements are
    /// flagged: DML writes (<c>INSERT</c> / <c>UPDATE</c> / <c>DELETE</c> / <c>MERGE</c>), DDL
    /// (<c>CREATE</c> / <c>ALTER</c> / <c>DROP</c> anything), <c>EXEC</c>, <c>SET</c>,
    /// <c>DECLARE</c>, <c>USE</c>, etc.</description></item>
    /// <item><description>It is not a <c>SELECT … INTO target</c> — that form creates and
    /// writes to the target table and is not read-only.</description></item>
    /// </list>
    /// <para>
    /// <b>Control-flow wrappers are flagged.</b>  <c>IF</c>, <c>WHILE</c>, <c>BEGIN/END</c>,
    /// and <c>TRY/CATCH</c> are not themselves <c>SelectStatement</c> instances, so they fail
    /// the rule <em>even when their bodies contain only read-only SELECTs</em> (e.g.
    /// <c>IF EXISTS (...) SELECT ...</c>).  This is a strict reading of "only read-only
    /// SELECT statements"; if more permissive semantics are needed, apply or write a different
    /// rule.
    /// </para>
    /// <para>
    /// Some notionally-not-read-only behaviors are NOT detectable at the parser level and are
    /// out of scope:
    /// </para>
    /// <list type="bullet">
    /// <item><description>Table hints that take write-style locks (<c>WITH (UPDLOCK)</c>,
    /// <c>WITH (XLOCK)</c>, <c>WITH (TABLOCKX)</c>) — these are still <c>SelectStatement</c>
    /// nodes in the AST and pass.</description></item>
    /// <item><description>Calls to functions or stored procedures that have side effects,
    /// including CLR functions and <c>OPENROWSET</c> / <c>OPENQUERY</c> with a write payload —
    /// the parser cannot inspect what those targets do.</description></item>
    /// <item><description>Dynamic SQL strings (e.g. inside <c>EXEC sp_executesql</c>) — but
    /// those are <c>ExecuteStatement</c> nodes, not <c>SelectStatement</c>, so they get
    /// flagged on the non-SELECT path rather than the SELECT-but-not-read-only path.</description></item>
    /// </list>
    /// <para>
    /// Variable assignment in a SELECT (e.g. <c>SELECT @x = col FROM t</c>) IS permitted — it
    /// is still a <c>SelectStatement</c>, and it writes to local variable state, not to a table.
    /// </para>
    /// </remarks>
    public partial class ReadOnlySelectSqlScriptValidationRule : SqlScriptValidationRuleBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlySelectSqlScriptValidationRule"/> class.
        /// </summary>
        /// <param name="id">OPTIONAL identifier.  DEFAULT is no identifier.</param>
        public ReadOnlySelectSqlScriptValidationRule(
            string id = null)
            : base(id)
        {
        }
    }
}
