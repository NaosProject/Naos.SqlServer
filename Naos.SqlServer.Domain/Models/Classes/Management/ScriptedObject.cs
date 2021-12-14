// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScriptedObject.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Type;

    /// <summary>
    /// Model object to hold a scripted object that can be applied to a different database.
    /// </summary>
    public partial class ScriptedObject : IModelViaCodeGen
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptedObject"/> class.
        /// </summary>
        /// <param name="name">Name of the object.</param>
        /// <param name="scriptableObjectType">The type of scripted object.</param>
        /// <param name="dropScript">Script to drop the object.</param>
        /// <param name="createScript">Script to create the object.</param>
        public ScriptedObject(
            string name,
            ScriptableObjectType scriptableObjectType,
            string dropScript,
            string createScript)
        {
            new { name }.AsArg().Must().NotBeNullNorWhiteSpace();
            new { scriptableObjectType }.AsArg().Must().NotBeEqualTo(ScriptableObjectType.Invalid);
            new { dropScript }.AsArg().Must().NotBeNullNorWhiteSpace();
            new { createScript }.AsArg().Must().NotBeNullNorWhiteSpace();

            this.Name = name;
            this.ScriptableObjectType = scriptableObjectType;
            this.DropScript = dropScript;
            this.CreateScript = createScript;
        }

        /// <summary>
        /// Gets the name of the object.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the type of scripted object.
        /// </summary>
        public ScriptableObjectType ScriptableObjectType { get; private set; }

        /// <summary>
        /// Gets the drop script.
        /// </summary>
        public string DropScript { get; private set; }

        /// <summary>
        /// Gets the create script.
        /// </summary>
        public string CreateScript { get; private set; }
    }
}
