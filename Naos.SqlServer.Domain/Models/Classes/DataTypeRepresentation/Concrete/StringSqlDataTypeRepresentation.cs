// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringSqlDataTypeRepresentation.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System;
    using System.Globalization;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Type;
    using OBeautifulCode.Type.Recipes;
    using static System.FormattableString;

    /// <summary>
    /// Represents the VARCHAR or NVARCHAR SQL Data Type.
    /// </summary>
    // ReSharper disable once RedundantExtendsListEntry
    public partial class StringSqlDataTypeRepresentation : SqlDataTypeRepresentationBase, IModelViaCodeGen
    {
        /// <summary>
        /// The maximum length of a non-unicode string, assuming single-byte encoding character sets such as Latin.
        /// </summary>
        public const int MaxNonUnicodeLengthConstant = int.MaxValue - 2;

        /// <summary>
        /// The maximum length of a unicode string, assuming double-byte encoding character sets such as USC-2.
        /// </summary>
        public const int MaxUnicodeLengthConstant = 1073741822;

        /// <summary>
        /// Initializes a new instance of the <see cref="StringSqlDataTypeRepresentation"/> class.
        /// </summary>
        /// <param name="supportUnicode">Specifies whether unicode is supported.</param>
        /// <param name="supportedLength">Specifies the maximum length of a string that is supported; <see cref="MaxUnicodeLengthConstant"/> will be maximum.</param>
        public StringSqlDataTypeRepresentation(
            bool supportUnicode,
            int supportedLength)
        {
            supportedLength.MustForArg(nameof(supportedLength)).BeGreaterThanOrEqualTo(1).And().BeLessThanOrEqualTo(supportUnicode ? MaxUnicodeLengthConstant : MaxNonUnicodeLengthConstant);

            this.SupportUnicode = supportUnicode;
            this.SupportedLength = supportedLength;
        }

        /// <summary>
        /// Gets a value indicating whether unicode is supported.
        /// </summary>
        public bool SupportUnicode { get; private set; }

        /// <summary>
        /// Gets the maximum length of a string that is supported.
        /// </summary>
        public int SupportedLength { get; private set; }

        /// <inheritdoc />
        public override string DeclarationInSqlSyntax
        {
            get
            {
                var supportedLength = this.SupportUnicode
                    ? this.SupportedLength > 4000
                        ? "MAX"
                        : this.SupportedLength.ToString(CultureInfo.InvariantCulture)
                    : this.SupportedLength > 8000
                        ? "MAX"
                        : this.SupportedLength.ToString(CultureInfo.InvariantCulture);

                var result = this.SupportUnicode
                    ? Invariant($"[NVARCHAR]({supportedLength})")
                    : Invariant($"[VARCHAR]({supportedLength})");

                return result;
            }
        }

        /// <inheritdoc />
        public override void ValidateObjectTypeIsCompatible(
            Type objectType,
            object value,
            bool validateValue)
        {
            objectType.MustForArg(nameof(objectType)).NotBeNull();

            if ((objectType != typeof(string)) && (!objectType.IsEnum) && (objectType != typeof(Version)))
            {
                throw new InvalidOperationException(Invariant($"String data can only be used for strings, enums, and versions; objectType {objectType.ToStringReadable()} is not supported."));
            }

            if (validateValue)
            {
                var valueAsString = value?.ToString();
                if (valueAsString != null && valueAsString.Length > this.SupportedLength)
                {
                    throw new ArgumentException(
                        Invariant($"Provided value has length {valueAsString.Length} exceeds maximum allowed value of {this.SupportedLength}."));
                }
            }
        }
    }
}
