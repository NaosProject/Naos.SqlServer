// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlInputParameterRepresentation{TValue}.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Type;

    /// <summary>
    /// A representation of a SQL input parameter.
    /// </summary>
    /// <typeparam name="TValue">Type of the input value.</typeparam>
    // ReSharper disable once RedundantExtendsListEntry
    public partial class SqlInputParameterRepresentation<TValue> : SqlInputParameterRepresentationBase, IModelViaCodeGen
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlInputParameterRepresentation{TValue}"/> class.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="sqlDataType">The SQL data type of the parameter.</param>
        /// <param name="value">The value of the parameter.</param>
        public SqlInputParameterRepresentation(
            string name,
            SqlDataTypeRepresentationBase sqlDataType,
            TValue value)
            : base(name, sqlDataType)
        {
            sqlDataType.MustForArg(nameof(sqlDataType)).NotBeNull();

            this.ThrowArgumentExceptionIfSqlDataTypeIsNotCompatibleWithDotNetDataType(sqlDataType, typeof(TValue));

            this.Value = value;
        }

        /// <summary>
        /// Gets the value of the parameter.
        /// </summary>
        public TValue Value { get; private set; }
    }
}
