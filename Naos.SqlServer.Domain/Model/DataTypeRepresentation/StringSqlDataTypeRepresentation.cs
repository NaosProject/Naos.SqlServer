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

    /// <summary>
    /// Top level .
    /// </summary>
    public partial class StringSqlDataTypeRepresentation : SqlDataTypeRepresentationBase, IModelViaCodeGen
    {
        /// <summary>
        /// The maximum length constant.
        /// </summary>
        public const int MaxLengthConstant = -1;

        /// <summary>
        /// Initializes a new instance of the <see cref="StringSqlDataTypeRepresentation"/> class.
        /// </summary>
        /// <param name="supportUnicode">if set to <c>true</c> [support unicode].</param>
        /// <param name="supportedLength">Length of the string that is supported, <see cref="MaxLengthConstant"/> will be maximum.</param>
        public StringSqlDataTypeRepresentation(
            bool supportUnicode,
            int supportedLength)
        {
            this.SupportUnicode = supportUnicode;
            this.SupportedLength = supportedLength;
        }

        /// <summary>
        /// Gets a value indicating whether [support unicode].
        /// </summary>
        /// <value><c>true</c> if [support unicode]; otherwise, <c>false</c>.</value>
        public bool SupportUnicode { get; private set; }

        /// <summary>
        /// Gets the length of the string that is supported.
        /// </summary>
        /// <value>The length of the supported.</value>
        public int SupportedLength { get; private set; }

        /// <inheritdoc />
        public override string DeclarationInSqlSyntax
        {
            get
            {
                var supportedLength = this.SupportedLength == MaxLengthConstant ? "MAX" : this.SupportedLength.ToString(CultureInfo.InvariantCulture);
                return this.SupportUnicode
                    ? FormattableString.Invariant($"[NVARCHAR]({supportedLength})")
                    : FormattableString.Invariant($"[VARCHAR]({supportedLength})");
            }
        }

        /// <inheritdoc />
        public override void ValidateObjectTypeIsCompatible(
            Type objectType)
        {
            if (objectType != typeof(string) && !objectType.IsEnum)
            {
                throw new NotSupportedException(FormattableString.Invariant($"String data can only be used for strings and enums, objectType {objectType.ToStringReadable()} is not supported."));
            }
        }
    }
}
