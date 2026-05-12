// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisallowSystemSchemasSqlScriptValidationRuleEvaluator.cs" company="Naos Project">
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
    /// Evaluates a <see cref="DisallowSystemSchemasSqlScriptValidationRule"/>.
    /// </summary>
    public class DisallowSystemSchemasSqlScriptValidationRuleEvaluator : SchemasUsedSqlScriptValidationRuleEvaluatorBase
    {
        private readonly HashSet<string> disallowedSchemas = new HashSet<string>(
            new[] { "sys", "INFORMATION_SCHEMA" },
            StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Initializes a new instance of the <see cref="DisallowSystemSchemasSqlScriptValidationRuleEvaluator"/> class.
        /// </summary>
        /// <param name="rule">The rule to evaluate.</param>
        public DisallowSystemSchemasSqlScriptValidationRuleEvaluator(
            DisallowSystemSchemasSqlScriptValidationRule rule)
            : base(rule)
        {
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
                    "disallowed reference to system schema: " + schema);
            }
        }
    }
}
