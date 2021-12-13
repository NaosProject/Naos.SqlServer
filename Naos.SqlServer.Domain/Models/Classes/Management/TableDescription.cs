// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TableDescription.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System.Collections.Generic;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Type;

    /// <summary>
    /// Detailed information about a table.
    /// </summary>
    public partial class TableDescription : IModelViaCodeGen
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableDescription"/> class.
        /// </summary>
        /// <param name="databaseName">Name of the database.</param>
        /// <param name="tableSchema">The table schema (e.g. 'dbo').</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="columns">The columns.</param>
        public TableDescription(
            string databaseName,
            string tableSchema,
            string tableName,
            IReadOnlyCollection<ColumnDescription> columns)
        {
            databaseName.MustForArg(nameof(databaseName)).NotBeNullNorWhiteSpace().And().BeAlphanumeric(DatabaseDefinition.DatabaseNameAlphanumericOtherAllowedCharacters);
            tableSchema.MustForArg(nameof(tableSchema)).NotBeNullNorWhiteSpace().And().BeAlphanumeric(TableDefinition.TableSchemaNameAlphanumericOtherAllowedCharacters);
            tableName.MustForArg(nameof(tableName)).NotBeNullNorWhiteSpace().And().BeAlphanumeric(TableDefinition.TableNameAlphanumericOtherAllowedCharacters);
            columns.MustForArg(nameof(columns)).NotBeNullNorEmptyEnumerableNorContainAnyNulls();

            this.DatabaseName = databaseName;
            this.TableSchema = tableSchema;
            this.TableName = tableName;
            this.Columns = columns;
        }

        /// <summary>
        /// Gets the name of the database the table is from.
        /// </summary>
        public string DatabaseName { get; private set; }

        /// <summary>
        /// Gets the schema of the table.
        /// </summary>
        public string TableSchema { get; private set; }

        /// <summary>
        /// Gets the name of the table.
        /// </summary>
        public string TableName { get; private set; }

        /// <summary>
        /// Gets the columns descriptions of the table.
        /// </summary>
        public IReadOnlyCollection<ColumnDescription> Columns { get; private set; }
    }
}