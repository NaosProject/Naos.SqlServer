// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlOutputParameterRepresentationBase.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    /// <summary>
    /// Top level base of a SQL Parameter.
    /// </summary>
    public abstract partial class SqlOutputParameterRepresentationBase : SqlParameterRepresentationBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlOutputParameterRepresentationBase"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="dataType">The type.</param>
        protected SqlOutputParameterRepresentationBase(
            string name,
            SqlDataTypeRepresentationBase dataType)
            : base(name, dataType)
        {
        }
    }
}
