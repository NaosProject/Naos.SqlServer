// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlInputParameterRepresentation{TValue}.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Type;
    using OBeautifulCode.Type.Recipes;

    /// <summary>
    /// Top level .
    /// </summary>
    /// <typeparam name="TValue">Type of the input value.</typeparam>
    public partial class SqlInputParameterRepresentation<TValue> : SqlParameterRepresentationBase, IModelViaCodeGen
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlInputParameterRepresentation{TValue}"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="dataType">The type.</param>
        /// <param name="value">The value.</param>
        public SqlInputParameterRepresentation(
            string name,
            SqlDataTypeRepresentationBase dataType,
            TValue value)
            : base(name, dataType)
        {
            dataType.MustForTest(nameof(dataType)).NotBeNull();

            var valueType = typeof(TValue);
            dataType.ValidateObjectTypeIsCompatible(valueType);

            this.Value = value;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public TValue Value { get; private set; }
    }
}
