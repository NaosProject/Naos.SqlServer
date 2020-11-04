// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DecimalSqlDataTypeRepresentation.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Type;

    /// <summary>
    /// Top level .
    /// </summary>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="DecimalSqlDataTypeRepresentation"/> class.
        /// </summary>
        /// <param name="precision">The precision.</param>
        /// <param name="scale">The scale.</param>
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
        /// <value>The precision.</value>
        public byte Precision { get; private set; }

        /// <summary>
        /// Gets the scale.
        /// </summary>
        /// <value>The scale.</value>
        public byte Scale { get; private set; }

        /// <inheritdoc />
        public override string DeclarationInSqlSyntax => FormattableString.Invariant($"[NUMERIC]({this.Precision},{this.Scale})");

        /// <inheritdoc />
        public override void ValidateObjectTypeIsCompatible(
            Type objectType)
        {
            objectType.MustForArg(nameof(objectType)).NotBeNull().And().BeEqualTo(typeof(decimal));
        }
    }
}
