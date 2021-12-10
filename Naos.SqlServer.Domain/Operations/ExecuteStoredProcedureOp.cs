// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExecuteStoredProcedureOp.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System.Collections.Generic;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Type;

    /// <summary>
    /// Stored procedure to execute.
    /// </summary>
    public partial class ExecuteStoredProcedureOp : ReturningOperationBase<StoredProcedureExecutionResult>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExecuteStoredProcedureOp"/> class.
        /// </summary>
        /// <param name="name">The name of the stored procedure.</param>
        /// <param name="parameters">The parameters.</param>
        public ExecuteStoredProcedureOp(
            string name,
            IReadOnlyList<SqlParameterRepresentationBase> parameters)
        {
            name.MustForArg(nameof(name)).NotBeNullNorWhiteSpace();
            parameters.MustForArg(nameof(parameters)).NotBeNull().And().NotContainAnyNullElements();

            this.Name = name;
            this.Parameters = parameters;
        }

        /// <summary>
        /// Gets the name of the stored procedure.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        public IReadOnlyList<SqlParameterRepresentationBase> Parameters { get; private set; }
    }
}