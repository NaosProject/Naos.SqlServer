// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamSchema.Sprocs.GetIdAddIfNecessaryTypeWithoutVersion.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System.Collections.Generic;
    using static System.FormattableString;

    public static partial class StreamSchema
    {
        public static partial class Sprocs
        {
            /// <summary>
            /// Stored procedure: GetIdAddIfNecessaryTypeWithoutVersion.
            /// </summary>
            public static class GetIdAddIfNecessaryTypeWithoutVersion
            {
                /// <summary>
                /// Gets the name of the stored procedure.
                /// </summary>
                public static string Name => nameof(GetIdAddIfNecessaryTypeWithoutVersion);

                /// <summary>
                /// Input parameter names.
                /// </summary>
                public enum InputParamName
                {
                    /// <summary>
                    /// The object assembly qualified name without version.
                    /// </summary>
                    AssemblyQualifiedNameWithoutVersion,
                }

                /// <summary>
                /// Output parameter names.
                /// </summary>
                public enum OutputParamName
                {
                    /// <summary>
                    /// The identifier.
                    /// </summary>
                    Id,
                }

                /// <summary>
                /// Builds the execute stored procedure operation.
                /// </summary>
                /// <param name="streamName">Name of the stream.</param>
                /// <param name="assemblyQualifiedNameWithoutVersion">The assembly qualified name.</param>
                /// <returns>Operation to execute stored procedure.</returns>
                public static ExecuteStoredProcedureOp BuildExecuteStoredProcedureOp(
                    string streamName,
                    string assemblyQualifiedNameWithoutVersion)
                {
                    var sprocName = Invariant($"[{streamName}].[{nameof(GetIdAddIfNecessaryTypeWithoutVersion)}]");

                    var parameters =
                        new List<ParameterDefinitionBase>
                        {
                            new InputParameterDefinition<string>(nameof(InputParamName.AssemblyQualifiedNameWithoutVersion), Tables.TypeWithoutVersion.AssemblyQualifiedName.SqlDataType, assemblyQualifiedNameWithoutVersion),
                            new OutputParameterDefinition<int>(nameof(OutputParamName.Id), Tables.TypeWithoutVersion.Id.SqlDataType),
                        };

                    var result = new ExecuteStoredProcedureOp(sprocName, parameters);

                    return result;
                }

                /// <summary>
                /// Builds the name of the put stored procedure.
                /// </summary>
                /// <param name="streamName">Name of the stream.</param>
                /// <param name="asAlter">An optional value indicating whether or not to generate a ALTER versus CREATE; DEFAULT is false and will generate a CREATE script.</param>
                /// <returns>Creation script for creating the stored procedure.</returns>
                public static string BuildCreationScript(
                    string streamName,
                    bool asAlter = false)
                {
                    const string transaction = "GetIdAddIfNecTypeWithoutVerTrans";
                    var createOrModify = asAlter ? "CREATE OR ALTER" : "CREATE";
                    var result = Invariant(
                        $@"
{createOrModify} PROCEDURE [{streamName}].[{Name}](
  @{nameof(InputParamName.AssemblyQualifiedNameWithoutVersion)} {Tables.TypeWithoutVersion.AssemblyQualifiedName.SqlDataType.DeclarationInSqlSyntax},
  @{nameof(OutputParamName.Id)} {Tables.TypeWithoutVersion.Id.SqlDataType.DeclarationInSqlSyntax} OUTPUT
  )
AS
BEGIN

    SELECT
        @{nameof(OutputParamName.Id)} = [{nameof(Tables.TypeWithoutVersion.Id)}]
    FROM [{streamName}].[{nameof(Tables.TypeWithoutVersion)}] WITH (NOLOCK)
        WHERE [{nameof(Tables.TypeWithoutVersion.AssemblyQualifiedName)}] = @{nameof(InputParamName.AssemblyQualifiedNameWithoutVersion)}

    IF (@{nameof(OutputParamName.Id)} IS NULL)
    BEGIN
        SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
        BEGIN TRANSACTION [{transaction}]
        BEGIN TRY
            SELECT
                @{nameof(OutputParamName.Id)} = [{nameof(Tables.TypeWithoutVersion.Id)}]
            FROM [{streamName}].[{nameof(Tables.TypeWithoutVersion)}]
                WHERE [{nameof(Tables.TypeWithoutVersion.AssemblyQualifiedName)}] = @{nameof(InputParamName.AssemblyQualifiedNameWithoutVersion)}

            IF (@{nameof(OutputParamName.Id)} IS NULL)
            BEGIN
                INSERT INTO [{streamName}].[{nameof(Tables.TypeWithoutVersion)}]
                (
                     [{nameof(Tables.TypeWithoutVersion.AssemblyQualifiedName)}]
                   , [{nameof(Tables.TypeWithoutVersion.RecordCreatedUtc)}]
                )
                VALUES
                (
                      @{nameof(InputParamName.AssemblyQualifiedNameWithoutVersion)}
                    , GETUTCDATE()
                )

                SET @{nameof(OutputParamName.Id)} = SCOPE_IDENTITY()
            END

            COMMIT TRANSACTION [{transaction}]
        END TRY
        BEGIN CATCH
            SET @{nameof(OutputParamName.Id)} = NULL
            DECLARE @ErrorMessage nvarchar(max), 
                  @ErrorSeverity int, 
                  @ErrorState int

            SELECT @ErrorMessage = ERROR_MESSAGE() + ' Line ' + cast(ERROR_LINE() as nvarchar(5)), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE()

            IF (@@trancount > 0)
            BEGIN
                ROLLBACK TRANSACTION [{transaction}]
            END

            RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
        END CATCH
    END
END
");
                    return result;
                }
            }
        }
    }
}