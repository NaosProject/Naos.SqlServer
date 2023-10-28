// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProtocolSqlOperations.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using Naos.Database.Domain;
    using OBeautifulCode.Type;

    /// <summary>
    /// Protocols for SQL Operations.
    /// </summary>
    public interface IProtocolSqlOperations
        : ISyncAndAsyncReturningProtocol<ExecuteStoredProcedureOp, StoredProcedureExecutionResult>,
          ISyncAndAsyncVoidProtocol<CreateDatabaseOp>,
          ISyncAndAsyncVoidProtocol<DeleteDatabaseOp>,
          ISyncAndAsyncReturningProtocol<GetLatestJobInformationOp, IJobInformation>

    {
    }
}