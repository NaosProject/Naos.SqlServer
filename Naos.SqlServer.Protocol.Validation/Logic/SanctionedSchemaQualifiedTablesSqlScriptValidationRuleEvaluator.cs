// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SanctionedSchemaQualifiedTablesSqlScriptValidationRuleEvaluator.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Protocol.Validation
{
    using System;
    using System.Collections.Generic;
    using Microsoft.SqlServer.TransactSql.ScriptDom;
    using Naos.SqlServer.Domain;

    /// <summary>
    /// Evaluates a <see cref="SanctionedSchemaQualifiedTablesSqlScriptValidationRule"/>.
    /// </summary>
    public class SanctionedSchemaQualifiedTablesSqlScriptValidationRuleEvaluator : SqlScriptValidationRuleEvaluatorBase
    {
        // Lookup structure: schema name → set of allowed table names within that schema, all
        // compared case-insensitively.  This avoids any ambiguity that a single concatenated
        // key would introduce if an identifier contained a "." (possible via bracketed
        // identifiers, e.g. [my.weird.name], though rare in practice).
        private readonly Dictionary<string, HashSet<string>> tablesBySchema;

        /// <summary>
        /// Initializes a new instance of the <see cref="SanctionedSchemaQualifiedTablesSqlScriptValidationRuleEvaluator"/> class.
        /// </summary>
        /// <param name="rule">The rule to evaluate.</param>
        public SanctionedSchemaQualifiedTablesSqlScriptValidationRuleEvaluator(
            SanctionedSchemaQualifiedTablesSqlScriptValidationRule rule)
            : base(rule)
        {
            this.tablesBySchema = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);

            foreach (var sanctioned in rule.SanctionedSchemaQualifiedTables)
            {
                if (!this.tablesBySchema.TryGetValue(sanctioned.SchemaName, out var tables))
                {
                    tables = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    this.tablesBySchema[sanctioned.SchemaName] = tables;
                }

                tables.Add(sanctioned.TableName);
            }
        }

        /// <inheritdoc />
        public override void Visit(
            NamedTableReference node)
        {
            // Like SchemaQualifiedTableReferencesSqlScriptValidationRule, this rule targets
            // NamedTableReference directly — that's the precise AST class for "a table appearing
            // in a DML statement's FROM/JOIN/INSERT-target/UPDATE-target/DELETE-target/MERGE-
            // target/USING-source position" — exactly the scope this rule cares about.
            if ((node == null) || (node.SchemaObject == null))
            {
                return;
            }

            // No schema specified — out of scope for this rule.  Compose with
            // SchemaQualifiedTableReferencesSqlScriptValidationRule to enforce qualification.
            if (node.SchemaObject.SchemaIdentifier == null)
            {
                return;
            }

            if (node.SchemaObject.BaseIdentifier == null)
            {
                return;
            }

            var schema = node.SchemaObject.SchemaIdentifier.Value;
            var table = node.SchemaObject.BaseIdentifier.Value;

            // Temp tables don't have a user-addressable schema.  In practice you can't actually
            // write "schema.#temp" anyway (the parser rejects it), but this is a defensive
            // check in case ScriptDom ever parses something weird into this shape.
            if (table.StartsWith("#", StringComparison.Ordinal))
            {
                return;
            }

            if (this.tablesBySchema.TryGetValue(schema, out var sanctionedTables) && sanctionedTables.Contains(table))
            {
                return;
            }

            this.AddViolation(node.StartOffset, "reference to unsanctioned table: " + schema + "." + table);
        }
    }
}
