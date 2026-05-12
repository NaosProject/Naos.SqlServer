// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SingleStatementSqlScriptValidationRule.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    /// <summary>
    /// A rule that requires the SQL script to consist of exactly one top-level SQL statement.
    /// </summary>
    /// <remarks>
    /// <para>
    /// "Top-level" means the statement appears directly in a batch — not nested inside another
    /// statement.  ScriptDom models a script as zero-or-more batches (separated by <c>GO</c>),
    /// each containing zero-or-more statements (separated by <c>;</c>).  This rule sums those
    /// counts across all batches and passes only when the total is exactly one.
    /// </para>
    /// <para>
    /// Statements nested inside a containing top-level statement do NOT count separately —
    /// the rule passes for, e.g.:
    /// </para>
    /// <list type="bullet">
    /// <item><description>A <c>BEGIN/END</c> block with multiple inner statements — the
    /// outer block is one top-level statement.</description></item>
    /// <item><description>An <c>IF</c>, <c>WHILE</c>, or <c>TRY/CATCH</c> with a
    /// multi-statement body — each is a single top-level statement.</description></item>
    /// <item><description>A <c>CREATE PROCEDURE</c> / <c>CREATE FUNCTION</c> /
    /// <c>CREATE TRIGGER</c> whose body contains many inner statements — the
    /// <c>CREATE …</c> statement itself is one top-level statement.</description></item>
    /// <item><description>A <c>MERGE … ;</c> — the trailing semicolon is a terminator
    /// (mandatory for <c>MERGE</c> in T-SQL), not a second statement.</description></item>
    /// </list>
    /// <para>
    /// A script that parses to zero top-level statements (e.g. a comment-only script) is also
    /// a violation — "single" means exactly one, not at most one.
    /// </para>
    /// </remarks>
    public partial class SingleStatementSqlScriptValidationRule : SqlScriptValidationRuleBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SingleStatementSqlScriptValidationRule"/> class.
        /// </summary>
        /// <param name="id">OPTIONAL identifier.  DEFAULT is no identifier.</param>
        public SingleStatementSqlScriptValidationRule(
            string id = null)
            : base(id)
        {
        }
    }
}
