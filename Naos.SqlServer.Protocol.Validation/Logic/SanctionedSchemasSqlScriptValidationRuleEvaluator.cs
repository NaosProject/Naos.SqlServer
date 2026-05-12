// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SanctionedSchemasSqlScriptValidationRuleEvaluator.cs" company="Naos Project">
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
    /// Evaluates a <see cref="SanctionedSchemasSqlScriptValidationRule"/>.
    /// </summary>
    public class SanctionedSchemasSqlScriptValidationRuleEvaluator : SchemasUsedSqlScriptValidationRuleEvaluatorBase
    {
        private readonly HashSet<string> sanctionedSchemas;

        /// <summary>
        /// Initializes a new instance of the <see cref="SanctionedSchemasSqlScriptValidationRuleEvaluator"/> class.
        /// </summary>
        /// <param name="rule">The rule to evaluate.</param>
        public SanctionedSchemasSqlScriptValidationRuleEvaluator(
            SanctionedSchemasSqlScriptValidationRule rule)
            : base(rule)
        {
            this.sanctionedSchemas = new HashSet<string>(rule.SanctionedSchemas, StringComparer.OrdinalIgnoreCase);
        }

        /// <inheritdoc />
        protected override void HandleSchemaUsed(
            Identifier schemaIdentifier,
            int offset)
        {
            if (schemaIdentifier == null)
            {
                return;
            }

            var schema = schemaIdentifier.Value;

            if (!this.sanctionedSchemas.Contains(schema))
            {
                this.AddViolation(
                    offset,
                    "reference to unsanctioned schema: " + schema);
            }
        }
    }
}
