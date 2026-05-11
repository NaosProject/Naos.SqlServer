// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlServerVersion.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    /// <summary>
    /// SQL Server version.
    /// </summary>
    public enum SqlServerVersion
    {
        /// <summary>
        /// Unknown (default).
        /// </summary>
        Unknown,

        /// <summary>
        /// SQL Server 2000.
        /// </summary>
        SqlServer2000,

        /// <summary>
        /// SQL Server 2005.
        /// </summary>
        SqlServer2005,

        /// <summary>
        /// SQL Server 2008 (covers 2008 and R2).
        /// </summary>
        SqlServer2008,

        /// <summary>
        /// SQL Server 2012.
        /// </summary>
        SqlServer2012,

        /// <summary>
        /// SQL Server 2014.
        /// </summary>
        SqlServer2014,

        /// <summary>
        /// SQL Server 2016.
        /// </summary>
        SqlServer2016,

        /// <summary>
        /// SQL Server 2017.
        /// </summary>
        SqlServer2017,

        /// <summary>
        /// SQL Server 2019.
        /// </summary>
        SqlServer2019,

        /// <summary>
        /// SQL Server 2022.
        /// </summary>
        SqlServer2022,
    }
}
