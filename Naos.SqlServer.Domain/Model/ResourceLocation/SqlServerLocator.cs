// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlServerLocator.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System;
    using System.Globalization;
    using Naos.Database.Domain;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Type;

    /// <summary>
    /// SQL implementation of an <see cref="ResourceLocatorBase" />.
    /// </summary>
    public partial class SqlServerLocator : ResourceLocatorBase, ISqlServerLocator, IModelViaCodeGen, IDeclareToStringMethod
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerLocator"/> class.
        /// </summary>
        /// <param name="serverName">Name of the server.</param>
        /// <param name="databaseName">Name of the database.</param>
        /// <param name="userName">Name of user.</param>
        /// <param name="password">Password of user.</param>
        /// <param name="instanceName">Optional name of the instance, default is null.</param>
        /// <param name="port">Optional port, default is 1433.</param>
        public SqlServerLocator(
            string serverName,
            string databaseName,
            string userName,
            string password,
            string instanceName = null,
            int? port = null)
        {
            serverName.MustForArg(nameof(serverName)).NotBeNullNorWhiteSpace();
            databaseName.MustForArg(nameof(databaseName)).NotBeNullNorWhiteSpace();
            this.ServerName = serverName;
            this.DatabaseName = databaseName;
            this.UserName = userName;
            this.Password = password;
            this.InstanceName = instanceName;
            this.Port = port;
        }

        /// <summary>
        /// Gets the name of the server.
        /// </summary>
        /// <value>The name of the server.</value>
        public string ServerName { get; private set; }

        /// <summary>
        /// Gets the name of the database.
        /// </summary>
        /// <value>The name of the database.</value>
        public string DatabaseName { get; private set; }

        /// <summary>
        /// Gets the name of the user.
        /// </summary>
        /// <value>The name of the user.</value>
        public string UserName { get; private set; }

        /// <summary>
        /// Gets the password.
        /// </summary>
        /// <value>The password.</value>
        public string Password { get; private set; }

        /// <summary>
        /// Gets the name of the instance name.
        /// </summary>
        /// <value>The name of the instance.</value>
        public string InstanceName { get; private set; }

        /// <summary>
        /// Gets the port.
        /// </summary>
        /// <value>The port.</value>
        public int? Port { get; private set; }

        /// <inheritdoc cref="IDeclareToStringMethod" />
        public override string ToString()
        {
            var result = FormattableString.Invariant($"Naos.SqlServer.Domain.SqlServerLocator: ServerName = {this.ServerName?.ToString(CultureInfo.InvariantCulture) ?? "<null>"}, DatabaseName = {this.DatabaseName?.ToString(CultureInfo.InvariantCulture) ?? "<null>"}, UserName = {this.UserName?.ToString(CultureInfo.InvariantCulture) ?? "<null>"}, Password = ***, InstanceName = {this.InstanceName?.ToString(CultureInfo.InvariantCulture) ?? "<null>"}, Port = {this.Port?.ToString(CultureInfo.InvariantCulture) ?? "<null>"}.");

            return result;
        }

        /// <summary>
        /// Builds the invalid stream locator type exception.
        /// </summary>
        /// <param name="typeOfLocator">The type of locator.</param>
        /// <returns>Correct exception.</returns>
        public static Exception BuildInvalidLocatorException(
            Type typeOfLocator)
        {
            var message = FormattableString.Invariant($"Only {nameof(SqlServerLocator)}'s are supported; provided was: {typeOfLocator}");
            var result = new NotSupportedException(message);

            return result;
        }
    }
}
