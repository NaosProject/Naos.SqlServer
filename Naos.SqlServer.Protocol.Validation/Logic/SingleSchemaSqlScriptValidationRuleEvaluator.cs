// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SingleSchemaSqlScriptValidationRuleEvaluator.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Protocol.Validation
{
    using System;
    using Microsoft.SqlServer.TransactSql.ScriptDom;
    using Naos.SqlServer.Domain;

    /// <summary>
    /// Evaluates a <see cref="SingleSchemaSqlScriptValidationRule"/>.
    /// </summary>
    public class SingleSchemaSqlScriptValidationRuleEvaluator : SchemasUsedSqlScriptValidationRuleEvaluatorBase
    {
        private string firstSchema;

        /// <summary>
        /// Initializes a new instance of the <see cref="SingleSchemaSqlScriptValidationRuleEvaluator"/> class.
        /// </summary>
        /// <param name="rule">The rule to evaluate.</param>
        public SingleSchemaSqlScriptValidationRuleEvaluator(
            SingleSchemaSqlScriptValidationRule rule)
            : base(rule)
        {
        }

        /// <inheritdoc />
        protected override void HandleSchemaUsed(
            Identifier schemaIdentifier,
            int offset)
        {
            // Bare references (no schema named) are silently ignored — they don't set the
            // canonical schema and aren't flagged.  Only explicit schema references count.
            if (schemaIdentifier == null)
            {
                return;
            }

            var schema = schemaIdentifier.Value;

            if (this.firstSchema == null)
            {
                // First explicit schema seen — this is the canonical schema for the script.
                this.firstSchema = schema;

                return;
            }

            // Case-insensitive comparison: SQL Server treats schema names as case-insensitive
            // under default collation, so "dbo", "Dbo", and "DBO" are the same schema.
            if (!string.Equals(this.firstSchema, schema, StringComparison.OrdinalIgnoreCase))
            {
                this.AddViolation(offset, "reference to additional schema: " + schema);
            }
        }
    }
}
