// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisallowedSchemasSqlScriptValidationRuleEvaluator.cs" company="Naos Project">
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
    /// Evaluates a <see cref="DisallowedSchemasSqlScriptValidationRule"/>.
    /// </summary>
    public class DisallowedSchemasSqlScriptValidationRuleEvaluator : SchemasUsedSqlScriptValidationRuleEvaluatorBase
    {
        private readonly HashSet<string> disallowedSchemas;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisallowedSchemasSqlScriptValidationRuleEvaluator"/> class.
        /// </summary>
        /// <param name="rule">The rule to evaluate.</param>
        public DisallowedSchemasSqlScriptValidationRuleEvaluator(
            DisallowedSchemasSqlScriptValidationRule rule)
            : base(rule)
        {
            this.disallowedSchemas = new HashSet<string>(rule.DisallowedSchemas, StringComparer.OrdinalIgnoreCase);
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

            if (this.disallowedSchemas.Contains(schema))
            {
                this.AddViolation(
                    offset,
                    "reference to disallowed schema: " + schema);
            }
        }
    }
}
