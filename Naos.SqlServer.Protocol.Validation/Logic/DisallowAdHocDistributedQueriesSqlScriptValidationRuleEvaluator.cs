// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisallowAdHocDistributedQueriesSqlScriptValidationRuleEvaluator.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Protocol.Validation
{
    using Microsoft.SqlServer.TransactSql.ScriptDom;
    using Naos.SqlServer.Domain;

    /// <summary>
    /// Evaluates a <see cref="DisallowAdHocDistributedQueriesSqlScriptValidationRule"/>.
    /// </summary>
    public class DisallowAdHocDistributedQueriesSqlScriptValidationRuleEvaluator : SqlScriptValidationRuleEvaluatorBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DisallowAdHocDistributedQueriesSqlScriptValidationRuleEvaluator"/> class.
        /// </summary>
        /// <param name="rule">The rule to evaluate.</param>
        public DisallowAdHocDistributedQueriesSqlScriptValidationRuleEvaluator(
            DisallowAdHocDistributedQueriesSqlScriptValidationRule rule)
            : base(rule)
        {
        }

        /// <inheritdoc />
        public override void Visit(
            OpenRowsetTableReference node)
        {
            // OPENROWSET('provider', 'conn', 'query') — OLE DB ad-hoc query.  The query string
            // is an opaque payload; other parser-level rules cannot see what it executes.
            if (node == null)
            {
                return;
            }

            this.AddViolation(node.StartOffset, "ad-hoc distributed query is not allowed: OPENROWSET");
        }

        /// <inheritdoc />
        public override void Visit(
            BulkOpenRowset node)
        {
            // OPENROWSET(BULK 'file', ...) — reads arbitrary file contents from the server's
            // file system.  Not a SQL-transport vector, but still an external-data ingress.
            if (node == null)
            {
                return;
            }

            this.AddViolation(node.StartOffset, "ad-hoc distributed query is not allowed: OPENROWSET BULK");
        }

        /// <inheritdoc />
        public override void Visit(
            OpenRowsetCosmos node)
        {
            // OPENROWSET Cosmos variant — queries against Azure Cosmos DB.
            if (node == null)
            {
                return;
            }

            this.AddViolation(node.StartOffset, "ad-hoc distributed query is not allowed: OPENROWSET (Cosmos)");
        }

        /// <inheritdoc />
        public override void Visit(
            OpenQueryTableReference node)
        {
            // OPENQUERY(linked_server, 'query') — sends the query string to a pre-configured
            // linked server.  Payload is opaque to the parser.
            if (node == null)
            {
                return;
            }

            this.AddViolation(node.StartOffset, "ad-hoc distributed query is not allowed: OPENQUERY");
        }

        /// <inheritdoc />
        public override void Visit(
            AdHocTableReference node)
        {
            // OPENDATASOURCE('provider', 'conn').db.schema.table — ad-hoc remote four-part name.
            // We override the outer AdHocTableReference (not the inner AdHocDataSource child)
            // to avoid double-firing on the same construct — both nodes share the same offset.
            if (node == null)
            {
                return;
            }

            this.AddViolation(node.StartOffset, "ad-hoc distributed query is not allowed: OPENDATASOURCE");
        }
    }
}
