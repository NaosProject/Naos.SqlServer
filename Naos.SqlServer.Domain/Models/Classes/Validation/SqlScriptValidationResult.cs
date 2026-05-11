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
        /// <param name="targetSqlServerVersion">The SQL Server release whose T-SQL grammar the parser was configured for.</param>
        /// <param name="parsingErrors">The parsing errors.</param>
        /// <param name="ruleViolations">The rule violations.</param>
        public SqlScriptValidationResult(
            SqlServerVersion targetSqlServerVersion,
            IReadOnlyList<SqlScriptParsingError> parsingErrors,
            IReadOnlyList<SqlScriptValidationRuleViolation> ruleViolations)
        {
            new { targetSqlServerVersion }.AsArg().Must().NotBeEqualTo(SqlServerVersion.Unknown);
            new { parsingErrors }.AsArg().Must().NotContainAnyNullElementsWhenNotNull();
            new { ruleViolations }.AsArg().Must().NotContainAnyNullElementsWhenNotNull();

            this.TargetSqlServerVersion = targetSqlServerVersion;
            this.ParsingErrors = parsingErrors;
            this.RuleViolations = ruleViolations;
        }

        /// <summary>
        /// Gets the SQL Server release whose T-SQL grammar the parser was configured for.
        /// </summary>
        public SqlServerVersion TargetSqlServerVersion { get; private set; }

        /// <summary>
        /// Gets the parsing errors.
        /// </summary>
        public IReadOnlyList<SqlScriptParsingError> ParsingErrors { get; private set; }

        /// <summary>
        /// Gets the rule violations.
        /// </summary>
        public IReadOnlyList<SqlScriptValidationRuleViolation> RuleViolations { get; private set; }

        /// <summary>
        /// Determines whether the SQL statement is valid.
        /// </summary>
        /// <returns>
        /// true if the SQL statement is valid, otherwise false.
        /// </returns>
        public bool IsValid()
        {
            var result =
                ((this.ParsingErrors == null) || !this.ParsingErrors.Any()) &&
                ((this.RuleViolations == null) || !this.RuleViolations.Any());

            return result;
        }
    }
}
