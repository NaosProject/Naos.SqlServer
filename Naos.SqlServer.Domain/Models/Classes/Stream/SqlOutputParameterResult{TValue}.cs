// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlOutputParameterResult{TValue}.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Type;
    using OBeautifulCode.Type.Recipes;

    /// <summary>
    /// A representation of a SQL output parameter with it's result.
    /// </summary>
    /// <typeparam name="TValue">Type of the output value.</typeparam>
    public partial class SqlOutputParameterResult<TValue> : ISqlOutputParameterResult, IModelViaCodeGen
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlOutputParameterResult{TValue}"/> class.
        /// </summary>
        /// <param name="outputParameter">The name.</param>
        /// <param name="value">The value.</param>
        public SqlOutputParameterResult(
            OutputParameterDefinition<TValue> outputParameter,
            TValue value)
        {
            outputParameter.MustForArg(nameof(outputParameter)).NotBeNull();

            this.OutputParameter = outputParameter;
            this.Value = value;
        }

        /// <summary>
        /// Gets the sql output parameter.
        /// </summary>
        public OutputParameterDefinition<TValue> OutputParameter { get; private set; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public TValue Value { get; private set; }

        /// <inheritdoc />
        public TValueAtMethod GetValueOfType<TValueAtMethod>()
        {
            var valueType = typeof(TValue);

            var requestedType = typeof(TValueAtMethod);

            if (valueType != requestedType)
            {
                throw new ArgumentException(FormattableString.Invariant($"Value type is {valueType.ToStringReadable()} but is being requested as a {requestedType.ToStringReadable()}."));
            }

            var result = (TValueAtMethod)(object)this.Value;

            return result;
        }
    }
}
