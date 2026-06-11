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
    /// <remarks>
    /// The rule's invariant is that every result row must satisfy the predicates on the
    /// constrained columns.  An <c>OR</c> or <c>NOT</c> only threatens that invariant when a
    /// constrained column is referenced WITHIN the <c>OR</c> / <c>NOT</c> subtree — an
    /// <c>OR</c> between predicates on other columns, AND-ed with a constrained-column
    /// predicate outside it (e.g.
    /// <c>WHERE entity_id = 'x' AND ((y = 2026 AND q = 1) OR (y = 2025 AND q = 4))</c>),
    /// leaves the constrained-column predicate in force for every result row and is allowed.
    /// The evaluator therefore checks each <c>OR</c> / <c>NOT</c> node's subtree for
    /// constrained-column references and flags only the ones that contain such a reference.
    /// </remarks>
    public class ColumnScopedSimpleConjunctiveFilterSqlScriptValidationRuleEvaluator : SqlScriptValidationRuleEvaluatorBase
    {
        // Index of configured columns by column name (case-insensitive).  Used both to
        // narrow full-identity matches for resolvable references, and to detect bare-name
        // matches in multi-table queries.  A single column name might map to multiple full
        // identities if the same name is configured for several tables — hence
        // List<SchemaQualifiedColumnName>.
        private readonly Dictionary<string, List<SchemaQualifiedColumnName>> columnsByName;

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnScopedSimpleConjunctiveFilterSqlScriptValidationRuleEvaluator"/> class.
        /// </summary>
        /// <param name="rule">The rule to evaluate.</param>
        public ColumnScopedSimpleConjunctiveFilterSqlScriptValidationRuleEvaluator(
            ColumnScopedSimpleConjunctiveFilterSqlScriptValidationRule rule)
            : base(rule)
        {
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
            if (node == null)
            {
                return;
            }

            // Each QuerySpecification is its own scope with its own alias map; nested
            // specs (subqueries, derived tables) get their own Visit call.
            var aliasMap = FromClauseAliasMapBuilder.Build(node.FromClause);

            this.CheckForDisjunctionOrNegation(node.WhereClause?.SearchCondition, aliasMap);
            this.CheckForDisjunctionOrNegation(node.HavingClause?.SearchCondition, aliasMap);
            this.WalkJoinSearchConditionsForDisjunctionOrNegation(node.FromClause, aliasMap);
        }

        private void CheckForDisjunctionOrNegation(
            BooleanExpression expression,
            IReadOnlyDictionary<string, SchemaQualifiedTableName> aliasMap)
        {
            // Walk the boolean tree.  AND connectors keep us in pure conjunction territory —
            // recurse into both branches.  Parenthesized expressions are transparent.  Leaf
            // predicates terminate recursion (the negated forms encoded inline within their
            // predicate types — <>, !=, NOT LIKE, NOT IN, NOT BETWEEN, IS NOT NULL — are
            // leaves, NOT BooleanNotExpression wrappers, so they pass).
            //
            // OR connectors and explicit NOT wrappers are violations ONLY when their subtree
            // references a constrained column.  If the subtree contains no constrained-column
            // reference, nothing inside it can weaken or invert a constrained-column
            // predicate, so the entire subtree is skipped (containment is transitive — a
            // nested OR/NOT inside a clean subtree is also clean).
            if (expression == null)
            {
                return;
            }

            if (expression is BooleanBinaryExpression binary)
            {
                if (binary.BinaryExpressionType == BooleanBinaryExpressionType.Or)
                {
                    var referenced = this.FindReferencedConstrainedColumns(binary, aliasMap);

                    if (referenced.Count > 0)
                    {
                        this.AddViolation(
                            binary.StartOffset,
                            Invariant($"OR not allowed in filter referencing {BuildReferencedConstrainedColumnsClause(referenced)}; filter must be a simple conjunction"));
                    }

                    // Either way, stop here: if the subtree referenced a constrained column
                    // we've emitted (one violation per outermost offending OR); if it
                    // didn't, nothing inside can.
                    return;
                }

                this.CheckForDisjunctionOrNegation(binary.FirstExpression, aliasMap);
                this.CheckForDisjunctionOrNegation(binary.SecondExpression, aliasMap);
            }
            else if (expression is BooleanNotExpression notExpression)
            {
                var referenced = this.FindReferencedConstrainedColumns(notExpression.Expression, aliasMap);

                if (referenced.Count > 0)
                {
                    this.AddViolation(
                        notExpression.StartOffset,
                        Invariant($"NOT not allowed in filter referencing {BuildReferencedConstrainedColumnsClause(referenced)}; filter must be a simple conjunction"));
                }
            }
            else if (expression is BooleanParenthesisExpression paren)
            {
                this.CheckForDisjunctionOrNegation(paren.Expression, aliasMap);
            }

            // Other leaf predicate types — no violation.
        }

        /// <summary>
        /// Collects every column reference in the supplied subtree and resolves each through
        /// the alias map, returning the (rendered) configured columns that were referenced.
        /// </summary>
        private SortedSet<string> FindReferencedConstrainedColumns(
            TSqlFragment subtree,
            IReadOnlyDictionary<string, SchemaQualifiedTableName> aliasMap)
        {
            var result = new SortedSet<string>(StringComparer.Ordinal);

            if (subtree == null)
            {
                return result;
            }

            var collector = new ColumnReferenceCollector();
            subtree.Accept(collector);

            foreach (var columnRef in collector.ColumnReferences)
            {
                this.ResolveAndRecord(columnRef, aliasMap, result);
            }

            return result;
        }

        private void ResolveAndRecord(
            ColumnReferenceExpression columnRef,
            IReadOnlyDictionary<string, SchemaQualifiedTableName> aliasMap,
            SortedSet<string> result)
        {
            var identifiers = columnRef?.MultiPartIdentifier?.Identifiers;

            if ((identifiers == null) || (identifiers.Count == 0))
            {
                return;
            }

            var columnName = identifiers[identifiers.Count - 1].Value;

            if (!this.columnsByName.TryGetValue(columnName, out var candidates))
            {
                return;
            }

            if (identifiers.Count == 1)
            {
                // Bare reference.  In a single-table FROM, resolves to that table — match by
                // full identity.  In a multi-table FROM, the reference cannot be resolved
                // without schema introspection; the bare name matches a configured column's
                // name, so err on the safe side and record ALL configured columns with that
                // name.
                if (aliasMap.Count == 1)
                {
                    SchemaQualifiedTableName soleTable = null;

                    foreach (var table in aliasMap.Values)
                    {
                        soleTable = table;
                        break;
                    }

                    RecordIdentityMatches(candidates, soleTable.SchemaName, soleTable.TableName, result);
                }
                else
                {
                    foreach (var candidate in candidates)
                    {
                        result.Add(candidate.ToString());
                    }
                }

                return;
            }

            if (identifiers.Count == 2)
            {
                // alias.column or table-name.column — look up the first identifier in the
                // alias map.  If the alias isn't found, the script would runtime-error
                // anyway; skip silently.
                if (aliasMap.TryGetValue(identifiers[0].Value, out var schemaTable))
                {
                    RecordIdentityMatches(candidates, schemaTable.SchemaName, schemaTable.TableName, result);
                }

                return;
            }

            // 3-part [schema, table, column] or 4-part [db, schema, table, column].
            var schema = identifiers[identifiers.Count - 3].Value;
            var table2 = identifiers[identifiers.Count - 2].Value;

            RecordIdentityMatches(candidates, schema, table2, result);
        }

        private static void RecordIdentityMatches(
            List<SchemaQualifiedColumnName> candidates,
            string schemaName,
            string tableName,
            SortedSet<string> result)
        {
            foreach (var candidate in candidates)
            {
                if (string.Equals(candidate.SchemaName, schemaName, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(candidate.TableName, tableName, StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(candidate.ToString());
                }
            }
        }

        private static string BuildReferencedConstrainedColumnsClause(
            SortedSet<string> referencedConstrainedColumns)
        {
            var joined = string.Join(", ", referencedConstrainedColumns);

            return (referencedConstrainedColumns.Count == 1)
                ? Invariant($"constrained column {joined}")
                : Invariant($"constrained columns {joined}");
        }

        private void WalkJoinSearchConditionsForDisjunctionOrNegation(
            FromClause fromClause,
            IReadOnlyDictionary<string, SchemaQualifiedTableName> aliasMap)
        {
            if ((fromClause == null) || (fromClause.TableReferences == null))
            {
                return;
            }

            foreach (var tableReference in fromClause.TableReferences)
            {
                this.WalkTableReferenceForJoinDisjunctionOrNegation(tableReference, aliasMap);
            }
        }

        private void WalkTableReferenceForJoinDisjunctionOrNegation(
            TableReference tableReference,
            IReadOnlyDictionary<string, SchemaQualifiedTableName> aliasMap)
        {
            if (tableReference is QualifiedJoin qualifiedJoin)
            {
                if (qualifiedJoin.SearchCondition != null)
                {
                    this.CheckForDisjunctionOrNegation(qualifiedJoin.SearchCondition, aliasMap);
                }

                this.WalkTableReferenceForJoinDisjunctionOrNegation(qualifiedJoin.FirstTableReference, aliasMap);
                this.WalkTableReferenceForJoinDisjunctionOrNegation(qualifiedJoin.SecondTableReference, aliasMap);
            }
            else if (tableReference is JoinParenthesisTableReference joinParen)
            {
                this.WalkTableReferenceForJoinDisjunctionOrNegation(joinParen.Join, aliasMap);
            }

            // Other table-reference types do not have ON clauses.
        }

        private sealed class ColumnReferenceCollector : TSqlFragmentVisitor
        {
            public List<ColumnReferenceExpression> ColumnReferences { get; } = new List<ColumnReferenceExpression>();

            public override void Visit(
                ColumnReferenceExpression node)
            {
                this.ColumnReferences.Add(node);
            }
        }
    }
}
