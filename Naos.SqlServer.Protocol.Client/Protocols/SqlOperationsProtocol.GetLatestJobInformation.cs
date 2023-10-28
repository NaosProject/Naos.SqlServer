// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlOperationsProtocol.GetLatestJobInformation.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Protocol.Client
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Naos.Database.Domain;
    using Naos.SqlServer.Domain;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Database.Recipes;
    using static System.FormattableString;

    /// <summary>
    /// Sql Operation Protocol.
    /// </summary>
    public partial class SqlOperationsProtocol
    {
        /// <inheritdoc />
        public IJobInformation Execute(
            GetLatestJobInformationOp operation)
        {
            operation.MustForArg(nameof(operation)).NotBeNull();

            var query = @"SELECT
   job.job_id as [Id],
   job.name as [JobName],
   history.step_name as [LatestStepName],
   (CASE ISNULL(history.run_status, -1)
                WHEN -1 THEN 'NeverRun'
                WHEN 0 THEN 'Failed'
                WHEN 1 THEN 'Succeeded'
                WHEN 2 THEN 'Retry'
                WHEN 3 THEN 'Canceled'
                ELSE 'Running' END) as [JobStatus],
	msdb.dbo.agent_datetime(history.run_date, history.run_time) as [LatestStepRunTime],
	STUFF(STUFF(RIGHT('000000' + CAST (history.run_duration AS VARCHAR(6)),6),5,0,':'),3,0,':') as [LatestStepRunDuration]
	FROM msdb.dbo.sysjobs job
	LEFT JOIN msdb.dbo.sysjobhistory AS history
		ON job.job_id = history.job_id
	LEFT JOIN msdb.dbo.sysjobhistory AS history1
		ON history.job_id = history1.job_id AND history.instance_id < history1.instance_id
	WHERE
		history1.instance_id IS NULL -- get latest entry
		AND job.name = @JobName";

            var parameters = new[]
                             {
                                 operation.JobName.CreateInputSqlParameter(nameof(operation.JobName)),
                             };
            var connectionString = this.sqlServerLocator
                                       .DeepCloneWithDatabaseName(SqlServerDatabaseManager.MasterDatabaseName)
                                       .BuildConnectionString(this.defaultConnectionTimeout);
            var rows = connectionString.ReadAllRowsWithNamedColumns(query, (int)this.defaultCommandTimeout.TotalSeconds, parameters);
            if (!rows.Any())
            {
                return null;
            }

            if (rows.Count > 1)
            {
                throw new InvalidOperationException(Invariant($"Query for Sql Job information (name: {operation.JobName}) expected a single row but returned {rows.Count}."));
            }

            var row = rows.Single();
            var id = row[nameof(SqlServerJobInformation.Id)]?.ToString();
            var jobName = row[nameof(SqlServerJobInformation.JobName)]?.ToString();
            var latestStepName = row[nameof(SqlServerJobInformation.LatestStepName)]?.ToString();
            var jobStatusRaw = row[nameof(SqlServerJobInformation.JobStatus)]?.ToString();
            var jobStatus = string.IsNullOrWhiteSpace(jobStatusRaw) ? JobStatus.Unknown : (JobStatus)Enum.Parse(typeof(JobStatus), jobStatusRaw);
            var latestStepRunTimeRaw = (DateTime?)row[nameof(SqlServerJobInformation.LatestStepRunTime)];

            // time comes back as Unspecified so update to Local since that's what is actually being returned from the server.
            var latestStepRunTime = latestStepRunTimeRaw == null
                ? (DateTime?)null
                : DateTime.SpecifyKind((DateTime)latestStepRunTimeRaw, DateTimeKind.Local);
            var latestStepRunDurationRaw = row[nameof(SqlServerJobInformation.LatestStepRunDuration)]?.ToString();
            var latestStepRunDuration = latestStepRunDurationRaw == null ? (TimeSpan?)null : TimeSpan.Parse(latestStepRunDurationRaw, CultureInfo.InvariantCulture);
            var result = new SqlServerJobInformation(
                id,
                jobName,
                latestStepName,
                jobStatus,
                latestStepRunTime,
                latestStepRunDuration);
            return result;
        }

        /// <inheritdoc />
        public async Task<IJobInformation> ExecuteAsync(
            GetLatestJobInformationOp operation)
        {
            var syncResult = this.Execute(operation);
            var result = await Task.FromResult(syncResult);
            return result;
        }
    }
}