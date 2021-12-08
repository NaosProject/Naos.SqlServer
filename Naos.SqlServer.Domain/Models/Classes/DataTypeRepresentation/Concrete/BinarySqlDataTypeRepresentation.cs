// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BinarySqlDataTypeRepresentation.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System;
    using System.Globalization;
    using OBeautifulCode.Type;
    using static System.FormattableString;

    /// <summary>
    /// Represents the VARBINARY SQL Data Type.
    /// </summary>
    // ReSharper disable once RedundantExtendsListEntry
    public partial class BinarySqlDataTypeRepresentation : SqlDataTypeRepresentationBase, IModelViaCodeGen
    {
        /// <summary>
        /// The maximum length constant.
        /// </summary>
        public const int MaxLengthConstant = -1;

        private static readonly Type[] AcceptableTypes =
        {
            typeof(byte[]),
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="BinarySqlDataTypeRepresentation"/> class.
        /// </summary>
        /// <param name="supportedLength">Length of the byte array that is supported, <see cref="MaxLengthConstant"/> will be maximum.</param>
        public BinarySqlDataTypeRepresentation(
            int supportedLength)
        {
            this.SupportedLength = supportedLength;
        }

        /// <summary>
        /// Gets the length of the byte array that is supported.
        /// </summary>
        public int SupportedLength { get; private set; }

        /// <inheritdoc />
        public override string DeclarationInSqlSyntax =>
            Invariant($"[VARBINARY]({(this.SupportedLength == MaxLengthConstant ? "MAX" : this.SupportedLength.ToString(CultureInfo.InvariantCulture))})");

        /// <inheritdoc />
        public override void ValidateObjectTypeIsCompatible(
            Type objectType)
        {
            this.InternalValidateObjectTypeIsCompatible(objectType, AcceptableTypes);
        }
    }
}
