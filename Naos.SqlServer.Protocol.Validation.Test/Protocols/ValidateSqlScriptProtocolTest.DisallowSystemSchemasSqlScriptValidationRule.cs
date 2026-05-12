// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidateSqlScriptProtocolTest.DisallowSystemSchemasSqlScriptValidationRule.cs" company="Naos Project">
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
        private static readonly IReadOnlyList<TestScenariosWithExpected> DisallowSystemSchemasSqlScriptValidationRuleTestScenariosWithExpected = new[]
        {
            // simple SELECT, schema-qualified table
            new TestScenariosWithExpected
            {
                Sql = "Select * From sys.objects",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 14, Details = "disallowed reference to system schema: sys" },
                },
            },

            // derived table (subquery in FROM)
            new TestScenariosWithExpected
            {
                Sql = "Select * From (Select * From sys.objects) A",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 29, Details = "disallowed reference to system schema: sys" },
                },
            },

            // INNER JOIN — violation is on the right-hand table
            new TestScenariosWithExpected
            {
                Sql = "Select a.id From dbo.other a Inner Join sys.objects b On a.id = b.id",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 40, Details = "disallowed reference to system schema: sys" },
                },
            },

            // LEFT JOIN with AS alias
            new TestScenariosWithExpected
            {
                Sql = "Select a.id From dbo.other a Left Join sys.objects As b On a.id = b.id",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 39, Details = "disallowed reference to system schema: sys" },
                },
            },

            // UPDATE target
            new TestScenariosWithExpected
            {
                Sql = "Update sys.objects Set x = 1",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 7, Details = "disallowed reference to system schema: sys" },
                },
            },

            // DELETE target
            new TestScenariosWithExpected
            {
                Sql = "Delete From sys.objects Where x = 1",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 12, Details = "disallowed reference to system schema: sys" },
                },
            },

            // INSERT target
            new TestScenariosWithExpected
            {
                Sql = "Insert Into sys.objects (x) Values (1)",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 12, Details = "disallowed reference to system schema: sys" },
                },
            },

            // MERGE target
            new TestScenariosWithExpected
            {
                Sql = "Merge Into sys.objects As t Using dbo.src As s On t.id = s.id When Matched Then Update Set x = s.x;",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 11, Details = "disallowed reference to system schema: sys" },
                },
            },

            // CTE body references a disallowed schema
            new TestScenariosWithExpected
            {
                Sql = "With cte As (Select id From sys.objects) Select * From cte",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 28, Details = "disallowed reference to system schema: sys" },
                },
            },

            // subquery in WHERE / IN
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.other Where id In (Select id From sys.objects)",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 52, Details = "disallowed reference to system schema: sys" },
                },
            },

            // EXISTS correlated subquery
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.other o Where Exists (Select 1 From sys.objects m Where m.id = o.id)",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 54, Details = "disallowed reference to system schema: sys" },
                },
            },

            // UNION — only one branch violates
            new TestScenariosWithExpected
            {
                Sql = "Select id From dbo.other Union Select id From sys.objects",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 46, Details = "disallowed reference to system schema: sys" },
                },
            },

            // CROSS APPLY with a subquery containing the violation
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.other o Cross Apply (Select Top 1 * From sys.objects m Where m.id = o.id) x",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 59, Details = "disallowed reference to system schema: sys" },
                },
            },

            // bracket-delimited schema identifier
            new TestScenariosWithExpected
            {
                Sql = "Select * From [INFORMATION_SCHEMA].objects",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 14, Details = "disallowed reference to system schema: INFORMATION_SCHEMA" },
                },
            },

            // bracket-delimited, uppercase to confirm case-insensitive matching
            new TestScenariosWithExpected
            {
                Sql = "Select * From [SYS].[objects]",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 14, Details = "disallowed reference to system schema: SYS" },
                },
            },

            // mixed case
            new TestScenariosWithExpected
            {
                Sql = "Select * From Sys.objects",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 14, Details = "disallowed reference to system schema: Sys" },
                },
            },

            // three-part name: database.schema.table
            new TestScenariosWithExpected
            {
                Sql = "Select * From mydb.sys.objects",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 14, Details = "disallowed reference to system schema: sys" },
                },
            },

            // INFORMATION_SCHEMA — the other system schema this rule enforces
            new TestScenariosWithExpected
            {
                Sql = "Select * From INFORMATION_SCHEMA.tables",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 14, Details = "disallowed reference to system schema: INFORMATION_SCHEMA" },
                },
            },

            // two violations in a single statement, one per system schema
            new TestScenariosWithExpected
            {
                Sql = "Select * From sys.t1 a Inner Join INFORMATION_SCHEMA.t2 b On a.id = b.id",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 14, Details = "disallowed reference to system schema: sys" },
                    new ExpectedViolation { Offset = 34, Details = "disallowed reference to system schema: INFORMATION_SCHEMA" },
                },
            },

            // multi-batch script separated by GO — one violation per batch
            new TestScenariosWithExpected
            {
                Sql = "Select * From sys.t1\r\nGO\r\nSelect * From INFORMATION_SCHEMA.t2",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 14, Details = "disallowed reference to system schema: sys" },
                    new ExpectedViolation { Offset = 40, Details = "disallowed reference to system schema: INFORMATION_SCHEMA" },
                },
            },

            // multi-statement (single batch), both statements violate
            new TestScenariosWithExpected
            {
                Sql = "Select * From sys.t1; Select * From INFORMATION_SCHEMA.t2;",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 14, Details = "disallowed reference to system schema: sys" },
                    new ExpectedViolation { Offset = 36, Details = "disallowed reference to system schema: INFORMATION_SCHEMA" },
                },
            },

            // the same disallowed table referenced twice — two distinct violations
            new TestScenariosWithExpected
            {
                Sql = "Select * From sys.objects a Inner Join sys.objects b On a.id = b.id",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 14, Details = "disallowed reference to system schema: sys" },
                    new ExpectedViolation { Offset = 39, Details = "disallowed reference to system schema: sys" },
                },
            },

            // EXEC of a stored procedure in a system schema (ProcedureReference holds a SchemaObjectName).
            new TestScenariosWithExpected
            {
                Sql = "Exec sys.sp_help",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 5, Details = "disallowed reference to system schema: sys" },
                },
            },

            // DROP TABLE (DropObjectsStatement.Objects is a list of SchemaObjectName).
            new TestScenariosWithExpected
            {
                Sql = "Drop Table information_schema.objects",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 11, Details = "disallowed reference to system schema: information_schema" },
                },
            },

            // TRUNCATE TABLE.
            new TestScenariosWithExpected
            {
                Sql = "Truncate Table sys.objects",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 15, Details = "disallowed reference to system schema: sys" },
                },
            },

            // ALTER TABLE.
            new TestScenariosWithExpected
            {
                Sql = "Alter Table sys.objects Add col1 Int Null",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 12, Details = "disallowed reference to system schema: sys" },
                },
            },

            // CREATE TABLE.  (SQL Server rejects this at runtime — the parser accepts it.)
            new TestScenariosWithExpected
            {
                Sql = "Create Table sys.objects (x Int)",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 13, Details = "disallowed reference to system schema: sys" },
                },
            },

            // CREATE INDEX ... ON.
            new TestScenariosWithExpected
            {
                Sql = "Create Index ix_objects_x On sys.objects (x)",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 29, Details = "disallowed reference to system schema: sys" },
                },
            },

            // Table-valued function in FROM (SchemaObjectFunctionTableReference holds a SchemaObjectName).
            new TestScenariosWithExpected
            {
                Sql = "Select * From sys.my_tvf()",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 14, Details = "disallowed reference to system schema: sys" },
                },
            },

            // Scalar function call in an expression — captured via Visit(FunctionCall) /
            // MultiPartIdentifierCallTarget (the qualifier excludes the function name).
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.other Where sys.fn_my_function(id) = 1",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 30, Details = "disallowed reference to system schema: sys" },
                },
            },

            // Three-part column reference (schema.table.col) — captured via Visit(ColumnReferenceExpression).
            new TestScenariosWithExpected
            {
                Sql = "Select 1 From dbo.alpha Where sys.t.col = 1",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 30, Details = "disallowed reference to system schema: sys" },
                },
            },

            // Four-part column reference (database.schema.table.col).
            new TestScenariosWithExpected
            {
                Sql = "Select 1 From dbo.alpha Where mydb.sys.t.col = 1",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 30, Details = "disallowed reference to system schema: sys" },
                },
            },

            // CREATE PROCEDURE in a system schema (ProcedureReference.Name).
            new TestScenariosWithExpected
            {
                Sql = "Create Procedure sys.my_sp As Begin Select 1 End",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 17, Details = "disallowed reference to system schema: sys" },
                },
            },

            // CREATE FUNCTION in a system schema.
            new TestScenariosWithExpected
            {
                Sql = "Create Function sys.my_fn () Returns Int As Begin Return 1 End",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 16, Details = "disallowed reference to system schema: sys" },
                },
            },

            // CREATE VIEW in a system schema.
            new TestScenariosWithExpected
            {
                Sql = "Create View sys.my_view As Select 1 As x",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 12, Details = "disallowed reference to system schema: sys" },
                },
            },

            // CREATE TRIGGER whose target table is in a system schema (TriggerObject.Name).
            new TestScenariosWithExpected
            {
                Sql = "Create Trigger dbo.my_tr On sys.my_table After Insert As Begin Select 1 End",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 28, Details = "disallowed reference to system schema: sys" },
                },
            },

            // CREATE SYNONYM whose target is in a system schema (CreateSynonymStatement.ForName).
            new TestScenariosWithExpected
            {
                Sql = "Create Synonym dbo.my_syn For sys.my_table",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 30, Details = "disallowed reference to system schema: sys" },
                },
            },

            // Foreign key REFERENCES a table in a system schema (ForeignKeyConstraintDefinition.ReferenceTableName).
            new TestScenariosWithExpected
            {
                Sql = "Create Table dbo.child (parent_id Int References sys.parent (id))",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 49, Details = "disallowed reference to system schema: sys" },
                },
            },

            // SELECT INTO a table in a system schema (SelectStatement.Into).
            new TestScenariosWithExpected
            {
                Sql = "Select Top 0 * Into sys.new_t From dbo.src",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 20, Details = "disallowed reference to system schema: sys" },
                },
            },

            // NEXT VALUE FOR a sequence in a system schema (NextValueForExpression.SequenceName).
            new TestScenariosWithExpected
            {
                Sql = "Select Next Value For sys.my_seq",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 22, Details = "disallowed reference to system schema: sys" },
                },
            },

            // DECLARE @var with a user-defined type in a system schema (UserDataTypeReference.Name).
            new TestScenariosWithExpected
            {
                Sql = "Declare @x sys.my_type",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 11, Details = "disallowed reference to system schema: sys" },
                },
            },

            // BULK INSERT into a table in a system schema (BulkInsertStatement.To).
            new TestScenariosWithExpected
            {
                Sql = "Bulk Insert sys.my_table From 'file.csv'",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 12, Details = "disallowed reference to system schema: sys" },
                },
            },

            // SET IDENTITY_INSERT against a table in a system schema (SetIdentityInsertStatement.Table).
            new TestScenariosWithExpected
            {
                Sql = "Set Identity_Insert sys.my_table On",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 20, Details = "disallowed reference to system schema: sys" },
                },
            },

            // CREATE SCHEMA whose name is a system schema (CreateSchemaStatement.Name is a bare Identifier).
            // (SQL Server rejects this at runtime — the parser accepts it.)
            new TestScenariosWithExpected
            {
                Sql = "Create Schema sys",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 14, Details = "disallowed reference to system schema: sys" },
                },
            },

            // DROP SCHEMA targeting a system schema (DropSchemaStatement.Schema.BaseIdentifier).
            new TestScenariosWithExpected
            {
                Sql = "Drop Schema sys",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 12, Details = "disallowed reference to system schema: sys" },
                },
            },

            // DROP SCHEMA with the schema name bracket-delimited.
            new TestScenariosWithExpected
            {
                Sql = "Drop Schema [sys]",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 12, Details = "disallowed reference to system schema: sys" },
                },
            },

            // ALTER SCHEMA <system-destination> TRANSFER <user-source> — destination is an Identifier.
            new TestScenariosWithExpected
            {
                Sql = "Alter Schema sys Transfer dbo.t",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 13, Details = "disallowed reference to system schema: sys" },
                },
            },

            // ALTER SCHEMA <user-destination> TRANSFER <system-source> — source is caught via Visit(SchemaObjectName).
            new TestScenariosWithExpected
            {
                Sql = "Alter Schema dbo Transfer sys.t",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 26, Details = "disallowed reference to system schema: sys" },
                },
            },

            // ALTER SCHEMA with both destination and source as system schemas — two violations (one per override path).
            new TestScenariosWithExpected
            {
                Sql = "Alter Schema sys Transfer INFORMATION_SCHEMA.t",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 13, Details = "disallowed reference to system schema: sys" },
                    new ExpectedViolation { Offset = 26, Details = "disallowed reference to system schema: INFORMATION_SCHEMA" },
                },
            },

            // GRANT ON SCHEMA::<system> — schema-scoped permission.
            new TestScenariosWithExpected
            {
                Sql = "Grant Select On Schema::sys To my_user",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 24, Details = "disallowed reference to system schema: sys" },
                },
            },

            // GRANT ON OBJECT::<system>.<table> — explicit Object kind.
            new TestScenariosWithExpected
            {
                Sql = "Grant Select On Object::sys.t To my_user",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 24, Details = "disallowed reference to system schema: sys" },
                },
            },

            // GRANT ON <system>.<table> — no kind prefix (SecurityObjectKind.NotSpecified).
            new TestScenariosWithExpected
            {
                Sql = "Grant Select On sys.t To my_user",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 16, Details = "disallowed reference to system schema: sys" },
                },
            },

            // REVOKE ON SCHEMA::<system>.
            new TestScenariosWithExpected
            {
                Sql = "Revoke Select On Schema::sys From my_user",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 25, Details = "disallowed reference to system schema: sys" },
                },
            },

            // DENY ON <system>.<table>.
            new TestScenariosWithExpected
            {
                Sql = "Deny Select On sys.t To my_user",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 15, Details = "disallowed reference to system schema: sys" },
                },
            },

            // ALTER AUTHORIZATION ON SCHEMA::<system> — same SecurityTargetObject path.
            new TestScenariosWithExpected
            {
                Sql = "Alter Authorization On Schema::sys To my_user",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 31, Details = "disallowed reference to system schema: sys" },
                },
            },
        };

        [Fact]
        public static void Execute___Should_return_violations___When_DisallowSystemSchemasSqlScriptValidationRule_has_been_violated()
        {
            // Arrange
            var testScenariosWithExpected = DisallowSystemSchemasSqlScriptValidationRuleTestScenariosWithExpected;

            var rule = new DisallowSystemSchemasSqlScriptValidationRule();

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
        public static void Execute___Should_return_no_violations___When_DisallowSystemSchemasSqlScriptValidationRule_has_not_been_violated()
        {
            // Arrange
            // The DisallowSchemas scenarios reference only non-system schemas (my_schema, myschema2, mydb, dbo, …), none of which DisallowSystemSchemas flags.
            var testScenariosWithExpected = DisallowSchemasSqlScriptValidationRuleTestScenariosWithExpected;

            var rule = new DisallowSystemSchemasSqlScriptValidationRule();

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
