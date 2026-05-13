// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColumnScopedSimpleConjunctiveFilterSqlScriptValidationRuleEvaluator.cs" company="Naos Project">
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
    /// Evaluates a <see cref="ColumnScopedSimpleConjunctiveFilterSqlScriptValidationRule"/>.
    /// </summary>
    public class ColumnScopedSimpleConjunctiveFilterSqlScriptValidationRuleEvaluator : FilterPredicateSqlScriptValidationRuleEvaluatorBase
    {
        private readonly ColumnScopedSimpleConjunctiveFilterSqlScriptValidationRule rule;

        // Index of configured columns by column name (case-insensitive).  Used both to
        // narrow full-identity matches in HandleResolvedFilterPredicate (via the values),
        // and to detect bare-name matches in HandleUnresolvedBareColumnReference (via the
        // keys).  A single column name might map to multiple full identities if the same
        // name is configured for several tables — hence List<SchemaQualifiedColumnName>.
        private readonly Dictionary<string, List<SchemaQualifiedColumnName>> columnsByName;

        // Per-QuerySpec state — reset at the start of each Visit(QuerySpecification).
        // Holds the set of configured columns (rendered as schema.table.column strings)
        // that were referenced in the current query's filter clauses, so the eventual
        // violation message can name them.
        private SortedSet<string> referencedConstrainedColumns;

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnScopedSimpleConjunctiveFilterSqlScriptValidationRuleEvaluator"/> class.
        /// </summary>
        /// <param name="rule">The rule to evaluate.</param>
        public ColumnScopedSimpleConjunctiveFilterSqlScriptValidationRuleEvaluator(
            ColumnScopedSimpleConjunctiveFilterSqlScriptValidationRule rule)
            : base(rule)
        {
            this.rule = rule;

            this.columnsByName = new Dictionary<string, List<SchemaQualifiedColumnName>>(StringComparer.OrdinalIgnoreCase);

            foreach (var column in rule.Columns)
            {
                if (!this.columnsByName.TryGetValue(column.ColumnName, out var list))
                {
                    list = new List<SchemaQualifiedColumnName>();
                    this.columnsByName[column.ColumnName] = list;
                }

                list.Add(column);
            }
        }

        /// <inheritdoc />
        public override void Visit(
            QuerySpecification node)
        {
            // Reset per-query state before the base walks the FROM/WHERE/HAVING/JOINs.
            this.referencedConstrainedColumns = new SortedSet<string>(StringComparer.Ordinal);

            base.Visit(node);
        }

        /// <inheritdoc />
        protected override void HandleResolvedFilterPredicate(
            SchemaQualifiedColumnName column,
            FilterOperator op,
            IReadOnlyList<ScalarExpression> values,
            int offset)
        {
            var match = this.FindMatchingConfiguredColumn(column);

            if (match != null)
            {
                this.referencedConstrainedColumns.Add(match.ToString());
            }
        }

        /// <inheritdoc />
        protected override void HandleUnresolvedBareColumnReference(
            string columnName,
            FilterOperator op,
            int offset)
        {
            // In a multi-table FROM, a bare column reference cannot be resolved to a specific
            // table without schema introspection.  If the bare name matches one or more
            // configured columns' names, the rule cannot prove the bare ref ISN'T one of
            // them — err on the safe side and record ALL configured columns with that name.
            // No "must be qualified" violation is emitted here; that's the auth rules' job.
            if (this.columnsByName.TryGetValue(columnName, out var matches))
            {
                foreach (var match in matches)
                {
                    this.referencedConstrainedColumns.Add(match.ToString());
                }
            }
        }

        /// <inheritdoc />
        protected override void OnQuerySpecificationComplete(
            QuerySpecification node,
            IReadOnlyDictionary<string, SchemaQualifiedTableName> aliasMap)
        {
            if (this.referencedConstrainedColumns.Count == 0)
            {
                return;
            }

            var referencedClause = BuildReferencedConstrainedColumnsClause(this.referencedConstrainedColumns);

            // The query references at least one configured column — enforce conjunctive
            // shape on all filter clauses in this scope.
            this.CheckForDisjunctionOrNegation(node.WhereClause?.SearchCondition, referencedClause);
            this.CheckForDisjunctionOrNegation(node.HavingClause?.SearchCondition, referencedClause);
            this.WalkJoinSearchConditionsForDisjunctionOrNegation(node.FromClause, referencedClause);
        }

        private SchemaQualifiedColumnName FindMatchingConfiguredColumn(
            SchemaQualifiedColumnName column)
        {
            if (!this.columnsByName.TryGetValue(column.ColumnName, out var candidates))
            {
                return null;
            }

            foreach (var candidate in candidates)
            {
                if (string.Equals(candidate.SchemaName, column.SchemaName, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(candidate.TableName, column.TableName, StringComparison.OrdinalIgnoreCase))
                {
                    return candidate;
                }
            }

            return null;
        }

        private static string BuildReferencedConstrainedColumnsClause(
            SortedSet<string> referencedConstrainedColumns)
        {
            var joined = string.Join(", ", referencedConstrainedColumns);

            return (referencedConstrainedColumns.Count == 1)
                ? Invariant($"constrained column {joined}")
                : Invariant($"constrained columns {joined}");
        }

        private void CheckForDisjunctionOrNegation(
            BooleanExpression expression,
            string referencedClause)
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
                    this.AddViolation(binary.StartOffset, Invariant($"OR not allowed in filter referencing {referencedClause}; filter must be a simple conjunction"));
                    return;
                }

                this.CheckForDisjunctionOrNegation(binary.FirstExpression, referencedClause);
                this.CheckForDisjunctionOrNegation(binary.SecondExpression, referencedClause);
            }
            else if (expression is BooleanNotExpression notExpression)
            {
                this.AddViolation(notExpression.StartOffset, Invariant($"NOT not allowed in filter referencing {referencedClause}; filter must be a simple conjunction"));
            }
            else if (expression is BooleanParenthesisExpression paren)
            {
                this.CheckForDisjunctionOrNegation(paren.Expression, referencedClause);
            }

            // Other leaf predicate types — no violation.
        }

        private void WalkJoinSearchConditionsForDisjunctionOrNegation(
            FromClause fromClause,
            string referencedClause)
        {
            if ((fromClause == null) || (fromClause.TableReferences == null))
            {
                return;
            }

            foreach (var tableReference in fromClause.TableReferences)
            {
                this.WalkTableReferenceForJoinDisjunctionOrNegation(tableReference, referencedClause);
            }
        }

        private void WalkTableReferenceForJoinDisjunctionOrNegation(
            TableReference tableReference,
            string referencedClause)
        {
            if (tableReference is QualifiedJoin qualifiedJoin)
            {
                if (qualifiedJoin.SearchCondition != null)
                {
                    this.CheckForDisjunctionOrNegation(qualifiedJoin.SearchCondition, referencedClause);
                }

                this.WalkTableReferenceForJoinDisjunctionOrNegation(qualifiedJoin.FirstTableReference, referencedClause);
                this.WalkTableReferenceForJoinDisjunctionOrNegation(qualifiedJoin.SecondTableReference, referencedClause);
            }
            else if (tableReference is JoinParenthesisTableReference joinParen)
            {
                this.WalkTableReferenceForJoinDisjunctionOrNegation(joinParen.Join, referencedClause);
            }

            // Other table-reference types do not have ON clauses.
        }
    }
}
