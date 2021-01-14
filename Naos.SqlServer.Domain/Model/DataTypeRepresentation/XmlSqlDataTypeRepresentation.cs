// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlSqlDataTypeRepresentation.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System;
    using System.Linq;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Collection.Recipes;
    using OBeautifulCode.Type;
    using OBeautifulCode.Type.Recipes;

    /// <summary>
    /// XML Type.
    /// </summary>
    public partial class XmlSqlDataTypeRepresentation : SqlDataTypeRepresentationBase, IModelViaCodeGen
    {
        /// <inheritdoc />
        public override string DeclarationInSqlSyntax => "[XML]";

        /// <inheritdoc />
        public override void ValidateObjectTypeIsCompatible(
            Type objectType)
        {
            objectType.MustForArg(nameof(objectType)).NotBeNull().And().BeEqualTo(typeof(string));
        }
    }
}
