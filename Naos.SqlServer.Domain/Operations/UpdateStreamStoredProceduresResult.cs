// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateStreamStoredProceduresResult.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using OBeautifulCode.Type;
    using static System.FormattableString;

    /// <summary>
    /// Result of <see cref="UpdateStreamStoredProceduresOp"/>.
    /// </summary>
    public partial class UpdateStreamStoredProceduresResult : IModelViaCodeGen
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateStreamStoredProceduresResult"/> class.
        /// </summary>
        /// <param name="priorVersion">The prior version; null if could not be determined, comma separated list if multiple versions on the different locators.</param>
        public UpdateStreamStoredProceduresResult(
            string priorVersion)
        {
            this.PriorVersion = priorVersion;
        }

        /// <summary>
        /// Gets the prior version; null if could not be determined, comma separated list if multiple versions on the different locators.
        /// </summary>
        /// <value>The prior version; null if could not be determined, comma separated list if multiple versions on the different locators.</value>
        public string PriorVersion { get; private set; }
    }
}