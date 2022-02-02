// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlOperationsProtocol.DeleteDatabase.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Protocol.Client
{
    using System.Threading.Tasks;
    using Naos.Database.Domain;
    using Naos.SqlServer.Domain;
    using OBeautifulCode.Assertion.Recipes;

    /// <summary>
    /// Sql Operation Protocol.
    /// </summary>
    public partial class SqlOperationsProtocol
    {
        /// <inheritdoc />
        public void Execute(
            DeleteDatabaseOp operation)
        {
            operation.MustForArg(nameof(operation)).NotBeNull();

            // Change to master database since it is guaranteed to be there.
            var connectionString = this.sqlServerLocator
                                       .DeepCloneWithDatabaseName(SqlServerDatabaseManager.MasterDatabaseName)
                                       .BuildConnectionString(this.defaultConnectionTimeout);

            SqlServerDatabaseManager.Delete(connectionString, operation.DatabaseName, this.defaultCommandTimeout);
        }

        /// <inheritdoc />
        public async Task ExecuteAsync(
            DeleteDatabaseOp operation)
        {
            this.Execute(operation);
            await Task.FromResult(true); // just for the await...
        }
    }
}