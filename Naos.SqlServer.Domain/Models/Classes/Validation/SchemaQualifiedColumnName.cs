// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SchemaQualifiedColumnName.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Type;
    using static System.FormattableString;

    /// <summary>
    /// A schema-qualified column name — a triple of schema, table, and column identifiers
    /// (e.g. <c>dbo.users.entity_id</c>).
    /// </summary>
    /// <remarks>
    /// Used as a canonical key for filter-validation rules that need to identify a specific
    /// column unambiguously, regardless of how the column is written in the source SQL
    /// (bare <c>entity_id</c>, alias-qualified <c>u.entity_id</c>, or fully qualified
    /// <c>dbo.users.entity_id</c>).
    /// </remarks>
    public partial class SchemaQualifiedColumnName : IModelViaCodeGen, IDeclareToStringMethod
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SchemaQualifiedColumnName"/> class.
        /// </summary>
        /// <param name="schemaName">The schema name.</param>
        /// <param name="tableName">The table name.</param>
        /// <param name="columnName">The column name.</param>
        public SchemaQualifiedColumnName(
            string schemaName,
            string tableName,
            string columnName)
        {
            new { schemaName }.AsArg().Must().NotBeNullNorWhiteSpace();
            new { tableName }.AsArg().Must().NotBeNullNorWhiteSpace();
            new { columnName }.AsArg().Must().NotBeNullNorWhiteSpace();

            this.SchemaName = schemaName;
            this.TableName = tableName;
            this.ColumnName = columnName;
        }

        /// <summary>
        /// Gets the schema name.
        /// </summary>
        public string SchemaName { get; private set; }

        /// <summary>
        /// Gets the table name.
        /// </summary>
        public string TableName { get; private set; }

        /// <summary>
        /// Gets the column name.
        /// </summary>
        public string ColumnName { get; private set; }

        /// <inheritdoc cref="IDeclareToStringMethod" />
        public override string ToString()
        {
            var result = Invariant($"{this.SchemaName}.{this.TableName}.{this.ColumnName}");

            return result;
        }
    }
}
