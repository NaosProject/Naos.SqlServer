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
        /// <param name="name">The name.</param>
        /// <param name="parameterNameToDetailsMap">The parameter name to details map.</param>
        public ExecuteStoredProcedureOp(
            string name,
            IReadOnlyDictionary<string, SqlParameterRepresentationBase> parameterNameToDetailsMap)
        {
            name.MustForArg(nameof(name)).NotBeNullNorWhiteSpace();
            parameterNameToDetailsMap.MustForArg(nameof(parameterNameToDetailsMap)).NotBeNull();

            this.Name = name;
            this.ParameterNameToDetailsMap = parameterNameToDetailsMap;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the parameter name to details map.
        /// </summary>
        /// <value>The parameter name to details map.</value>
        public IReadOnlyDictionary<string, SqlParameterRepresentationBase> ParameterNameToDetailsMap { get; private set; }
    }
}