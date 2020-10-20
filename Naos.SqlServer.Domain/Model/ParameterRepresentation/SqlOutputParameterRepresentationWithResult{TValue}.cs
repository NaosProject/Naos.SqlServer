// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlOutputParameterRepresentationWithResult{TValue}.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System;
    using OBeautifulCode.Type.Recipes;

    /// <summary>
    /// Top level .
    /// </summary>
    /// <typeparam name="TValue">Type of the Output value.</typeparam>
    public partial class SqlOutputParameterRepresentationWithResult<TValue> : SqlOutputParameterRepresentationBase, ISqlOutputParameterRepresentationWithResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlOutputParameterRepresentationWithResult{TValue}"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="dataType">The type.</param>
        /// <param name="value">The value.</param>
        public SqlOutputParameterRepresentationWithResult(
            string name,
            SqlDataTypeRepresentationBase dataType,
            TValue value)
            : base(name, dataType)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification = "The GetValue is really for use by the interface and the Value property when you have the object type exact type.")]
        public TValue Value { get; private set; }

        /// <inheritdoc />
        public TValueAtMethod GetValue<TValueAtMethod>()
        {
            var valueType = typeof(TValue);
            var requestedType = typeof(TValueAtMethod);
            if (valueType != requestedType)
            {
                throw new ArgumentException(FormattableString.Invariant($"Value type is {valueType.ToStringReadable()} but is being requested as a {requestedType.ToStringReadable()}."));
            }

            return (TValueAtMethod)(object)this.Value;
        }
    }
}
