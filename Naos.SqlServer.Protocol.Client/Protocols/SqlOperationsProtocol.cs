// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlOperationsProtocol.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Protocol.Client
{
    using System;
    using Naos.SqlServer.Domain;
    using OBeautifulCode.Assertion.Recipes;

    /// <summary>
    /// Sql Operation Protocol.
    /// </summary>
    public partial class SqlOperationsProtocol : IProtocolSqlOperations
    {
        private readonly SqlServerLocator sqlServerLocator;
        private readonly TimeSpan defaultConnectionTimeout;
        private readonly TimeSpan defaultCommandTimeout;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlOperationsProtocol"/> class.
        /// </summary>
        /// <param name="sqlServerLocator">The SQL locator.</param>
        /// <param name="defaultConnectionTimeout">The default connection timeout.</param>
        /// <param name="defaultCommandTimeout">The default command timeout.</param>
        public SqlOperationsProtocol(
            SqlServerLocator sqlServerLocator,
            TimeSpan defaultConnectionTimeout = default(TimeSpan),
            TimeSpan defaultCommandTimeout = default(TimeSpan))
        {
            sqlServerLocator.MustForArg(nameof(sqlServerLocator)).NotBeNull();

            this.sqlServerLocator = sqlServerLocator;

            this.defaultConnectionTimeout = defaultConnectionTimeout == default(TimeSpan) ? TimeSpan.FromSeconds(30) : defaultConnectionTimeout;
            this.defaultCommandTimeout = defaultCommandTimeout == default(TimeSpan) ? TimeSpan.FromSeconds(30) : defaultCommandTimeout;
        }
    }
}