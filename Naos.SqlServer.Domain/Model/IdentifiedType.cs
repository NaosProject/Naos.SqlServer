// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IdentifiedType.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System.Collections.Generic;
    using OBeautifulCode.Type;

    /// <summary>
    /// Container for identifiers of a type (with and without version).
    /// </summary>
    public partial class IdentifiedType : IModelViaCodeGen
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IdentifiedType"/> class.
        /// </summary>
        /// <param name="idWithoutVersion">The identifier without version.</param>
        /// <param name="idWithVersion">The identifier with version.</param>
        public IdentifiedType(
#pragma warning disable SA1305 // Field names should not use Hungarian notation
#pragma warning disable SA1114 // Parameter list should follow declaration
            int idWithoutVersion,
#pragma warning restore SA1114 // Parameter list should follow declaration
#pragma warning restore SA1305 // Field names should not use Hungarian notation
#pragma warning disable SA1305 // Field names should not use Hungarian notation
            int idWithVersion)
#pragma warning restore SA1305 // Field names should not use Hungarian notation
        {
            this.IdWithoutVersion = idWithoutVersion;
            this.IdWithVersion = idWithVersion;
        }

        /// <summary>
        /// Gets the identifier without version.
        /// </summary>
        /// <value>The identifier without version.</value>
        public int IdWithoutVersion { get; private set; }

        /// <summary>
        /// Gets the identifier with version.
        /// </summary>
        /// <value>The identifier with version.</value>
        public int IdWithVersion { get; private set; }
    }
}
