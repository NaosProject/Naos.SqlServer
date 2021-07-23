// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateDatabaseOp.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Type;

    /// <summary>
    /// Create a database using the specified configuration.
    /// </summary>
    public partial class CreateDatabaseOp : VoidOperationBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateDatabaseOp"/> class.
        /// </summary>
        /// <param name="databaseConfiguration">The database configuration.</param>
        public CreateDatabaseOp(
            DatabaseConfiguration databaseConfiguration)
        {
            databaseConfiguration.MustForArg(nameof(databaseConfiguration)).NotBeNull();

            this.DatabaseConfiguration = databaseConfiguration;
        }

        /// <summary>
        /// Gets the database configuration.
        /// </summary>
        /// <value>The database configuration.</value>
        public DatabaseConfiguration DatabaseConfiguration { get; private set; }
    }
}