// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TableRepresentation.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System.Collections.Generic;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Type;

    /// <summary>
    /// Top level base of a SQL Parameter.
    /// </summary>
    public class TableRepresentation : IModelViaCodeGen
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableRepresentation"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="columnNameToRepresentationMap">The column name to representation map.</param>
        public TableRepresentation(
            string name,
            IReadOnlyDictionary<string, ColumnRepresentation> columnNameToRepresentationMap)
        {
            name.MustForArg(nameof(name)).NotBeNull().And().BeAlphanumeric();

            this.Name = name;
            this.ColumnNameToRepresentationMap = columnNameToRepresentationMap;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the column name to representation map.
        /// </summary>
        /// <value>The column name to representation map.</value>
        public IReadOnlyDictionary<string, ColumnRepresentation> ColumnNameToRepresentationMap { get; private set; }
    }
}
