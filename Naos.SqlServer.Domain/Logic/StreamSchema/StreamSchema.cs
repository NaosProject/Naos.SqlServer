// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamSchema.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
    using Naos.CodeAnalysis.Recipes;
    using Naos.Database.Domain;
    using OBeautifulCode.Representation.System;
    using static System.FormattableString;

    /// <summary>
    /// Container for schema.
    /// </summary>
    public partial class StreamSchema
    {
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
            var allRoles = CreateStreamUserOp.SupportedProtocolTypeRepresentations;

            var result = new StringBuilder();

            foreach (var role in allRoles)
            {
                var roleName = GetRoleNameFromProtocolType(role, streamName);

                result.AppendLine(FormattableString.Invariant($@"CREATE ROLE [{roleName}] AUTHORIZATION db_owner"));

                result.AppendLine(BuildGrantScriptByType(role, streamName));
            }

            return result.ToString();
        }

        /// <summary>
        /// Builds the grant script by protocol type.
        /// </summary>
        /// <param name="protocolType">Type of the protocol.</param>
        /// <param name="streamName">Name of the stream.</param>
        /// <returns>Grant script.</returns>
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = NaosSuppressBecause.CA1506_AvoidExcessiveClassCoupling_DisagreeWithAssessment)]
        public static string BuildGrantScriptByType(
            TypeRepresentation protocolType,
            string streamName)
        {
            var result = new StringBuilder();

            var roleName = GetRoleNameFromProtocolType(protocolType, streamName);

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
            result.AppendLine(BuildGrant(Funcs.GetTagsTableVariableFromTagsXml.Name, read));
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

            if (protocolType == typeof(IStreamReadProtocols).ToRepresentation())
            {
                result.AppendLine(BuildGrant(Sprocs.GetLatestRecordMetadataById.Name, execute));
                result.AppendLine(BuildGrant(Sprocs.GetLatestRecordById.Name, execute));
                result.AppendLine(BuildGrant(Tables.Record.Table.Name, read));
                result.AppendLine(BuildGrant(Tables.RecordTag.Table.Name, read));
            }
            else if (protocolType == typeof(IStreamWriteProtocols).ToRepresentation())
            {
                result.AppendLine(BuildGrant(Sprocs.PutRecord.Name, execute));
                result.AppendLine(BuildGrant(Sprocs.GetNextUniqueLong.Name, execute));
                result.AppendLine(BuildGrant(Tables.NextUniqueLong.Table.Name, readWrite));
                result.AppendLine(BuildGrant(Tables.Record.Table.Name, readWrite));
                result.AppendLine(BuildGrant(Tables.RecordTag.Table.Name, readWrite));
            }
            else if (protocolType == typeof(IStreamRecordHandlingProtocols).ToRepresentation())
            {
                result.AppendLine(BuildGrant(Sprocs.TryHandleRecord.Name, execute));
                result.AppendLine(BuildGrant(Sprocs.PutHandling.Name, execute));
                result.AppendLine(BuildGrant(Sprocs.GetCompositeHandlingStatus.Name, execute));
                result.AppendLine(BuildGrant(Tables.Record.Table.Name, read));
                result.AppendLine(BuildGrant(Tables.RecordTag.Table.Name, read));
                result.AppendLine(BuildGrant(Tables.Handling.Table.Name, readWrite));
                result.AppendLine(BuildGrant(Tables.HandlingTag.Table.Name, readWrite));
                // result.AppendLine(BuildGrant(Tables.CompositeHandlingStatusSortOrder.Table.Name, read));
            }
            else if (protocolType == typeof(IStreamManagementProtocols).ToRepresentation())
            {
                result.AppendLine(BuildGrant(Sprocs.CreateStreamUser.Name, execute));
            }
            else
            {
                throw new NotSupportedException(Invariant($"Type {protocolType} is not supported for granting table/sproc/function access."));
            }

            return result.ToString();
        }

        /// <summary>
        /// Gets the role name from the type and the stream name.
        /// </summary>
        /// <param name="protocolType">Type of the protocol.</param>
        /// <param name="streamName">Name of the stream.</param>
        /// <returns>Name of role.</returns>
        public static string GetRoleNameFromProtocolType(
            TypeRepresentation protocolType,
            string streamName)
        {
            var roleName = protocolType.Name.Substring(1, protocolType.Name.Length - 1); // strip off I for the interface.

            var result = Invariant($@"{streamName}-{roleName}");

            return result;
        }
    }
}
