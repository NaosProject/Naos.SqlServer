// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisallowSchemasSqlScriptValidationRule.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System.Collections.Generic;
    using OBeautifulCode.Assertion.Recipes;

    /// <summary>
    /// A rule that disallows the usage of the specified schemas in a SQL script.
    /// </summary>
    public partial class DisallowSchemasSqlScriptValidationRule : SqlScriptValidationRuleBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DisallowSchemasSqlScriptValidationRule"/> class.
        /// </summary>
        /// <param name="disallowedSchemas">The disallowed schemas.</param>
        /// <param name="id">OPTIONAL identifier.  DEFAULT is no identifier.</param>
        public DisallowSchemasSqlScriptValidationRule(
            IReadOnlyCollection<string> disallowedSchemas,
            string id = null)
            : base(id)
        {
            new { disallowedSchemas }.AsArg().Must().NotBeNullNorEmptyEnumerableNorContainAnyNulls().And().Each().NotBeNullNorWhiteSpace();

            this.DisallowedSchemas = disallowedSchemas;
        }

        /// <summary>
        /// Gets the disallowed schemas.
        /// </summary>
        public IReadOnlyCollection<string> DisallowedSchemas { get; private set; }
    }
}
