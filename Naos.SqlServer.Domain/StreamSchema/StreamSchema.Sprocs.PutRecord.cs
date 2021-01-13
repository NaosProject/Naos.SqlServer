// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamSchema.Sprocs.PutRecord.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Naos.Database.Domain;

    /// <summary>
    /// Container for schema.
    /// </summary>
    public static partial class StreamSchema
    {
        /// <summary>
        /// Stored procedures.
        /// </summary>
        public static partial class Sprocs
        {
            /// <summary>
            /// Stored procedure: PutRecord.
            /// </summary>
            public static class PutRecord
            {
                /// <summary>
                /// Gets the name.
                /// </summary>
                /// <value>The name.</value>
                public static string Name => nameof(PutRecord);

                /// <summary>
                /// Input parameter names.
                /// </summary>
                public enum InputParamName
                {
                    /// <summary>
                    /// The identifier assembly qualified name without version
                    /// </summary>
                    IdentifierTypeWithoutVersionId,

                    /// <summary>
                    /// The identifier assembly qualified name with version
                    /// </summary>
                    IdentifierTypeWithVersionId,

                    /// <summary>
                    /// The object assembly qualified name without version
                    /// </summary>
                    ObjectTypeWithoutVersionId,

                    /// <summary>
                    /// The object assembly qualified name with version
                    /// </summary>
                    ObjectTypeWithVersionId,

                    /// <summary>
                    /// The serializer description identifier.
                    /// </summary>
                    SerializerRepresentationId,

                    /// <summary>
                    /// The serialized object identifier.
                    /// </summary>
                    StringSerializedId,

                    /// <summary>
                    /// The serialized object string.
                    /// </summary>
                    StringSerializedObject,

                    /// <summary>
                    /// The object's date time UTC if available.
                    /// </summary>
                    ObjectDateTimeUtc,

                    /// <summary>
                    /// The tags xml as a string.
                    /// </summary>
                    TagIdsXml,

                    /// <summary>
                    /// The existing record encountered strategy.
                    /// </summary>
                    ExistingRecordEncounteredStrategy,
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
                /// <param name="serializerRepresentation">The serializer representation.</param>
                /// <param name="identifierType">The identifier type.</param>
                /// <param name="objectType">The object type.</param>
                /// <param name="serializedObjectId">The serialized object identifier.</param>
                /// <param name="serializedObjectString">The serialized object as a string (should have data IFF the serializer is set to SerializationFormat.String, otherwise null).</param>
                /// <param name="objectDateTimeUtc">The date time of the object if exists.</param>
                /// <param name="tagIdsXml">The tags in xml structure.</param>
                /// <param name="existingRecordEncounteredStrategy">Existing record encountered strategy.</param>
                /// <returns>ExecuteStoredProcedureOp.</returns>
                public static ExecuteStoredProcedureOp BuildExecuteStoredProcedureOp(
                    string streamName,
                    IdentifiedSerializerRepresentation serializerRepresentation,
                    IdentifiedType identifierType,
                    IdentifiedType objectType,
                    string serializedObjectId,
                    string serializedObjectString,
                    DateTime? objectDateTimeUtc,
                    string tagIdsXml,
                    ExistingRecordEncounteredStrategy existingRecordEncounteredStrategy)
                {
                    var sprocName = FormattableString.Invariant($"[{streamName}].{nameof(PutRecord)}");

                    if (tagIdsXml == null)
                    {
                        var tagIds = new Dictionary<string, string>();
                        tagIdsXml = TagConversionTool.GetTagsXmlString(tagIds);
                    }

                    var parameters = new List<SqlParameterRepresentationBase>()
                                     {
                                         new SqlInputParameterRepresentation<int>(nameof(InputParamName.SerializerRepresentationId), Tables.SerializerRepresentation.Id.DataType, serializerRepresentation.Id),
                                         new SqlInputParameterRepresentation<int?>(nameof(InputParamName.IdentifierTypeWithoutVersionId), Tables.TypeWithoutVersion.Id.DataType, identifierType?.IdWithoutVersion),
                                         new SqlInputParameterRepresentation<int?>(nameof(InputParamName.IdentifierTypeWithVersionId), Tables.TypeWithVersion.Id.DataType, identifierType?.IdWithVersion),
                                         new SqlInputParameterRepresentation<int?>(nameof(InputParamName.ObjectTypeWithoutVersionId), Tables.TypeWithoutVersion.Id.DataType, objectType?.IdWithoutVersion),
                                         new SqlInputParameterRepresentation<int?>(nameof(InputParamName.ObjectTypeWithVersionId), Tables.TypeWithVersion.Id.DataType, objectType?.IdWithVersion),
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamName.StringSerializedId), Tables.Record.StringSerializedId.DataType, serializedObjectId),
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamName.StringSerializedObject), Tables.Record.StringSerializedObject.DataType, serializedObjectString),
                                         new SqlInputParameterRepresentation<DateTime?>(nameof(InputParamName.ObjectDateTimeUtc), Tables.Record.ObjectDateTimeUtc.DataType, objectDateTimeUtc),
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamName.TagIdsXml), new StringSqlDataTypeRepresentation(true, -1), tagIdsXml),
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamName.ExistingRecordEncounteredStrategy), new StringSqlDataTypeRepresentation(false, 50), existingRecordEncounteredStrategy.ToString()),
                                         new SqlOutputParameterRepresentation<long>(nameof(OutputParamName.Id), Tables.Record.Id.DataType),
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
                    const string recordCreatedUtc = "RecordCreatedUtc";
                    var result = FormattableString.Invariant(
                        $@"
CREATE PROCEDURE [{streamName}].{nameof(PutRecord)}(
  @{InputParamName.SerializerRepresentationId} AS {Tables.SerializerRepresentation.Id.DataType.DeclarationInSqlSyntax}
, @{InputParamName.IdentifierTypeWithoutVersionId} AS {Tables.TypeWithoutVersion.Id.DataType.DeclarationInSqlSyntax}
, @{InputParamName.IdentifierTypeWithVersionId} AS {Tables.TypeWithVersion.Id.DataType.DeclarationInSqlSyntax}
, @{InputParamName.ObjectTypeWithoutVersionId} AS {Tables.TypeWithoutVersion.Id.DataType.DeclarationInSqlSyntax}
, @{InputParamName.ObjectTypeWithVersionId} AS {Tables.TypeWithVersion.Id.DataType.DeclarationInSqlSyntax}
, @{InputParamName.StringSerializedId} AS {Tables.Record.StringSerializedId.DataType.DeclarationInSqlSyntax}
, @{InputParamName.StringSerializedObject} AS {Tables.Record.StringSerializedObject.DataType.DeclarationInSqlSyntax}
, @{InputParamName.ObjectDateTimeUtc} AS {Tables.Record.ObjectDateTimeUtc.DataType.DeclarationInSqlSyntax}
, @{InputParamName.TagIdsXml} AS xml
, @{InputParamName.ExistingRecordEncounteredStrategy} AS {new StringSqlDataTypeRepresentation(false, 50).DeclarationInSqlSyntax}
, @{OutputParamName.Id} AS {Tables.Record.Id.DataType.DeclarationInSqlSyntax} OUTPUT
)
AS
BEGIN

BEGIN TRANSACTION [{nameof(PutRecord)}]
  BEGIN TRY
--TODO:Support {InputParamName.ExistingRecordEncounteredStrategy}// SEE if the strategy needs honoring...do we table lock here???
	  DECLARE @{recordCreatedUtc} {Tables.Record.RecordCreatedUtc.DataType.DeclarationInSqlSyntax}
	  SET @RecordCreatedUtc = GETUTCDATE()
	  INSERT INTO [{streamName}].[{Tables.Record.Table.Name}] (
		  [{nameof(Tables.Record.IdentifierTypeWithoutVersionId)}]
		, [{nameof(Tables.Record.IdentifierTypeWithVersionId)}]
		, [{nameof(Tables.Record.ObjectTypeWithoutVersionId)}]
		, [{nameof(Tables.Record.ObjectTypeWithVersionId)}]
		, [{nameof(Tables.Record.SerializerRepresentationId)}]
		, [{nameof(Tables.Record.StringSerializedId)}]
		, [{nameof(Tables.Record.StringSerializedObject)}]
		, [{nameof(Tables.Record.ObjectDateTimeUtc)}]
		, [{nameof(Tables.Record.RecordCreatedUtc)}]
		) VALUES (
		  @{InputParamName.IdentifierTypeWithoutVersionId}
		, @{InputParamName.IdentifierTypeWithVersionId}
		, @{InputParamName.ObjectTypeWithoutVersionId}
		, @{InputParamName.ObjectTypeWithVersionId}
		, @{InputParamName.SerializerRepresentationId}
		, @{InputParamName.StringSerializedId}
		, @{InputParamName.StringSerializedObject}
		, @{InputParamName.ObjectDateTimeUtc}
		, @{recordCreatedUtc}
		)

      SET @{OutputParamName.Id} = SCOPE_IDENTITY()
	  
      INSERT INTO [{streamName}].[{Tables.RecordTag.Table.Name}](
	    [{Tables.RecordTag.RecordId.Name}]
	  , [{Tables.RecordTag.TagId.Name}]
	  , [{Tables.RecordTag.RecordCreatedUtc.Name}]
	  )
     SELECT 
  	    @{OutputParamName.Id}
	  , t.[{Tables.Tag.TagValue.Name}]
	  , @{recordCreatedUtc}
     FROM [{streamName}].[{Funcs.GetTagsTableVariableFromTagsXml.Name}](@{InputParamName.TagIdsXml}) t

      COMMIT TRANSACTION [{nameof(PutRecord)}]

  END TRY
  BEGIN CATCH
      DECLARE @ErrorMessage nvarchar(max), 
              @ErrorSeverity int, 
              @ErrorState int

      SELECT @ErrorMessage = ERROR_MESSAGE() + ' Line ' + cast(ERROR_LINE() as nvarchar(5)), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE()

      IF (@@trancount > 0)
      BEGIN
         ROLLBACK TRANSACTION [{nameof(PutRecord)}]
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
