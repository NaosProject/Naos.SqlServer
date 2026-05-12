// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisallowAdHocDistributedQueriesSqlScriptValidationRule.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    /// <summary>
    /// A rule that disallows ad-hoc distributed query constructs — <c>OPENROWSET</c> (all
    /// variants), <c>OPENQUERY</c>, and <c>OPENDATASOURCE</c> — in a SQL script.
    /// </summary>
    /// <remarks>
    /// <para>
    /// These constructs each provide a "back door" through which arbitrary SQL or data flow
    /// can enter the script:
    /// </para>
    /// <list type="bullet">
    /// <item><description><c>OPENROWSET(provider, conn, 'sql')</c> — connects to an OLE DB
    /// provider and executes the given query string against it.  The query string is opaque to
    /// the T-SQL parser, so any other rule that inspects script content (read-only enforcement,
    /// schema allow-lists, etc.) cannot see what the OPENROWSET payload does.</description></item>
    /// <item><description><c>OPENROWSET(BULK 'file', …)</c> — reads the contents of an
    /// arbitrary file on the server file system.  Not a SQL-transport vector but still an
    /// external-data ingress that bypasses normal table-permission controls.</description></item>
    /// <item><description><c>OPENROWSET</c> Cosmos variant — same back-door shape for Azure
    /// Cosmos DB queries.</description></item>
    /// <item><description><c>OPENQUERY(linked_server, 'sql')</c> — sends the given query
    /// string to a pre-configured linked server.  Again, the query string is opaque.</description></item>
    /// <item><description><c>OPENDATASOURCE(provider, conn).db.schema.table</c> — connects
    /// ad-hoc to a remote data source and treats it as a four-part name.  No string payload,
    /// but the data source is uncontrolled.</description></item>
    /// </list>
    /// <para>
    /// What is NOT flagged by this rule:
    /// </para>
    /// <list type="bullet">
    /// <item><description><c>OPENJSON(@json)</c> and <c>OPENXML(@idoc, '…')</c> — these are
    /// in-memory parsing functions that operate on JSON / XML strings already inside the
    /// script.  They do not connect to external data sources and are not distributed queries.</description></item>
    /// <item><description>Standard T-SQL <c>EXEC ('…')</c> or <c>EXEC sp_executesql N'…'</c>
    /// — these are <c>ExecuteStatement</c> nodes, not table references, and are handled by
    /// other rules (e.g. <see cref="ReadOnlySelectSqlScriptValidationRule"/> flags any
    /// non-SELECT statement at the top level).</description></item>
    /// <item><description>Calls to user-defined functions, CLR functions, or stored
    /// procedures that may internally use dynamic SQL — those are not visible to the parser.</description></item>
    /// </list>
    /// <para>
    /// The rule's name mirrors the SQL Server server-configuration option
    /// <c>'Ad Hoc Distributed Queries'</c>, which when disabled at the server level prevents
    /// <c>OPENROWSET</c> and <c>OPENDATASOURCE</c> from being used at runtime.  Applying this
    /// rule at the parser level catches violations before the script reaches the server, and
    /// also covers <c>OPENQUERY</c> (which the server-level setting does not control).
    /// </para>
    /// </remarks>
    public partial class DisallowAdHocDistributedQueriesSqlScriptValidationRule : SqlScriptValidationRuleBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DisallowAdHocDistributedQueriesSqlScriptValidationRule"/> class.
        /// </summary>
        /// <param name="id">OPTIONAL identifier.  DEFAULT is no identifier.</param>
        public DisallowAdHocDistributedQueriesSqlScriptValidationRule(
            string id = null)
            : base(id)
        {
        }
    }
}
