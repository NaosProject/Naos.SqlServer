// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CheckJobsProtocol.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Protocol.Client
{
    using System;
    using System.Collections.Generic;
    using Naos.Database.Domain;
    using Naos.Diagnostics.Domain;
    using Naos.SqlServer.Domain;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Type;

    /// <summary>
    /// Protocol to execute <see cref="CheckJobsOp" />.
    /// </summary>
    public class CheckJobsProtocol : SyncSpecificReturningProtocolBase<CheckJobsOp, CheckJobsReport>
    {
        private readonly SqlServerLocator sqlServerLocator;
        private readonly TimeSpan connectionTimeout;
        private readonly TimeSpan commandTimeout;

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckJobsProtocol"/> class.
        /// </summary>
        /// <param name="getUtcNow">Function to get the sampling time; allows for a single time to be used in multiple protocols.</param>
        /// <param name="sqlServerLocator">The SQL server locator.</param>
        /// <param name="connectionTimeout">The connection timeout.</param>
        /// <param name="commandTimeout">The command timeout.</param>
        public CheckJobsProtocol(
            Func<DateTime> getUtcNow,
            SqlServerLocator sqlServerLocator,
            TimeSpan connectionTimeout,
            TimeSpan commandTimeout)
        {
            getUtcNow.MustForArg(nameof(getUtcNow)).NotBeNull();
            sqlServerLocator.MustForArg(nameof(sqlServerLocator)).NotBeNull();
            connectionTimeout.MustForArg(nameof(connectionTimeout)).BeGreaterThan(TimeSpan.Zero);
            commandTimeout.MustForArg(nameof(commandTimeout)).BeGreaterThan(TimeSpan.Zero);

            this.GetUtcNow = getUtcNow;
            this.sqlServerLocator = sqlServerLocator;
            this.connectionTimeout = connectionTimeout;
            this.commandTimeout = commandTimeout;
        }

        /// <summary>
        /// Gets or sets the function to get the sampling time; allows for a single time to be used in multiple protocols.
        /// </summary>
        public Func<DateTime> GetUtcNow { get; set; }

        /// <inheritdoc />
        public override CheckJobsReport Execute(
            CheckJobsOp operation)
        {
            operation.MustForArg(nameof(operation)).NotBeNull();

            var utcNow = this.GetUtcNow();
            var jobNameToInfoMap = new Dictionary<string, IJobInformation>();
            var status = CheckStatus.Success;
            var sqlOperationProtocol = new SqlOperationsProtocol(this.sqlServerLocator, this.connectionTimeout, this.commandTimeout);
            foreach (var jobToCheck in operation.Jobs)
            {
                var getJobInfoOp = new GetLatestJobInformationOp(jobToCheck.Name);
                var jobInfo = (SqlServerJobInformation)sqlOperationProtocol.Execute(getJobInfoOp);

                if (jobInfo                   == null
                 || jobInfo.JobStatus         == JobStatus.Failed
                 || jobInfo.LatestStepRunTime == null
                 || utcNow                    > ((DateTime)jobInfo.LatestStepRunTime).Add(jobToCheck.Threshold))
                {
                    status = CheckStatus.Failure;
                }

                jobNameToInfoMap.Add(jobToCheck.Name, jobInfo);
            }

            var result = new CheckJobsReport(status, jobNameToInfoMap, utcNow);
            return result;
        }
    }
}