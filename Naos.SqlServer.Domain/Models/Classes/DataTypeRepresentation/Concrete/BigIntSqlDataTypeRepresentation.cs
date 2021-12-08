// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BigIntSqlDataTypeRepresentation.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System;
    using OBeautifulCode.Type;

    /// <summary>
    /// Represents the BIGINT SQL Data Type.
    /// </summary>
    // ReSharper disable once RedundantExtendsListEntry
    public partial class BigIntSqlDataTypeRepresentation : SqlDataTypeRepresentationBase, IModelViaCodeGen
    {
        private static readonly Type[] AcceptableTypes =
        {
            typeof(long),
            typeof(long?),
        };

        /// <inheritdoc />
        public override string DeclarationInSqlSyntax => "[BIGINT]";

        /// <inheritdoc />
        public override void ValidateObjectTypeIsCompatible(
            Type objectType)
        {
            this.InternalValidateObjectTypeIsCompatible(objectType, AcceptableTypes);
        }
    }
}
