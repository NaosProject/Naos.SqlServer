// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlServerFileConstants.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System.Diagnostics.CodeAnalysis;
    using Naos.CodeAnalysis.Recipes;

    /// <summary>
    /// Constants used in project.
    /// </summary>
    public static class SqlServerFileConstants
    {
        /// <summary>
        /// The extension (WITHOUT the PERIOD) of an MS SQL Server Data File.
        /// </summary>
        public const string MicrosoftSqlDataFileExtension = "mdf";

        /// <summary>
        /// The extension (WITHOUT the PERIOD) of an MS SQL Server Log File.
        /// </summary>
        public const string MicrosoftSqlLogFileExtension = "ldf";

        /// <summary>
        /// The value to be specified for unlimited file growth in an MS SQL Server file.
        /// </summary>
        public const int InfinityMaxSize = -1;

        /// <summary>
        /// The default size of the data and log files for MS SQL Server.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Kb", Justification = NaosSuppressBecause.CA1709_IdentifiersShouldBeCasedCorrectly_CasingIsAsPreferred)]
        public const long MicrosoftSqlDefaultCurrentFileSizeInKb = 8000L;

        /// <summary>
        /// The default size of growth for the data and log files for MS SQL Server.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Kb", Justification = NaosSuppressBecause.CA1709_IdentifiersShouldBeCasedCorrectly_CasingIsAsPreferred)]
        public const long MicrosoftSqlDefaultFileGrowthSizeInKb = 64000L;
    }
}
