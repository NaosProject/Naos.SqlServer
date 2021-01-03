// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamSchema.Sprocs.PutRecord.cs" company="Naos Project">
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
                    IdentifierAssemblyQualifiedNameWithoutVersion,

                    /// <summary>
                    /// The identifier assembly qualified name with version
                    /// </summary>
                    IdentifierAssemblyQualifiedNameWithVersion,

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
                    TagsXml,
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
                /// <param name="identifierAssemblyQualifiedNameWithoutVersion">The identifier type assembly qualified name without version.</param>
                /// <param name="identifierAssemblyQualifiedNameWithVersion">The identifier type assembly qualified name with version.</param>
                /// <param name="objectAssemblyQualifiedNameWithoutVersion">The object type assembly qualified name without version.</param>
                /// <param name="objectAssemblyQualifiedNameWithVersion">The object type assembly qualified name with version.</param>
                /// <param name="serializerDescriptionId">The serializer description identifier.</param>
                /// <param name="serializedObjectId">The serialized object identifier.</param>
                /// <param name="serializedObjectString">The serialized object as a string (should have data IFF the serializer is set to SerializationFormat.String, otherwise null).</param>
                /// <param name="objectDateTimeUtc">The date time of the object if exists.</param>
                /// <param name="tagsXml">The tags in xml structure.</param>
                /// <returns>ExecuteStoredProcedureOp.</returns>
                public static ExecuteStoredProcedureOp BuildExecuteStoredProcedureOp(
                    string streamName,
                    string identifierAssemblyQualifiedNameWithoutVersion,
                    string identifierAssemblyQualifiedNameWithVersion,
                    string objectAssemblyQualifiedNameWithoutVersion,
                    string objectAssemblyQualifiedNameWithVersion,
                    int serializerDescriptionId,
                    string serializedObjectId,
                    string serializedObjectString,
                    DateTime? objectDateTimeUtc,
                    string tagsXml)
                {
                    var sprocName = FormattableString.Invariant($"[{streamName}].{nameof(PutRecord)}");

                    var parameters = new List<SqlParameterRepresentationBase>()
                                     {
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamName.IdentifierAssemblyQualifiedNameWithoutVersion), Tables.TypeWithoutVersion.AssemblyQualifiedName.DataType, identifierAssemblyQualifiedNameWithoutVersion),
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamName.IdentifierAssemblyQualifiedNameWithVersion), Tables.TypeWithVersion.AssemblyQualifiedName.DataType, identifierAssemblyQualifiedNameWithVersion),
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamName.ObjectAssemblyQualifiedNameWithoutVersion), Tables.TypeWithoutVersion.AssemblyQualifiedName.DataType, objectAssemblyQualifiedNameWithoutVersion),
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamName.ObjectAssemblyQualifiedNameWithVersion), Tables.TypeWithVersion.AssemblyQualifiedName.DataType, objectAssemblyQualifiedNameWithVersion),
                                         new SqlInputParameterRepresentation<int>(nameof(InputParamName.SerializerRepresentationId), Tables.SerializerRepresentation.Id.DataType, serializerDescriptionId),
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamName.StringSerializedId), Tables.Object.StringSerializedId.DataType, serializedObjectId),
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamName.StringSerializedObject), Tables.Object.StringSerializedObject.DataType, serializedObjectString),
                                         new SqlInputParameterRepresentation<DateTime?>(nameof(InputParamName.ObjectDateTimeUtc), Tables.Object.ObjectDateTimeUtc.DataType, objectDateTimeUtc),
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamName.TagsXml), new StringSqlDataTypeRepresentation(true, -1), tagsXml),
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
                    const string recordCreatedUtc = "RecordCreatedUtc";
                    const string identifierTypeWithoutVersionId = "IdentifierTypeWithoutVersionId";
                    const string identifierTypeWithVersionId = "IdentifierTypeWithVersionId";
                    const string objectTypeWithoutVersionId = "ObjectTypeWithoutVersionId";
                    const string objectTypeWithVersionId = "ObjectTypeWithVersionId";
                    var result = FormattableString.Invariant(
                        $@"
CREATE PROCEDURE [{streamName}].{nameof(PutRecord)}(
  @{InputParamName.IdentifierAssemblyQualifiedNameWithoutVersion} AS {Tables.TypeWithoutVersion.AssemblyQualifiedName.DataType.DeclarationInSqlSyntax}
, @{InputParamName.IdentifierAssemblyQualifiedNameWithVersion} AS {Tables.TypeWithVersion.AssemblyQualifiedName.DataType.DeclarationInSqlSyntax}
, @{InputParamName.ObjectAssemblyQualifiedNameWithoutVersion} AS {Tables.TypeWithoutVersion.AssemblyQualifiedName.DataType.DeclarationInSqlSyntax}
, @{InputParamName.ObjectAssemblyQualifiedNameWithVersion} AS {Tables.TypeWithVersion.AssemblyQualifiedName.DataType.DeclarationInSqlSyntax}
, @{InputParamName.SerializerRepresentationId} AS {Tables.Object.SerializerRepresentationId.DataType.DeclarationInSqlSyntax}
, @{InputParamName.StringSerializedId} AS {Tables.Object.StringSerializedId.DataType.DeclarationInSqlSyntax}
, @{InputParamName.StringSerializedObject} AS {Tables.Object.StringSerializedObject.DataType.DeclarationInSqlSyntax}
, @{InputParamName.ObjectDateTimeUtc} AS {Tables.Object.ObjectDateTimeUtc.DataType.DeclarationInSqlSyntax}
, @{InputParamName.TagsXml} AS xml
, @{OutputParamName.Id} AS {Tables.Object.Id.DataType.DeclarationInSqlSyntax} OUTPUT
)
AS
BEGIN

BEGIN TRANSACTION [{nameof(PutRecord)}]
  BEGIN TRY
      DECLARE @{identifierTypeWithoutVersionId} {Tables.TypeWithoutVersion.Id.DataType.DeclarationInSqlSyntax}
      EXEC [{streamName}].[{GetIdAddIfNecessaryTypeWithoutVersion.Name}] @{InputParamName.IdentifierAssemblyQualifiedNameWithoutVersion}, @{identifierTypeWithoutVersionId} OUTPUT
      DECLARE @{identifierTypeWithVersionId} {Tables.TypeWithVersion.Id.DataType.DeclarationInSqlSyntax}
      EXEC [{streamName}].[{GetIdAddIfNecessaryTypeWithVersion.Name}] @{InputParamName.IdentifierAssemblyQualifiedNameWithVersion}, @{identifierTypeWithVersionId} OUTPUT

      DECLARE @{objectTypeWithoutVersionId} {Tables.TypeWithoutVersion.Id.DataType.DeclarationInSqlSyntax}
      EXEC [{streamName}].[{GetIdAddIfNecessaryTypeWithoutVersion.Name}] @{InputParamName.ObjectAssemblyQualifiedNameWithoutVersion}, @{objectTypeWithoutVersionId} OUTPUT
      DECLARE @{objectTypeWithVersionId} {Tables.TypeWithVersion.Id.DataType.DeclarationInSqlSyntax}
      EXEC [{streamName}].[{GetIdAddIfNecessaryTypeWithVersion.Name}] @{InputParamName.ObjectAssemblyQualifiedNameWithVersion}, @{objectTypeWithVersionId} OUTPUT

	  DECLARE @{recordCreatedUtc} {Tables.Object.RecordCreatedUtc.DataType.DeclarationInSqlSyntax}
	  SET @RecordCreatedUtc = GETUTCDATE()
	  INSERT INTO [{streamName}].[Object] (
		  [{nameof(Tables.Object.IdentifierTypeWithoutVersionId)}]
		, [{nameof(Tables.Object.IdentifierTypeWithVersionId)}]
		, [{nameof(Tables.Object.ObjectTypeWithoutVersionId)}]
		, [{nameof(Tables.Object.ObjectTypeWithVersionId)}]
		, [{nameof(Tables.Object.SerializerRepresentationId)}]
		, [{nameof(Tables.Object.StringSerializedId)}]
		, [{nameof(Tables.Object.StringSerializedObject)}]
		, [{nameof(Tables.Object.ObjectDateTimeUtc)}]
		, [{nameof(Tables.Object.RecordCreatedUtc)}]
		) VALUES (
		  @{identifierTypeWithoutVersionId}
		, @{identifierTypeWithVersionId}
		, @{objectTypeWithoutVersionId}
		, @{objectTypeWithVersionId}
		, @{nameof(InputParamName.SerializerRepresentationId)}
		, @{nameof(InputParamName.StringSerializedId)}
		, @{nameof(InputParamName.StringSerializedObject)}
		, @{nameof(InputParamName.ObjectDateTimeUtc)}
		, @{recordCreatedUtc}
		)

      SET @{OutputParamName.Id} = SCOPE_IDENTITY()
	  
	  IF (@{nameof(InputParamName.TagsXml)} IS NOT NULL)
	  BEGIN
	      INSERT INTO [{streamName}].Tag
		  SELECT
  		    @{OutputParamName.Id}
          , @{objectTypeWithoutVersionId}
		  , C.value('({TagConversionTool.TagEntryElementName}/@{TagConversionTool.TagEntryKeyAttributeName})[1]', '{Tables.Tag.TagKey.DataType.DeclarationInSqlSyntax}') as [{Tables.Tag.TagKey.Name}]
		  , C.value('({TagConversionTool.TagEntryElementName}/@{TagConversionTool.TagEntryValueAttributeName})[1]', '{Tables.Tag.TagValue.DataType.DeclarationInSqlSyntax}') as [{Tables.Tag.TagValue.Name}]
		  , @{recordCreatedUtc} as {Tables.Tag.RecordCreatedUtc.Name}
		  FROM
			@{nameof(InputParamName.TagsXml)}.nodes('/{TagConversionTool.TagSetElementName}') AS T(C)
	  END

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
