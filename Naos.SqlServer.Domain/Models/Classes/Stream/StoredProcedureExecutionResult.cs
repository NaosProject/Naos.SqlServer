// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StoredProcedureExecutionResult.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System.Collections.Generic;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Type;
    using static System.FormattableString;

    /// <summary>
    /// Results of executing a stored procedure.
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
            operation.MustForArg(nameof(operation)).NotBeNull();
            outputParameters.MustForArg(nameof(outputParameters)).NotBeNull().And().NotContainAnyKeyValuePairsWithNullValue();
            outputParameters.Values.MustForArg(Invariant($"{nameof(outputParameters)}.{nameof(IReadOnlyDictionary<string, ISqlOutputParameterRepresentationWithResult>.Values)}")).Each().NotBeNullNorWhiteSpace().And().BeAlphanumeric(new[] { '@', '_' });

            this.Operation = operation;
            this.OutputParameters = outputParameters;
        }

        /// <summary>
        /// Gets the operation.
        /// </summary>
        public ExecuteStoredProcedureOp Operation { get; private set; }

        /// <summary>
        /// Gets the output parameters.
        /// </summary>
        public IReadOnlyDictionary<string, ISqlOutputParameterRepresentationWithResult> OutputParameters { get; private set; }
    }
}
