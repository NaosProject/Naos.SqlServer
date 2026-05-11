// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlScriptValidationRuleBase.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using OBeautifulCode.Type;

    /// <summary>
    /// Base class for a rule that validates a SQL script.
    /// </summary>
    public abstract partial class SqlScriptValidationRuleBase : IModelViaCodeGen, IHaveStringId
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlScriptValidationRuleBase"/> class.
        /// </summary>
        /// <param name="id">OPTIONAL identifier.  DEFAULT is no identifier.</param>
        protected SqlScriptValidationRuleBase(
            string id = null)
        {
            this.Id = id;
        }

        /// <inheritdoc />
        public string Id { get; private set; }
    }
}
