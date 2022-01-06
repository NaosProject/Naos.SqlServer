﻿// --------------------------------------------------------------------------------------------------------------------
// <auto-generated>
//   Generated using OBeautifulCode.CodeGen.ModelObject (1.0.172.0)
// </auto-generated>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using global::System;
    using global::System.CodeDom.Compiler;
    using global::System.Collections.Concurrent;
    using global::System.Collections.Generic;
    using global::System.Collections.ObjectModel;
    using global::System.Diagnostics.CodeAnalysis;
    using global::System.Globalization;
    using global::System.Linq;

    using global::OBeautifulCode.Cloning.Recipes;
    using global::OBeautifulCode.Equality.Recipes;
    using global::OBeautifulCode.Type;
    using global::OBeautifulCode.Type.Recipes;

    using static global::System.FormattableString;

    [Serializable]
    public partial class InputParameterDefinition<TValue> : IModel<InputParameterDefinition<TValue>>
    {
        /// <summary>
        /// Determines whether two objects of type <see cref="InputParameterDefinition{TValue}"/> are equal.
        /// </summary>
        /// <param name="left">The object to the left of the equality operator.</param>
        /// <param name="right">The object to the right of the equality operator.</param>
        /// <returns>true if the two items are equal; otherwise false.</returns>
        public static bool operator ==(InputParameterDefinition<TValue> left, InputParameterDefinition<TValue> right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
            {
                return false;
            }

            var result = left.Equals(right);

            return result;
        }

        /// <summary>
        /// Determines whether two objects of type <see cref="InputParameterDefinition{TValue}"/> are not equal.
        /// </summary>
        /// <param name="left">The object to the left of the equality operator.</param>
        /// <param name="right">The object to the right of the equality operator.</param>
        /// <returns>true if the two items are not equal; otherwise false.</returns>
        public static bool operator !=(InputParameterDefinition<TValue> left, InputParameterDefinition<TValue> right) => !(left == right);

        /// <inheritdoc />
        public bool Equals(InputParameterDefinition<TValue> other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (ReferenceEquals(other, null))
            {
                return false;
            }

            var result = this.Name.IsEqualTo(other.Name, StringComparer.Ordinal)
                      && this.SqlDataType.IsEqualTo(other.SqlDataType)
                      && this.Value.IsEqualTo(other.Value);

            return result;
        }

        /// <inheritdoc />
        public override bool Equals(object obj) => this == (obj as InputParameterDefinition<TValue>);

        /// <inheritdoc />
        public override int GetHashCode() => HashCodeHelper.Initialize()
            .Hash(this.Name)
            .Hash(this.SqlDataType)
            .Hash(this.Value)
            .Value;

        /// <inheritdoc />
        public new InputParameterDefinition<TValue> DeepClone() => (InputParameterDefinition<TValue>)this.DeepCloneInternal();

        /// <inheritdoc />
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        protected override ParameterDefinitionBase DeepCloneInternal()
        {
            var result = new InputParameterDefinition<TValue>(
                                 this.Name?.DeepClone(),
                                 this.SqlDataType?.DeepClone(),
                                 this.Value == null ? default : this.Value.DeepClone());

            return result;
        }

        /// <inheritdoc />
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        public override string ToString()
        {
            var result = Invariant($"Naos.SqlServer.Domain.{this.GetType().ToStringReadable()}: Name = {this.Name?.ToString(CultureInfo.InvariantCulture) ?? "<null>"}, SqlDataType = {this.SqlDataType?.ToString() ?? "<null>"}, Value = {this.Value?.ToString() ?? "<null>"}.");

            return result;
        }
    }
}