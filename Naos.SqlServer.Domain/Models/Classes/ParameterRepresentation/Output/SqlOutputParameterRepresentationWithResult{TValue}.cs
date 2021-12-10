// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlOutputParameterRepresentationWithResult{TValue}.cs" company="Naos Project">
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
    /// <typeparam name="TValue">Type of the Output value.</typeparam>
    public partial class SqlOutputParameterRepresentationWithResult<TValue> : SqlOutputParameterRepresentationBase, ISqlOutputParameterRepresentationWithResult, IModelViaCodeGen
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlOutputParameterRepresentationWithResult{TValue}"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="sqlDataType">The data type.</param>
        /// <param name="value">The value.</param>
        public SqlOutputParameterRepresentationWithResult(
            string name,
            SqlDataTypeRepresentationBase sqlDataType,
            TValue value)
            : base(name, sqlDataType)
        {
            sqlDataType.MustForArg(nameof(sqlDataType)).NotBeNull();

            this.ThrowArgumentExceptionIfSqlDataTypeIsNotCompatibleWithDotNetDataType(sqlDataType, typeof(TValue));

            this.Value = value;
        }

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
