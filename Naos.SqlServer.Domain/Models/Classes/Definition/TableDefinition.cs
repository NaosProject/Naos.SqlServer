// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TableDefinition.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System.Collections.Generic;
    using System.Linq;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Type;

    /// <summary>
    /// Defines a table in a database.
    /// </summary>
    public partial class TableDefinition : IModelViaCodeGen
    {
        /// <summary>
        /// The characters that are allowed in a table name, in addition to alphanumeric characters.
        /// </summary>
        public static readonly IReadOnlyCollection<char> TableNameAlphanumericOtherAllowedCharacters = new[] { ' ', '_' };

        /// <summary>
        /// The characters that are allowed in a table schema name, in addition to alphanumeric characters.
        /// </summary>
        public static readonly IReadOnlyCollection<char> TableSchemaNameAlphanumericOtherAllowedCharacters = new[] { ' ', '_' };

        /// <summary>
        /// Initializes a new instance of the <see cref="TableDefinition"/> class.
        /// </summary>
        /// <param name="name">The name of the table.</param>
        /// <param name="columns">The columns.</param>
        public TableDefinition(
            string name,
            IReadOnlyList<ColumnDefinition> columns)
        {
            name.MustForArg(nameof(name)).NotBeNullNorWhiteSpace().And().BeAlphanumeric(TableNameAlphanumericOtherAllowedCharacters);
            columns.MustForArg(nameof(columns)).NotBeNullNorEmptyEnumerableNorContainAnyNulls();
            columns.Select(_ => _.Name.ToUpperInvariant()).MustForArg("case-insensitive column names").ContainOnlyDistinctElements();

            this.Name = name;
            this.Columns = columns;
        }

        /// <summary>
        /// Gets the name of the table.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the columns.
        /// </summary>
        public IReadOnlyList<ColumnDefinition> Columns { get; private set; }
    }
}
