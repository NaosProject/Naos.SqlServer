// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidateSqlScriptOp.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System.Collections.Generic;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Type;

    /// <summary>
    /// Validates a SQL script.
    /// </summary>
    public partial class ValidateSqlScriptOp : ReturningOperationBase<SqlScriptValidationResult>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidateSqlScriptOp"/> class.
        /// </summary>
        /// <param name="sql">The SQL script to validate.</param>
        /// <param name="rules">The rules to use for validation.</param>
        public ValidateSqlScriptOp(
            string sql,
            IReadOnlyList<SqlScriptValidationRuleBase> rules)
        {
            new { sql }.AsArg().Must().NotBeNullNorWhiteSpace();
            new { rules }.AsArg().Must().NotBeNullNorEmptyEnumerableNorContainAnyNulls();

            this.Sql = sql;
            this.Rules = rules;
        }

        /// <summary>
        /// Gets the SQL script to validate.
        /// </summary>
        public string Sql { get; private set; }

        /// <summary>
        /// Gets the rules to use for validation.
        /// </summary>
        public IReadOnlyList<SqlScriptValidationRuleBase> Rules { get; private set; }
    }
}