// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SingleStatementSqlScriptValidationRuleEvaluator.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Protocol.Validation
{
    using Microsoft.SqlServer.TransactSql.ScriptDom;
    using Naos.SqlServer.Domain;
    using static System.FormattableString;

    /// <summary>
    /// Evaluates a <see cref="SingleStatementSqlScriptValidationRule"/>.
    /// </summary>
    public class SingleStatementSqlScriptValidationRuleEvaluator : SqlScriptValidationRuleEvaluatorBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SingleStatementSqlScriptValidationRuleEvaluator"/> class.
        /// </summary>
        /// <param name="rule">The rule to evaluate.</param>
        public SingleStatementSqlScriptValidationRuleEvaluator(
            SingleStatementSqlScriptValidationRule rule)
            : base(rule)
        {
        }

        /// <inheritdoc />
        public override void Visit(
            TSqlScript node)
        {
            // A T-SQL script is zero-or-more batches, each holding zero-or-more top-level
            // statements (semicolons separate statements within a batch; GO separates batches).
            // This rule passes only when the total number of top-level statements across all
            // batches is exactly one.
            //
            // Inner statements inside a BEGIN/END block, an IF/WHILE/TRY-CATCH body, or a
            // CREATE PROCEDURE/FUNCTION/TRIGGER body do NOT count separately — they belong to
            // their single containing top-level statement (BeginEndBlockStatement, IfStatement,
            // WhileStatement, TryCatchStatement, CreateProcedureStatement, etc.).
            if (node == null)
            {
                return;
            }

            var statementCount = 0;
            TSqlStatement firstExtraStatement = null;

            if (node.Batches != null)
            {
                foreach (var batch in node.Batches)
                {
                    if ((batch == null) || (batch.Statements == null))
                    {
                        continue;
                    }

                    foreach (var statement in batch.Statements)
                    {
                        statementCount++;

                        if ((statementCount == 2) && (firstExtraStatement == null))
                        {
                            firstExtraStatement = statement;
                        }
                    }
                }
            }

            if (statementCount == 1)
            {
                return;
            }

            // For >1 statements, point at the second statement — the first "extra" the script
            // should not contain.  For 0 statements (e.g. a comment-only script), there is no
            // statement to point at, so fall back to the script's start.
            var offset = (firstExtraStatement != null)
                ? firstExtraStatement.StartOffset
                : node.StartOffset;

            this.AddViolation(
                offset,
                Invariant($"script must contain a single SQL statement; found {statementCount}"));
        }
    }
}
