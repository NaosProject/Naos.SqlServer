// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterDefinitionBase.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Naos.CodeAnalysis.Recipes;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Type;
    using static System.FormattableString;

    /// <summary>
    /// Base class representation of a SQL parameter.
    /// </summary>
    public abstract partial class ParameterDefinitionBase : IModelViaCodeGen
    {
        /// <summary>
        /// The characters that are allowed in a parameter name, in addition to alphanumeric characters.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = NaosSuppressBecause.CA2104_DoNotDeclareReadOnlyMutableReferenceTypes_TypeIsImmutable)]
        public static readonly IReadOnlyCollection<char> ParameterNameAlphanumericOtherAllowedCharacters = new[] { '@', '_' };

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterDefinitionBase"/> class.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="sqlDataType">The SQL data type of the parameter.</param>
        protected ParameterDefinitionBase(
            string name,
            SqlDataTypeRepresentationBase sqlDataType)
        {
            name.MustForArg(nameof(name)).NotBeNullNorWhiteSpace().And().BeAlphanumeric(ParameterNameAlphanumericOtherAllowedCharacters);
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
        /// <param name="value">The value to validate.</param>
        /// <param name="validateValue">Validate the value (not used for output parameters as there is no value).</param>
        protected static void ThrowArgumentExceptionIfSqlDataTypeIsNotCompatibleWithDotNetDataType(
            SqlDataTypeRepresentationBase sqlDataType,
            Type dotNetDataType,
            object value,
            bool validateValue)
        {
            try
            {
                sqlDataType.ValidateObjectTypeIsCompatible(dotNetDataType, value, validateValue);
            }
            catch (InvalidOperationException ex)
            {
                throw new ArgumentException(Invariant($"The specified {nameof(sqlDataType)} is not compatible with the specified {nameof(dotNetDataType)}.  See inner exception."), ex);
            }
        }
    }
}
