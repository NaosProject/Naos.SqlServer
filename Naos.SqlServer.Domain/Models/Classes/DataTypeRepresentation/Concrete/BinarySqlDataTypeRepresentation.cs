// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BinarySqlDataTypeRepresentation.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System;
    using System.Globalization;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Type;
    using static System.FormattableString;

    /// <summary>
    /// Represents the VARBINARY SQL Data Type.
    /// </summary>
    // ReSharper disable once RedundantExtendsListEntry
    public partial class BinarySqlDataTypeRepresentation : SqlDataTypeRepresentationBase, IModelViaCodeGen
    {
        /// <summary>
        /// The maximum length of a byte array.
        /// </summary>
        public const int MaxLengthConstant = int.MaxValue - 2;

        private static readonly Type[] AcceptableTypes =
        {
            typeof(byte[]),
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="BinarySqlDataTypeRepresentation"/> class.
        /// </summary>
        /// <param name="supportedLength">The maximum length of a byte array that is supported.</param>
        public BinarySqlDataTypeRepresentation(
            int supportedLength)
        {
            supportedLength.MustForArg(nameof(supportedLength)).BeGreaterThanOrEqualTo(1).And().BeLessThanOrEqualTo(MaxLengthConstant);

            this.SupportedLength = supportedLength;
        }

        /// <summary>
        /// Gets the maximum length of a byte array that is supported.
        /// </summary>
        public int SupportedLength { get; private set; }

        /// <inheritdoc />
        public override string DeclarationInSqlSyntax =>
            Invariant($"[VARBINARY]({(this.SupportedLength > 8000 ? "MAX" : this.SupportedLength.ToString(CultureInfo.InvariantCulture))})");

        /// <inheritdoc />
        public override void ValidateObjectTypeIsCompatible(
            Type objectType)
        {
            InternalValidateObjectTypeIsCompatible(objectType, AcceptableTypes);
        }
    }
}
