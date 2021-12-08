// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISqlServerLocator.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using Naos.Database.Domain;

    /// <summary>
    /// Interface for the properties needed to construct a SQL connection string, implemented by <see cref="SqlServerLocator"/>.
    /// </summary>
    public interface ISqlServerLocator : IResourceLocator
    {
        /// <summary>
        /// Gets the name of the server.
        /// </summary>
        string ServerName { get; }

        /// <summary>
        /// Gets the name of the database.
        /// </summary>
        string DatabaseName { get; }

        /// <summary>
        /// Gets the name of the user.
        /// </summary>
        string UserName { get; }

        /// <summary>
        /// Gets the password.
        /// </summary>
        string Password { get; }

        /// <summary>
        /// Gets the name of the instance name.
        /// </summary>
        string InstanceName { get; }

        /// <summary>
        /// Gets the port.
        /// </summary>
        int? Port { get; }
    }
}
