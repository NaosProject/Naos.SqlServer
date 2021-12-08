// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DecimalSqlDataTypeRepresentation.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System;
    using OBeautifulCode.Type;
    using static System.FormattableString;

    /// <summary>
    /// Represents the NUMERIC SQL Data Type.
    /// </summary>
    // ReSharper disable once RedundantExtendsListEntry
    public partial class DecimalSqlDataTypeRepresentation : SqlDataTypeRepresentationBase, IModelViaCodeGen
    {
        /// <summary>
        /// The default numeric precision to hold dot net decimal.
        /// </summary>
        public const byte DefaultNumericPrecisionToHoldDotNetDecimal = 19;

        /// <summary>
        /// The default numeric scale to hold dot net decimal.
        /// </summary>
        public const byte DefaultNumericScaleToHoldDotNetDecimal = 5;

        private static readonly Type[] AcceptableTypes =
        {
            typeof(decimal),
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="DecimalSqlDataTypeRepresentation"/> class.
        /// </summary>
        /// <param name="precision">OPTIONAL precision.  DEFAULT is to use <see cref="DefaultNumericPrecisionToHoldDotNetDecimal"/>.</param>
        /// <param name="scale">OPTIONAL scale.  DEFAULT is to use <see cref="DefaultNumericScaleToHoldDotNetDecimal"/>.</param>
        public DecimalSqlDataTypeRepresentation(
            byte precision = DefaultNumericPrecisionToHoldDotNetDecimal,
            byte scale = DefaultNumericScaleToHoldDotNetDecimal)
        {
            this.Precision = precision;
            this.Scale = scale;
        }

        /// <summary>
        /// Gets the precision.
        /// </summary>
        public byte Precision { get; private set; }

        /// <summary>
        /// Gets the scale.
        /// </summary>
        public byte Scale { get; private set; }

        /// <inheritdoc />
        public override string DeclarationInSqlSyntax => Invariant($"[NUMERIC]({this.Precision},{this.Scale})");

        /// <inheritdoc />
        public override void ValidateObjectTypeIsCompatible(
            Type objectType)
        {
            this.InternalValidateObjectTypeIsCompatible(objectType, AcceptableTypes);
        }
    }
}
