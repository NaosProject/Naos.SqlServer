// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamSchema.Schema.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System;

    /// <summary>
    /// Object table schema.
    /// </summary>
    public partial class StreamSchema
    {
        /// <summary>
        /// Builds the creation script for object table.
        /// </summary>
        /// <param name="streamName">Name of the stream.</param>
        /// <returns>Creation script for object table.</returns>
        public static string BuildCreationScriptForSchema(
            string streamName)
        {
            var result = FormattableString.Invariant($@"
			CREATE SCHEMA {streamName} AUTHORIZATION db_owner
			");

            return result;
        }
    }
}
