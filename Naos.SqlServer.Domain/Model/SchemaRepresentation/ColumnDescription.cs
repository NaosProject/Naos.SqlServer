// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColumnDescription.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using OBeautifulCode.Type;

    /// <summary>
    /// Detailed information about the column.
    /// </summary>
    public partial class ColumnDescription : IModelViaCodeGen
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnDescription"/> class.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="ordinalPosition">The ordinal position.</param>
        /// <param name="columnDefault">The column default value.</param>
        /// <param name="isNullable">if set to <c>true</c> [is nullable].</param>
        /// <param name="dataType">Type of the data.</param>
        public ColumnDescription(
            string columnName,
            int ordinalPosition,
            string columnDefault,
            bool isNullable,
            string dataType)
        {
            this.ColumnName = columnName;
            this.OrdinalPosition = ordinalPosition;
            this.ColumnDefault = columnDefault;
            this.IsNullable = isNullable;
            this.DataType = dataType;
        }

        /// <summary>
        /// Gets the name of the column.
        /// </summary>
        public string ColumnName { get; private set; }

        /// <summary>
        /// Gets the ordinal position of the column.
        /// </summary>
        public int OrdinalPosition { get; private set; }

        /// <summary>
        /// Gets the default value of the column.
        /// </summary>
        public string ColumnDefault { get; private set; }

        /// <summary>
        /// Gets a value indicating whether or not the column is nullable.
        /// </summary>
        public bool IsNullable { get; private set; }

        /// <summary>
        /// Gets the data type of the column.
        /// </summary>
        public string DataType { get; private set; }
    }
}