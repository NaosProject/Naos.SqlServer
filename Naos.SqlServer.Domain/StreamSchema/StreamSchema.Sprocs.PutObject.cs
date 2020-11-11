// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamSchema.Sprocs.PutObject.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

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
            /// Stored procedure: PutObject.
            /// </summary>
            public static class PutObject
            {
                /// <summary>
                /// Input parameter names.
                /// </summary>
                public enum InputParamName
                {
                    /// <summary>
                    /// The object assembly qualified name without version
                    /// </summary>
                    ObjectAssemblyQualifiedNameWithoutVersion,

                    /// <summary>
                    /// The object assembly qualified name with version
                    /// </summary>
                    ObjectAssemblyQualifiedNameWithVersion,

                    /// <summary>
                    /// The serializer description identifier.
                    /// </summary>
                    SerializerRepresentationId,

                    /// <summary>
                    /// The serialized object identifier.
                    /// </summary>
                    SerializedObjectId,

                    /// <summary>
                    /// The serialized object string.
                    /// </summary>
                    SerializedObjectString,

                    /// <summary>
                    /// The serialized object bytes.
                    /// </summary>
                    SerializedObjectBinary,

                    /// <summary>
                    /// The tags xml as a string.
                    /// </summary>
                    Tags,
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
                /// <param name="objectAssemblyQualifiedNameWithoutVersion">The object assembly qualified name without version.</param>
                /// <param name="objectAssemblyQualifiedNameWithVersion">The object assembly qualified name with version.</param>
                /// <param name="serializerDescriptionId">The serializer description identifier.</param>
                /// <param name="serializedObjectId">The serialized object identifier.</param>
                /// <param name="serializedObjectString">The serialized object as a string (should have data IFF the serializer is set to SerializationFormat.String, otherwise null).</param>
                /// <param name="serializedObjectBinary">The serialized object as bytes (should have data IFF the serializer is set to SerializationFormat.Binary, otherwise null).</param>
                /// <param name="tagsXml">The tags in xml structure.</param>
                /// <returns>ExecuteStoredProcedureOp.</returns>
                public static ExecuteStoredProcedureOp BuildExecuteStoredProcedureOp(
                    string streamName,
                    string objectAssemblyQualifiedNameWithoutVersion,
                    string objectAssemblyQualifiedNameWithVersion,
                    int serializerDescriptionId,
                    string serializedObjectId,
                    string serializedObjectString,
                    byte[] serializedObjectBinary,
                    string tagsXml)
                {
                    var sprocName = FormattableString.Invariant($"[{streamName}].{nameof(PutObject)}");

                    var parameters = new List<SqlParameterRepresentationBase>()
                                     {
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamName.ObjectAssemblyQualifiedNameWithoutVersion), Tables.TypeWithoutVersion.AssemblyQualifiedName.DataType, objectAssemblyQualifiedNameWithoutVersion),
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamName.ObjectAssemblyQualifiedNameWithVersion), Tables.TypeWithVersion.AssemblyQualifiedName.DataType, objectAssemblyQualifiedNameWithVersion),
                                         new SqlInputParameterRepresentation<int>(nameof(InputParamName.SerializerRepresentationId), Tables.SerializerRepresentation.Id.DataType, serializerDescriptionId),
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamName.SerializedObjectId), Tables.Object.SerializedObjectId.DataType, serializedObjectId),
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamName.SerializedObjectString), Tables.Object.SerializedObjectString.DataType, serializedObjectString),
                                         new SqlInputParameterRepresentation<byte[]>(nameof(InputParamName.SerializedObjectBinary), Tables.Object.SerializedObjectBinary.DataType, serializedObjectBinary),
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamName.Tags), new StringSqlDataTypeRepresentation(true, -1), tagsXml),
                                         new SqlOutputParameterRepresentation<long>(nameof(OutputParamName.Id), Tables.Object.Id.DataType),
                                     };

                    var parameterNameToDetailsMap = parameters.ToDictionary(k => k.Name, v => v);

                    var result = new ExecuteStoredProcedureOp(sprocName, parameterNameToDetailsMap);

                    return result;
                }

                /// <summary>
                /// Builds the creation script for put stored procedure.
                /// </summary>
                /// <param name="streamName">Name of the stream.</param>
                /// <returns>System.String.</returns>
                public static string BuildCreationScript(string streamName)
                {
                    var result = FormattableString.Invariant(
                        $@"
CREATE PROCEDURE [{streamName}].PutObject(
  @{nameof(InputParamName.ObjectAssemblyQualifiedNameWithoutVersion)} AS {Tables.TypeWithoutVersion.AssemblyQualifiedName.DataType.DeclarationInSqlSyntax}
, @{nameof(InputParamName.ObjectAssemblyQualifiedNameWithVersion)} AS {Tables.TypeWithVersion.AssemblyQualifiedName.DataType.DeclarationInSqlSyntax}
, @{nameof(InputParamName.SerializerRepresentationId)} AS {Tables.Object.SerializerRepresentationId.DataType.DeclarationInSqlSyntax}
, @{nameof(InputParamName.SerializedObjectId)} AS {Tables.Object.SerializedObjectId.DataType.DeclarationInSqlSyntax}
, @{nameof(InputParamName.SerializedObjectString)} AS {Tables.Object.SerializedObjectString.DataType.DeclarationInSqlSyntax}
, @{nameof(InputParamName.SerializedObjectBinary)} AS {Tables.Object.SerializedObjectBinary.DataType.DeclarationInSqlSyntax}
, @{nameof(InputParamName.Tags)} AS xml
, @{nameof(OutputParamName.Id)} AS {Tables.Object.Id.DataType.DeclarationInSqlSyntax} OUTPUT
)
AS
BEGIN

BEGIN TRANSACTION [PutObject]
  BEGIN TRY
      DECLARE @TypeWithoutVersionId {Tables.TypeWithoutVersion.Id.DataType.DeclarationInSqlSyntax}
      EXEC [{streamName}].[{nameof(GetIdAddIfNecessaryTypeWithoutVersion)}] @{nameof(InputParamName.ObjectAssemblyQualifiedNameWithoutVersion)}, @TypeWithoutVersionId OUTPUT
      DECLARE @TypeWithVersionId {Tables.TypeWithVersion.Id.DataType.DeclarationInSqlSyntax}
      EXEC [{streamName}].[{nameof(GetIdAddIfNecessaryTypeWithVersion)}] @{nameof(InputParamName.ObjectAssemblyQualifiedNameWithVersion)}, @TypeWithVersionId OUTPUT

	  DECLARE @RecordCreatedUtc datetime2
	  SET @RecordCreatedUtc = GETUTCDATE()
	  INSERT INTO [{streamName}].[Object] (
		  [{nameof(Tables.Object.ObjectTypeWithoutVersionId)}]
		, [{nameof(Tables.Object.ObjectTypeWithVersionId)}]
		, [{nameof(Tables.Object.SerializerRepresentationId)}]
		, [{nameof(Tables.Object.SerializedObjectId)}]
		, [{nameof(Tables.Object.SerializedObjectString)}]
		, [{nameof(Tables.Object.SerializedObjectBinary)}]
		, [{nameof(Tables.Object.RecordCreatedUtc)}]
		) VALUES (
		  @TypeWithoutVersionId
		, @TypeWithVersionId
		, @{nameof(InputParamName.SerializerRepresentationId)}
		, @{nameof(InputParamName.SerializedObjectId)}
		, @{nameof(InputParamName.SerializedObjectString)}
		, @{nameof(InputParamName.SerializedObjectBinary)}
		, @RecordCreatedUtc
		)

      SET @Id = SCOPE_IDENTITY()
	  
	  IF (@{nameof(InputParamName.Tags)} IS NOT NULL)
	  BEGIN
	      INSERT INTO [{streamName}].Tag
		  SELECT
  		    @Id
          , @TypeWithoutVersionId
		  , C.value('(Tag/@Key)[1]', 'nvarchar(450)') as [TagKey]
		  , C.value('(Tag/@Value)[1]', 'nvarchar(4000)') as [TagValue]
		  , @RecordCreatedUtc as RecordCreatedUtc
		  FROM
			@{nameof(InputParamName.Tags)}.nodes('/Tags') AS T(C)
	  END

      COMMIT TRANSACTION [PutObject]

  END TRY
  BEGIN CATCH
      DECLARE @ErrorMessage nvarchar(max), 
              @ErrorSeverity int, 
              @ErrorState int

      SELECT @ErrorMessage = ERROR_MESSAGE() + ' Line ' + cast(ERROR_LINE() as nvarchar(5)), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE()

      IF (@@trancount > 0)
      BEGIN
         ROLLBACK TRANSACTION [PutObject]
      END
    RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
  END CATCH
END
			");

                    return result;
                }
            }
        }
    }
}
