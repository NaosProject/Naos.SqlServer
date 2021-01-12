// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BigIntSqlDataTypeRepresentation.cs" company="Naos Project">
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
    /// Top level .
    /// </summary>
    public partial class BigIntSqlDataTypeRepresentation : SqlDataTypeRepresentationBase, IModelViaCodeGen
    {
        /// <inheritdoc />
        public override string DeclarationInSqlSyntax => "[BIGINT]";

        private static readonly Type[] AcceptableTypes = new[]
                                                         {
                                                             typeof(long),
                                                             typeof(long?),
                                                         };

        /// <inheritdoc />
        public override void ValidateObjectTypeIsCompatible(
            Type objectType)
        {
            objectType.MustForArg(nameof(objectType)).NotBeNull();

            AcceptableTypes.MustForArg(nameof(objectType)).ContainElement(objectType, FormattableString.Invariant($"Supported object types: {AcceptableTypes.Select(_ => _.ToStringReadable()).ToDelimitedString(",")}; provided type: {objectType.ToStringReadable()}."));
        }
    }
}
