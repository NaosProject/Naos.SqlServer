// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamSchema.Sprocs.GetStreamDetails.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System;
    using System.Collections.Generic;
    using static System.FormattableString;

    public static partial class StreamSchema
    {
        public static partial class Sprocs
        {
            /// <summary>
            /// Stored procedure: GetStreamDetails.
            /// </summary>
            public static class GetStreamDetails
            {
                /// <summary>
                /// Gets the name of the stored procedure.
                /// </summary>
                public static string Name => nameof(GetStreamDetails);

                /// <summary>
                /// Gets the version key used in the output dictionary.
                /// </summary>
                public static string VersionKey => Invariant(
                    $"{nameof(Naos)}.{nameof(Naos.SqlServer)}.{nameof(Naos.SqlServer.Domain)}.{nameof(Type.Assembly)}.{nameof(Version)}");

                /// <summary>
                /// Output parameter names.
                /// </summary>
                public enum OutputParamName
                {
                    /// <summary>
                    /// The details.
                    /// </summary>
                    DetailsXml,
                }

                /// <summary>
                /// Builds the execute stored procedure operation.
                /// </summary>
                /// <param name="streamName">Name of the stream.</param>
                /// <returns>Operation to execute stored procedure.</returns>
                public static ExecuteStoredProcedureOp BuildExecuteStoredProcedureOp(
                    string streamName)
                {
                    var sprocName = Invariant($"[{streamName}].[{nameof(GetStreamDetails)}]");

                    var parameters = new List<SqlParameterDefinitionBase>
                                     {
                                         new SqlOutputParameterDefinition<string>(nameof(OutputParamName.DetailsXml), new XmlSqlDataTypeRepresentation()),
                                     };

                    var result = new ExecuteStoredProcedureOp(sprocName, parameters);

                    return result;
                }

                /// <summary>
                /// Builds the name of the put stored procedure.
                /// </summary>
                /// <param name="streamName">Name of the stream.</param>
                /// <param name="recordTagAssociationManagementStrategy">The optional record tag association management strategy; DEFAULT is AssociatedDuringPutInSprocInTransaction."/>.</param>
                /// <param name="maxConcurrentHandlingCount">The optional maximum concurrent handling count; DEFAULT is no limit.</param>
                /// <param name="asAlter">An optional value indicating whether or not to generate a ALTER versus CREATE; DEFAULT is false and will generate a CREATE script.</param>
                /// <returns>Creation script for creating the stored procedure.</returns>
                public static string BuildCreationScript(
                    string streamName,
                    RecordTagAssociationManagementStrategy recordTagAssociationManagementStrategy,
                    int? maxConcurrentHandlingCount,
                    bool asAlter = false)
                {
                    var createOrModify = asAlter ? "ALTER" : "CREATE";
                    var result = Invariant(
                        $@"
{createOrModify} PROCEDURE [{streamName}].[{GetStreamDetails.Name}](
  @{OutputParamName.DetailsXml} {new XmlSqlDataTypeRepresentation().DeclarationInSqlSyntax} OUTPUT
)
AS
BEGIN
    SELECT @{OutputParamName.DetailsXml} = '<Tags><Tag Key=""{VersionKey}"" Value=""{typeof(GetStreamDetails).Assembly.GetName().Version}"" /><Tag Key=""{nameof(UpdateStreamStoredProceduresOp.RecordTagAssociationManagementStrategy)}"" Value=""{recordTagAssociationManagementStrategy}"" /><Tag Key=""{nameof(UpdateStreamStoredProceduresOp.MaxConcurrentHandlingCount)}"" Value=""{maxConcurrentHandlingCount}"" /></Tags>'
END");

                    return result;
                }
            }
        }
    }
}
