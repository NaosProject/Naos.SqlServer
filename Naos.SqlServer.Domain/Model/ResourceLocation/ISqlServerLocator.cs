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
        /// <value>The name of the server.</value>
        string ServerName { get; }

        /// <summary>
        /// Gets the name of the database.
        /// </summary>
        /// <value>The name of the database.</value>
        string DatabaseName { get; }

        /// <summary>
        /// Gets the name of the user.
        /// </summary>
        /// <value>The name of the user.</value>
        string UserName { get; }

        /// <summary>
        /// Gets the password.
        /// </summary>
        /// <value>The password.</value>
        string Password { get; }

        /// <summary>
        /// Gets the name of the instance name.
        /// </summary>
        /// <value>The name of the instance.</value>
        string InstanceName { get; }

        /// <summary>
        /// Gets the port.
        /// </summary>
        /// <value>The port.</value>
        int? Port { get; }
    }
}
