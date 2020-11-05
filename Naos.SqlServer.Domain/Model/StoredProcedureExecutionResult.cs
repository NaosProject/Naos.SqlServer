// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StoredProcedureExecutionResult.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System.Collections.Generic;
    using OBeautifulCode.Type;

    /// <summary>
    /// Results of the stored procedure.
    /// </summary>
    public partial class StoredProcedureExecutionResult : IModelViaCodeGen
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StoredProcedureExecutionResult"/> class.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="outputParameters">The output parameters.</param>
        public StoredProcedureExecutionResult(
            ExecuteStoredProcedureOp operation,
            IReadOnlyDictionary<string, ISqlOutputParameterRepresentationWithResult> outputParameters)
        {
            this.Operation = operation;
            this.OutputParameters = outputParameters;
        }

        /// <summary>
        /// Gets the operation.
        /// </summary>
        /// <value>The operation.</value>
        public ExecuteStoredProcedureOp Operation { get; private set; }

        /// <summary>
        /// Gets the output parameters.
        /// </summary>
        /// <value>The output parameters.</value>
        public IReadOnlyDictionary<string, ISqlOutputParameterRepresentationWithResult> OutputParameters { get; private set; }
    }
}
