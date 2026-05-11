// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlScriptValidationRuleViolation.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Type;
    using OBeautifulCode.Type.Recipes;
    using static System.FormattableString;

    /// <summary>
    /// A validation rule violation when validating a SQL script.
    /// </summary>
    public partial class SqlScriptValidationRuleViolation : IHaveDetails, IModelViaCodeGen, IDeclareToStringMethod
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlScriptValidationRuleViolation"/> class.
        /// </summary>
        /// <param name="rule">The rule that was violated.</param>
        /// <param name="offset">The offset relative to the start of the script where the violation occurred.</param>
        /// <param name="details">Details about the violation.</param>
        public SqlScriptValidationRuleViolation(
            SqlScriptValidationRuleBase rule,
            int offset,
            string details)
        {
            new { rule }.AsArg().Must().NotBeNull();
            new { offset }.AsArg().Must().BeGreaterThanOrEqualTo(0);
            new { details }.AsArg().Must().NotBeNullNorWhiteSpace();

            this.Rule = rule;
            this.Offset = offset;
            this.Details = details;
        }

        /// <summary>
        /// Gets the rule that was violated.
        /// </summary>
        public SqlScriptValidationRuleBase Rule { get; private set; }

        /// <summary>
        /// Gets the offset relative to the start of the script where the violation occurred.
        /// </summary>
        public int Offset { get; private set; }

        /// <inheritdoc />
        public string Details { get; private set; }

        /// <inheritdoc cref="IDeclareToStringMethod" />
        public override string ToString()
        {
            var result = Invariant($"[{this.Rule.GetType().ToStringReadable()}] ({this.Offset}): {this.Details}");

            return result;
        }
    }
}
