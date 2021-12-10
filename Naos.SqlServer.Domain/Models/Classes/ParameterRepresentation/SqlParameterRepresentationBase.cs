﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlParameterRepresentationBase.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Type;
    using static System.FormattableString;

    /// <summary>
    /// Base class representation of a SQL parameter.
    /// </summary>
    public abstract partial class SqlParameterRepresentationBase : IModelViaCodeGen
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlParameterRepresentationBase"/> class.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="sqlDataType">The SQL data type of the parameter.</param>
        protected SqlParameterRepresentationBase(
            string name,
            SqlDataTypeRepresentationBase sqlDataType)
        {
            name.MustForArg(nameof(name)).NotBeNullNorWhiteSpace().And().BeAlphanumeric(new[] { '@', '_' });
            sqlDataType.MustForArg(nameof(sqlDataType)).NotBeNull();

            this.Name = name;
            this.SqlDataType = sqlDataType;
        }

        /// <summary>
        /// Gets the name of the parameter.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the data type of the parameter.
        /// </summary>
        public SqlDataTypeRepresentationBase SqlDataType { get; private set; }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> if the specified SQL data type is not compatible with the specified .NET data type.
        /// </summary>
        /// <param name="sqlDataType">The SQL data type.</param>
        /// <param name="dotNetDataType">The .NET data type.</param>
        protected void ThrowArgumentExceptionIfSqlDataTypeIsNotCompatibleWithDotNetDataType(
            SqlDataTypeRepresentationBase sqlDataType,
            Type dotNetDataType)
        {
            try
            {
                sqlDataType.ValidateObjectTypeIsCompatible(dotNetDataType);
            }
            catch (InvalidOperationException ex)
            {
                throw new ArgumentException(Invariant($"The specified {nameof(sqlDataType)} is not compatible with the specified {nameof(dotNetDataType)}.  See inner exception."), ex);
            }
        }
    }
}
