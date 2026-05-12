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
    /// Base class for a SQL script validation rule evaluator that observes schemas in use
    /// throughout a script.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Invokes <see cref="HandleSchemaUsed"/> once per AST position where a schema name can
    /// appear.  The schema-identifier argument is non-null when the script names a schema at
    /// that position (e.g. <c>FROM dbo.my_table</c>) and null when the position could have
    /// carried a schema qualifier but the script left it bare (e.g. <c>FROM my_table</c>,
    /// <c>GRANT SELECT ON t TO u</c>).  Allow/deny-list rules typically inspect the value when
    /// non-null and early-return on null.  Rules that enforce schema qualification flag the null
    /// case as a violation.
    /// </para>
    /// <para>
    /// Not every "no schema present" position reaches <see cref="HandleSchemaUsed"/>; the
    /// following are deliberately filtered, because signaling absence at these positions would
    /// either be wrong or be indistinguishable from valid grammar:
    /// </para>
    /// <list type="bullet">
    /// <item><description>Bare function calls — a <see cref="FunctionCall"/> with a null
    /// <see cref="FunctionCall.CallTarget"/>.  In the AST a built-in (<c>SUM(x)</c>,
    /// <c>GETDATE()</c>) is indistinguishable from a bare user-defined function call
    /// (<c>my_fn(x)</c>); signaling absence here would force a strict rule to flag
    /// <c>SUM(price)</c>, which is wrong because built-ins cannot be schema-qualified.  Bare
    /// UDFs slip through as the cost of that exemption.</description></item>
    /// <item><description>Column references with fewer than three identifiers
    /// (<see cref="ColumnReferenceExpression"/>).  Bare (<c>col</c>) and alias-qualified
    /// (<c>a.col</c>) column references are normal T-SQL, the parser cannot reliably
    /// distinguish "alias.column" from "schema.column" with only two parts, and column refs are
    /// not a meaningful target for schema-qualification policy.</description></item>
    /// <item><description><see cref="CreateSchemaStatement"/>, <see cref="DropSchemaStatement"/>,
    /// and <see cref="AlterSchemaStatement"/> — the operand IS the schema being targeted; there
    /// is no "schema qualifier" slot that could be present or absent.</description></item>
    /// <item><description><see cref="SecurityTargetObject"/> with a principal-style
    /// <see cref="SecurityObjectKind"/> (e.g. DATABASE, LOGIN, ROLE, SERVER, FULLTEXT CATALOG)
    /// whose operands are inherently single-part and have no schema concept.</description></item>
    /// </list>
    /// </remarks>
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
            // Tables in FROM/JOIN/UPDATE/DELETE/INSERT/MERGE, DDL targets (CREATE/DROP/ALTER
            // TABLE, CREATE INDEX, CREATE VIEW/PROCEDURE/FUNCTION/TRIGGER/SYNONYM, FK REFERENCES,
            // SELECT ... INTO, BULK INSERT, SET IDENTITY_INSERT, NEXT VALUE FOR,
            // UserDataTypeReference, ...) and EXEC procedure references all carry their
            // qualified name as a SchemaObjectName.  When the script omits the schema (e.g.
            // "FROM my_table"), SchemaIdentifier is null and we pass that null through so
            // strict-qualification rules can flag it.
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
            // MultiPartIdentifierCallTarget.  Bare function calls (CallTarget == null) are
            // intentionally NOT signaled: in the AST a built-in like SUM(x)/GETDATE() is
            // indistinguishable from a bare user-defined function call my_fn(x), so signaling
            // absence here would force any strict-qualification rule to flag SUM(price), which
            // is nonsense — built-ins cannot be schema-qualified.  Bare UDFs slip through as the
            // cost of that exemption.
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
            // Schema-qualified column references (schema.table.col, database.schema.table.col)
            // carry the qualifier in MultiPartIdentifier, not SchemaObjectName.  Bare ("col")
            // and alias-qualified ("a.col") forms are intentionally NOT signaled: (a) the parser
            // cannot reliably distinguish "alias.column" from "schema.column" with only two
            // parts, and (b) column refs are not a meaningful target for schema-qualification
            // policy — they are normally bare or alias-qualified.
            if ((node == null) || (node.MultiPartIdentifier == null))
            {
                return;
            }

            var identifiers = node.MultiPartIdentifier.Identifiers;

            // Forms: col, alias.col, schema.table.col, database.schema.table.col — schema, if
            // present, is two identifiers before the column name.
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
            // CREATE SCHEMA holds the schema name as a bare Identifier on Name (not a
            // SchemaObjectName).  The operand IS the schema being created, so there is no
            // "schema qualifier" slot — Name is always populated in valid input.
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
            // As with CREATE SCHEMA, the operand IS the schema being targeted — no
            // qualification concept.
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
            // ALTER SCHEMA <dest> TRANSFER <src> — destination is an Identifier on Name (the
            // operand IS a schema, no qualification concept).  The source (ObjectName) is a
            // SchemaObjectName already covered by Visit(SchemaObjectName).
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
            // Visit(SchemaObjectName) never fires on them.  Behavior splits by ObjectKind:
            //
            //   - Schema (ON SCHEMA::name): the single identifier IS the schema name.
            //   - Schema-qualifiable kinds (Object/Type/XmlSchemaCollection/NotSpecified):
            //       ON [kind::][database.]schema.object — the schema is the identifier two
            //       positions before the object name.  When the operand is bare (Count == 1),
            //       e.g. "GRANT SELECT ON t TO u", we pass a null identifier through so
            //       strict-qualification rules can flag the missing schema; allow/deny-list
            //       rules early-return on null and ignore it.
            //   - Principal kinds (DATABASE::, LOGIN::, ROLE::, SERVER::, FULLTEXT CATALOG::, ...):
            //       the operand is inherently single-part and has no schema concept — skip.
            if ((node == null) || (node.ObjectName == null) || (node.ObjectName.MultiPartIdentifier == null))
            {
                return;
            }

            var identifiers = node.ObjectName.MultiPartIdentifier.Identifiers;

            if (identifiers == null)
            {
                return;
            }

            if (node.ObjectKind == SecurityObjectKind.Schema)
            {
                // ON SCHEMA::name — the only identifier IS the schema name itself.
                if (identifiers.Count != 1)
                {
                    return;
                }

                var schemaIdentifier = identifiers[0];

                this.HandleSchemaUsed(schemaIdentifier, schemaIdentifier.StartOffset);

                return;
            }

            if (!IsSchemaQualifiableSecurityObjectKind(node.ObjectKind))
            {
                // Principal-style kinds: single-part operand, no schema concept.
                return;
            }

            if (identifiers.Count == 0)
            {
                // Malformed AST — nothing to report.
                return;
            }

            if (identifiers.Count == 1)
            {
                // Bare object reference, e.g. "GRANT SELECT ON t TO u".  Signal the absent
                // schema (null Identifier) so strict-qualification rules can flag it.  The
                // offset is the position of the bare object name, where the schema would
                // have appeared.
                this.HandleSchemaUsed(null, identifiers[0].StartOffset);

                return;
            }

            // 2+ parts: schema is the identifier immediately before the object name.
            var qualifierSchemaIdentifier = identifiers[identifiers.Count - 2];

            this.HandleSchemaUsed(qualifierSchemaIdentifier, qualifierSchemaIdentifier.StartOffset);
        }

        /// <summary>
        /// Invoked once for each AST position where a schema can appear.
        /// </summary>
        /// <param name="schemaIdentifier">
        /// The schema identifier at this position, or <c>null</c> if the position could have
        /// carried a schema qualifier but the script left it bare (e.g. <c>FROM my_table</c>,
        /// <c>GRANT SELECT ON t TO u</c>).  Allow/deny-list rules typically early-return on
        /// null; rules that enforce schema qualification flag the null case as a violation.
        /// </param>
        /// <param name="offset">The offset to report on the violation, if any.</param>
        protected abstract void HandleSchemaUsed(
            Identifier schemaIdentifier,
            int offset);

        private static bool IsSchemaQualifiableSecurityObjectKind(
            SecurityObjectKind kind)
        {
            // Kinds whose security-target operand can be schema-qualified ("schema.object" form):
            //   GRANT ... ON [OBJECT::]schema.object              — Object / NotSpecified
            //   GRANT ... ON TYPE::schema.type                    — Type
            //   GRANT ... ON XML SCHEMA COLLECTION::schema.coll   — XmlSchemaCollection
            // Other kinds (Database, Login, Role, Server, FullTextCatalog, ...) take single-part
            // operands and have no schema concept.
            return (kind == SecurityObjectKind.NotSpecified)
                || (kind == SecurityObjectKind.Object)
                || (kind == SecurityObjectKind.Type)
                || (kind == SecurityObjectKind.XmlSchemaCollection);
        }
    }
}
