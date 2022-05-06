// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OutputParameterDefinition{TValue}.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Type;

    /// <summary>
    /// A representation of a SQL output parameter.
    /// </summary>
    /// <typeparam name="TValue">Type of the output value.</typeparam>
    // ReSharper disable once RedundantExtendsListEntry
    public partial class OutputParameterDefinition<TValue> : OutputParameterDefinitionBase, IForsakeDeepCloneWithVariantsViaCodeGen, IModelViaCodeGen
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OutputParameterDefinition{TValue}"/> class.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="sqlDataType">The SQL data type of the parameter.</param>
        public OutputParameterDefinition(
            string name,
            SqlDataTypeRepresentationBase sqlDataType)
            : base(name, sqlDataType)
        {
            sqlDataType.MustForArg(nameof(sqlDataType)).NotBeNull();

            ThrowArgumentExceptionIfSqlDataTypeIsNotCompatibleWithDotNetDataType(sqlDataType, typeof(TValue), default(TValue), false);
        }
    }
}
