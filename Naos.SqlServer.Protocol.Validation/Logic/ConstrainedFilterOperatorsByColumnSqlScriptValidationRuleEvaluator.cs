// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConstrainedFilterOperatorsByColumnSqlScriptValidationRuleEvaluator.cs" company="Naos Project">
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
    /// Evaluates a <see cref="ConstrainedFilterOperatorsByColumnSqlScriptValidationRule"/>.
    /// </summary>
    public class ConstrainedFilterOperatorsByColumnSqlScriptValidationRuleEvaluator : FilterPredicateSqlScriptValidationRuleEvaluatorBase
    {
        // Per-column allow-list lookup.  Outer key: column-name (case-insensitive) — used for
        // the unresolved-bare-reference path.  Inner: list of configured (column, allowed-ops)
        // tuples whose column-name matches the outer key.  In typical configurations the
        // inner list is length 1, but multiple configured columns can share a column name
        // across different schemas/tables.
        private readonly Dictionary<string, List<ColumnFilterOperators>> entriesByColumnName;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstrainedFilterOperatorsByColumnSqlScriptValidationRuleEvaluator"/> class.
        /// </summary>
        /// <param name="rule">The rule to evaluate.</param>
        public ConstrainedFilterOperatorsByColumnSqlScriptValidationRuleEvaluator(
            ConstrainedFilterOperatorsByColumnSqlScriptValidationRule rule)
            : base(rule)
        {
            this.entriesByColumnName = new Dictionary<string, List<ColumnFilterOperators>>(StringComparer.OrdinalIgnoreCase);

            foreach (var entry in rule.ColumnFilterOperators)
            {
                if (!this.entriesByColumnName.TryGetValue(entry.Column.ColumnName, out var list))
                {
                    list = new List<ColumnFilterOperators>();
                    this.entriesByColumnName[entry.Column.ColumnName] = list;
                }

                list.Add(entry);
            }
        }

        /// <inheritdoc />
        protected override void HandleResolvedFilterPredicate(
            SchemaQualifiedColumnName column,
            FilterOperator op,
            IReadOnlyList<ScalarExpression> values,
            int offset)
        {
            // Look up entries that share this column's name, then narrow by full
            // (schema, table) match.  If no matching configuration, the column isn't
            // constrained — pass.
            if (!this.entriesByColumnName.TryGetValue(column.ColumnName, out var candidates))
            {
                return;
            }

            foreach (var candidate in candidates)
            {
                if (!IsSameColumn(candidate.Column, column))
                {
                    continue;
                }

                if (!candidate.AllowedOperators.HasFlag(op.ToFilterOperators()))
                {
                    this.AddViolation(
                        offset,
                        Invariant($"operator {op} is not allowed on column {column}"));
                }

                // Found the matching config; no need to keep looking — column identity is unique.
                return;
            }
        }

        /// <inheritdoc />
        protected override void HandleUnresolvedBareColumnReference(
            string columnName,
            FilterOperator op,
            int offset)
        {
            // Only emit the "must be qualified" violation if the bare column NAME matches
            // one of the rule's configured columns by name.  Bare references whose names
            // don't appear in the config aren't relevant to this rule.
            if (this.entriesByColumnName.ContainsKey(columnName))
            {
                this.AddViolation(
                    offset,
                    "column reference must be table-qualified in multi-table queries: " + columnName);
            }
        }

        private static bool IsSameColumn(
            SchemaQualifiedColumnName left,
            SchemaQualifiedColumnName right)
        {
            return string.Equals(left.SchemaName, right.SchemaName, StringComparison.OrdinalIgnoreCase)
                && string.Equals(left.TableName, right.TableName, StringComparison.OrdinalIgnoreCase)
                && string.Equals(left.ColumnName, right.ColumnName, StringComparison.OrdinalIgnoreCase);
        }
    }
}
