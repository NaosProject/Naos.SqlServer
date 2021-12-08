// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColumnRepresentation.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Type;

    /// <summary>
    /// A representation of a column in a table.
    /// </summary>
    public partial class ColumnRepresentation : IModelViaCodeGen
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnRepresentation"/> class.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        /// <param name="sqlDataType">The SQL data type of the column.</param>
        public ColumnRepresentation(
            string name,
            SqlDataTypeRepresentationBase sqlDataType)
        {
            name.MustForArg(nameof(name)).NotBeNullNorWhiteSpace().And().BeAlphanumeric();
            sqlDataType.MustForArg(nameof(sqlDataType)).NotBeNull();

            this.Name = name;
            this.SqlDataType = sqlDataType;
        }

        /// <summary>
        /// Gets the name of the column.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the SQL data type of the column.
        /// </summary>
        public SqlDataTypeRepresentationBase SqlDataType { get; private set; }
    }
}
