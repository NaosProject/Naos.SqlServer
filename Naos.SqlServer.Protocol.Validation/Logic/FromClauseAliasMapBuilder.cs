// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FromClauseAliasMapBuilder.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Protocol.Validation
{
    using System;
    using System.Collections.Generic;
    using Microsoft.SqlServer.TransactSql.ScriptDom;
    using Naos.SqlServer.Domain;

    /// <summary>
    /// Builds the <c>alias → SchemaQualifiedTableName</c> map for a <c>FromClause</c>.  Both
    /// explicit aliases (<c>FROM dbo.users u</c>) and implicit aliases (the table's base name
    /// when no <c>AS</c> is present) are captured.  Lookups are case-insensitive.
    /// </summary>
    /// <remarks>
    /// Shared between filter-predicate rules (WHERE / HAVING / ON column resolution) and
    /// join-shape rules (resolving the columns on each side of a JOIN ON predicate).  Two
    /// <c>NamedTableReference</c>s with no alias and the same base name will collide in the
    /// map — the second overwrites the first.  Self-join detection that needs to see every
    /// occurrence should NOT rely on the alias map; it should walk the FROM tree directly.
    /// </remarks>
    public static class FromClauseAliasMapBuilder
    {
        /// <summary>
        /// Builds the alias map for the supplied <paramref name="fromClause"/>.  Returns an
        /// empty map when <paramref name="fromClause"/> is <c>null</c> or has no table
        /// references.
        /// </summary>
        /// <param name="fromClause">The FROM clause to inspect.</param>
        /// <returns>The alias map.</returns>
        public static Dictionary<string, SchemaQualifiedTableName> Build(
            FromClause fromClause)
        {
            var map = new Dictionary<string, SchemaQualifiedTableName>(StringComparer.OrdinalIgnoreCase);

            if ((fromClause == null) || (fromClause.TableReferences == null))
            {
                return map;
            }

            foreach (var tableReference in fromClause.TableReferences)
            {
                CollectAliases(tableReference, map);
            }

            return map;
        }

        private static void CollectAliases(
            TableReference tableReference,
            Dictionary<string, SchemaQualifiedTableName> map)
        {
            if (tableReference == null)
            {
                return;
            }

            if (tableReference is NamedTableReference namedTableReference)
            {
                AddAliasForNamedTableReference(namedTableReference, map);
            }
            else if (tableReference is QualifiedJoin qualifiedJoin)
            {
                CollectAliases(qualifiedJoin.FirstTableReference, map);
                CollectAliases(qualifiedJoin.SecondTableReference, map);
            }
            else if (tableReference is UnqualifiedJoin unqualifiedJoin)
            {
                CollectAliases(unqualifiedJoin.FirstTableReference, map);
                CollectAliases(unqualifiedJoin.SecondTableReference, map);
            }
            else if (tableReference is JoinParenthesisTableReference joinParen)
            {
                CollectAliases(joinParen.Join, map);
            }

            // Other table-reference types (QueryDerivedTable, OpenRowsetTableReference, etc.)
            // either contribute nothing to an alias map (e.g. a derived table is its own
            // scope) or are blocked by other rules (FlatQuery, DisallowAdHocDistributedQueries).
        }

        private static void AddAliasForNamedTableReference(
            NamedTableReference namedTableReference,
            Dictionary<string, SchemaQualifiedTableName> map)
        {
            if ((namedTableReference == null) || (namedTableReference.SchemaObject == null) || (namedTableReference.SchemaObject.BaseIdentifier == null))
            {
                return;
            }

            var schemaIdentifier = namedTableReference.SchemaObject.SchemaIdentifier;
            var baseIdentifier = namedTableReference.SchemaObject.BaseIdentifier;

            // No schema → skip.  SchemaQualifiedTableReferencesSqlScriptValidationRule would
            // have already flagged this; the alias map deliberately ignores it so we don't
            // synthesize a SchemaQualifiedTableName with a null schema.
            if (schemaIdentifier == null)
            {
                return;
            }

            var schemaQualifiedTable = new SchemaQualifiedTableName(schemaIdentifier.Value, baseIdentifier.Value);

            var aliasKey = (namedTableReference.Alias != null) ? namedTableReference.Alias.Value : baseIdentifier.Value;

            map[aliasKey] = schemaQualifiedTable;
        }
    }
}
