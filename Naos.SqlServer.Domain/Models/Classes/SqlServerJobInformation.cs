// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlServerJobInformation.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System;
    using Naos.Database.Domain;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Type;

    /// <summary>
    /// Detailed information about a job on server.
    /// </summary>
    public partial class SqlServerJobInformation : IJobInformation, IHaveId<string>, IModelViaCodeGen
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerJobInformation"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="jobName">Name of the job.</param>
        /// <param name="latestStepName">Name of the latest step.</param>
        /// <param name="jobStatus">The job status.</param>
        /// <param name="latestStepRunTime">The latest step run time.</param>
        /// <param name="latestStepRunDuration">Duration of the latest step run.</param>
        public SqlServerJobInformation(
            string id,
            string jobName,
            string latestStepName,
            JobStatus jobStatus,
            DateTime? latestStepRunTime,
            TimeSpan? latestStepRunDuration)
        {
            id.MustForArg(nameof(id)).NotBeNullNorWhiteSpace();
            jobName.MustForArg(nameof(jobName)).NotBeNullNorWhiteSpace();
            jobStatus.MustForArg(nameof(jobStatus)).NotBeEqualTo(JobStatus.Invalid);

            this.Id = id;
            this.JobName = jobName;
            this.LatestStepName = latestStepName;
            this.JobStatus = jobStatus;
            this.LatestStepRunTime = latestStepRunTime;
            this.LatestStepRunDuration = latestStepRunDuration;
        }

        /// <inheritdoc />
        public string Id { get; private set; }

        /// <inheritdoc />
        public string JobName { get; private set; }

        /// <summary>
        /// Gets the name of the latest step.
        /// </summary>
        public string LatestStepName { get; private set; }

        /// <summary>
        /// Gets the job status.
        /// </summary>
        public JobStatus JobStatus { get; private set; }

        /// <summary>
        /// Gets the latest step run time.
        /// </summary>
        public DateTime? LatestStepRunTime { get; private set; }

        /// <summary>
        /// Gets the duration of the latest step run.
        /// </summary>
        public TimeSpan? LatestStepRunDuration { get; private set; }
    }
}