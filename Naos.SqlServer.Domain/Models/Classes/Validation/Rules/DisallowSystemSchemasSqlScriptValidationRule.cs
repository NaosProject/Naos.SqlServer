// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisallowSystemSchemasSqlScriptValidationRule.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    /// <summary>
    /// A rule that disallows explicit references to the system schemas <c>sys</c> and
    /// <c>INFORMATION_SCHEMA</c> in a SQL script.
    /// </summary>
    /// <remarks>
    /// This is a "disallow system schemas" rule, not a "disallow system objects" rule.  It
    /// flags occurrences of the literal schema names <c>sys</c> and <c>INFORMATION_SCHEMA</c>
    /// as they appear in the script's AST — anywhere those names are syntactically present
    /// as a schema qualifier (table references, function calls, column references, DDL targets,
    /// security statements, schema-level statements, etc.).
    /// <para>
    /// It does NOT flag usages of system constructs that SQL Server resolves to the <c>sys</c>
    /// schema implicitly, because the literal schema name does not appear in the script.  In
    /// particular, the following are NOT considered violations of this rule even though they
    /// effectively use the <c>sys</c> schema at runtime:
    /// </para>
    /// <list type="bullet">
    /// <item><description>System stored procedure invocations without a schema qualifier
    /// (e.g. <c>EXEC sp_help</c>, <c>EXEC sp_executesql</c>, <c>EXEC sp_who</c>,
    /// <c>EXEC sp_rename</c>).</description></item>
    /// <item><description>Built-in / system functions called without a schema qualifier
    /// (e.g. <c>GETDATE()</c>, <c>OBJECT_ID(...)</c>, <c>DB_NAME()</c>, <c>SUSER_NAME()</c>,
    /// <c>SERVERPROPERTY(...)</c>).</description></item>
    /// <item><description>Backwards-compatibility views referenced without a schema qualifier
    /// (e.g. <c>sysobjects</c>, <c>syscolumns</c>, <c>sysindexes</c>) — these are bare table
    /// names in the AST with no <c>SchemaIdentifier</c>.</description></item>
    /// <item><description>DBCC commands (e.g. <c>DBCC CHECKDB</c>, <c>DBCC SQLPERF</c>) —
    /// these are statements, not schema-qualified references.</description></item>
    /// <item><description>Global / system variables (e.g. <c>@@VERSION</c>,
    /// <c>@@ROWCOUNT</c>) — these are not schema references at all.</description></item>
    /// </list>
    /// <para>
    /// If detection of those implicit usages is required, use a separate rule (e.g. one that
    /// targets a curated allowlist of bare system procedure / function / view names) — that
    /// is a fundamentally different policy and does not belong here.
    /// </para>
    /// </remarks>
    public partial class DisallowSystemSchemasSqlScriptValidationRule : SqlScriptValidationRuleBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DisallowSystemSchemasSqlScriptValidationRule"/> class.
        /// </summary>
        /// <param name="id">OPTIONAL identifier.  DEFAULT is no identifier.</param>
        public DisallowSystemSchemasSqlScriptValidationRule(
            string id = null)
            : base(id)
        {
        }
    }
}
