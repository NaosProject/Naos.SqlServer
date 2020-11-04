// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MigrationOptions.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// <summary>
//   Class to pass necessary options to runner.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Protocol.Migration
{
    using FluentMigrator;
    using OBeautifulCode.Type;

    /// <summary>
    /// Class to pass necessary options to runner.
    /// </summary>
    public class MigrationOptions : IMigrationProcessorOptions, IModelViaCodeGen
    {
        /// <summary>
        /// Gets or sets a value indicating whether to run in preview only mode.
        /// </summary>
        public bool PreviewOnly { get; set; }

        /// <summary>
        /// Gets or sets the timeout for the run.
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        /// Gets or sets the provider switches to use.
        /// </summary>
        public string ProviderSwitches { get; set; }
    }
}
