// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UtcDateTimeSqlDataTypeRepresentation.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System;
    using OBeautifulCode.Type;

    /// <summary>
    /// Represents the DATETIME2 SQL Data Type.
    /// </summary>
    // ReSharper disable once RedundantExtendsListEntry
    public partial class UtcDateTimeSqlDataTypeRepresentation : SqlDataTypeRepresentationBase, IModelViaCodeGen
    {
        private static readonly Type[] AcceptableTypes =
        {
            typeof(DateTime),
            typeof(DateTime?),
        };

        /// <inheritdoc />
        public override string DeclarationInSqlSyntax => "[DATETIME2]";

        /// <inheritdoc />
        public override void ValidateObjectTypeIsCompatible(
            Type objectType)
        {
            this.InternalValidateObjectTypeIsCompatible(objectType, AcceptableTypes);
        }
    }
}
