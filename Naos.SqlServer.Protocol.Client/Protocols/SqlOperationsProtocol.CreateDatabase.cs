// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlOperationsProtocol.CreateDatabase.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Protocol.Client
{
    using System.Threading.Tasks;
    using Naos.SqlServer.Domain;
    using OBeautifulCode.Assertion.Recipes;

    /// <summary>
    /// Sql Operation Protocol.
    /// </summary>
    public partial class SqlOperationsProtocol
    {
        /// <inheritdoc />
        public void Execute(
            CreateDatabaseOp operation)
        {
            operation.MustForArg(nameof(operation)).NotBeNull();

            var connectionString = this.sqlServerLocator.BuildConnectionString(this.defaultConnectionTimeout);
            SqlServerDatabaseManager.Create(connectionString, operation.DatabaseConfiguration, this.defaultCommandTimeout);
        }

        /// <inheritdoc />
        public async Task ExecuteAsync(
            CreateDatabaseOp operation)
        {
            this.Execute(operation);
            await Task.FromResult(true); // just for the await...
        }
    }
}