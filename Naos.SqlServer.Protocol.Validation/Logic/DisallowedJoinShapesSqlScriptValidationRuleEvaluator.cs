// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisallowedJoinShapesSqlScriptValidationRuleEvaluator.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Protocol.Validation
{
    using System;
    using System.Collections.Generic;
    using Microsoft.SqlServer.TransactSql.ScriptDom;
    using Naos.SqlServer.Domain;
    using static System.FormattableString;

    /// <summary>
    /// Evaluates a <see cref="DisallowedJoinShapesSqlScriptValidationRule"/>.
    /// </summary>
    public class DisallowedJoinShapesSqlScriptValidationRuleEvaluator : SqlScriptValidationRuleEvaluatorBase
    {
        private const JoinShapes JoinTypeFlags =
            JoinShapes.InnerJoin
            | JoinShapes.LeftOuterJoin
            | JoinShapes.RightOuterJoin
            | JoinShapes.FullOuterJoin
            | JoinShapes.CrossJoin
            | JoinShapes.CrossApply
            | JoinShapes.OuterApply;

        private const JoinShapes OnClauseFlags =
            JoinShapes.ConstantOn
            | JoinShapes.LiteralInOn
            | JoinShapes.NonEqualityOn
            | JoinShapes.FunctionInOn;

        private readonly DisallowedJoinShapesSqlScriptValidationRule rule;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisallowedJoinShapesSqlScriptValidationRuleEvaluator"/> class.
        /// </summary>
        /// <param name="rule">The rule to evaluate.</param>
        public DisallowedJoinShapesSqlScriptValidationRuleEvaluator(
            DisallowedJoinShapesSqlScriptValidationRule rule)
            : base(rule)
        {
            this.rule = rule;
        }

        /// <inheritdoc />
        public override void Visit(
            QuerySpecification node)
        {
            if (node == null)
            {
                return;
            }

            var shapes = this.rule.DisallowedShapes;

            if (shapes.HasFlag(JoinShapes.SelfJoin))
            {
                this.CheckSelfJoin(node.FromClause);
            }

            // Single walk of the FROM tree dispatching join-type checks (InnerJoin,
            // LeftOuterJoin, RightOuterJoin, FullOuterJoin, CrossJoin, CrossApply,
            // OuterApply) based on which flags are enabled.
            if ((shapes & JoinTypeFlags) != JoinShapes.None)
            {
                this.CheckJoinTypes(node.FromClause);
            }

            // WhereBasedJoin and ImplicitCrossJoin both key off "comma-FROM" (FROM with
            // multiple top-level table references) and are mutually exclusive based on
            // whether the WHERE contains a column-on-column equality bridging the tables.
            if (shapes.HasFlag(JoinShapes.WhereBasedJoin) || shapes.HasFlag(JoinShapes.ImplicitCrossJoin))
            {
                this.CheckCommaFromShape(node);
            }

            // Per-ON-clause checks.  Walk every QualifiedJoin's SearchCondition.
            if ((shapes & OnClauseFlags) != JoinShapes.None)
            {
                this.WalkOnClauses(node.FromClause);
            }
        }

        private void CheckSelfJoin(
            FromClause fromClause)
        {
            if ((fromClause == null) || (fromClause.TableReferences == null))
            {
                return;
            }

            var seenTables = new Dictionary<string, NamedTableReference>(StringComparer.OrdinalIgnoreCase);

            foreach (var tableReference in fromClause.TableReferences)
            {
                this.CollectNamedTablesForSelfJoinCheck(tableReference, seenTables);
            }
        }

        private void CollectNamedTablesForSelfJoinCheck(
            TableReference tableReference,
            Dictionary<string, NamedTableReference> seenTables)
        {
            if (tableReference is NamedTableReference named)
            {
                if ((named.SchemaObject == null) || (named.SchemaObject.BaseIdentifier == null) || (named.SchemaObject.SchemaIdentifier == null))
                {
                    return;
                }

                var key = named.SchemaObject.SchemaIdentifier.Value + "." + named.SchemaObject.BaseIdentifier.Value;

                if (seenTables.ContainsKey(key))
                {
                    this.AddViolation(
                        named.StartOffset,
                        Invariant($"self-join: table {key} is referenced multiple times in the FROM clause"));
                }
                else
                {
                    seenTables[key] = named;
                }
            }
            else if (tableReference is QualifiedJoin qualifiedJoin)
            {
                this.CollectNamedTablesForSelfJoinCheck(qualifiedJoin.FirstTableReference, seenTables);
                this.CollectNamedTablesForSelfJoinCheck(qualifiedJoin.SecondTableReference, seenTables);
            }
            else if (tableReference is UnqualifiedJoin unqualifiedJoin)
            {
                this.CollectNamedTablesForSelfJoinCheck(unqualifiedJoin.FirstTableReference, seenTables);
                this.CollectNamedTablesForSelfJoinCheck(unqualifiedJoin.SecondTableReference, seenTables);
            }
            else if (tableReference is JoinParenthesisTableReference joinParen)
            {
                this.CollectNamedTablesForSelfJoinCheck(joinParen.Join, seenTables);
            }

            // Other table-reference types (QueryDerivedTable, etc.) are blocked by FlatQuery.
        }

        private void CheckJoinTypes(
            FromClause fromClause)
        {
            if ((fromClause == null) || (fromClause.TableReferences == null))
            {
                return;
            }

            foreach (var tableReference in fromClause.TableReferences)
            {
                this.FindAndCheckJoinTypes(tableReference);
            }
        }

        private void FindAndCheckJoinTypes(
            TableReference tableReference)
        {
            if (tableReference is QualifiedJoin qualifiedJoin)
            {
                this.CheckQualifiedJoinType(qualifiedJoin);
                this.FindAndCheckJoinTypes(qualifiedJoin.FirstTableReference);
                this.FindAndCheckJoinTypes(qualifiedJoin.SecondTableReference);
            }
            else if (tableReference is UnqualifiedJoin unqualifiedJoin)
            {
                this.CheckUnqualifiedJoinType(unqualifiedJoin);
                this.FindAndCheckJoinTypes(unqualifiedJoin.FirstTableReference);
                this.FindAndCheckJoinTypes(unqualifiedJoin.SecondTableReference);
            }
            else if (tableReference is JoinParenthesisTableReference joinParen)
            {
                this.FindAndCheckJoinTypes(joinParen.Join);
            }
        }

        private void CheckQualifiedJoinType(
            QualifiedJoin qualifiedJoin)
        {
            JoinShapes flag;
            string message;

            switch (qualifiedJoin.QualifiedJoinType)
            {
                case QualifiedJoinType.Inner:
                    flag = JoinShapes.InnerJoin;
                    message = "INNER JOIN not allowed";
                    break;
                case QualifiedJoinType.LeftOuter:
                    flag = JoinShapes.LeftOuterJoin;
                    message = "LEFT OUTER JOIN not allowed";
                    break;
                case QualifiedJoinType.RightOuter:
                    flag = JoinShapes.RightOuterJoin;
                    message = "RIGHT OUTER JOIN not allowed";
                    break;
                case QualifiedJoinType.FullOuter:
                    flag = JoinShapes.FullOuterJoin;
                    message = "FULL OUTER JOIN not allowed";
                    break;
                default:
                    return;
            }

            if (this.rule.DisallowedShapes.HasFlag(flag))
            {
                // Emit at the right-hand table — the table being attached via the join.
                // QualifiedJoin.StartOffset points at the LEFT-hand table, which is
                // misleading when the violation is about the join shape itself.
                var offset = (qualifiedJoin.SecondTableReference != null)
                    ? qualifiedJoin.SecondTableReference.StartOffset
                    : qualifiedJoin.StartOffset;

                this.AddViolation(offset, message);
            }
        }

        private void CheckUnqualifiedJoinType(
            UnqualifiedJoin unqualifiedJoin)
        {
            JoinShapes flag;
            string message;

            switch (unqualifiedJoin.UnqualifiedJoinType)
            {
                case UnqualifiedJoinType.CrossJoin:
                    flag = JoinShapes.CrossJoin;
                    message = "CROSS JOIN not allowed";
                    break;
                case UnqualifiedJoinType.CrossApply:
                    flag = JoinShapes.CrossApply;
                    message = "CROSS APPLY not allowed";
                    break;
                case UnqualifiedJoinType.OuterApply:
                    flag = JoinShapes.OuterApply;
                    message = "OUTER APPLY not allowed";
                    break;
                default:
                    return;
            }

            if (this.rule.DisallowedShapes.HasFlag(flag))
            {
                var offset = (unqualifiedJoin.SecondTableReference != null)
                    ? unqualifiedJoin.SecondTableReference.StartOffset
                    : unqualifiedJoin.StartOffset;

                this.AddViolation(offset, message);
            }
        }

        private void CheckCommaFromShape(
            QuerySpecification node)
        {
            var topLevel = node.FromClause?.TableReferences;

            if ((topLevel == null) || (topLevel.Count < 2))
            {
                return;
            }

            var bridgingBce = (node.WhereClause != null) && (node.WhereClause.SearchCondition != null)
                ? FindColumnOnColumnEqualityBce(node.WhereClause.SearchCondition)
                : null;

            if (bridgingBce != null)
            {
                if (this.rule.DisallowedShapes.HasFlag(JoinShapes.WhereBasedJoin))
                {
                    this.AddViolation(
                        bridgingBce.StartOffset,
                        "old-style WHERE-based join not allowed; use JOIN ... ON");
                }
            }
            else
            {
                if (this.rule.DisallowedShapes.HasFlag(JoinShapes.ImplicitCrossJoin))
                {
                    // Emit at the second top-level table reference — where the implicit
                    // cartesian product enters via the comma.
                    this.AddViolation(
                        topLevel[1].StartOffset,
                        "implicit cross join (comma-separated tables in FROM with no condition tying them together) not allowed");
                }
            }
        }

        private static BooleanComparisonExpression FindColumnOnColumnEqualityBce(
            BooleanExpression expression)
        {
            if (expression == null)
            {
                return null;
            }

            if (expression is BooleanBinaryExpression binary)
            {
                return FindColumnOnColumnEqualityBce(binary.FirstExpression)
                    ?? FindColumnOnColumnEqualityBce(binary.SecondExpression);
            }

            if (expression is BooleanNotExpression notExpression)
            {
                return FindColumnOnColumnEqualityBce(notExpression.Expression);
            }

            if (expression is BooleanParenthesisExpression paren)
            {
                return FindColumnOnColumnEqualityBce(paren.Expression);
            }

            if ((expression is BooleanComparisonExpression bce)
                && (bce.ComparisonType == BooleanComparisonType.Equals)
                && (bce.FirstExpression is ColumnReferenceExpression)
                && (bce.SecondExpression is ColumnReferenceExpression))
            {
                return bce;
            }

            return null;
        }

        private void WalkOnClauses(
            FromClause fromClause)
        {
            if ((fromClause == null) || (fromClause.TableReferences == null))
            {
                return;
            }

            foreach (var tableReference in fromClause.TableReferences)
            {
                this.WalkOnClausesIn(tableReference);
            }
        }

        private void WalkOnClausesIn(
            TableReference tableReference)
        {
            if (tableReference is QualifiedJoin qualifiedJoin)
            {
                if (qualifiedJoin.SearchCondition != null)
                {
                    this.CheckOnClause(qualifiedJoin.SearchCondition);
                }

                this.WalkOnClausesIn(qualifiedJoin.FirstTableReference);
                this.WalkOnClausesIn(qualifiedJoin.SecondTableReference);
            }
            else if (tableReference is UnqualifiedJoin unqualifiedJoin)
            {
                this.WalkOnClausesIn(unqualifiedJoin.FirstTableReference);
                this.WalkOnClausesIn(unqualifiedJoin.SecondTableReference);
            }
            else if (tableReference is JoinParenthesisTableReference joinParen)
            {
                this.WalkOnClausesIn(joinParen.Join);
            }
        }

        private void CheckOnClause(
            BooleanExpression searchCondition)
        {
            // ConstantOn: short-circuit if the whole ON has no column refs anywhere.  When
            // ConstantOn fires, the per-BCE checks are skipped — the constant predicate is
            // already the violation; emitting LiteralInOn/NonEqualityOn for `1 = 1` would
            // be noise.
            if (this.rule.DisallowedShapes.HasFlag(JoinShapes.ConstantOn))
            {
                if (!HasAnyColumnReference(searchCondition))
                {
                    this.AddViolation(
                        searchCondition.StartOffset,
                        "ON clause has no column references (constant condition)");

                    return;
                }
            }

            if (this.rule.DisallowedShapes.HasFlag(JoinShapes.LiteralInOn)
                || this.rule.DisallowedShapes.HasFlag(JoinShapes.NonEqualityOn)
                || this.rule.DisallowedShapes.HasFlag(JoinShapes.FunctionInOn))
            {
                this.WalkBoolForPredicateChecks(searchCondition);
            }
        }

        private void WalkBoolForPredicateChecks(
            BooleanExpression expression)
        {
            if (expression == null)
            {
                return;
            }

            if (expression is BooleanBinaryExpression binary)
            {
                this.WalkBoolForPredicateChecks(binary.FirstExpression);
                this.WalkBoolForPredicateChecks(binary.SecondExpression);
                return;
            }

            if (expression is BooleanNotExpression notExpression)
            {
                this.WalkBoolForPredicateChecks(notExpression.Expression);
                return;
            }

            if (expression is BooleanParenthesisExpression paren)
            {
                this.WalkBoolForPredicateChecks(paren.Expression);
                return;
            }

            if (expression is BooleanComparisonExpression bce)
            {
                this.CheckBceForShapeIssues(bce);
                return;
            }

            // Non-BCE leaf predicates that count as "non-equality" in ON.
            if (this.rule.DisallowedShapes.HasFlag(JoinShapes.NonEqualityOn))
            {
                if (expression is LikePredicate likePredicate)
                {
                    this.AddViolation(
                        likePredicate.StartOffset,
                        "non-equality predicate (LIKE) in ON; ON predicates should use equality (=)");
                }
                else if (expression is InPredicate inPredicate)
                {
                    this.AddViolation(
                        inPredicate.StartOffset,
                        "non-equality predicate (IN) in ON; ON predicates should use equality (=)");
                }
                else if (expression is BooleanTernaryExpression ternary)
                {
                    this.AddViolation(
                        ternary.StartOffset,
                        "non-equality predicate (BETWEEN) in ON; ON predicates should use equality (=)");
                }
                else if (expression is BooleanIsNullExpression isNull)
                {
                    this.AddViolation(
                        isNull.StartOffset,
                        "non-equality predicate (IS NULL) in ON; ON predicates should use equality (=)");
                }
            }
        }

        private void CheckBceForShapeIssues(
            BooleanComparisonExpression bce)
        {
            if (this.rule.DisallowedShapes.HasFlag(JoinShapes.NonEqualityOn))
            {
                if (bce.ComparisonType != BooleanComparisonType.Equals)
                {
                    this.AddViolation(
                        bce.StartOffset,
                        "non-equality operator in ON; ON predicates should use equality (=)");
                }
            }

            if (this.rule.DisallowedShapes.HasFlag(JoinShapes.LiteralInOn))
            {
                var firstIsColumn = bce.FirstExpression is ColumnReferenceExpression;
                var secondIsColumn = bce.SecondExpression is ColumnReferenceExpression;
                var firstIsLiteral = bce.FirstExpression is Literal;
                var secondIsLiteral = bce.SecondExpression is Literal;

                if ((firstIsColumn && secondIsLiteral) || (firstIsLiteral && secondIsColumn))
                {
                    this.AddViolation(
                        bce.StartOffset,
                        "literal value in ON clause comparison; ON predicates should reference columns on both sides");
                }
            }

            if (this.rule.DisallowedShapes.HasFlag(JoinShapes.FunctionInOn))
            {
                if ((bce.FirstExpression is FunctionCall) || (bce.SecondExpression is FunctionCall))
                {
                    this.AddViolation(
                        bce.StartOffset,
                        "function call in ON clause; use bare column references on both sides");
                }
            }
        }

        private static bool HasAnyColumnReference(
            BooleanExpression expression)
        {
            var visitor = new ContainsColumnReferenceVisitor();
            expression.Accept(visitor);
            return visitor.Found;
        }

        private sealed class ContainsColumnReferenceVisitor : TSqlFragmentVisitor
        {
            public bool Found { get; private set; }

            public override void Visit(
                ColumnReferenceExpression node)
            {
                this.Found = true;
            }
        }
    }
}
