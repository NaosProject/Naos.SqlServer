// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlParameterRepresentationBase.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Type;

    /// <summary>
    /// Top level base of a SQL Parameter.
    /// </summary>
    public abstract partial class SqlParameterRepresentationBase : IModelViaCodeGen
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlParameterRepresentationBase"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        protected SqlParameterRepresentationBase(
            string name)
        {
            name.MustForArg(nameof(name)).NotBeNullNorWhiteSpace().And().BeAlphanumeric();

            this.Name = name;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; private set; }
    }
}
