// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisallowSystemSchemasSqlScriptValidationRule.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    /// <summary>
    /// A rule that disallows the usage of system schemas in a SQL script.
    /// </summary>
    public partial class DisallowSystemSchemasSqlScriptValidationRule : SqlScriptValidationRuleBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DisallowSystemSchemasSqlScriptValidationRule"/> class.
        /// </summary>
        /// <param name="id">OPTIONAL identifier.  DEFAULT is no identifier.</param>
        public DisallowSystemSchemasSqlScriptValidationRule(
            string id = null)
            : base(id)
        {
        }
    }
}
