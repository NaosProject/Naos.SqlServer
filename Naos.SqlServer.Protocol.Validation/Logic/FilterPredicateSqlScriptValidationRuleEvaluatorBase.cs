// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilterPredicateSqlScriptValidationRuleEvaluatorBase.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Protocol.Validation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.SqlServer.TransactSql.ScriptDom;
    using Naos.SqlServer.Domain;

    /// <summary>
    /// Base class for a SQL script validation rule evaluator that observes individual filter
    /// predicates (in WHERE / HAVING / ON clauses), decomposing each into a normalized
    /// <c>(column, operator, values)</c> triple and resolving column references against the
    /// FROM-clause alias map.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Each <c>QuerySpecification</c> in the AST is processed independently — for a nested
    /// query (subquery, CTE body, derived table, set-op branch) the alias map and filter
    /// walk are local to that scope.  When composed with <c>FlatQuerySqlScriptValidationRule</c>,
    /// there is only one such scope per script.
    /// </para>
    /// <para>
    /// For each query specification the base class:
    /// </para>
    /// <list type="number">
    /// <item><description>Walks the <c>FromClause</c> and builds an alias map
    /// <c>alias → SchemaQualifiedTableName</c>.  Both explicit aliases (<c>FROM dbo.users u</c>)
    /// and implicit aliases (the table's base name when no <c>AS</c> is present) are
    /// captured.  Comparisons are case-insensitive.</description></item>
    /// <item><description>Walks the WHERE / HAVING <c>SearchCondition</c>s and the
    /// <c>SearchCondition</c> of every <c>QualifiedJoin</c> in the FROM clause, descending
    /// through <c>AND</c> / <c>OR</c> / <c>NOT</c> / parenthesized expressions to reach each
    /// leaf predicate.</description></item>
    /// <item><description>Decomposes each leaf predicate into a normalized triple and
    /// resolves the column reference(s) through the alias map.  Calls
    /// <see cref="HandleResolvedFilterPredicate"/> for each resolved column reference.</description></item>
    /// </list>
    /// <para>
    /// Supported predicate types and how they decompose:
    /// </para>
    /// <list type="bullet">
    /// <item><description><c>BooleanComparisonExpression</c> (<c>=</c>, <c>!=</c>, <c>&lt;&gt;</c>,
    /// <c>&lt;</c>, <c>&gt;</c>, <c>&lt;=</c>, <c>&gt;=</c>, <c>!&lt;</c>, <c>!&gt;</c>): when one
    /// side is a column reference and the other is anything, emits one predicate for the
    /// column with the OTHER side as the single value.  For column-on-each-side comparisons
    /// (JOIN-style), emits ONE predicate per side, with the operator reversed on the second
    /// side as needed (<c>&lt;</c> becomes <c>&gt;</c> when viewed from the right column's
    /// perspective).</description></item>
    /// <item><description><c>LikePredicate</c>: emits one predicate with the LHS column,
    /// operator <see cref="FilterOperator.Like"/> or <see cref="FilterOperator.NotLike"/>,
    /// values <c>= [pattern]</c>.</description></item>
    /// <item><description><c>InPredicate</c>: emits one predicate with the subject column,
    /// operator <see cref="FilterOperator.In"/> or <see cref="FilterOperator.NotIn"/>,
    /// values <c>= [v1, v2, ...]</c>.  <c>IN (SELECT ...)</c> subquery form is skipped
    /// (already blocked by <c>FlatQuery</c>).</description></item>
    /// <item><description><c>BooleanTernaryExpression</c> (<c>BETWEEN low AND high</c>): emits
    /// one predicate with the subject column, operator <see cref="FilterOperator.Between"/>
    /// or <see cref="FilterOperator.NotBetween"/>, values <c>= [low, high]</c>.</description></item>
    /// <item><description><c>BooleanIsNullExpression</c>: emits one predicate with the
    /// subject column, operator <see cref="FilterOperator.IsNull"/> or
    /// <see cref="FilterOperator.IsNotNull"/>, values <c>= []</c>.</description></item>
    /// </list>
    /// <para>
    /// Column reference resolution:
    /// </para>
    /// <list type="bullet">
    /// <item><description><b>3- or 4-part name</b> (<c>[schema, table, column]</c> or
    /// <c>[db, schema, table, column]</c>) — resolves directly using the middle two
    /// identifiers.</description></item>
    /// <item><description><b>2-part name</b> (<c>[alias, column]</c>) — looks up the first
    /// identifier in the alias map; if found, resolves; if not found, silently skipped
    /// (would be a runtime error anyway).</description></item>
    /// <item><description><b>1-part name</b> in a single-table FROM — unambiguously resolves
    /// to the single table in scope.</description></item>
    /// <item><description><b>1-part name</b> in a multi-table FROM — cannot be resolved
    /// without schema introspection; the base class invokes
    /// <see cref="HandleUnresolvedBareColumnReference"/> instead.  Concrete rules override
    /// that method to emit a "must be table-qualified" violation iff the bare column name
    /// matches one of the rule's configured columns.</description></item>
    /// </list>
    /// </remarks>
    public abstract class FilterPredicateSqlScriptValidationRuleEvaluatorBase : SqlScriptValidationRuleEvaluatorBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FilterPredicateSqlScriptValidationRuleEvaluatorBase"/> class.
        /// </summary>
        /// <param name="rule">The rule to evaluate.</param>
        protected FilterPredicateSqlScriptValidationRuleEvaluatorBase(
            SqlScriptValidationRuleBase rule)
            : base(rule)
        {
        }

        /// <inheritdoc />
        public override void Visit(
            QuerySpecification node)
        {
            if (node == null)
            {
                return;
            }

            var aliasMap = FromClauseAliasMapBuilder.Build(node.FromClause);

            if ((node.WhereClause != null) && (node.WhereClause.SearchCondition != null))
            {
                this.WalkBooleanExpression(node.WhereClause.SearchCondition, aliasMap);
            }

            if ((node.HavingClause != null) && (node.HavingClause.SearchCondition != null))
            {
                this.WalkBooleanExpression(node.HavingClause.SearchCondition, aliasMap);
            }

            this.WalkJoinSearchConditions(node.FromClause, aliasMap);

            // Hook for rules that need a per-query callback (e.g. "filter on column X is
            // required" — fires AFTER all predicates have been observed, with the
            // accumulated state and the alias map so rules can determine whether their
            // constrained column is even in scope for this query).  Default no-op.
            this.OnQuerySpecificationComplete(node, aliasMap);
        }

        /// <summary>
        /// Called once per filter predicate where the column reference resolved to a
        /// fully-qualified <see cref="SchemaQualifiedColumnName"/>.
        /// </summary>
        /// <param name="column">The resolved column.</param>
        /// <param name="op">The filter operator.</param>
        /// <param name="values">
        /// The value expressions on the value-side of the comparison.  Single-element for
        /// binary comparisons / LIKE; multi-element for IN; two-element [low, high] for
        /// BETWEEN; empty for IS NULL / IS NOT NULL.
        /// </param>
        /// <param name="offset">The start offset of the originating predicate, suitable for
        /// reporting violations.</param>
        protected abstract void HandleResolvedFilterPredicate(
            SchemaQualifiedColumnName column,
            FilterOperator op,
            IReadOnlyList<ScalarExpression> values,
            int offset);

        /// <summary>
        /// Called when a bare column reference appears in a multi-table FROM and cannot be
        /// resolved to a specific table without schema introspection.  Default no-op;
        /// concrete rules should override to emit a "must be table-qualified" violation
        /// when the bare column name matches a configured column the rule cares about.
        /// </summary>
        /// <param name="columnName">The bare column name as it appeared in the source.</param>
        /// <param name="op">The filter operator applied to the bare column.</param>
        /// <param name="offset">The start offset of the originating predicate.</param>
        protected virtual void HandleUnresolvedBareColumnReference(
            string columnName,
            FilterOperator op,
            int offset)
        {
            // Default: do nothing.  Each concrete rule decides whether to emit a violation
            // based on whether the bare name matches one of its configured columns.
        }

        /// <summary>
        /// Called once after every predicate in the current <c>QuerySpecification</c> has
        /// been observed.  Default no-op; concrete rules override to fire violations that
        /// require accumulated state across the whole query (e.g. "no filter on column X
        /// was found anywhere").
        /// </summary>
        /// <param name="node">The query specification that was just walked.</param>
        /// <param name="aliasMap">The alias map built for this query specification —
        /// useful to determine whether a constrained column's table is in scope.</param>
        protected virtual void OnQuerySpecificationComplete(
            QuerySpecification node,
            IReadOnlyDictionary<string, SchemaQualifiedTableName> aliasMap)
        {
        }

        private void WalkJoinSearchConditions(
            FromClause fromClause,
            Dictionary<string, SchemaQualifiedTableName> aliasMap)
        {
            if ((fromClause == null) || (fromClause.TableReferences == null))
            {
                return;
            }

            foreach (var tableReference in fromClause.TableReferences)
            {
                this.WalkTableReferenceForJoinConditions(tableReference, aliasMap);
            }
        }

        private void WalkTableReferenceForJoinConditions(
            TableReference tableReference,
            Dictionary<string, SchemaQualifiedTableName> aliasMap)
        {
            if (tableReference is QualifiedJoin qualifiedJoin)
            {
                if (qualifiedJoin.SearchCondition != null)
                {
                    this.WalkBooleanExpression(qualifiedJoin.SearchCondition, aliasMap);
                }

                this.WalkTableReferenceForJoinConditions(qualifiedJoin.FirstTableReference, aliasMap);
                this.WalkTableReferenceForJoinConditions(qualifiedJoin.SecondTableReference, aliasMap);
            }
            else if (tableReference is JoinParenthesisTableReference joinParen)
            {
                this.WalkTableReferenceForJoinConditions(joinParen.Join, aliasMap);
            }

            // Other table-reference types do not have ON clauses.
        }

        private void WalkBooleanExpression(
            BooleanExpression expression,
            Dictionary<string, SchemaQualifiedTableName> aliasMap)
        {
            if (expression == null)
            {
                return;
            }

            if (expression is BooleanBinaryExpression binary)
            {
                this.WalkBooleanExpression(binary.FirstExpression, aliasMap);
                this.WalkBooleanExpression(binary.SecondExpression, aliasMap);
            }
            else if (expression is BooleanNotExpression not)
            {
                this.WalkBooleanExpression(not.Expression, aliasMap);
            }
            else if (expression is BooleanParenthesisExpression paren)
            {
                this.WalkBooleanExpression(paren.Expression, aliasMap);
            }
            else
            {
                this.DecomposeLeafPredicate(expression, aliasMap);
            }
        }

        private void DecomposeLeafPredicate(
            BooleanExpression expression,
            Dictionary<string, SchemaQualifiedTableName> aliasMap)
        {
            if (expression is BooleanComparisonExpression comparison)
            {
                this.HandleComparison(comparison, aliasMap);
            }
            else if (expression is LikePredicate like)
            {
                this.HandleLike(like, aliasMap);
            }
            else if (expression is InPredicate inPredicate)
            {
                this.HandleIn(inPredicate, aliasMap);
            }
            else if (expression is BooleanTernaryExpression ternary)
            {
                this.HandleBetween(ternary, aliasMap);
            }
            else if (expression is BooleanIsNullExpression isNull)
            {
                this.HandleIsNull(isNull, aliasMap);
            }

            // Other leaf predicate types (FullTextPredicate, ExistsPredicate,
            // SubqueryComparisonPredicate, etc.) are out of scope for this base.  ExistsPredicate
            // and SubqueryComparisonPredicate are blocked by FlatQuery anyway.
        }

        private void HandleComparison(
            BooleanComparisonExpression comparison,
            Dictionary<string, SchemaQualifiedTableName> aliasMap)
        {
            var op = MapComparisonType(comparison.ComparisonType);

            if (op == FilterOperator.Unknown)
            {
                return;
            }

            var firstColumn = comparison.FirstExpression as ColumnReferenceExpression;
            var secondColumn = comparison.SecondExpression as ColumnReferenceExpression;

            if (firstColumn != null)
            {
                this.TryEmitForColumn(firstColumn, op, new[] { comparison.SecondExpression }, comparison.StartOffset, aliasMap);
            }

            if (secondColumn != null)
            {
                // Reverse the operator when emitting from the right-hand column's perspective
                // (e.g. "a < b" means a is less than b, but from b's perspective, b is greater
                // than a).
                this.TryEmitForColumn(secondColumn, ReverseDirectionalOperator(op), new[] { comparison.FirstExpression }, comparison.StartOffset, aliasMap);
            }
        }

        private void HandleLike(
            LikePredicate like,
            Dictionary<string, SchemaQualifiedTableName> aliasMap)
        {
            var columnRef = like.FirstExpression as ColumnReferenceExpression;

            if (columnRef == null)
            {
                return;
            }

            var op = like.NotDefined ? FilterOperator.NotLike : FilterOperator.Like;
            var values = (like.SecondExpression != null) ? new[] { like.SecondExpression } : Array.Empty<ScalarExpression>();

            this.TryEmitForColumn(columnRef, op, values, like.StartOffset, aliasMap);
        }

        private void HandleIn(
            InPredicate inPredicate,
            Dictionary<string, SchemaQualifiedTableName> aliasMap)
        {
            // IN (SELECT ...) subquery form is out of scope here; FlatQuery blocks it
            // independently.  Only handle the value-list form.
            if (inPredicate.Subquery != null)
            {
                return;
            }

            var columnRef = inPredicate.Expression as ColumnReferenceExpression;

            if (columnRef == null)
            {
                return;
            }

            var op = inPredicate.NotDefined ? FilterOperator.NotIn : FilterOperator.In;
            var values = (IReadOnlyList<ScalarExpression>)(inPredicate.Values ?? (IList<ScalarExpression>)Array.Empty<ScalarExpression>());

            this.TryEmitForColumn(columnRef, op, values, inPredicate.StartOffset, aliasMap);
        }

        private void HandleBetween(
            BooleanTernaryExpression ternary,
            Dictionary<string, SchemaQualifiedTableName> aliasMap)
        {
            var columnRef = ternary.FirstExpression as ColumnReferenceExpression;

            if (columnRef == null)
            {
                return;
            }

            FilterOperator op;

            switch (ternary.TernaryExpressionType)
            {
                case BooleanTernaryExpressionType.Between:
                    op = FilterOperator.Between;
                    break;
                case BooleanTernaryExpressionType.NotBetween:
                    op = FilterOperator.NotBetween;
                    break;
                default:
                    return;
            }

            var values = new[] { ternary.SecondExpression, ternary.ThirdExpression };

            this.TryEmitForColumn(columnRef, op, values, ternary.StartOffset, aliasMap);
        }

        private void HandleIsNull(
            BooleanIsNullExpression isNull,
            Dictionary<string, SchemaQualifiedTableName> aliasMap)
        {
            var columnRef = isNull.Expression as ColumnReferenceExpression;

            if (columnRef == null)
            {
                return;
            }

            var op = isNull.IsNot ? FilterOperator.IsNotNull : FilterOperator.IsNull;

            this.TryEmitForColumn(columnRef, op, Array.Empty<ScalarExpression>(), isNull.StartOffset, aliasMap);
        }

        private void TryEmitForColumn(
            ColumnReferenceExpression columnRef,
            FilterOperator op,
            IReadOnlyList<ScalarExpression> values,
            int offset,
            Dictionary<string, SchemaQualifiedTableName> aliasMap)
        {
            if ((columnRef == null) || (columnRef.MultiPartIdentifier == null))
            {
                return;
            }

            var identifiers = columnRef.MultiPartIdentifier.Identifiers;

            if ((identifiers == null) || (identifiers.Count == 0))
            {
                return;
            }

            var columnName = identifiers[identifiers.Count - 1].Value;

            if (identifiers.Count == 1)
            {
                // Bare reference.  In a single-table query, resolves to that table.  In a
                // multi-table query, cannot be resolved without schema introspection.
                if (aliasMap.Count == 1)
                {
                    var soleTable = aliasMap.Values.First();
                    var resolved = new SchemaQualifiedColumnName(soleTable.SchemaName, soleTable.TableName, columnName);
                    this.HandleResolvedFilterPredicate(resolved, op, values, offset);
                }
                else
                {
                    this.HandleUnresolvedBareColumnReference(columnName, op, offset);
                }

                return;
            }

            if (identifiers.Count == 2)
            {
                // alias.column or table-name.column — look up first identifier in the alias map.
                if (aliasMap.TryGetValue(identifiers[0].Value, out var schemaTable))
                {
                    var resolved = new SchemaQualifiedColumnName(schemaTable.SchemaName, schemaTable.TableName, columnName);
                    this.HandleResolvedFilterPredicate(resolved, op, values, offset);
                }

                // If the alias key isn't in the map, the script would runtime-error anyway —
                // skip silently rather than synthesize a violation.
                return;
            }

            // 3-part [schema, table, column] or 4-part [db, schema, table, column].
            var schema = identifiers[identifiers.Count - 3].Value;
            var table = identifiers[identifiers.Count - 2].Value;
            var resolvedFromQualified = new SchemaQualifiedColumnName(schema, table, columnName);
            this.HandleResolvedFilterPredicate(resolvedFromQualified, op, values, offset);
        }

        private static FilterOperator MapComparisonType(
            BooleanComparisonType comparisonType)
        {
            switch (comparisonType)
            {
                case BooleanComparisonType.Equals:
                    return FilterOperator.Equal;
                case BooleanComparisonType.NotEqualToBrackets:
                case BooleanComparisonType.NotEqualToExclamation:
                    return FilterOperator.NotEqual;
                case BooleanComparisonType.LessThan:
                    return FilterOperator.LessThan;
                case BooleanComparisonType.GreaterThan:
                    return FilterOperator.GreaterThan;
                case BooleanComparisonType.LessThanOrEqualTo:
                case BooleanComparisonType.NotGreaterThan:
                    return FilterOperator.LessThanOrEqual;
                case BooleanComparisonType.GreaterThanOrEqualTo:
                case BooleanComparisonType.NotLessThan:
                    return FilterOperator.GreaterThanOrEqual;
                default:
                    // LeftOuterJoin / RightOuterJoin are deprecated *= and =* outer-join
                    // syntax that we don't need to model in filter rules.  IsDistinctFrom /
                    // IsNotDistinctFrom are uncommon.
                    return FilterOperator.Unknown;
            }
        }

        private static FilterOperator ReverseDirectionalOperator(
            FilterOperator op)
        {
            switch (op)
            {
                case FilterOperator.LessThan:
                    return FilterOperator.GreaterThan;
                case FilterOperator.GreaterThan:
                    return FilterOperator.LessThan;
                case FilterOperator.LessThanOrEqual:
                    return FilterOperator.GreaterThanOrEqual;
                case FilterOperator.GreaterThanOrEqual:
                    return FilterOperator.LessThanOrEqual;
                default:
                    // Equal / NotEqual are symmetric; non-directional operators don't reverse.
                    return op;
            }
        }
    }
}
