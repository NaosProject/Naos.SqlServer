// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlSqlDataTypeRepresentation.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System;
    using OBeautifulCode.Type;

    /// <summary>
    /// Represents the XML SQL Data Type.
    /// </summary>
    // ReSharper disable once RedundantExtendsListEntry
    public partial class XmlSqlDataTypeRepresentation : SqlDataTypeRepresentationBase, IModelViaCodeGen
    {
        private static readonly Type[] AcceptableTypes =
        {
            typeof(string),
        };

        /// <inheritdoc />
        public override string DeclarationInSqlSyntax => "[XML]";

        /// <inheritdoc />
        public override void ValidateObjectTypeIsCompatible(
            Type objectType,
            object value,
            bool validateValue)
        {
            // no need to validate size as the utilized SQL type supports this already.
            InternalValidateObjectTypeIsCompatible(objectType, AcceptableTypes);
        }
    }
}
