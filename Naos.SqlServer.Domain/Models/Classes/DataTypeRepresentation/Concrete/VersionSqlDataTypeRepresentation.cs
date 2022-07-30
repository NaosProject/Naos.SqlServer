// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VersionSqlDataTypeRepresentation.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System;
    using OBeautifulCode.Type;
    using static System.FormattableString;

    /// <summary>
    /// Represents the INT SQL Data Type.
    /// </summary>
    // ReSharper disable once RedundantExtendsListEntry
    public partial class VersionSqlDataTypeRepresentation : SqlDataTypeRepresentationBase, IModelViaCodeGen
    {
        private static readonly Type[] AcceptableTypes =
        {
            typeof(Version),
        };

        /// <inheritdoc />
        public override string DeclarationInSqlSyntax => Invariant($"[NVARCHAR](23)"); // SO says it's 23 based on windows published limitations (only applies to FileVersion so I guess it could deviate for others): https://stackoverflow.com/questions/7386345/maximum-possible-length-of-fileversioninfo-fileversion-string-on-windows

        /// <inheritdoc />
        public override void ValidateObjectTypeIsCompatible(
            Type objectType,
            object value,
            bool validateValue)
        {
            // no need to validate size as the utilized .NET type restricts this already.
            InternalValidateObjectTypeIsCompatible(objectType, AcceptableTypes);
        }
    }
}
