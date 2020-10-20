// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlOutputParameterRepresentation{TValue}.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Type.Recipes;

    /// <summary>
    /// Top level .
    /// </summary>
    /// <typeparam name="TValue">Type of the Output value.</typeparam>
    public partial class SqlOutputParameterRepresentation<TValue> : SqlOutputParameterRepresentationBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlOutputParameterRepresentation{TValue}"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="dataType">The type.</param>
        public SqlOutputParameterRepresentation(
            string name,
            SqlDataTypeRepresentationBase dataType)
            : base(name, dataType)
        {
            dataType.MustForTest(nameof(dataType)).NotBeNull();

            var valueType = typeof(TValue);
            dataType.ValidateObjectTypeIsCompatible(valueType);
        }
    }
}
