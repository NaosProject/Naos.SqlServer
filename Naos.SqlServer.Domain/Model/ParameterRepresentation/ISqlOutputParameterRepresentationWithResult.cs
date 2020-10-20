// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISqlOutputParameterRepresentationWithResult.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    /// <summary>
    /// Top level base of a SQL Parameter.
    /// </summary>
    public interface ISqlOutputParameterRepresentationWithResult
    {
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <returns>Value as TValue.</returns>
         TValue GetValue<TValue>();
    }
}
