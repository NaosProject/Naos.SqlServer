// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntSqlDataTypeRepresentation.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System;
    using OBeautifulCode.Type;

    /// <summary>
    /// Represents the INT SQL Data Type.
    /// </summary>
    // ReSharper disable once RedundantExtendsListEntry
    public partial class IntSqlDataTypeRepresentation : SqlDataTypeRepresentationBase, IModelViaCodeGen
    {
        private static readonly Type[] AcceptableTypes =
        {
            typeof(int),
            typeof(int?),
        };

        /// <inheritdoc />
        public override string DeclarationInSqlSyntax => "[INT]";

        /// <inheritdoc />
        public override void ValidateObjectTypeIsCompatible(
            Type objectType)
        {
            this.InternalValidateObjectTypeIsCompatible(objectType, AcceptableTypes);
        }
    }
}
