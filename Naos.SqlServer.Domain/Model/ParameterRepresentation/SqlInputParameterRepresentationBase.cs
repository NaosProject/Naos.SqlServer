// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlInputParameterRepresentationBase.cs" company="Naos Project">
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
    /// Base class to SQL input parameters of any type.
    /// </summary>
    public abstract partial class SqlInputParameterRepresentationBase : SqlParameterRepresentationBase, IModelViaCodeGen
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlInputParameterRepresentationBase"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="dataType">The type.</param>
        protected SqlInputParameterRepresentationBase(
            string name,
            SqlDataTypeRepresentationBase dataType)
            : base(name)
        {
            dataType.MustForArg().NotBeNull();

            this.DataType = dataType;
        }

        /// <summary>
        /// Gets the type of the data.
        /// </summary>
        /// <value>The type of the data.</value>
        public SqlDataTypeRepresentationBase DataType { get; private set; }
    }
}
