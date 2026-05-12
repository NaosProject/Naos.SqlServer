// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SchemaQualifiedTableReferencesSqlScriptValidationRuleEvaluator.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Protocol.Validation
{
    using System;
    using Microsoft.SqlServer.TransactSql.ScriptDom;
    using Naos.SqlServer.Domain;

    /// <summary>
    /// Evaluates a <see cref="SchemaQualifiedTableReferencesSqlScriptValidationRule"/>.
    /// </summary>
    public class SchemaQualifiedTableReferencesSqlScriptValidationRuleEvaluator : SqlScriptValidationRuleEvaluatorBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SchemaQualifiedTableReferencesSqlScriptValidationRuleEvaluator"/> class.
        /// </summary>
        /// <param name="rule">The rule to evaluate.</param>
        public SchemaQualifiedTableReferencesSqlScriptValidationRuleEvaluator(
            SchemaQualifiedTableReferencesSqlScriptValidationRule rule)
            : base(rule)
        {
        }

        /// <inheritdoc />
        public override void Visit(
            NamedTableReference node)
        {
            // This rule deliberately targets NamedTableReference directly rather than going
            // through SchemasUsedSqlScriptValidationRuleEvaluatorBase — that base class fires
            // for every SchemaObjectName position in the AST (DDL targets, EXEC procedure
            // references, FK references, security-target objects, …), which is too broad.
            // NamedTableReference is the precise AST class for "a table appearing in a DML
            // statement's FROM/JOIN/INSERT-target/UPDATE-target/DELETE-target/MERGE-target
            // position" — exactly the scope this rule cares about.
            if ((node == null) || (node.SchemaObject == null))
            {
                return;
            }

            // Schema is specified — pass.
            if (node.SchemaObject.SchemaIdentifier != null)
            {
                return;
            }

            if (node.SchemaObject.BaseIdentifier == null)
            {
                return;
            }

            var baseName = node.SchemaObject.BaseIdentifier.Value;

            // Temp tables (#local, ##global) live in tempdb and don't have a user-addressable
            // schema — T-SQL convention is to write them without any schema prefix.  Exempt.
            if (baseName.StartsWith("#", StringComparison.Ordinal))
            {
                return;
            }

            this.AddViolation(node.StartOffset, "table reference is not schema-qualified: " + baseName);
        }
    }
}
