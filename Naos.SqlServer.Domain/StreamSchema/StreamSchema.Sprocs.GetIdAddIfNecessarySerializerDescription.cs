// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamSchema.Sprocs.GetIdAddIfNecessarySerializerDescription.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Naos.CodeAnalysis.Recipes;
    using OBeautifulCode.Compression;
    using OBeautifulCode.Serialization;

    using SerializationFormat = OBeautifulCode.Serialization.SerializationFormat;

    /// <summary>
    /// Stream schema.
    /// </summary>
    public static partial class StreamSchema
    {
        /// <summary>
        /// Stored procedures.
        /// </summary>
        public static partial class Sprocs
        {
            /// <summary>
            /// Stored procedure: GetIdAddIfNecessarySerializerRepresentation.
            /// </summary>
            public static class GetIdAddIfNecessarySerializerRepresentation
            {
                /// <summary>
                /// Input parameter names.
                /// </summary>
                public enum InputParamNames
                {
                    /// <summary>
                    /// The serialization configuration assembly qualified name without version
                    /// </summary>
                    ConfigAssemblyQualifiedNameWithoutVersion,

                    /// <summary>
                    /// The serialization configuration assembly qualified name with version
                    /// </summary>
                    ConfigAssemblyQualifiedNameWithVersion,

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
                public enum OutputParamNames
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
                /// <param name="configAssemblyQualifiedNameWithoutVersion">The serialization configuration assembly qualified name without version.</param>
                /// <param name="configAssemblyQualifiedNameWithVersion">The serialization configuration assembly qualified name with version.</param>
                /// <param name="serializationKind">The <see cref="SerializationKind"/>.</param>
                /// <param name="serializationFormat">The <see cref="SerializationFormat"/>.</param>
                /// <param name="compressionKind">The <see cref="CompressionKind"/>.</param>
                /// <param name="unregisteredTypeEncounteredStrategy">The <see cref="UnregisteredTypeEncounteredStrategy"/>.</param>
                /// <returns>ExecuteStoredProcedureOp.</returns>
                public static ExecuteStoredProcedureOp BuildExecuteStoredProcedureOp(
                    string streamName,
                    string configAssemblyQualifiedNameWithoutVersion,
                    string configAssemblyQualifiedNameWithVersion,
                    SerializationKind serializationKind,
                    SerializationFormat serializationFormat,
                    CompressionKind compressionKind,
                    UnregisteredTypeEncounteredStrategy unregisteredTypeEncounteredStrategy)
                {
                    var sprocName = FormattableString.Invariant($"[{streamName}].{nameof(GetIdAddIfNecessarySerializerRepresentation)}");

                    var parameters = new List<SqlParameterRepresentationBase>()
                                     {
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamNames.ConfigAssemblyQualifiedNameWithoutVersion), Tables.TypeWithoutVersion.AssemblyQualifiedName.DataType, configAssemblyQualifiedNameWithoutVersion),
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamNames.ConfigAssemblyQualifiedNameWithVersion), Tables.TypeWithVersion.AssemblyQualifiedName.DataType, configAssemblyQualifiedNameWithVersion),
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamNames.SerializationKind), Tables.SerializerRepresentation.SerializationKind.DataType, serializationKind.ToString()),
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamNames.SerializationFormat), Tables.SerializerRepresentation.SerializationFormat.DataType, serializationFormat.ToString()),
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamNames.CompressionKind), Tables.SerializerRepresentation.CompressionKind.DataType, compressionKind.ToString()),
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamNames.UnregisteredTypeEncounteredStrategy), Tables.SerializerRepresentation.SerializationKind.DataType, unregisteredTypeEncounteredStrategy.ToString()),
                                         new SqlOutputParameterRepresentation<int>(nameof(OutputParamNames.Id), Tables.SerializerRepresentation.Id.DataType),
                                     };

                    var parameterNameToRepresentationMap = parameters.ToDictionary(k => k.Name, v => v);

                    var result = new ExecuteStoredProcedureOp(sprocName, parameterNameToRepresentationMap);

                    return result;
                }

                /// <summary>
                /// Builds the name of the put stored procedure.
                /// </summary>
                /// <param name="streamName">Name of the stream.</param>
                /// <returns>Name of the put stored procedure.</returns>
                [System.Diagnostics.CodeAnalysis.SuppressMessage(
                    "Microsoft.Naming",
                    "CA1704:IdentifiersShouldBeSpelledCorrectly",
                    MessageId = "Sproc",
                    Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
                public static string BuildCreationScript(
                    string streamName)
                {
                    return FormattableString.Invariant(
                        $@"
CREATE PROCEDURE [{streamName}].GetIdAddIfNecessarySerializerRepresentation(
  @{InputParamNames.ConfigAssemblyQualifiedNameWithoutVersion} AS {Tables.TypeWithoutVersion.AssemblyQualifiedName.DataType.DeclarationInSqlSyntax}
, @{InputParamNames.ConfigAssemblyQualifiedNameWithVersion} AS {Tables.TypeWithVersion.AssemblyQualifiedName.DataType.DeclarationInSqlSyntax}
, @{InputParamNames.SerializationKind} {Tables.SerializerRepresentation.SerializationKind.DataType.DeclarationInSqlSyntax}
, @{InputParamNames.SerializationFormat} AS {Tables.SerializerRepresentation.SerializationFormat.DataType.DeclarationInSqlSyntax}
, @{InputParamNames.CompressionKind} AS {Tables.SerializerRepresentation.CompressionKind.DataType.DeclarationInSqlSyntax}
, @{InputParamNames.UnregisteredTypeEncounteredStrategy} AS {Tables.SerializerRepresentation.UnregisteredTypeEncounteredStrategy.DataType.DeclarationInSqlSyntax}
, @{OutputParamNames.Id} {Tables.SerializerRepresentation.Id.DataType.DeclarationInSqlSyntax} OUTPUT
)
AS
BEGIN

BEGIN TRANSACTION [GetIdAddSerializerRepresentation]
  BEGIN TRY
      DECLARE @TypeWithoutVersionId {Tables.TypeWithoutVersion.Id.DataType.DeclarationInSqlSyntax}
      EXEC [{streamName}].[GetIdAddIfNecessaryTypeWithoutVersion] @{InputParamNames.ConfigAssemblyQualifiedNameWithoutVersion}, @TypeWithoutVersionId OUTPUT
      DECLARE @TypeWithVersionId {Tables.TypeWithVersion.Id.DataType.DeclarationInSqlSyntax}
      EXEC [{streamName}].[GetIdAddIfNecessaryTypeWithVersion] @{InputParamNames.ConfigAssemblyQualifiedNameWithVersion}, @TypeWithVersionId OUTPUT
      
      SELECT @{nameof(OutputParamNames.Id)} = [{nameof(Tables.SerializerRepresentation.Id)}] FROM [{streamName}].[{nameof(Tables.SerializerRepresentation)}]
        WHERE [{nameof(Tables.SerializerRepresentation.SerializationConfigurationTypeWithVersionId)}] = @TypeWithVersionId
        AND [{nameof(Tables.SerializerRepresentation.SerializationConfigurationTypeWithoutVersionId)}] = @TypeWithoutVersionId
        AND [{nameof(Tables.SerializerRepresentation.SerializationKind)}] = @{InputParamNames.SerializationKind}
        AND [{nameof(Tables.SerializerRepresentation.SerializationFormat)}] = @{InputParamNames.SerializationFormat}
        AND [{nameof(Tables.SerializerRepresentation.CompressionKind)}] = @{InputParamNames.CompressionKind}
        AND [{nameof(Tables.SerializerRepresentation.UnregisteredTypeEncounteredStrategy)}] = @{InputParamNames.UnregisteredTypeEncounteredStrategy}

	  IF (@{nameof(OutputParamNames.Id)} IS NULL)
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
	        @{InputParamNames.SerializationKind}
		  , @{InputParamNames.SerializationFormat}
		  , @TypeWithoutVersionId
		  , @TypeWithVersionId
		  , @{InputParamNames.CompressionKind}
		  , @{InputParamNames.UnregisteredTypeEncounteredStrategy}
		  , GETUTCDATE()
		  )

	      SET @{nameof(OutputParamNames.Id)} = SCOPE_IDENTITY()
	  END

      COMMIT TRANSACTION [GetIdAddSerializerRepresentation]

  END TRY

  BEGIN CATCH
      SET @{nameof(OutputParamNames.Id)} = NULL
      DECLARE @ErrorMessage nvarchar(max), 
              @ErrorSeverity int, 
              @ErrorState int

      SELECT @ErrorMessage = ERROR_MESSAGE() + ' Line ' + cast(ERROR_LINE() as nvarchar(5)), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE()

      IF (@@trancount > 0)
      BEGIN
         ROLLBACK TRANSACTION [GetIdAddSerializerRepresentation]
      END
    RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
  END CATCH
END");
                }
            }
        }
    }
}
