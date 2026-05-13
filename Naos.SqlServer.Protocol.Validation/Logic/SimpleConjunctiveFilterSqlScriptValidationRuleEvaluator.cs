// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SimpleConjunctiveFilterSqlScriptValidationRuleEvaluator.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Protocol.Validation
{
    using Microsoft.SqlServer.TransactSql.ScriptDom;
    using Naos.SqlServer.Domain;

    /// <summary>
    /// Evaluates a <see cref="SimpleConjunctiveFilterSqlScriptValidationRule"/>.
    /// </summary>
    public class SimpleConjunctiveFilterSqlScriptValidationRuleEvaluator : SqlScriptValidationRuleEvaluatorBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleConjunctiveFilterSqlScriptValidationRuleEvaluator"/> class.
        /// </summary>
        /// <param name="rule">The rule to evaluate.</param>
        public SimpleConjunctiveFilterSqlScriptValidationRuleEvaluator(
            SimpleConjunctiveFilterSqlScriptValidationRule rule)
            : base(rule)
        {
        }

        /// <inheritdoc />
        public override void Visit(
            WhereClause node)
        {
            // WHERE clause search condition.
            this.CheckForDisjunctionOrNegation(node?.SearchCondition);
        }

        /// <inheritdoc />
        public override void Visit(
            HavingClause node)
        {
            // HAVING clause search condition.
            this.CheckForDisjunctionOrNegation(node?.SearchCondition);
        }

        /// <inheritdoc />
        public override void Visit(
            QualifiedJoin node)
        {
            // JOIN ... ON search condition.
            this.CheckForDisjunctionOrNegation(node?.SearchCondition);
        }

        private void CheckForDisjunctionOrNegation(
            BooleanExpression expression)
        {
            // Walk the boolean tree.  Leaf predicates (BooleanComparisonExpression,
            // LikePredicate, InPredicate, BooleanTernaryExpression, BooleanIsNullExpression)
            // are fine and terminate recursion.  AND connectors keep us in pure conjunction
            // territory.  OR connectors and explicit NOT wrappers are violations.
            //
            // Parenthesized expressions are transparent — we recurse through them.
            //
            // The negated forms encoded inline within their predicate types (<>, !=, NOT
            // LIKE, NOT IN, NOT BETWEEN, IS NOT NULL) are leaves, NOT BooleanNotExpression
            // wrappers, so they pass.
            if (expression == null)
            {
                return;
            }

            if (expression is BooleanBinaryExpression binary)
            {
                if (binary.BinaryExpressionType == BooleanBinaryExpressionType.Or)
                {
                    this.AddViolation(binary.StartOffset, "OR not allowed in filter; filter must be a simple conjunction");
                    return;
                }

                this.CheckForDisjunctionOrNegation(binary.FirstExpression);
                this.CheckForDisjunctionOrNegation(binary.SecondExpression);
            }
            else if (expression is BooleanNotExpression notExpression)
            {
                this.AddViolation(notExpression.StartOffset, "NOT not allowed in filter; filter must be a simple conjunction");
            }
            else if (expression is BooleanParenthesisExpression paren)
            {
                this.CheckForDisjunctionOrNegation(paren.Expression);
            }

            // Other leaf predicate types — no violation.
        }
    }
}
