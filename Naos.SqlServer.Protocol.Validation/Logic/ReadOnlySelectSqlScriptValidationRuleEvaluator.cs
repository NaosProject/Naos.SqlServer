// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReadOnlySelectSqlScriptValidationRuleEvaluator.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Protocol.Validation
{
    using Microsoft.SqlServer.TransactSql.ScriptDom;
    using Naos.SqlServer.Domain;

    /// <summary>
    /// Evaluates a <see cref="ReadOnlySelectSqlScriptValidationRule"/>.
    /// </summary>
    public class ReadOnlySelectSqlScriptValidationRuleEvaluator : SqlScriptValidationRuleEvaluatorBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlySelectSqlScriptValidationRuleEvaluator"/> class.
        /// </summary>
        /// <param name="rule">The rule to evaluate.</param>
        public ReadOnlySelectSqlScriptValidationRuleEvaluator(
            ReadOnlySelectSqlScriptValidationRule rule)
            : base(rule)
        {
        }

        /// <inheritdoc />
        public override void Visit(
            TSqlScript node)
        {
            // The rule emits two distinct kinds of violation, both per top-level statement:
            //
            //   1. The statement is not a SelectStatement at all — INSERT, UPDATE, DELETE,
            //      MERGE, EXEC, SET, DECLARE, USE, CREATE/ALTER/DROP <anything>, etc.  IF /
            //      WHILE / BEGIN/END / TRY-CATCH wrappers are also flagged here, even when
            //      their bodies are themselves read-only SELECTs — the top-level statement IS
            //      the IfStatement / WhileStatement / BeginEndBlockStatement / TryCatchStatement,
            //      not the inner SELECT.  This is the strict interpretation of the rule's spec
            //      ("only read-only SELECT statements").
            //
            //   2. The statement IS a SelectStatement, but it writes — currently the only
            //      parser-detectable write form of SELECT is "SELECT ... INTO target".  Other
            //      conceivably-not-read-only behaviors (table hints that take write locks like
            //      UPDLOCK/XLOCK, calls to functions or procedures with side effects, dynamic
            //      SQL inside an inner function, etc.) are not detectable at the parser level
            //      and are out of scope for this rule.
            if (node == null)
            {
                return;
            }

            if (node.Batches == null)
            {
                return;
            }

            foreach (var batch in node.Batches)
            {
                if ((batch == null) || (batch.Statements == null))
                {
                    continue;
                }

                foreach (var statement in batch.Statements)
                {
                    if (statement == null)
                    {
                        continue;
                    }

                    if (statement is SelectStatement selectStatement)
                    {
                        if (selectStatement.Into != null)
                        {
                            this.AddViolation(
                                statement.StartOffset,
                                "SELECT INTO is not read-only");
                        }
                    }
                    else
                    {
                        this.AddViolation(
                            statement.StartOffset,
                            "statement is not a read-only SELECT statement");
                    }
                }
            }
        }
    }
}
