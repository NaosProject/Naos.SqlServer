// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidateSqlScriptProtocolTest.SanctionedSchemasSqlScriptValidationRule.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Protocol.Validation.Test
{
    using System.Collections.Generic;
    using System.Linq;
    using Naos.SqlServer.Domain;
    using Xunit;

    public static partial class ValidateSqlScriptProtocolTest
    {
        // The violations scenarios assume a rule sanctioning ["MY_SCHEMA", "myschema2"].
        // Within these scripts, "my_schema" and "myschema2" (case-insensitive) are sanctioned
        // negative controls and "dbo" / "sys" are the unsanctioned schemas that should fire.
        private static readonly IReadOnlyList<TestScenariosWithExpected> SanctionedSchemasSqlScriptValidationRuleTestScenariosWithExpected = new[]
        {
            // simple SELECT, schema-qualified table
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.my_table",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 14, Details = "reference to unsanctioned schema: dbo" },
                },
            },

            // derived table (subquery in FROM)
            new TestScenariosWithExpected
            {
                Sql = "Select * From (Select * From dbo.my_table) A",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 29, Details = "reference to unsanctioned schema: dbo" },
                },
            },

            // INNER JOIN — left side is sanctioned, right side is the violation
            new TestScenariosWithExpected
            {
                Sql = "Select a.id From my_schema.other a Inner Join dbo.my_table b On a.id = b.id",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 46, Details = "reference to unsanctioned schema: dbo" },
                },
            },

            // LEFT JOIN with AS alias
            new TestScenariosWithExpected
            {
                Sql = "Select a.id From my_schema.other a Left Join dbo.my_table As b On a.id = b.id",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 45, Details = "reference to unsanctioned schema: dbo" },
                },
            },

            // UPDATE target
            new TestScenariosWithExpected
            {
                Sql = "Update dbo.my_table Set x = 1",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 7, Details = "reference to unsanctioned schema: dbo" },
                },
            },

            // DELETE target
            new TestScenariosWithExpected
            {
                Sql = "Delete From dbo.my_table Where x = 1",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 12, Details = "reference to unsanctioned schema: dbo" },
                },
            },

            // INSERT target
            new TestScenariosWithExpected
            {
                Sql = "Insert Into dbo.my_table (x) Values (1)",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 12, Details = "reference to unsanctioned schema: dbo" },
                },
            },

            // MERGE target is unsanctioned; USING source is sanctioned
            new TestScenariosWithExpected
            {
                Sql = "Merge Into dbo.my_table As t Using my_schema.src As s On t.id = s.id When Matched Then Update Set x = s.x;",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 11, Details = "reference to unsanctioned schema: dbo" },
                },
            },

            // CTE body references an unsanctioned schema
            new TestScenariosWithExpected
            {
                Sql = "With cte As (Select id From dbo.my_table) Select * From cte",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 28, Details = "reference to unsanctioned schema: dbo" },
                },
            },

            // subquery in WHERE / IN
            new TestScenariosWithExpected
            {
                Sql = "Select * From my_schema.other Where id In (Select id From dbo.my_table)",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 58, Details = "reference to unsanctioned schema: dbo" },
                },
            },

            // EXISTS correlated subquery
            new TestScenariosWithExpected
            {
                Sql = "Select * From my_schema.other o Where Exists (Select 1 From dbo.my_table m Where m.id = o.id)",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 60, Details = "reference to unsanctioned schema: dbo" },
                },
            },

            // UNION — only the unsanctioned branch violates
            new TestScenariosWithExpected
            {
                Sql = "Select id From my_schema.other Union Select id From dbo.my_table",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 52, Details = "reference to unsanctioned schema: dbo" },
                },
            },

            // CROSS APPLY with a subquery containing the violation
            new TestScenariosWithExpected
            {
                Sql = "Select * From my_schema.other o Cross Apply (Select Top 1 * From dbo.my_table m Where m.id = o.id) x",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 65, Details = "reference to unsanctioned schema: dbo" },
                },
            },

            // bracket-delimited schema identifier
            new TestScenariosWithExpected
            {
                Sql = "Select * From [dbo].my_table",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 14, Details = "reference to unsanctioned schema: dbo" },
                },
            },

            // bracket-delimited, uppercase — case preserved in the violation message
            new TestScenariosWithExpected
            {
                Sql = "Select * From [DBO].[my_table]",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 14, Details = "reference to unsanctioned schema: DBO" },
                },
            },

            // mixed case — case preserved in the violation message
            new TestScenariosWithExpected
            {
                Sql = "Select * From Dbo.my_table",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 14, Details = "reference to unsanctioned schema: Dbo" },
                },
            },

            // three-part name: database.schema.table — middle identifier is the schema
            new TestScenariosWithExpected
            {
                Sql = "Select * From mydb.dbo.my_table",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 14, Details = "reference to unsanctioned schema: dbo" },
                },
            },

            // second unsanctioned schema (sys) referenced on its own
            new TestScenariosWithExpected
            {
                Sql = "Select * From sys.my_table",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 14, Details = "reference to unsanctioned schema: sys" },
                },
            },

            // two violations in a single statement, two different unsanctioned schemas
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.t1 a Inner Join sys.t2 b On a.id = b.id",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 14, Details = "reference to unsanctioned schema: dbo" },
                    new ExpectedViolation { Offset = 34, Details = "reference to unsanctioned schema: sys" },
                },
            },

            // multi-batch script separated by GO — one violation per batch
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.t1\r\nGO\r\nSelect * From sys.t2",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 14, Details = "reference to unsanctioned schema: dbo" },
                    new ExpectedViolation { Offset = 40, Details = "reference to unsanctioned schema: sys" },
                },
            },

            // multi-statement (single batch), both statements violate
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.t1; Select * From sys.t2;",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 14, Details = "reference to unsanctioned schema: dbo" },
                    new ExpectedViolation { Offset = 36, Details = "reference to unsanctioned schema: sys" },
                },
            },

            // the same unsanctioned table referenced twice — two distinct violations
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.my_table a Inner Join dbo.my_table b On a.id = b.id",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 14, Details = "reference to unsanctioned schema: dbo" },
                    new ExpectedViolation { Offset = 40, Details = "reference to unsanctioned schema: dbo" },
                },
            },

            // EXEC of a stored procedure in an unsanctioned schema (ProcedureReference holds a SchemaObjectName).
            new TestScenariosWithExpected
            {
                Sql = "Exec dbo.my_sproc",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 5, Details = "reference to unsanctioned schema: dbo" },
                },
            },

            // DROP TABLE (DropObjectsStatement.Objects is a list of SchemaObjectName).
            new TestScenariosWithExpected
            {
                Sql = "Drop Table dbo.my_table",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 11, Details = "reference to unsanctioned schema: dbo" },
                },
            },

            // TRUNCATE TABLE.
            new TestScenariosWithExpected
            {
                Sql = "Truncate Table dbo.my_table",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 15, Details = "reference to unsanctioned schema: dbo" },
                },
            },

            // ALTER TABLE.
            new TestScenariosWithExpected
            {
                Sql = "Alter Table dbo.my_table Add col1 Int Null",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 12, Details = "reference to unsanctioned schema: dbo" },
                },
            },

            // CREATE TABLE.
            new TestScenariosWithExpected
            {
                Sql = "Create Table dbo.my_table (x Int)",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 13, Details = "reference to unsanctioned schema: dbo" },
                },
            },

            // CREATE INDEX ... ON.
            new TestScenariosWithExpected
            {
                Sql = "Create Index ix_my_table_x On dbo.my_table (x)",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 30, Details = "reference to unsanctioned schema: dbo" },
                },
            },

            // Table-valued function in FROM (SchemaObjectFunctionTableReference holds a SchemaObjectName).
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.my_tvf()",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 14, Details = "reference to unsanctioned schema: dbo" },
                },
            },

            // Scalar function call in an expression — captured via Visit(FunctionCall) /
            // MultiPartIdentifierCallTarget (the qualifier excludes the function name).
            new TestScenariosWithExpected
            {
                Sql = "Select * From my_schema.other Where dbo.is_active(id) = 1",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 36, Details = "reference to unsanctioned schema: dbo" },
                },
            },

            // Three-part column reference (schema.table.col) — captured via Visit(ColumnReferenceExpression).
            new TestScenariosWithExpected
            {
                Sql = "Select 1 From my_schema.alpha Where dbo.t.col = 1",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 36, Details = "reference to unsanctioned schema: dbo" },
                },
            },

            // Four-part column reference (database.schema.table.col).
            new TestScenariosWithExpected
            {
                Sql = "Select 1 From my_schema.alpha Where mydb.dbo.t.col = 1",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 36, Details = "reference to unsanctioned schema: dbo" },
                },
            },

            // CREATE PROCEDURE in an unsanctioned schema (ProcedureReference.Name).
            new TestScenariosWithExpected
            {
                Sql = "Create Procedure dbo.my_sp As Begin Select 1 End",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 17, Details = "reference to unsanctioned schema: dbo" },
                },
            },

            // CREATE FUNCTION in an unsanctioned schema.
            new TestScenariosWithExpected
            {
                Sql = "Create Function dbo.my_fn () Returns Int As Begin Return 1 End",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 16, Details = "reference to unsanctioned schema: dbo" },
                },
            },

            // CREATE VIEW in an unsanctioned schema.
            new TestScenariosWithExpected
            {
                Sql = "Create View dbo.my_view As Select 1 As x",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 12, Details = "reference to unsanctioned schema: dbo" },
                },
            },

            // CREATE TRIGGER whose target table is in an unsanctioned schema (TriggerObject.Name).
            new TestScenariosWithExpected
            {
                Sql = "Create Trigger my_schema.my_tr On dbo.my_table After Insert As Begin Select 1 End",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 34, Details = "reference to unsanctioned schema: dbo" },
                },
            },

            // CREATE SYNONYM whose target is in an unsanctioned schema (CreateSynonymStatement.ForName).
            new TestScenariosWithExpected
            {
                Sql = "Create Synonym my_schema.my_syn For dbo.my_table",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 36, Details = "reference to unsanctioned schema: dbo" },
                },
            },

            // Foreign key REFERENCES a table in an unsanctioned schema (ForeignKeyConstraintDefinition.ReferenceTableName).
            new TestScenariosWithExpected
            {
                Sql = "Create Table my_schema.child (parent_id Int References dbo.parent (id))",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 55, Details = "reference to unsanctioned schema: dbo" },
                },
            },

            // SELECT INTO a table in an unsanctioned schema (SelectStatement.Into).
            new TestScenariosWithExpected
            {
                Sql = "Select Top 0 * Into dbo.new_t From my_schema.src",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 20, Details = "reference to unsanctioned schema: dbo" },
                },
            },

            // NEXT VALUE FOR a sequence in an unsanctioned schema (NextValueForExpression.SequenceName).
            new TestScenariosWithExpected
            {
                Sql = "Select Next Value For dbo.my_seq",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 22, Details = "reference to unsanctioned schema: dbo" },
                },
            },

            // DECLARE @var with a user-defined type in an unsanctioned schema (UserDataTypeReference.Name).
            new TestScenariosWithExpected
            {
                Sql = "Declare @x dbo.my_type",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 11, Details = "reference to unsanctioned schema: dbo" },
                },
            },

            // BULK INSERT into a table in an unsanctioned schema (BulkInsertStatement.To).
            new TestScenariosWithExpected
            {
                Sql = "Bulk Insert dbo.my_table From 'file.csv'",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 12, Details = "reference to unsanctioned schema: dbo" },
                },
            },

            // SET IDENTITY_INSERT against a table in an unsanctioned schema (SetIdentityInsertStatement.Table).
            new TestScenariosWithExpected
            {
                Sql = "Set Identity_Insert dbo.my_table On",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 20, Details = "reference to unsanctioned schema: dbo" },
                },
            },

            // CREATE SCHEMA whose name is unsanctioned (CreateSchemaStatement.Name is a bare Identifier).
            new TestScenariosWithExpected
            {
                Sql = "Create Schema dbo",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 14, Details = "reference to unsanctioned schema: dbo" },
                },
            },

            // DROP SCHEMA targeting an unsanctioned schema (DropSchemaStatement.Schema.BaseIdentifier).
            new TestScenariosWithExpected
            {
                Sql = "Drop Schema dbo",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 12, Details = "reference to unsanctioned schema: dbo" },
                },
            },

            // DROP SCHEMA with the schema name bracket-delimited.
            new TestScenariosWithExpected
            {
                Sql = "Drop Schema [dbo]",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 12, Details = "reference to unsanctioned schema: dbo" },
                },
            },

            // ALTER SCHEMA <unsanctioned-destination> TRANSFER <sanctioned-source> — destination is an Identifier.
            new TestScenariosWithExpected
            {
                Sql = "Alter Schema dbo Transfer my_schema.t",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 13, Details = "reference to unsanctioned schema: dbo" },
                },
            },

            // ALTER SCHEMA <sanctioned-destination> TRANSFER <unsanctioned-source> — source caught via Visit(SchemaObjectName).
            new TestScenariosWithExpected
            {
                Sql = "Alter Schema my_schema Transfer dbo.t",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 32, Details = "reference to unsanctioned schema: dbo" },
                },
            },

            // ALTER SCHEMA with both destination and source unsanctioned — two violations (one per override path).
            new TestScenariosWithExpected
            {
                Sql = "Alter Schema dbo Transfer sys.t",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 13, Details = "reference to unsanctioned schema: dbo" },
                    new ExpectedViolation { Offset = 26, Details = "reference to unsanctioned schema: sys" },
                },
            },

            // GRANT ON SCHEMA::<unsanctioned> — schema-scoped permission.
            new TestScenariosWithExpected
            {
                Sql = "Grant Select On Schema::dbo To my_user",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 24, Details = "reference to unsanctioned schema: dbo" },
                },
            },

            // GRANT ON OBJECT::<unsanctioned>.<table> — explicit Object kind.
            new TestScenariosWithExpected
            {
                Sql = "Grant Select On Object::dbo.t To my_user",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 24, Details = "reference to unsanctioned schema: dbo" },
                },
            },

            // GRANT ON <unsanctioned>.<table> — no kind prefix (SecurityObjectKind.NotSpecified).
            new TestScenariosWithExpected
            {
                Sql = "Grant Select On dbo.t To my_user",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 16, Details = "reference to unsanctioned schema: dbo" },
                },
            },

            // REVOKE ON SCHEMA::<unsanctioned>.
            new TestScenariosWithExpected
            {
                Sql = "Revoke Select On Schema::dbo From my_user",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 25, Details = "reference to unsanctioned schema: dbo" },
                },
            },

            // DENY ON <unsanctioned>.<table>.
            new TestScenariosWithExpected
            {
                Sql = "Deny Select On dbo.t To my_user",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 15, Details = "reference to unsanctioned schema: dbo" },
                },
            },

            // ALTER AUTHORIZATION ON SCHEMA::<unsanctioned> — same SecurityTargetObject path.
            new TestScenariosWithExpected
            {
                Sql = "Alter Authorization On Schema::dbo To my_user",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 31, Details = "reference to unsanctioned schema: dbo" },
                },
            },
        };

        [Fact]
        public static void Execute___Should_return_violations___When_SanctionedSchemasSqlScriptValidationRule_has_been_violated()
        {
            // Arrange
            var testScenariosWithExpected = SanctionedSchemasSqlScriptValidationRuleTestScenariosWithExpected;

            var rule = new SanctionedSchemasSqlScriptValidationRule(
                new[]
                {
                    "MY_SCHEMA",
                    "myschema2",
                });

            var operations = testScenariosWithExpected
                .Select(_ => new ValidateSqlScriptOp(SqlServerVersion, _.Sql, new[] { rule }))
                .ToList();

            var systemUnderTest = new ValidateSqlScriptProtocol();

            // Act
            var actual = operations.Select(_ => systemUnderTest.Execute(_)).ToList();

            // Assert
            actual.MustBeEqualTo(testScenariosWithExpected);
        }

        [Fact]
        public static void Execute___Should_return_no_violations___When_SanctionedSchemasSqlScriptValidationRule_has_not_been_violated()
        {
            // Arrange — reuse the DisallowedSchemas scenarios; sanction every schema they reference
            // (my_schema / myschema2 / dbo).  Database-qualifier "mydb" in 3-part names is not
            // extracted as a schema, so we don't need to sanction it.
            var testScenariosWithExpected = DisallowedSchemasSqlScriptValidationRuleTestScenariosWithExpected;

            var rule = new SanctionedSchemasSqlScriptValidationRule(
                new[]
                {
                    "MY_SCHEMA",
                    "myschema2",
                    "dbo",
                });

            var operations = testScenariosWithExpected
                .Select(_ => new ValidateSqlScriptOp(SqlServerVersion, _.Sql, new[] { rule }))
                .ToList();

            var systemUnderTest = new ValidateSqlScriptProtocol();

            // Act
            var actual = operations.Select(_ => systemUnderTest.Execute(_)).ToList();

            // Assert
            actual.MustNotHaveAnyViolations();
        }
    }
}
