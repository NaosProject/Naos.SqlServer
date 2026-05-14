// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SanctionedJoinPairsSqlScriptValidationRuleEvaluator.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Protocol.Validation
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.SqlServer.TransactSql.ScriptDom;
    using Naos.CodeAnalysis.Recipes;
    using Naos.SqlServer.Domain;
    using static System.FormattableString;

    /// <summary>
    /// Evaluates a <see cref="SanctionedJoinPairsSqlScriptValidationRule"/>.
    /// </summary>
    public class SanctionedJoinPairsSqlScriptValidationRuleEvaluator : SqlScriptValidationRuleEvaluatorBase
    {
        // Canonical, case-insensitive set of sanctioned pair keys.  Each pair is keyed by
        // the two columns' canonical strings, sorted ordinal-insensitive so that (A, B) and
        // (B, A) yield the same key.
        private readonly HashSet<string> sanctionedPairKeys;

        // Case-insensitive set of canonical column strings that appear in any sanctioned
        // pair.  Used to distinguish "uncovered" joins (skip silently) from "constrained
        // column in unsanctioned pair" (violation).
        private readonly HashSet<string> constrainedColumnKeys;

        // For violation messages: map from constrained column key (lowercased ToString) back
        // to the human-readable rendering supplied in config.
        private readonly Dictionary<string, string> columnKeyToDisplay;

        /// <summary>
        /// Initializes a new instance of the <see cref="SanctionedJoinPairsSqlScriptValidationRuleEvaluator"/> class.
        /// </summary>
        /// <param name="rule">The rule to evaluate.</param>
        public SanctionedJoinPairsSqlScriptValidationRuleEvaluator(
            SanctionedJoinPairsSqlScriptValidationRule rule)
            : base(rule)
        {
            this.sanctionedPairKeys = new HashSet<string>(StringComparer.Ordinal);
            this.constrainedColumnKeys = new HashSet<string>(StringComparer.Ordinal);
            this.columnKeyToDisplay = new Dictionary<string, string>(StringComparer.Ordinal);

            foreach (var pair in rule.SanctionedJoinPairs)
            {
                var leftKey = ColumnKey(pair.LeftColumn);
                var rightKey = ColumnKey(pair.RightColumn);

                this.sanctionedPairKeys.Add(MakePairKey(leftKey, rightKey));

                this.constrainedColumnKeys.Add(leftKey);
                this.constrainedColumnKeys.Add(rightKey);

                this.columnKeyToDisplay[leftKey] = pair.LeftColumn.ToString();
                this.columnKeyToDisplay[rightKey] = pair.RightColumn.ToString();
            }
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

            this.WalkJoinSearchConditions(node.FromClause, aliasMap);
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
                    this.WalkOnExpression(qualifiedJoin.SearchCondition, aliasMap);
                }

                this.WalkTableReferenceForJoinConditions(qualifiedJoin.FirstTableReference, aliasMap);
                this.WalkTableReferenceForJoinConditions(qualifiedJoin.SecondTableReference, aliasMap);
            }
            else if (tableReference is JoinParenthesisTableReference joinParen)
            {
                this.WalkTableReferenceForJoinConditions(joinParen.Join, aliasMap);
            }

            // Other table-reference types (NamedTableReference, UnqualifiedJoin/CROSS JOIN,
            // QueryDerivedTable) do not contribute ON clauses to inspect.
        }

        private void WalkOnExpression(
            BooleanExpression expression,
            Dictionary<string, SchemaQualifiedTableName> aliasMap)
        {
            // Descend through AND / OR / NOT / parentheses to reach each leaf BCE.  We walk
            // through OR / NOT even though they're a smell because the user composes this
            // rule with ColumnScopedSimpleConjunctiveFilter to reject them; walking through
            // here keeps us robust if that rule isn't configured.
            if (expression == null)
            {
                return;
            }

            if (expression is BooleanBinaryExpression binary)
            {
                this.WalkOnExpression(binary.FirstExpression, aliasMap);
                this.WalkOnExpression(binary.SecondExpression, aliasMap);
            }
            else if (expression is BooleanNotExpression notExpression)
            {
                this.WalkOnExpression(notExpression.Expression, aliasMap);
            }
            else if (expression is BooleanParenthesisExpression paren)
            {
                this.WalkOnExpression(paren.Expression, aliasMap);
            }
            else if (expression is BooleanComparisonExpression bce)
            {
                this.CheckBce(bce, aliasMap);
            }

            // Other leaf predicate types (LIKE, IN, BETWEEN, IS NULL) are not joins.
        }

        private void CheckBce(
            BooleanComparisonExpression bce,
            Dictionary<string, SchemaQualifiedTableName> aliasMap)
        {
            // Only equality is a "join pair" — non-equality joins are someone else's job
            // (DisallowedJoinShapes.NonEqualityOn).
            if (bce.ComparisonType != BooleanComparisonType.Equals)
            {
                return;
            }

            // Both sides must be column references.  Literal / function / expression on
            // either side means this isn't a join through a pair (DisallowedJoinShapes
            // handles those shapes).
            var leftColumn = ResolveColumn(bce.FirstExpression, aliasMap);
            var rightColumn = ResolveColumn(bce.SecondExpression, aliasMap);

            if ((leftColumn == null) || (rightColumn == null))
            {
                return;
            }

            var leftKey = ColumnKey(leftColumn);
            var rightKey = ColumnKey(rightColumn);

            var pairKey = MakePairKey(leftKey, rightKey);

            if (this.sanctionedPairKeys.Contains(pairKey))
            {
                return;
            }

            // Pair is not sanctioned.  If neither column appears in any sanctioned pair,
            // the rule doesn't cover this join — skip.
            var leftConstrained = this.constrainedColumnKeys.Contains(leftKey);
            var rightConstrained = this.constrainedColumnKeys.Contains(rightKey);

            if (!leftConstrained && !rightConstrained)
            {
                return;
            }

            this.AddViolation(
                bce.StartOffset,
                Invariant($"join pair ({leftColumn} <-> {rightColumn}) is not sanctioned"));
        }

        private static SchemaQualifiedColumnName ResolveColumn(
            ScalarExpression expression,
            Dictionary<string, SchemaQualifiedTableName> aliasMap)
        {
            var columnRef = expression as ColumnReferenceExpression;

            if ((columnRef == null) || (columnRef.MultiPartIdentifier == null))
            {
                return null;
            }

            var identifiers = columnRef.MultiPartIdentifier.Identifiers;

            if ((identifiers == null) || (identifiers.Count == 0))
            {
                return null;
            }

            var columnName = identifiers[identifiers.Count - 1].Value;

            if (identifiers.Count == 1)
            {
                // Bare reference.  In a single-table FROM, resolves unambiguously; in a
                // multi-table FROM, we can't tell which table it belongs to without schema
                // introspection — skip.  (Joins are by definition multi-table, but the
                // visitor still walks single-table queries that have no joins — those won't
                // reach this code path because there are no ON clauses to inspect.)
                if (aliasMap.Count == 1)
                {
                    var sole = default(SchemaQualifiedTableName);
                    foreach (var t in aliasMap.Values)
                    {
                        sole = t;
                        break;
                    }

                    return new SchemaQualifiedColumnName(sole.SchemaName, sole.TableName, columnName);
                }

                return null;
            }

            if (identifiers.Count == 2)
            {
                if (aliasMap.TryGetValue(identifiers[0].Value, out var schemaTable))
                {
                    return new SchemaQualifiedColumnName(schemaTable.SchemaName, schemaTable.TableName, columnName);
                }

                return null;
            }

            // 3-part (schema.table.column) or 4-part (db.schema.table.column).
            var schema = identifiers[identifiers.Count - 3].Value;
            var table = identifiers[identifiers.Count - 2].Value;

            return new SchemaQualifiedColumnName(schema, table, columnName);
        }

        [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "Prefer lower case here.")]
        private static string ColumnKey(
            SchemaQualifiedColumnName column)
        {
            // Lowercased canonical form for case-insensitive equality.  Comparable across
            // any combination of schema/table/column casings the AST might present.
            return column.ToString().ToLowerInvariant();
        }

        private static string MakePairKey(
            string leftKey,
            string rightKey)
        {
            // Canonical order: smaller string first.  Makes the pair key order-agnostic so
            // (A, B) and (B, A) yield the same key.
            return (string.CompareOrdinal(leftKey, rightKey) <= 0)
                ? leftKey + "|" + rightKey
                : rightKey + "|" + leftKey;
        }
    }
}
