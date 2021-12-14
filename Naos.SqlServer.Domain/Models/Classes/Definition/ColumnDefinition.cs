// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColumnDefinition.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Naos.CodeAnalysis.Recipes;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Type;

    /// <summary>
    /// Defines a column in a table.
    /// </summary>
    public partial class ColumnDefinition : IModelViaCodeGen
    {
        /// <summary>
        /// The characters that are allowed in a column name, in addition to alphanumeric characters.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = NaosSuppressBecause.CA2104_DoNotDeclareReadOnlyMutableReferenceTypes_TypeIsImmutable)]
        public static readonly IReadOnlyCollection<char> ColumnNameAlphanumericOtherAllowedCharacters = new[] { '_' };

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnDefinition"/> class.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        /// <param name="sqlDataType">The SQL data type of the column.</param>
        public ColumnDefinition(
            string name,
            SqlDataTypeRepresentationBase sqlDataType)
        {
            name.MustForArg(nameof(name)).NotBeNullNorWhiteSpace().And().BeAlphanumeric(ColumnNameAlphanumericOtherAllowedCharacters);
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
