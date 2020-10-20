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

    /// <summary>
    /// Top level .
    /// </summary>
    public partial class BinarySqlDataTypeRepresentation : SqlDataTypeRepresentationBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BinarySqlDataTypeRepresentation"/> class.
        /// </summary>
        /// <param name="supportedLength">Length of the byte array that is supported, -1 will be maximum.</param>
        public BinarySqlDataTypeRepresentation(
            int supportedLength)
        {
            this.SupportedLength = supportedLength;
        }

        /// <summary>
        /// Gets the length of the string that is supported.
        /// </summary>
        /// <value>The length of the supported.</value>
        public int SupportedLength { get; private set; }

        /// <inheritdoc />
        public override string DeclarationInSqlSyntax =>
            FormattableString.Invariant($"[VARBINARY]({(this.SupportedLength == -1 ? "MAX" : this.SupportedLength.ToString(CultureInfo.InvariantCulture))})");

        /// <inheritdoc />
        public override void ValidateObjectTypeIsCompatible(
            Type objectType)
        {
            objectType.MustForArg(nameof(objectType)).NotBeNull().And().BeEqualTo(typeof(byte[]));
        }
    }
}
