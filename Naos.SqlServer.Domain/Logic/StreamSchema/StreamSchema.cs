// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamSchema.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text;
    using Naos.CodeAnalysis.Recipes;
    using Naos.Database.Domain;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Enum.Recipes;
    using static System.FormattableString;

    /// <summary>
    /// Container for schema.
    /// </summary>
    public partial class StreamSchema
    {
        /// <summary>
        /// The 'throw' statement must be within a certain range to bubble the error to ADO.NET, this is a general purpose number used for errors thrown directly from stored procedures in this data layer.
        /// </summary>
        public const int GeneralPurposeErrorNumberForThrowStatement = 60000;

        /// <summary>
        /// The 'throw' statement is not used but generally set to 1.
        /// </summary>
        public const int GeneralPurposeErrorStateForThrowStatement = 1;

        /// <summary>
        /// Prefix to add to all local copies of stored procedure parameters.
        /// </summary>
        public const string LocalVariablePrefix = "Local_";

        /// <summary>
        /// Comment to use for local variables.
        /// </summary>
        public const string LocalVariableComment = @"
    -- SQL Server caches execution plans keyed in part by the connection's SET options. ADO.NET connects with SET ARITHABORT OFF.
    -- Because of this, the first call to this stored procedure from .NET will cache query plans for the statements contained within
    -- the stored procedure (NOT one plan per SP, but actually one plan per statement - this SP may generate a handful of plans).
    -- The cached query plans are based on the particular set of parameter values that the stored procedure is called with.
    -- This may cause the optimizer to generate a terrible plan (e.g., one that does a table scan instead of an index seek)
    -- for a different set of parameter values.  Because these stored procedures provide many parameters and a lot of flexibility
    -- in which are used and what values they contain, we should not cache a query plan based on the first call to the SP.
    -- Local variables are the solution.  SQL Server only sniffs the procedure's parameters, not local variables. When the optimizer
    -- sees a local variable, it can't peek at the runtime value during compilation, so it falls back to average density statistics.
    -- It won't produce the absolute best plan for any particular value, but it'll produce a reasonably good plan for all values.
    -- SQL Server updates statistics automatically in the background when a sufficient percentage of rows have changed.
    -- However, it's also a good practice to supplement that with a job that keeps the statistics fresh.";

        /// <summary>
        /// Builds the creation script for object table.
        /// </summary>
        /// <param name="streamName">Name of the stream.</param>
        /// <returns>Creation script for object table.</returns>
        public static string BuildCreationScriptForSchema(
            string streamName)
        {
            var result = FormattableString.Invariant($@"
			CREATE SCHEMA [{streamName}] AUTHORIZATION db_owner
			");

            return result;
        }

        /// <summary>
        /// Builds the creation script for roles.
        /// </summary>
        /// <param name="streamName">Name of the stream.</param>
        /// <returns>Creation script for roles.</returns>
        public static string BuildCreationScriptForRoles(
            string streamName)
        {
            var allRoles = CreateStreamUserOp.SupportedStreamAccessKinds;

            var result = new StringBuilder();

            foreach (var role in allRoles)
            {
                var roleName = GetRoleNameFromStreamAccessKind(role, streamName);

                result.AppendLine(FormattableString.Invariant($@"CREATE ROLE [{roleName}] AUTHORIZATION db_owner"));

                result.AppendLine(BuildGrantScriptByStreamAccessKind(role, streamName));
            }

            return result.ToString();
        }

        /// <summary>
        /// Builds the grant script for single <see cref="StreamAccessKinds"/>.
        /// </summary>
        /// <param name="singleStreamAccessKind">Single <see cref="StreamAccessKinds"/> to grant.</param>
        /// <param name="streamName">Name of the stream.</param>
        /// <returns>Grant script.</returns>
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = NaosSuppressBecause.CA1506_AvoidExcessiveClassCoupling_DisagreeWithAssessment)]
        public static string BuildGrantScriptByStreamAccessKind(
            StreamAccessKinds singleStreamAccessKind,
            string streamName)
        {
            var result = new StringBuilder();

            var roleName = GetRoleNameFromStreamAccessKind(singleStreamAccessKind, streamName);

            string BuildGrant(string resourceName, string permission)
            {
                var fullName = Invariant($"[{streamName}].[{resourceName}]");

                var grantExec = Invariant($"GRANT {permission} ON {fullName} TO [{roleName}]");

                return grantExec;
            }

            var execute = "EXECUTE";
            var read = "SELECT";
            var readWrite = "SELECT,INSERT";

            // Base permissions for all roles.
            result.AppendLine(BuildGrant(Funcs.AdjustForGetStringSerializedId.Name, execute));
            result.AppendLine(BuildGrant(Funcs.AdjustForPutStringSerializedId.Name, execute));
            result.AppendLine(BuildGrant(Funcs.GetTagsTableVariableFromTagsXml.Name, read));
            result.AppendLine(BuildGrant(Sprocs.GetStreamDetails.Name, execute));
            result.AppendLine(BuildGrant(Sprocs.GetIdAddIfNecessarySerializerRepresentation.Name, execute));
            result.AppendLine(BuildGrant(Sprocs.GetIdAddIfNecessaryTypeWithoutVersion.Name, execute));
            result.AppendLine(BuildGrant(Sprocs.GetIdAddIfNecessaryTypeWithVersion.Name, execute));
            result.AppendLine(BuildGrant(Sprocs.GetIdsAddIfNecessaryTagSet.Name, execute));
            result.AppendLine(BuildGrant(Sprocs.GetSerializerRepresentationFromId.Name, execute));
            result.AppendLine(BuildGrant(Sprocs.GetTypeFromId.Name, execute));
            result.AppendLine(BuildGrant(Sprocs.GetTagSetFromIds.Name, execute));
            result.AppendLine(BuildGrant(Tables.SerializerRepresentation.Table.Name, readWrite));
            result.AppendLine(BuildGrant(Tables.TypeWithoutVersion.Table.Name, readWrite));
            result.AppendLine(BuildGrant(Tables.TypeWithVersion.Table.Name, readWrite));
            result.AppendLine(BuildGrant(Tables.Tag.Table.Name, readWrite));

            if (singleStreamAccessKind == StreamAccessKinds.Read)
            {
                result.AppendLine(BuildGrant(Sprocs.GetLatestRecord.Name, execute));
                result.AppendLine(BuildGrant(Sprocs.GetDistinctStringSerializedIds.Name, execute));
                result.AppendLine(BuildGrant(Sprocs.GetLatestStringSerializedObject.Name, execute));
                result.AppendLine(BuildGrant(Sprocs.GetInternalRecordIds.Name, execute));
                result.AppendLine(BuildGrant(Tables.Record.Table.Name, read));
                result.AppendLine(BuildGrant(Tables.RecordTag.Table.Name, read));
            }
            else if (singleStreamAccessKind == StreamAccessKinds.Write)
            {
                result.AppendLine(BuildGrant(Sprocs.PutRecord.Name, execute));
                result.AppendLine(BuildGrant(Sprocs.GetNextUniqueLong.Name, execute));
                result.AppendLine(BuildGrant(Tables.NextUniqueLong.Table.Name, readWrite));
                result.AppendLine(BuildGrant(Tables.Record.Table.Name, readWrite));
                result.AppendLine(BuildGrant(Tables.RecordTag.Table.Name, readWrite));
            }
            else if (singleStreamAccessKind == StreamAccessKinds.Handle)
            {
                result.AppendLine(BuildGrant(Sprocs.TryHandleRecord.Name, execute));
                result.AppendLine(BuildGrant(Sprocs.PutHandling.Name, execute));
                result.AppendLine(BuildGrant(Sprocs.GetHandlingStatuses.Name, execute));
                result.AppendLine(BuildGrant(Sprocs.GetHandlingHistory.Name, execute));
                result.AppendLine(BuildGrant(Tables.Record.Table.Name, read));
                result.AppendLine(BuildGrant(Tables.RecordTag.Table.Name, read));
                result.AppendLine(BuildGrant(Tables.Handling.Table.Name, readWrite));
                result.AppendLine(BuildGrant(Tables.HandlingTag.Table.Name, readWrite));
            }
            else if (singleStreamAccessKind == StreamAccessKinds.Manage)
            {
                result.AppendLine(BuildGrant(Sprocs.CreateStreamUser.Name, execute));
            }
            else
            {
                throw new NotSupportedException(Invariant($"{nameof(StreamAccessKinds)} {singleStreamAccessKind} is not supported for granting table/sproc/function access."));
            }

            return result.ToString();
        }

        /// <summary>
        /// Gets the role name from the <see cref="StreamAccessKinds"/> and the stream name.
        /// </summary>
        /// <param name="singleStreamAccessKind">Single <see cref="StreamAccessKinds"/>.</param>
        /// <param name="streamName">Name of the stream.</param>
        /// <returns>Name of role.</returns>
        public static string GetRoleNameFromStreamAccessKind(
            StreamAccessKinds singleStreamAccessKind,
            string streamName)
        {
            var streamAccessKindsIndividualItems = singleStreamAccessKind.GetIndividualFlags();
            streamAccessKindsIndividualItems.Count.MustForArg(nameof(streamAccessKindsIndividualItems)).BeEqualTo(1, "Can only convert a single flag to a role name.");
            var roleName = streamAccessKindsIndividualItems.Single().ToString();

            var result = Invariant($@"{streamName}-{roleName}");

            return result;
        }
    }
}
