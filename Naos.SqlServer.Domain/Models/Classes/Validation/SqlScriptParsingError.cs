// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlScriptParsingError.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Type;
    using static System.FormattableString;

    /// <summary>
    /// An error parsing a SQL script.
    /// </summary>
    public partial class SqlScriptParsingError : IHaveDetails, IModelViaCodeGen, IDeclareToStringMethod
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlScriptParsingError"/> class.
        /// </summary>
        /// <param name="offset">The offset relative to the start of the script where the parsing error occurred.</param>
        /// <param name="details">Details about the parsing error.</param>
        public SqlScriptParsingError(
            int offset,
            string details)
        {
            new { offset }.AsArg().Must().BeGreaterThanOrEqualTo(0);
            new { details }.AsArg().Must().NotBeNullNorWhiteSpace();

            this.Offset = offset;
            this.Details = details;
        }

        /// <summary>
        /// Gets the offset relative to the start of the script where the parsing error occurred.
        /// </summary>
        public int Offset { get; private set; }

        /// <inheritdoc />
        public string Details { get; private set; }

        /// <inheritdoc cref="IDeclareToStringMethod" />
        public override string ToString()
        {
            var result = Invariant($"[{this.Offset}]: {this.Details}");

            return result;
        }
    }
}
