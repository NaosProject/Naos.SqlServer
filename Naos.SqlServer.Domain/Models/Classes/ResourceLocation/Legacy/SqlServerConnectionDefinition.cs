// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlServerConnectionDefinition.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using Naos.Database.Domain.Legacy;
    using OBeautifulCode.Type;

    /// <summary>
    /// Contains the information required to connect to a SQL Server database.
    /// </summary>
    public partial class SqlServerConnectionDefinition : IConnectionDefinition, IModelViaCodeGen
    {
        /// <summary>
        /// Gets or sets the server name, IP, DNS, etc.
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        /// Gets or sets the instance name (if applicable).
        /// </summary>
        public string InstanceName { get; set; }

        /// <summary>
        /// Gets or sets the database name.
        /// </summary>
        public string DatabaseName { get; set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public string Password { get; set; }
    }
}
