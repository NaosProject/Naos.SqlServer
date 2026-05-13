// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BenchmarkingFilterValuesByColumnSqlScriptValidationRuleEvaluator.cs" company="Naos Project">
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
    using static System.FormattableString;

    /// <summary>
    /// Evaluates a <see cref="BenchmarkingFilterValuesByColumnSqlScriptValidationRule"/>.
    /// </summary>
    public class BenchmarkingFilterValuesByColumnSqlScriptValidationRuleEvaluator : FilterPredicateSqlScriptValidationRuleEvaluatorBase
    {
        private readonly BenchmarkingFilterValuesByColumnSqlScriptValidationRule rule;

        // Case-sensitive set of owned values.  Entity ids are typically opaque external
        // identifiers (UUIDs, GUIDs, etc.) where case matters; mismatched case = different id.
        private readonly HashSet<string> ownedValues;

        // Per-QuerySpec state — reset at the start of each Visit(QuerySpecification).
        private List<ConstrainedColumnFilter> constrainedColumnFilters;

        // Set true when we encountered a bare reference in a multi-table FROM whose name
        // matches the constrained column.  Used to suppress the "filter on constrained column
        // is required" violation in OnQuerySpecificationComplete — the bare reference may
        // well have been the required filter, just unresolvable.  Emitting both violations
        // for the same underlying issue is noisy.
        private bool sawBareReferenceMatchingConstrainedColumnName;

        /// <summary>
        /// Initializes a new instance of the <see cref="BenchmarkingFilterValuesByColumnSqlScriptValidationRuleEvaluator"/> class.
        /// </summary>
        /// <param name="rule">The rule to evaluate.</param>
        public BenchmarkingFilterValuesByColumnSqlScriptValidationRuleEvaluator(
            BenchmarkingFilterValuesByColumnSqlScriptValidationRule rule)
            : base(rule)
        {
            this.rule = rule;
            this.ownedValues = new HashSet<string>(rule.OwnedValues, StringComparer.Ordinal);
        }

        /// <inheritdoc />
        public override void Visit(
            QuerySpecification node)
        {
            // Reset per-query state before the base walks the FROM/WHERE/HAVING/JOINs.
            this.constrainedColumnFilters = new List<ConstrainedColumnFilter>();
            this.sawBareReferenceMatchingConstrainedColumnName = false;

            base.Visit(node);
        }

        /// <inheritdoc />
        protected override void HandleResolvedFilterPredicate(
            SchemaQualifiedColumnName column,
            FilterOperator op,
            IReadOnlyList<ScalarExpression> values,
            int offset)
        {
            if (!IsSameColumn(column, this.rule.Column))
            {
                return;
            }

            this.constrainedColumnFilters.Add(new ConstrainedColumnFilter(op, values, offset));
        }

        /// <inheritdoc />
        protected override void HandleUnresolvedBareColumnReference(
            string columnName,
            FilterOperator op,
            int offset)
        {
            // Emit "must be qualified" only when the bare column name matches the constrained
            // column's name — the bare ref MIGHT be the constrained column, but without
            // table qualification the rule cannot tell.
            if (string.Equals(columnName, this.rule.Column.ColumnName, StringComparison.OrdinalIgnoreCase))
            {
                this.sawBareReferenceMatchingConstrainedColumnName = true;

                this.AddViolation(
                    offset,
                    "column reference must be table-qualified in multi-table queries: " + columnName);
            }
        }

        /// <inheritdoc />
        protected override void OnQuerySpecificationComplete(
            QuerySpecification node,
            IReadOnlyDictionary<string, SchemaQualifiedTableName> aliasMap)
        {
            // Determine whether the constrained column's table is in scope for this query.
            // If it isn't, the column simply cannot be filtered on here — skip rather than
            // emit a spurious "filter required" violation in a subquery that doesn't touch
            // the constrained table.
            var constrainedTableInScope = aliasMap.Values.Any(t =>
                string.Equals(t.SchemaName, this.rule.Column.SchemaName, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(t.TableName, this.rule.Column.TableName, StringComparison.OrdinalIgnoreCase));

            if (!constrainedTableInScope)
            {
                return;
            }

            if (this.constrainedColumnFilters.Count == 0)
            {
                // Suppress the "filter required" violation if we already emitted a
                // "must be qualified" violation for a bare reference matching this column's
                // name — they're symptoms of the same underlying issue, and the user should
                // resolve the qualification first.
                if (this.rule.RequireFilterOnConstrainedColumn && !this.sawBareReferenceMatchingConstrainedColumnName)
                {
                    this.AddViolation(
                        node.StartOffset,
                        Invariant($"filter on constrained column {this.rule.Column} is required"));
                }

                return;
            }

            if (this.constrainedColumnFilters.Count > 1)
            {
                // Multiple filters on the constrained column — the intersection-of-filters
                // semantics would require solver-style analysis; the rule rejects rather than
                // approximate.  Emit at each extra filter's offset so the user sees every
                // redundant one.
                foreach (var extra in this.constrainedColumnFilters.Skip(1))
                {
                    this.AddViolation(
                        extra.Offset,
                        Invariant($"multiple filters on constrained column {this.rule.Column} are not allowed; only one filter is permitted"));
                }

                return;
            }

            // Exactly one filter on the constrained column — validate it.
            var pred = this.constrainedColumnFilters[0];

            var shape = ClassifyOperator(pred.Op);

            if (shape == FilterShape.Unsupported)
            {
                this.AddViolation(
                    pred.Offset,
                    Invariant($"filter operator {pred.Op} on constrained column {this.rule.Column} is not supported for benchmarking; use =, IN, <>, or NOT IN"));

                return;
            }

            // Extract values.  Each must be a literal constant — not a parameter, function
            // call, expression, or NULL — because the rule needs to enumerate values and
            // compare them against the owned set.
            var extractedValues = new List<string>();

            foreach (var valueExpression in pred.Values)
            {
                if (!TryExtractLiteralValue(valueExpression, out var literalValue, out var rejectionReason))
                {
                    this.AddViolation(
                        pred.Offset,
                        Invariant($"filter value on constrained column {this.rule.Column} must be a literal constant; found {rejectionReason}"));

                    return;
                }

                extractedValues.Add(literalValue);
            }

            var distinctValues = new HashSet<string>(extractedValues, StringComparer.Ordinal);

            var ownedCount = distinctValues.Count(v => this.ownedValues.Contains(v));
            var peerCount = distinctValues.Count - ownedCount;

            if (shape == FilterShape.Include)
            {
                this.ValidateIncludeShape(pred, distinctValues, ownedCount, peerCount);
            }
            else
            {
                this.ValidateExcludeShape(pred, peerCount);
            }
        }

        private void ValidateIncludeShape(
            ConstrainedColumnFilter pred,
            HashSet<string> distinctValues,
            int ownedCount,
            int peerCount)
        {
            if (peerCount == 0)
            {
                // (a) all owned — pass.
                return;
            }

            if (ownedCount > 0)
            {
                // Mixed — neither (a) nor (b) holds.
                this.AddViolation(
                    pred.Offset,
                    Invariant($"filter on constrained column {this.rule.Column} mixes owned and peer values; must be either all owned (your data) OR all peer with at least {this.rule.MinimumDistinctPeerValues} distinct values"));

                return;
            }

            // All peer — check the distinct-count threshold for (b).
            if (distinctValues.Count < this.rule.MinimumDistinctPeerValues)
            {
                this.AddViolation(
                    pred.Offset,
                    Invariant($"filter on constrained column {this.rule.Column} contains {distinctValues.Count} distinct peer value(s); minimum is {this.rule.MinimumDistinctPeerValues}"));

                return;
            }

            // (b) all peer AND sufficient distinct count — pass.
        }

        private void ValidateExcludeShape(
            ConstrainedColumnFilter pred,
            int peerCount)
        {
            if (peerCount == 0)
            {
                // (c) all owned — pass.  Caller is excluding their own entities to see the
                // peer universe for exploration.
                return;
            }

            // Any peer value present in an exclusion filter — fails, whether mixed
            // (some owned + some peer) or all-peer.  There is no NOT-IN equivalent of
            // (b): excluding a peer set still leaks all owned rows plus all un-listed
            // peer rows, so listing more peers makes the result bigger, not smaller.
            this.AddViolation(
                pred.Offset,
                Invariant($"exclusion filter (<> / NOT IN) on constrained column {this.rule.Column} must list only owned values; found peer value(s)"));
        }

        private static FilterShape ClassifyOperator(
            FilterOperator op)
        {
            switch (op)
            {
                case FilterOperator.Equal:
                case FilterOperator.In:
                    return FilterShape.Include;
                case FilterOperator.NotEqual:
                case FilterOperator.NotIn:
                    return FilterShape.Exclude;
                default:
                    return FilterShape.Unsupported;
            }
        }

        private static bool TryExtractLiteralValue(
            ScalarExpression expression,
            out string value,
            out string rejectionReason)
        {
            value = null;
            rejectionReason = null;

            if (expression is NullLiteral)
            {
                rejectionReason = "NULL";
                return false;
            }

            if (expression is Literal literal)
            {
                value = literal.Value;
                return true;
            }

            if (expression is VariableReference)
            {
                rejectionReason = "parameter";
                return false;
            }

            rejectionReason = expression.GetType().Name;
            return false;
        }

        private static bool IsSameColumn(
            SchemaQualifiedColumnName left,
            SchemaQualifiedColumnName right)
        {
            return string.Equals(left.SchemaName, right.SchemaName, StringComparison.OrdinalIgnoreCase)
                && string.Equals(left.TableName, right.TableName, StringComparison.OrdinalIgnoreCase)
                && string.Equals(left.ColumnName, right.ColumnName, StringComparison.OrdinalIgnoreCase);
        }

        private enum FilterShape
        {
            Unsupported,
            Include,
            Exclude,
        }

        private sealed class ConstrainedColumnFilter
        {
            public ConstrainedColumnFilter(
                FilterOperator op,
                IReadOnlyList<ScalarExpression> values,
                int offset)
            {
                this.Op = op;
                this.Values = values;
                this.Offset = offset;
            }

            public FilterOperator Op { get; }

            public IReadOnlyList<ScalarExpression> Values { get; }

            public int Offset { get; }
        }
    }
}
