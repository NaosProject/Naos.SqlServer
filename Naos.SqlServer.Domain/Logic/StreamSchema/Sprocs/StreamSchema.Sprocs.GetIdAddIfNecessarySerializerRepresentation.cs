// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamSchema.Sprocs.GetIdAddIfNecessarySerializerRepresentation.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System.Collections.Generic;
    using System.Linq;
    using OBeautifulCode.Compression;
    using OBeautifulCode.Serialization;
    using static System.FormattableString;
    using SerializationFormat = OBeautifulCode.Serialization.SerializationFormat;

    public static partial class StreamSchema
    {
        public static partial class Sprocs
        {
            /// <summary>
            /// Stored procedure: GetIdAddIfNecessarySerializerRepresentation.
            /// </summary>
            public static class GetIdAddIfNecessarySerializerRepresentation
            {
                /// <summary>
                /// Gets the name of the stored procedure.
                /// </summary>
                public static string Name => nameof(GetIdAddIfNecessarySerializerRepresentation);

                /// <summary>
                /// Input parameter names.
                /// </summary>
                public enum InputParamName
                {
                    /// <summary>
                    /// The serialization configuration type without version identifier.
                    /// </summary>
                    ConfigTypeWithoutVersionId,

                    /// <summary>
                    /// The serialization configuration type with version identifier.
                    /// </summary>
                    ConfigTypeWithVersionId,

                    /// <summary>
                    /// The serialization kind.
                    /// </summary>
                    SerializationKind,

                    /// <summary>
                    /// The serialization format.
                    /// </summary>
                    SerializationFormat,

                    /// <summary>
                    /// The compression kind.
                    /// </summary>
                    CompressionKind,

                    /// <summary>
                    /// The unregistered type encountered strategy.
                    /// </summary>
                    UnregisteredTypeEncounteredStrategy,
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
                /// <param name="configType">The serialization configuration type identifiers.</param>
                /// <param name="serializationKind">The <see cref="SerializationKind"/>.</param>
                /// <param name="serializationFormat">The <see cref="SerializationFormat"/>.</param>
                /// <param name="compressionKind">The <see cref="CompressionKind"/>.</param>
                /// <param name="unregisteredTypeEncounteredStrategy">The <see cref="UnregisteredTypeEncounteredStrategy"/>.</param>
                /// <returns>Operation to execute stored procedure.</returns>
                public static ExecuteStoredProcedureOp BuildExecuteStoredProcedureOp(
                    string streamName,
                    IdentifiedType configType,
                    SerializationKind serializationKind,
                    SerializationFormat serializationFormat,
                    CompressionKind compressionKind,
                    UnregisteredTypeEncounteredStrategy unregisteredTypeEncounteredStrategy)
                {
                    var sprocName = Invariant($"[{streamName}].[{nameof(GetIdAddIfNecessarySerializerRepresentation)}]");

                    var parameters = new List<SqlParameterRepresentationBase>()
                                     {
                                         new SqlInputParameterRepresentation<int>(nameof(InputParamName.ConfigTypeWithoutVersionId), Tables.TypeWithoutVersion.Id.SqlDataType, configType.IdWithoutVersion),
                                         new SqlInputParameterRepresentation<int>(nameof(InputParamName.ConfigTypeWithVersionId), Tables.TypeWithVersion.Id.SqlDataType, configType.IdWithVersion),
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamName.SerializationKind), Tables.SerializerRepresentation.SerializationKind.SqlDataType, serializationKind.ToString()),
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamName.SerializationFormat), Tables.SerializerRepresentation.SerializationFormat.SqlDataType, serializationFormat.ToString()),
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamName.CompressionKind), Tables.SerializerRepresentation.CompressionKind.SqlDataType, compressionKind.ToString()),
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamName.UnregisteredTypeEncounteredStrategy), Tables.SerializerRepresentation.SerializationKind.SqlDataType, unregisteredTypeEncounteredStrategy.ToString()),
                                         new SqlOutputParameterRepresentation<int>(nameof(OutputParamName.Id), Tables.SerializerRepresentation.Id.SqlDataType),
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
                    const string transaction = "GetIdAddIfSerializerRepTran";
                    var createOrModify = asAlter ? "ALTER" : "CREATE";
                    var result = Invariant(
                        $@"
{createOrModify} PROCEDURE [{streamName}].[{GetIdAddIfNecessarySerializerRepresentation.Name}](
  @{InputParamName.ConfigTypeWithoutVersionId} AS {Tables.TypeWithoutVersion.Id.SqlDataType.DeclarationInSqlSyntax}
, @{InputParamName.ConfigTypeWithVersionId} AS {Tables.TypeWithVersion.Id.SqlDataType.DeclarationInSqlSyntax}
, @{InputParamName.SerializationKind} {Tables.SerializerRepresentation.SerializationKind.SqlDataType.DeclarationInSqlSyntax}
, @{InputParamName.SerializationFormat} AS {Tables.SerializerRepresentation.SerializationFormat.SqlDataType.DeclarationInSqlSyntax}
, @{InputParamName.CompressionKind} AS {Tables.SerializerRepresentation.CompressionKind.SqlDataType.DeclarationInSqlSyntax}
, @{InputParamName.UnregisteredTypeEncounteredStrategy} AS {Tables.SerializerRepresentation.UnregisteredTypeEncounteredStrategy.SqlDataType.DeclarationInSqlSyntax}
, @{OutputParamName.Id} AS {Tables.SerializerRepresentation.Id.SqlDataType.DeclarationInSqlSyntax} OUTPUT
)
AS
BEGIN
    SELECT
        @{nameof(OutputParamName.Id)} = [{nameof(Tables.SerializerRepresentation.Id)}]
    FROM [{streamName}].[{nameof(Tables.SerializerRepresentation)}] WITH (NOLOCK)
        WHERE [{nameof(Tables.SerializerRepresentation.SerializationConfigurationTypeWithVersionId)}] = @{InputParamName.ConfigTypeWithVersionId}
          AND [{nameof(Tables.SerializerRepresentation.SerializationConfigurationTypeWithoutVersionId)}] = @{InputParamName.ConfigTypeWithoutVersionId}
          AND [{nameof(Tables.SerializerRepresentation.SerializationKind)}] = @{InputParamName.SerializationKind}
          AND [{nameof(Tables.SerializerRepresentation.SerializationFormat)}] = @{InputParamName.SerializationFormat}
          AND [{nameof(Tables.SerializerRepresentation.CompressionKind)}] = @{InputParamName.CompressionKind}
          AND [{nameof(Tables.SerializerRepresentation.UnregisteredTypeEncounteredStrategy)}] = @{InputParamName.UnregisteredTypeEncounteredStrategy}

    IF (@{nameof(OutputParamName.Id)} IS NULL)
    BEGIN
        SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
        BEGIN TRANSACTION [{transaction}]
        BEGIN TRY
            SELECT
                @{nameof(OutputParamName.Id)} = [{nameof(Tables.SerializerRepresentation.Id)}]
            FROM [{streamName}].[{nameof(Tables.SerializerRepresentation)}]
                WHERE [{nameof(Tables.SerializerRepresentation.SerializationConfigurationTypeWithVersionId)}] = @{InputParamName.ConfigTypeWithVersionId}
                  AND [{nameof(Tables.SerializerRepresentation.SerializationConfigurationTypeWithoutVersionId)}] = @{InputParamName.ConfigTypeWithoutVersionId}
                  AND [{nameof(Tables.SerializerRepresentation.SerializationKind)}] = @{InputParamName.SerializationKind}
                  AND [{nameof(Tables.SerializerRepresentation.SerializationFormat)}] = @{InputParamName.SerializationFormat}
                  AND [{nameof(Tables.SerializerRepresentation.CompressionKind)}] = @{InputParamName.CompressionKind}
                  AND [{nameof(Tables.SerializerRepresentation.UnregisteredTypeEncounteredStrategy)}] = @{InputParamName.UnregisteredTypeEncounteredStrategy}

              IF (@{nameof(OutputParamName.Id)} IS NULL)
              BEGIN
                  INSERT INTO [{streamName}].[{nameof(Tables.SerializerRepresentation)}] (
                    [{nameof(Tables.SerializerRepresentation.SerializationKind)}]
                  , [{nameof(Tables.SerializerRepresentation.SerializationFormat)}]
                  , [{nameof(Tables.SerializerRepresentation.SerializationConfigurationTypeWithoutVersionId)}]
                  , [{nameof(Tables.SerializerRepresentation.SerializationConfigurationTypeWithVersionId)}]
                  , [{nameof(Tables.SerializerRepresentation.CompressionKind)}]
                  , [{nameof(Tables.SerializerRepresentation.UnregisteredTypeEncounteredStrategy)}]
                  , [{nameof(Tables.SerializerRepresentation.RecordCreatedUtc)}]
                  ) VALUES (
                    @{InputParamName.SerializationKind}
                  , @{InputParamName.SerializationFormat}
                  , @{InputParamName.ConfigTypeWithoutVersionId}
                  , @{InputParamName.ConfigTypeWithVersionId}
                  , @{InputParamName.CompressionKind}
                  , @{InputParamName.UnregisteredTypeEncounteredStrategy}
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
