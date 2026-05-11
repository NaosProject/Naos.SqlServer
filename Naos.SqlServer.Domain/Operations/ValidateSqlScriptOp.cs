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
        /// <param name="targetSqlServerVersion">The SQL Server release whose T-SQL grammar the parser should use.</param>
        /// <param name="sql">The SQL script to validate.</param>
        /// <param name="rules">The rules to use for validation.</param>
        public ValidateSqlScriptOp(
            SqlServerVersion targetSqlServerVersion,
            string sql,
            IReadOnlyList<SqlScriptValidationRuleBase> rules)
        {
            new { targetSqlServerVersion }.AsArg().Must().NotBeEqualTo(SqlServerVersion.Unknown);
            new { sql }.AsArg().Must().NotBeNullNorWhiteSpace();
            new { rules }.AsArg().Must().NotBeNullNorEmptyEnumerableNorContainAnyNulls();

            this.TargetSqlServerVersion = targetSqlServerVersion;
            this.Sql = sql;
            this.Rules = rules;
        }

        /// <summary>
        /// Gets the SQL Server release whose T-SQL grammar the parser should use.
        /// </summary>
        public SqlServerVersion TargetSqlServerVersion { get; private set; }

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