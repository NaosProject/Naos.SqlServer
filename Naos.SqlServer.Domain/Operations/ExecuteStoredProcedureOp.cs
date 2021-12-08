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
        /// <param name="parameterNameToRepresentationMap">A map of parameter name to the parameter's representation.</param>
        public ExecuteStoredProcedureOp(
            string name,
            IReadOnlyDictionary<string, SqlParameterRepresentationBase> parameterNameToRepresentationMap)
        {
            name.MustForArg(nameof(name)).NotBeNullNorWhiteSpace();
            parameterNameToRepresentationMap.MustForArg(nameof(parameterNameToRepresentationMap)).NotBeNull();

            this.Name = name;
            this.ParameterNameToRepresentationMap = parameterNameToRepresentationMap;
        }

        /// <summary>
        /// Gets the name of the stored procedure.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets a map of parameter name to the parameter's representation.
        /// </summary>
        public IReadOnlyDictionary<string, SqlParameterRepresentationBase> ParameterNameToRepresentationMap { get; private set; }
    }
}