// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RestoreFile.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System.Diagnostics.CodeAnalysis;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Type;

    /// <summary>
    /// Represents a file to restore.
    /// </summary>
    public partial class RestoreFile : IModelViaCodeGen
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RestoreFile"/> class.
        /// </summary>
        /// <param name="logicalName">Name of the logical.</param>
        /// <param name="physicalName">Name of the physical.</param>
        /// <param name="type">The type.</param>
        public RestoreFile(
            string logicalName,
            string physicalName,
            string type)
        {
            logicalName.MustForArg(nameof(logicalName)).NotBeNullNorWhiteSpace();
            physicalName.MustForArg(nameof(physicalName)).NotBeNullNorWhiteSpace();
            type.MustForArg(nameof(type)).NotBeNullNorWhiteSpace();

            this.LogicalName = logicalName;
            this.PhysicalName = physicalName;
            this.Type = type;
        }

        /// <summary>
        /// Gets the metadata name of the file.
        /// </summary>
        public string LogicalName { get; private set; }

        /// <summary>
        /// Gets the path to the file.
        /// </summary>
        public string PhysicalName { get; private set; }

        /// <summary>
        /// Gets the type of file.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification = "Matches sys schema in SQL Server.")]
        public string Type { get; private set; }
    }
}
