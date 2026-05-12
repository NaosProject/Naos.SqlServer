// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SchemasUsedSqlScriptValidationRuleEvaluatorBase.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Protocol.Validation
{
    using Microsoft.SqlServer.TransactSql.ScriptDom;
    using Naos.SqlServer.Domain;

    /// <summary>
    /// Base class for a SQL script validation rule evaluator that evaluates schemas used.
    /// </summary>
    public abstract class SchemasUsedSqlScriptValidationRuleEvaluatorBase : SqlScriptValidationRuleEvaluatorBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SchemasUsedSqlScriptValidationRuleEvaluatorBase"/> class.
        /// </summary>
        /// <param name="rule">The rule to evaluate.</param>
        protected SchemasUsedSqlScriptValidationRuleEvaluatorBase(
            SqlScriptValidationRuleBase rule)
            : base(rule)
        {
        }

        /// <inheritdoc />
        public override void Visit(
            SchemaObjectName node)
        {
            if (node == null)
            {
                return;
            }

            this.HandleSchemaUsed(node.SchemaIdentifier, node.StartOffset);
        }

        /// <inheritdoc />
        public override void Visit(
            FunctionCall node)
        {
            // Scalar function and CLR static method calls carry their qualifier in
            // MultiPartIdentifierCallTarget; built-ins (GETDATE, etc.) have a null CallTarget.
            if (node == null)
            {
                return;
            }

            var callTarget = node.CallTarget as MultiPartIdentifierCallTarget;

            if ((callTarget == null) || (callTarget.MultiPartIdentifier == null))
            {
                return;
            }

            var identifiers = callTarget.MultiPartIdentifier.Identifiers;

            // The CallTarget holds only the qualifier — the function name itself lives on
            // FunctionCall.FunctionName — so for schema.fn() or database.schema.fn() the
            // schema is the LAST identifier in the qualifier.
            if ((identifiers == null) || (identifiers.Count < 1))
            {
                return;
            }

            this.HandleSchemaUsed(identifiers[identifiers.Count - 1], node.StartOffset);
        }

        /// <inheritdoc />
        public override void Visit(
            ColumnReferenceExpression node)
        {
            // Schema-qualified column references (e.g. schema.table.col) carry the
            // qualifier in MultiPartIdentifier, not SchemaObjectName.
            if ((node == null) || (node.MultiPartIdentifier == null))
            {
                return;
            }

            var identifiers = node.MultiPartIdentifier.Identifiers;

            // Forms: col, alias.col, schema.table.col, database.schema.table.col — schema,
            // if present, is two identifiers before the column name.
            if ((identifiers == null) || (identifiers.Count < 3))
            {
                return;
            }

            this.HandleSchemaUsed(identifiers[identifiers.Count - 3], node.StartOffset);
        }

        /// <inheritdoc />
        public override void Visit(
            CreateSchemaStatement node)
        {
            // CREATE SCHEMA holds the schema name as a bare Identifier on Name (no SchemaObjectName).
            if ((node == null) || (node.Name == null))
            {
                return;
            }

            this.HandleSchemaUsed(node.Name, node.Name.StartOffset);
        }

        /// <inheritdoc />
        public override void Visit(
            DropSchemaStatement node)
        {
            // DROP SCHEMA's Schema is a SchemaObjectName, but the schema name lives in
            // BaseIdentifier (SchemaIdentifier is null) so Visit(SchemaObjectName) skips it.
            if ((node == null) || (node.Schema == null) || (node.Schema.BaseIdentifier == null))
            {
                return;
            }

            this.HandleSchemaUsed(node.Schema.BaseIdentifier, node.Schema.BaseIdentifier.StartOffset);
        }

        /// <inheritdoc />
        public override void Visit(
            AlterSchemaStatement node)
        {
            // ALTER SCHEMA <dest> TRANSFER <src> — destination is an Identifier on Name.
            // The source (ObjectName) is a SchemaObjectName already covered by Visit(SchemaObjectName).
            if ((node == null) || (node.Name == null))
            {
                return;
            }

            this.HandleSchemaUsed(node.Name, node.Name.StartOffset);
        }

        /// <inheritdoc />
        public override void Visit(
            SecurityTargetObject node)
        {
            // GRANT/REVOKE/DENY/ALTER AUTHORIZATION targets carry the operand as a
            // SecurityTargetObjectName.MultiPartIdentifier — not a SchemaObjectName — so
            // Visit(SchemaObjectName) never fires on them.
            if ((node == null) || (node.ObjectName == null) || (node.ObjectName.MultiPartIdentifier == null))
            {
                return;
            }

            var identifiers = node.ObjectName.MultiPartIdentifier.Identifiers;

            if (identifiers == null)
            {
                return;
            }

            Identifier schemaIdentifier;

            if (node.ObjectKind == SecurityObjectKind.Schema)
            {
                // ON SCHEMA::name — the only identifier is the schema name itself.
                if (identifiers.Count != 1)
                {
                    return;
                }

                schemaIdentifier = identifiers[0];
            }
            else
            {
                // ON [kind::][database.]schema.object — schema is two identifiers before the object.
                // Single-part operands (DATABASE::db, LOGIN::login, ROLE::role, …) fall through.
                if (identifiers.Count < 2)
                {
                    return;
                }

                schemaIdentifier = identifiers[identifiers.Count - 2];
            }

            this.HandleSchemaUsed(schemaIdentifier, schemaIdentifier.StartOffset);
        }

        /// <summary>
        /// Handles the detection of a schema being used.
        /// </summary>
        /// <param name="schemaIdentifier">The schema identifier.</param>
        /// <param name="offset">The offset.</param>
        protected abstract void HandleSchemaUsed(
            Identifier schemaIdentifier,
            int offset);
    }
}
