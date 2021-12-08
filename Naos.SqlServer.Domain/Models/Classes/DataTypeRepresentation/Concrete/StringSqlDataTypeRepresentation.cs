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
        /// The maximum length constant.
        /// </summary>
        public const int MaxLengthConstant = -1;

        /// <summary>
        /// Initializes a new instance of the <see cref="StringSqlDataTypeRepresentation"/> class.
        /// </summary>
        /// <param name="supportUnicode">Specifies whether unicode is supported.</param>
        /// <param name="supportedLength">Specifies the length of the string that is supported; <see cref="MaxLengthConstant"/> will be maximum.</param>
        public StringSqlDataTypeRepresentation(
            bool supportUnicode,
            int supportedLength)
        {
            this.SupportUnicode = supportUnicode;
            this.SupportedLength = supportedLength;
        }

        /// <summary>
        /// Gets a value indicating whether unicode is supported.
        /// </summary>
        public bool SupportUnicode { get; private set; }

        /// <summary>
        /// Gets the length of the string that is supported.
        /// </summary>
        public int SupportedLength { get; private set; }

        /// <inheritdoc />
        public override string DeclarationInSqlSyntax
        {
            get
            {
                var supportedLength = this.SupportedLength == MaxLengthConstant
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
            Type objectType)
        {
            objectType.MustForArg(nameof(objectType)).NotBeNull();

            if (objectType != typeof(string) && !objectType.IsEnum)
            {
                throw new InvalidOperationException(Invariant($"String data can only be used for strings and enums, objectType {objectType.ToStringReadable()} is not supported."));
            }
        }
    }
}
