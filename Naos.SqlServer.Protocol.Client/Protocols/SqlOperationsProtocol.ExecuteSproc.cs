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
            var outputParametersWithExecutionResult = new Dictionary<string, ISqlOutputParameterResult>();
            using (var sqlConnection = this.sqlServerLocator.OpenSqlConnection(this.defaultConnectionTimeout))
            {
                using (var command = sqlConnection.BuildSqlCommand(operation.Name, (int)this.defaultCommandTimeout.TotalSeconds))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    var outputParameters = new List<Tuple<SqlParameter, OutputParameterDefinitionBase>>();
                    var parameterDefinitions = operation.Parameters;
                    LoadParametersIntoCommand(parameterDefinitions, outputParameters, command);

                    command.ExecuteNonQuery();

                    foreach (var outputParameter in outputParameters)
                    {
                        var outputParameterWithResult = outputParameter.Item2.CreateResult(outputParameter.Item1.Value);

                        outputParametersWithExecutionResult.Add(outputParameter.Item2.Name, outputParameterWithResult);
                    }
                }
            }

            var result = new StoredProcedureExecutionResult(operation, outputParametersWithExecutionResult);

            return result;
        }

        /// <inheritdoc />
        public async Task<StoredProcedureExecutionResult> ExecuteAsync(
            ExecuteStoredProcedureOp operation)
        {
            var outputParametersWithExecutionResult = new Dictionary<string, ISqlOutputParameterResult>();
            using (var sqlConnection = this.sqlServerLocator.OpenSqlConnection(this.defaultConnectionTimeout))
            {
                using (var command = sqlConnection.BuildSqlCommand(operation.Name, (int)this.defaultCommandTimeout.TotalSeconds))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    var outputParameters = new List<Tuple<SqlParameter, OutputParameterDefinitionBase>>();
                    var parameterDefinitions = operation.Parameters;
                    LoadParametersIntoCommand(parameterDefinitions, outputParameters, command);

                    await command.ExecuteNonQueryAsync();

                    foreach (var outputParameter in outputParameters)
                    {
                        var outputParameterWithResult = outputParameter.Item2.CreateResult(outputParameter.Item1.Value);

                        outputParametersWithExecutionResult.Add(outputParameter.Item2.Name, outputParameterWithResult);
                    }
                }
            }

            var result = new StoredProcedureExecutionResult(operation, outputParametersWithExecutionResult);

            return result;
        }

        private static void LoadParametersIntoCommand(
            IReadOnlyList<ParameterDefinitionBase> parameterDefinitions,
            List<Tuple<SqlParameter, OutputParameterDefinitionBase>> outputParameters,
            SqlCommand command)
        {
            foreach (var parameterRepresentation in parameterDefinitions)
            {
                var parameter = parameterRepresentation.ToSqlParameter();

                if (parameter.Direction == ParameterDirection.Output)
                {
                    if (parameterRepresentation is OutputParameterDefinitionBase outputParameterRepresentation)
                    {
                        outputParameters.Add(new Tuple<SqlParameter, OutputParameterDefinitionBase>(parameter, outputParameterRepresentation));
                    }
                    else
                    {
                        throw new NotSupportedException(
                            FormattableString.Invariant(
                                $"Cannot have a {nameof(SqlParameter)} with {nameof(SqlParameter.Direction)} equal to {nameof(ParameterDirection.Output)} and the representation not be a {nameof(OutputParameterDefinitionBase)}, it was a {parameterRepresentation.GetType().ToStringReadable()}."));
                    }
                }

                command.Parameters.Add(parameter);
            }
        }
    }
}