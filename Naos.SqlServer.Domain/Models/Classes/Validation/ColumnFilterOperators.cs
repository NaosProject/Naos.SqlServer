// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColumnFilterOperators.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Type;

    /// <summary>
    /// A pairing of a schema-qualified column name with the set of filter operators that
    /// are allowed when that column is used in a filter predicate (WHERE / HAVING / ON).
    /// </summary>
    /// <remarks>
    /// Used by <c>ConstrainedFilterOperatorsByColumnSqlScriptValidationRule</c> to express
    /// per-column operator allow-lists.  For example, restricting <c>dbo.users.entity_id</c>
    /// to <c>{ Equal, In }</c> permits <c>WHERE entity_id = 'abc'</c> and
    /// <c>WHERE entity_id IN ('a', 'b')</c> while rejecting <c>WHERE entity_id LIKE 'a%'</c>,
    /// <c>WHERE entity_id &gt; 'm'</c>, <c>WHERE entity_id BETWEEN 'a' AND 'z'</c>, etc.
    /// </remarks>
    public partial class ColumnFilterOperators : IModelViaCodeGen
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnFilterOperators"/> class.
        /// </summary>
        /// <param name="column">The schema-qualified column.</param>
        /// <param name="allowedOperators">The set of operators allowed on the column.</param>
        public ColumnFilterOperators(
            SchemaQualifiedColumnName column,
            FilterOperators allowedOperators)
        {
            new { column }.AsArg().Must().NotBeNull();
            new { allowedOperators }.AsArg().Must().NotBeEqualTo(FilterOperators.None);

            this.Column = column;
            this.AllowedOperators = allowedOperators;
        }

        /// <summary>
        /// Gets the schema-qualified column the operator allow-list applies to.
        /// </summary>
        public SchemaQualifiedColumnName Column { get; private set; }

        /// <summary>
        /// Gets the set of operators allowed on <see cref="Column"/>.  Any filter predicate
        /// that uses <see cref="Column"/> with an operator NOT in this set is flagged.
        /// </summary>
        public FilterOperators AllowedOperators { get; private set; }
    }
}
