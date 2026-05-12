// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SchemaQualifiedTableName.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Type;
    using static System.FormattableString;

    /// <summary>
    /// A schema-qualified table name — a pair of schema and table identifiers
    /// (e.g. <c>dbo.users</c>).
    /// </summary>
    public partial class SchemaQualifiedTableName : IModelViaCodeGen, IDeclareToStringMethod
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SchemaQualifiedTableName"/> class.
        /// </summary>
        /// <param name="schemaName">The schema name.</param>
        /// <param name="tableName">The table name.</param>
        public SchemaQualifiedTableName(
            string schemaName,
            string tableName)
        {
            new { schemaName }.AsArg().Must().NotBeNullNorWhiteSpace();
            new { tableName }.AsArg().Must().NotBeNullNorWhiteSpace();

            this.SchemaName = schemaName;
            this.TableName = tableName;
        }

        /// <summary>
        /// Gets the schema name.
        /// </summary>
        public string SchemaName { get; private set; }

        /// <summary>
        /// Gets the table name.
        /// </summary>
        public string TableName { get; private set; }

        /// <inheritdoc cref="IDeclareToStringMethod" />
        public override string ToString()
        {
            var result = Invariant($"{this.SchemaName}.{this.TableName}");

            return result;
        }
    }
}
