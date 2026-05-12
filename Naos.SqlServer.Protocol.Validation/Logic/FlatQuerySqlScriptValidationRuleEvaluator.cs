// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FlatQuerySqlScriptValidationRuleEvaluator.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Protocol.Validation
{
    using Microsoft.SqlServer.TransactSql.ScriptDom;
    using Naos.SqlServer.Domain;

    /// <summary>
    /// Evaluates a <see cref="FlatQuerySqlScriptValidationRule"/>.
    /// </summary>
    public class FlatQuerySqlScriptValidationRuleEvaluator : SqlScriptValidationRuleEvaluatorBase
    {
        private int querySpecificationCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="FlatQuerySqlScriptValidationRuleEvaluator"/> class.
        /// </summary>
        /// <param name="rule">The rule to evaluate.</param>
        public FlatQuerySqlScriptValidationRuleEvaluator(
            FlatQuerySqlScriptValidationRule rule)
            : base(rule)
        {
        }

        /// <inheritdoc />
        public override void Visit(
            QuerySpecification node)
        {
            // Every nesting construct in T-SQL — CTE body, set-op branch, derived table,
            // scalar subquery, EXISTS / IN / ANY / ALL / SOME subquery, APPLY body — produces
            // an additional QuerySpecification in the AST.  The rule counts them and fires
            // once when the second one is encountered.  JOINs, window functions (OVER clauses),
            // CASE expressions, VALUES inline-derived-tables, GROUP BY / HAVING / ORDER BY /
            // TOP / OFFSET-FETCH, and view references all stay within a single QuerySpecification
            // and therefore pass.
            if (node == null)
            {
                return;
            }

            this.querySpecificationCount++;

            if (this.querySpecificationCount == 2)
            {
                this.AddViolation(node.StartOffset, "query is not flat (nested query scope)");
            }
        }
    }
}
