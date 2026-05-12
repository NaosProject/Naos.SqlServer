// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidateSqlScriptProtocolTest.DisallowedSchemasSqlScriptValidationRule.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Protocol.Validation.Test
{
    using System.Collections.Generic;
    using System.Linq;
    using FakeItEasy;
    using Naos.SqlServer.Domain;
    using Xunit;

    public static partial class ValidateSqlScriptProtocolTest
    {
        private static readonly IReadOnlyList<TestScenariosWithExpected> DisallowedSchemasSqlScriptValidationRuleTestScenariosWithExpected = new[]
        {
            // simple SELECT, schema-qualified table
            new TestScenariosWithExpected
            {
                Sql = "Select * From my_schema.my_table",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 14, Details = "reference to disallowed schema: my_schema" },
                },
            },

            // derived table (subquery in FROM)
            new TestScenariosWithExpected
            {
                Sql = "Select * From (Select * From my_schema.my_table) A",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 29, Details = "reference to disallowed schema: my_schema" },
                },
            },

            // INNER JOIN — violation is on the right-hand table
            new TestScenariosWithExpected
            {
                Sql = "Select a.id From dbo.other a Inner Join my_schema.my_table b On a.id = b.id",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 40, Details = "reference to disallowed schema: my_schema" },
                },
            },

            // LEFT JOIN with AS alias
            new TestScenariosWithExpected
            {
                Sql = "Select a.id From dbo.other a Left Join my_schema.my_table As b On a.id = b.id",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 39, Details = "reference to disallowed schema: my_schema" },
                },
            },

            // UPDATE target
            new TestScenariosWithExpected
            {
                Sql = "Update my_schema.my_table Set x = 1",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 7, Details = "reference to disallowed schema: my_schema" },
                },
            },

            // DELETE target
            new TestScenariosWithExpected
            {
                Sql = "Delete From my_schema.my_table Where x = 1",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 12, Details = "reference to disallowed schema: my_schema" },
                },
            },

            // INSERT target
            new TestScenariosWithExpected
            {
                Sql = "Insert Into my_schema.my_table (x) Values (1)",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 12, Details = "reference to disallowed schema: my_schema" },
                },
            },

            // MERGE target
            new TestScenariosWithExpected
            {
                Sql = "Merge Into my_schema.my_table As t Using dbo.src As s On t.id = s.id When Matched Then Update Set x = s.x;",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 11, Details = "reference to disallowed schema: my_schema" },
                },
            },

            // CTE body references a disallowed schema
            new TestScenariosWithExpected
            {
                Sql = "With cte As (Select id From my_schema.my_table) Select * From cte",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 28, Details = "reference to disallowed schema: my_schema" },
                },
            },

            // subquery in WHERE / IN
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.other Where id In (Select id From my_schema.my_table)",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 52, Details = "reference to disallowed schema: my_schema" },
                },
            },

            // EXISTS correlated subquery
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.other o Where Exists (Select 1 From my_schema.my_table m Where m.id = o.id)",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 54, Details = "reference to disallowed schema: my_schema" },
                },
            },

            // UNION — only one branch violates
            new TestScenariosWithExpected
            {
                Sql = "Select id From dbo.other Union Select id From my_schema.my_table",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 46, Details = "reference to disallowed schema: my_schema" },
                },
            },

            // CROSS APPLY with a subquery containing the violation
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.other o Cross Apply (Select Top 1 * From my_schema.my_table m Where m.id = o.id) x",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 59, Details = "reference to disallowed schema: my_schema" },
                },
            },

            // bracket-delimited schema identifier
            new TestScenariosWithExpected
            {
                Sql = "Select * From [my_schema].my_table",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 14, Details = "reference to disallowed schema: my_schema" },
                },
            },

            // bracket-delimited, uppercase to confirm case-insensitive matching
            new TestScenariosWithExpected
            {
                Sql = "Select * From [MY_SCHEMA].[my_table]",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 14, Details = "reference to disallowed schema: MY_SCHEMA" },
                },
            },

            // mixed case
            new TestScenariosWithExpected
            {
                Sql = "Select * From My_Schema.my_table",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 14, Details = "reference to disallowed schema: My_Schema" },
                },
            },

            // three-part name: database.schema.table
            new TestScenariosWithExpected
            {
                Sql = "Select * From mydb.my_schema.my_table",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 14, Details = "reference to disallowed schema: my_schema" },
                },
            },

            // the second disallowed schema configured on the rule
            new TestScenariosWithExpected
            {
                Sql = "Select * From myschema2.my_table",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 14, Details = "reference to disallowed schema: myschema2" },
                },
            },

            // two violations in a single statement, two different disallowed schemas
            new TestScenariosWithExpected
            {
                Sql = "Select * From my_schema.t1 a Inner Join myschema2.t2 b On a.id = b.id",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 14, Details = "reference to disallowed schema: my_schema" },
                    new ExpectedViolation { Offset = 40, Details = "reference to disallowed schema: myschema2" },
                },
            },

            // multi-batch script separated by GO — one violation per batch
            new TestScenariosWithExpected
            {
                Sql = "Select * From my_schema.t1\r\nGO\r\nSelect * From myschema2.t2",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 14, Details = "reference to disallowed schema: my_schema" },
                    new ExpectedViolation { Offset = 46, Details = "reference to disallowed schema: myschema2" },
                },
            },

            // multi-statement (single batch), both statements violate
            new TestScenariosWithExpected
            {
                Sql = "Select * From my_schema.t1; Select * From myschema2.t2;",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 14, Details = "reference to disallowed schema: my_schema" },
                    new ExpectedViolation { Offset = 42, Details = "reference to disallowed schema: myschema2" },
                },
            },

            // the same disallowed table referenced twice — two distinct violations
            new TestScenariosWithExpected
            {
                Sql = "Select * From my_schema.my_table a Inner Join my_schema.my_table b On a.id = b.id",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 14, Details = "reference to disallowed schema: my_schema" },
                    new ExpectedViolation { Offset = 46, Details = "reference to disallowed schema: my_schema" },
                },
            },

            // EXEC of a stored procedure in a disallowed schema (ProcedureReference holds a SchemaObjectName).
            new TestScenariosWithExpected
            {
                Sql = "Exec my_schema.my_sproc",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 5, Details = "reference to disallowed schema: my_schema" },
                },
            },

            // DROP TABLE (DropObjectsStatement.Objects is a list of SchemaObjectName).
            new TestScenariosWithExpected
            {
                Sql = "Drop Table my_schema.my_table",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 11, Details = "reference to disallowed schema: my_schema" },
                },
            },

            // TRUNCATE TABLE.
            new TestScenariosWithExpected
            {
                Sql = "Truncate Table my_schema.my_table",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 15, Details = "reference to disallowed schema: my_schema" },
                },
            },

            // ALTER TABLE.
            new TestScenariosWithExpected
            {
                Sql = "Alter Table my_schema.my_table Add col1 Int Null",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 12, Details = "reference to disallowed schema: my_schema" },
                },
            },

            // CREATE TABLE.
            new TestScenariosWithExpected
            {
                Sql = "Create Table my_schema.my_table (x Int)",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 13, Details = "reference to disallowed schema: my_schema" },
                },
            },

            // CREATE INDEX ... ON.
            new TestScenariosWithExpected
            {
                Sql = "Create Index ix_my_table_x On my_schema.my_table (x)",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 30, Details = "reference to disallowed schema: my_schema" },
                },
            },

            // Table-valued function in FROM (SchemaObjectFunctionTableReference holds a SchemaObjectName).
            new TestScenariosWithExpected
            {
                Sql = "Select * From my_schema.my_tvf()",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 14, Details = "reference to disallowed schema: my_schema" },
                },
            },

            // Scalar function call in an expression — captured via Visit(FunctionCall) /
            // MultiPartIdentifierCallTarget (the qualifier excludes the function name).
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.other Where my_schema.is_active(id) = 1",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 30, Details = "reference to disallowed schema: my_schema" },
                },
            },

            // Three-part column reference (schema.table.col) — captured via Visit(ColumnReferenceExpression).
            new TestScenariosWithExpected
            {
                Sql = "Select 1 From dbo.alpha Where my_schema.t.col = 1",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 30, Details = "reference to disallowed schema: my_schema" },
                },
            },

            // Four-part column reference (database.schema.table.col).
            new TestScenariosWithExpected
            {
                Sql = "Select 1 From dbo.alpha Where mydb.my_schema.t.col = 1",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 30, Details = "reference to disallowed schema: my_schema" },
                },
            },

            // CREATE PROCEDURE in a disallowed schema (ProcedureReference.Name).
            new TestScenariosWithExpected
            {
                Sql = "Create Procedure my_schema.my_sp As Begin Select 1 End",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 17, Details = "reference to disallowed schema: my_schema" },
                },
            },

            // CREATE FUNCTION in a disallowed schema.
            new TestScenariosWithExpected
            {
                Sql = "Create Function my_schema.my_fn () Returns Int As Begin Return 1 End",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 16, Details = "reference to disallowed schema: my_schema" },
                },
            },

            // CREATE VIEW in a disallowed schema.
            new TestScenariosWithExpected
            {
                Sql = "Create View my_schema.my_view As Select 1 As x",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 12, Details = "reference to disallowed schema: my_schema" },
                },
            },

            // CREATE TRIGGER whose target table is in a disallowed schema (TriggerObject.Name).
            new TestScenariosWithExpected
            {
                Sql = "Create Trigger dbo.my_tr On my_schema.my_table After Insert As Begin Select 1 End",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 28, Details = "reference to disallowed schema: my_schema" },
                },
            },

            // CREATE SYNONYM whose target is in a disallowed schema (CreateSynonymStatement.ForName).
            new TestScenariosWithExpected
            {
                Sql = "Create Synonym dbo.my_syn For my_schema.my_table",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 30, Details = "reference to disallowed schema: my_schema" },
                },
            },

            // Foreign key REFERENCES a table in a disallowed schema (ForeignKeyConstraintDefinition.ReferenceTableName).
            new TestScenariosWithExpected
            {
                Sql = "Create Table dbo.child (parent_id Int References my_schema.parent (id))",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 49, Details = "reference to disallowed schema: my_schema" },
                },
            },

            // SELECT INTO a table in a disallowed schema (SelectStatement.Into).
            new TestScenariosWithExpected
            {
                Sql = "Select Top 0 * Into my_schema.new_t From dbo.src",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 20, Details = "reference to disallowed schema: my_schema" },
                },
            },

            // NEXT VALUE FOR a sequence in a disallowed schema (NextValueForExpression.SequenceName).
            new TestScenariosWithExpected
            {
                Sql = "Select Next Value For my_schema.my_seq",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 22, Details = "reference to disallowed schema: my_schema" },
                },
            },

            // DECLARE @var with a user-defined type in a disallowed schema (UserDataTypeReference.Name).
            new TestScenariosWithExpected
            {
                Sql = "Declare @x my_schema.my_type",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 11, Details = "reference to disallowed schema: my_schema" },
                },
            },

            // BULK INSERT into a table in a disallowed schema (BulkInsertStatement.To).
            new TestScenariosWithExpected
            {
                Sql = "Bulk Insert my_schema.my_table From 'file.csv'",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 12, Details = "reference to disallowed schema: my_schema" },
                },
            },

            // SET IDENTITY_INSERT against a table in a disallowed schema (SetIdentityInsertStatement.Table).
            new TestScenariosWithExpected
            {
                Sql = "Set Identity_Insert my_schema.my_table On",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 20, Details = "reference to disallowed schema: my_schema" },
                },
            },

            // CREATE SCHEMA whose name is disallowed (CreateSchemaStatement.Name is a bare Identifier).
            new TestScenariosWithExpected
            {
                Sql = "Create Schema my_schema",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 14, Details = "reference to disallowed schema: my_schema" },
                },
            },

            // DROP SCHEMA whose name is disallowed (DropSchemaStatement.Schema.BaseIdentifier).
            new TestScenariosWithExpected
            {
                Sql = "Drop Schema my_schema",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 12, Details = "reference to disallowed schema: my_schema" },
                },
            },

            // DROP SCHEMA with the schema name bracket-delimited.
            new TestScenariosWithExpected
            {
                Sql = "Drop Schema [my_schema]",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 12, Details = "reference to disallowed schema: my_schema" },
                },
            },

            // ALTER SCHEMA <disallowed-destination> TRANSFER <allowed-source> — destination is an Identifier.
            new TestScenariosWithExpected
            {
                Sql = "Alter Schema my_schema Transfer dbo.t",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 13, Details = "reference to disallowed schema: my_schema" },
                },
            },

            // ALTER SCHEMA <allowed-destination> TRANSFER <disallowed-source> — source is caught via Visit(SchemaObjectName).
            new TestScenariosWithExpected
            {
                Sql = "Alter Schema dbo Transfer my_schema.t",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 26, Details = "reference to disallowed schema: my_schema" },
                },
            },

            // ALTER SCHEMA with both destination and source disallowed — two violations (one per override path).
            new TestScenariosWithExpected
            {
                Sql = "Alter Schema my_schema Transfer myschema2.t",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 13, Details = "reference to disallowed schema: my_schema" },
                    new ExpectedViolation { Offset = 32, Details = "reference to disallowed schema: myschema2" },
                },
            },

            // GRANT ON SCHEMA::<disallowed> — schema-scoped permission.
            new TestScenariosWithExpected
            {
                Sql = "Grant Select On Schema::my_schema To my_user",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 24, Details = "reference to disallowed schema: my_schema" },
                },
            },

            // GRANT ON OBJECT::<disallowed>.<table> — explicit Object kind.
            new TestScenariosWithExpected
            {
                Sql = "Grant Select On Object::my_schema.t To my_user",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 24, Details = "reference to disallowed schema: my_schema" },
                },
            },

            // GRANT ON <disallowed>.<table> — no kind prefix (SecurityObjectKind.NotSpecified).
            new TestScenariosWithExpected
            {
                Sql = "Grant Select On my_schema.t To my_user",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 16, Details = "reference to disallowed schema: my_schema" },
                },
            },

            // REVOKE ON SCHEMA::<disallowed>.
            new TestScenariosWithExpected
            {
                Sql = "Revoke Select On Schema::my_schema From my_user",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 25, Details = "reference to disallowed schema: my_schema" },
                },
            },

            // DENY ON <disallowed>.<table>.
            new TestScenariosWithExpected
            {
                Sql = "Deny Select On my_schema.t To my_user",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 15, Details = "reference to disallowed schema: my_schema" },
                },
            },

            // ALTER AUTHORIZATION ON SCHEMA::<disallowed> — same SecurityTargetObject path.
            new TestScenariosWithExpected
            {
                Sql = "Alter Authorization On Schema::my_schema To my_user",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 31, Details = "reference to disallowed schema: my_schema" },
                },
            },
        };

        [Fact]
        public static void Execute___Should_return_violations___When_DisallowedSchemasSqlScriptValidationRule_has_been_violated()
        {
            // Arrange
            var testScenariosWithExpected = DisallowedSchemasSqlScriptValidationRuleTestScenariosWithExpected;

            var rule = new DisallowedSchemasSqlScriptValidationRule(
                new[]
                {
                    A.Dummy<string>(),
                    "MY_SCHEMA",
                    "myschema2",
                    A.Dummy<string>(),
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
        public static void Execute___Should_return_no_violations___When_DisallowedSchemasSqlScriptValidationRule_has_not_been_violated()
        {
            // Arrange
            var testScenariosWithExpected = DisallowedSchemasSqlScriptValidationRuleTestScenariosWithExpected;

            var rule = new DisallowedSchemasSqlScriptValidationRule(
                new[]
                {
                    A.Dummy<string>(),
                    A.Dummy<string>(),
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
