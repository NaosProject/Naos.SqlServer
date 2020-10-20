// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntSqlDataTypeRepresentation.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System;
    using OBeautifulCode.Assertion.Recipes;

    /// <summary>
    /// Top level .
    /// </summary>
    public partial class IntSqlDataTypeRepresentation : SqlDataTypeRepresentationBase
    {
        /// <inheritdoc />
        public override string DeclarationInSqlSyntax => "[INT]";

        /// <inheritdoc />
        public override void ValidateObjectTypeIsCompatible(
            Type objectType)
        {
            objectType.MustForArg(nameof(objectType)).NotBeNull().And().BeEqualTo(typeof(int));
        }
    }
}
