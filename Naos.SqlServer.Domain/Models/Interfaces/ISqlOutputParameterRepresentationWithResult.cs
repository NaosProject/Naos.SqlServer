// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISqlOutputParameterRepresentationWithResult.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    /// <summary>
    /// A SQL output parameter with it's value.
    /// </summary>
    public interface ISqlOutputParameterRepresentationWithResult
    {
        /// <summary>
        /// Gets the value of the parameter as a specified type.
        /// </summary>
        /// <typeparam name="TValue">The type of the value to return.</typeparam>
        /// <returns>The value as the specified type of value to return.</returns>
        TValue GetValueOfType<TValue>();
    }
}
