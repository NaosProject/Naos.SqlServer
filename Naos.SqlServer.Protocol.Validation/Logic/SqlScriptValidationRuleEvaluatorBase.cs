// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlScriptValidationRuleEvaluatorBase.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Protocol.Validation
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Microsoft.SqlServer.TransactSql.ScriptDom;
    using Naos.SqlServer.Domain;
    using OBeautifulCode.Assertion.Recipes;

    /// <summary>
    /// Base class for a <see cref="SqlScriptValidationRuleBase"/> evaluator.
    /// </summary>
    public abstract class SqlScriptValidationRuleEvaluatorBase : TSqlFragmentVisitor
    {
        private readonly List<SqlScriptValidationRuleViolation> violations = new List<SqlScriptValidationRuleViolation>();

        private readonly SqlScriptValidationRuleBase rule;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlScriptValidationRuleEvaluatorBase"/> class.
        /// </summary>
        /// <param name="rule">The rule to evaluate.</param>
        protected SqlScriptValidationRuleEvaluatorBase(
            SqlScriptValidationRuleBase rule)
        {
            new { rule }.AsArg().Must().NotBeNull();

            this.rule = rule;
        }

        /// <summary>
        /// Adds a violation.
        /// </summary>
        /// <param name="offset">The offset relative to the start of the script where the violation occurred.</param>
        /// <param name="details">Details about the violation.</param>
        public void AddViolation(
            int offset,
            string details)
        {
            var violation = new SqlScriptValidationRuleViolation(
                this.rule,
                offset,
                details);

            this.violations.Add(violation);
        }

        /// <summary>
        /// Gets the rule violations.
        /// </summary>
        /// <returns>
        /// The rule violations.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Prefer a method here.")]
        public IReadOnlyList<SqlScriptValidationRuleViolation> GetViolations()
        {
            var result = this.violations.ToList();

            return result;
        }
    }
}
