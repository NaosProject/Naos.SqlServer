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
        /// <param name="definition">The database definition.</param>
        public CreateDatabaseOp(
            DatabaseDefinition definition)
        {
            definition.MustForArg(nameof(definition)).NotBeNull();

            this.Definition = definition;
        }

        /// <summary>
        /// Gets the database definition.
        /// </summary>
        public DatabaseDefinition Definition { get; private set; }
    }
}