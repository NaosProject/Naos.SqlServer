// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlInputParameterRepresentationBase.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using OBeautifulCode.Type;

    /// <summary>
    /// Base class representation of a SQL input parameter.
    /// </summary>
    // ReSharper disable once RedundantExtendsListEntry
    public abstract partial class SqlInputParameterRepresentationBase : SqlParameterRepresentationBase, IModelViaCodeGen
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlInputParameterRepresentationBase"/> class.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="sqlDataType">The SQL data type of the parameter.</param>
        protected SqlInputParameterRepresentationBase(
            string name,
            SqlDataTypeRepresentationBase sqlDataType)
            : base(name, sqlDataType)
        {
        }
    }
}
