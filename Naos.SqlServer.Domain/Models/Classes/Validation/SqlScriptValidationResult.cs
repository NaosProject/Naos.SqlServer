// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlScriptValidationResult.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System.Collections.Generic;
    using System.Linq;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Type;

    /// <summary>
    /// A validation rule violation when validating a SQL script.
    /// </summary>
    public partial class SqlScriptValidationResult : IModelViaCodeGen
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlScriptValidationResult"/> class.
        /// </summary>
        /// <param name="violations">The rule that was violated.</param>
        public SqlScriptValidationResult(
            IReadOnlyList<SqlScriptValidationRuleViolation> violations)
        {
            new { violations }.AsArg().Must().NotContainAnyNullElementsWhenNotNull();

            this.Violations = violations;
        }

        /// <summary>
        /// Gets the rule that was violated.
        /// </summary>
        public IReadOnlyList<SqlScriptValidationRuleViolation> Violations { get; private set; }

        /// <summary>
        /// Determines whether there are any rule violations.
        /// </summary>
        /// <returns>
        /// true if there are any rule violations, otherwise false.
        /// </returns>
        public bool HasAnyRuleViolation()
        {
            var result = (this.Violations != null) && this.Violations.Any();

            return result;
        }
    }
}
