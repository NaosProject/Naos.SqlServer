// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlDataTypeRepresentationBase.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Collection.Recipes;
    using OBeautifulCode.Type;
    using OBeautifulCode.Type.Recipes;
    using static System.FormattableString;

    /// <summary>
    /// Base class representation of a SQL Data Type.
    /// </summary>
    public abstract partial class SqlDataTypeRepresentationBase : IModelViaCodeGen
    {
        /// <summary>
        /// Gets the string of SQL that declares the type correctly, for use in Stored Procedures and Table declarations.
        /// </summary>
        public abstract string DeclarationInSqlSyntax { get; }

        /// <summary>
        /// Validates the specified object type is compatible with this SQL data type; throws if not.
        /// </summary>
        /// <param name="objectType">The type of the .NET object.</param>
        public abstract void ValidateObjectTypeIsCompatible(Type objectType);

        /// <summary>
        /// Validates the specified object type is is within a set of acceptable types; throws if not.
        /// </summary>
        /// <param name="objectType">The type of .NET object.</param>
        /// <param name="acceptableTypes">The set of acceptable types.</param>
        protected void InternalValidateObjectTypeIsCompatible(
            Type objectType,
            IReadOnlyCollection<Type> acceptableTypes)
        {
            objectType.MustForArg(nameof(objectType)).NotBeNull();

            objectType.MustForOp(nameof(objectType)).BeElementIn(acceptableTypes, Invariant($"Supported object types: {acceptableTypes.Select(_ => _.ToStringReadable()).ToDelimitedString(",")}; provided type: {objectType.ToStringReadable()}."));
        }
    }
}
