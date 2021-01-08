// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeleteDatabaseOp.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using Naos.Protocol.Domain;
    using OBeautifulCode.Assertion.Recipes;

    /// <summary>
    /// Delete a database using the specified configuration.
    /// </summary>
    public partial class DeleteDatabaseOp : VoidOperationBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteDatabaseOp"/> class.
        /// </summary>
        /// <param name="databaseName">The database configuration.</param>
        public DeleteDatabaseOp(
            string databaseName)
        {
            databaseName.MustForArg(nameof(databaseName)).NotBeNullNorWhiteSpace();
            SqlInjectorChecker.ThrowIfNotAlphanumericOrSpaceOrUnderscore(databaseName);

            this.DatabaseName = databaseName;
        }

        /// <summary>
        /// Gets the database configuration.
        /// </summary>
        /// <value>The database configuration.</value>
        public string DatabaseName { get; private set; }
    }
}