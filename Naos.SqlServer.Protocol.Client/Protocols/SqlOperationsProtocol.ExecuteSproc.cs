// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlOperationsProtocol.ExecuteSproc.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Protocol.Client
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Threading.Tasks;
    using Naos.SqlServer.Domain;
    using Naos.SqlServer.Protocol.Client;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Database.Recipes;
    using OBeautifulCode.Type.Recipes;

    /// <summary>
    /// Sql Operation Protocol.
    /// </summary>
    public partial class SqlOperationsProtocol
    {
        /// <inheritdoc />
        public StoredProcedureExecutionResult Execute(
            ExecuteStoredProcedureOp operation)
        {
            var outputParametersWithExecutionResult = new Dictionary<string, ISqlOutputParameterRepresentationWithResult>();
            using (var sqlConnection = this.sqlServerLocator.OpenSqlConnection(this.defaultConnectionTimeout))
            {
                using (var command = sqlConnection.BuildSqlCommand(operation.Name, (int)this.defaultCommandTimeout.TotalSeconds))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    var outputParameters = new List<Tuple<SqlParameter, SqlOutputParameterRepresentationBase>>();
                    foreach (var paramNameAndDetails in operation.ParameterNameToDetailsMap)
                    {
                        var parameter = paramNameAndDetails.Value.ToSqlParameter();

                        if (parameter.Direction == ParameterDirection.Output)
                        {
                            if (paramNameAndDetails.Value is SqlOutputParameterRepresentationBase outputParameterRepresentation)
                            {
                                outputParameters.Add(new Tuple<SqlParameter, SqlOutputParameterRepresentationBase>(parameter, outputParameterRepresentation));
                            }
                            else
                            {
                                throw new NotSupportedException(FormattableString.Invariant($"Cannot have a {nameof(SqlParameter)} with {nameof(SqlParameter.Direction)} equal to {nameof(ParameterDirection.Output)} and the representation not be a {nameof(SqlOutputParameterRepresentationBase)}, it was a {paramNameAndDetails.Value.GetType().ToStringReadable()}."));
                            }
                        }

                        command.Parameters.Add(parameter);
                    }

                    command.ExecuteNonQuery();

                    foreach (var outputParameter in outputParameters)
                    {
                        var outputParameterWithResult = outputParameter.Item2.CreateWithResult(outputParameter.Item1.Value);
                        outputParametersWithExecutionResult.Add(outputParameter.Item2.Name,  outputParameterWithResult);
                    }
                }
            }

            var result = new StoredProcedureExecutionResult(operation, outputParametersWithExecutionResult);
            return result;
        }

        /// <inheritdoc />
        public Task<StoredProcedureExecutionResult> ExecuteAsync(
            ExecuteStoredProcedureOp operation)
        {
            // TODO: mirror fully ASYNC code here
            throw new System.NotImplementedException();
        }
    }
}