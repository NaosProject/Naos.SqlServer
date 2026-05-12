// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SingleStatementSqlScriptValidationRule.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    /// <summary>
    /// A rule that requires the SQL script to be a single SQL statement.
    /// </summary>
    public partial class SingleStatementSqlScriptValidationRule : SqlScriptValidationRuleBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SingleStatementSqlScriptValidationRule"/> class.
        /// </summary>
        /// <param name="id">OPTIONAL identifier.  DEFAULT is no identifier.</param>
        public SingleStatementSqlScriptValidationRule(
            string id = null)
            : base(id)
        {
        }
    }
}
