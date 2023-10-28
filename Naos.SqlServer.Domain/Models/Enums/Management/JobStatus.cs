// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JobStatus.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    /// <summary>
    /// Status of a job.
    /// </summary>
    public enum JobStatus
    {
        /// <summary>
        /// Invalid default value.
        /// </summary>
        Invalid,

        /// <summary>
        /// The unknown status.
        /// </summary>
        Unknown,

        /// <summary>
        /// The not run status.
        /// </summary>
        NeverRun,

        /// <summary>
        /// The failed status.
        /// </summary>
        Failed,

        /// <summary>
        /// The succeeded status.
        /// </summary>
        Succeeded,

        /// <summary>
        /// The retry status.
        /// </summary>
        Retry,

        /// <summary>
        /// The canceled status.
        /// </summary>
        Canceled,
    }
}
