// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamSchema.Sprocs.TryHandleRecord.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Naos.CodeAnalysis.Recipes;
    using Naos.Database.Domain;
    using Naos.Protocol.Domain;
    using OBeautifulCode.Compression;
    using OBeautifulCode.Representation.System;
    using OBeautifulCode.Serialization;
    using OBeautifulCode.Type;

    /// <summary>
    /// Container for schema.
    /// </summary>
    public static partial class StreamSchema
    {
        /// <summary>
        /// Stored procedures.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sprocs", Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
        public partial class Sprocs
        {
            /// <summary>
            /// Stored procedure: GetLatestByIdAndType.
            /// </summary>
            public static class TryHandleRecord
            {
                /// <summary>
                /// Gets the name.
                /// </summary>
                /// <value>The name.</value>
                public static string Name => nameof(TryHandleRecord);

                /// <summary>
                /// Input parameter names.
                /// </summary>
                [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Param", Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
                public enum InputParamName
                {
                    /// <summary>
                    /// The concern.
                    /// </summary>
                    Concern,

                    /// <summary>
                    /// The resource details.
                    /// </summary>
                    ResourceDetails,

                    /// <summary>
                    /// The identifier assembly qualified name without version
                    /// </summary>
                    IdentifierAssemblyQualifiedNameWithoutVersionQuery,

                    /// <summary>
                    /// The identifier assembly qualified name with version
                    /// </summary>
                    IdentifierAssemblyQualifiedNameWithVersionQuery,

                    /// <summary>
                    /// The object assembly qualified name without version
                    /// </summary>
                    ObjectAssemblyQualifiedNameWithoutVersionQuery,

                    /// <summary>
                    /// The object assembly qualified name with version
                    /// </summary>
                    ObjectAssemblyQualifiedNameWithVersionQuery,

                    /// <summary>
                    /// The order record strategy
                    /// </summary>
                    OrderRecordsStrategy,

                    /// <summary>
                    /// The type version match strategy
                    /// </summary>
                    TypeVersionMatchStrategy,
                }

                /// <summary>
                /// Output parameter names.
                /// </summary>
                [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Param", Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
                public enum OutputParamName
                {
                    /// <summary>
                    /// The identifier.
                    /// </summary>
                    Id,

                    /// <summary>
                    /// The internal record identifier.
                    /// </summary>
                    InternalRecordId,

                    /// <summary>
                    /// The serialization kind
                    /// </summary>
                    SerializationKind,

                    /// <summary>
                    /// The serialization format
                    /// </summary>
                    SerializationFormat,

                    /// <summary>
                    /// The serialization configuration assembly qualified name without version
                    /// </summary>
                    SerializationConfigAssemblyQualifiedNameWithoutVersion,

                    /// <summary>
                    /// The compression kind
                    /// </summary>
                    CompressionKind,

                    /// <summary>
                    /// The identifier assembly qualified name with version
                    /// </summary>
                    IdentifierAssemblyQualifiedNameWithVersion,

                    /// <summary>
                    /// The object assembly qualified name with version
                    /// </summary>
                    ObjectAssemblyQualifiedNameWithVersion,

                    /// <summary>
                    /// The serialized object identifier
                    /// </summary>
                    StringSerializedId,

                    /// <summary>
                    /// The serialized object string
                    /// </summary>
                    StringSerializedObject,

                    /// <summary>
                    /// The record's date and time.
                    /// </summary>
                    RecordDateTime,

                    /// <summary>
                    /// The object's date and time if it was a <see cref="IHaveTimestampUtc"/>.
                    /// </summary>
                    ObjectDateTime,

                    /// <summary>
                    /// Any tags returned as an XML tag set that can be converted using <see cref="TagConversionTool"/>.
                    /// </summary>
                    TagsXml,

                    /// <summary>
                    /// An indicator of whether or not to handle.
                    /// </summary>
                    ShouldHandle,
                }

                /// <summary>
                /// Builds the execute stored procedure operation.
                /// </summary>
                /// <param name="streamName">Name of the stream.</param>
                /// <param name="concern">The concern.</param>
                /// <param name="resourceDetails">The resource details.</param>
                /// <param name="identifierType">The identifier assembly qualified name with and without version.</param>
                /// <param name="objectType">The object assembly qualified name with and without version.</param>
                /// <param name="orderRecordsStrategy">The <see cref="OrderRecordsStrategy"/>.</param>
                /// <param name="typeVersionMatchStrategy">The type version match strategy.</param>
                /// <returns>ExecuteStoredProcedureOp.</returns>
                public static ExecuteStoredProcedureOp BuildExecuteStoredProcedureOp(
                    string streamName,
                    string concern,
                    string resourceDetails,
                    TypeRepresentationWithAndWithoutVersion identifierType,
                    TypeRepresentationWithAndWithoutVersion objectType,
                    OrderRecordsStrategy orderRecordsStrategy,
                    TypeVersionMatchStrategy typeVersionMatchStrategy)
                {
                    var sprocName = FormattableString.Invariant($"[{streamName}].{nameof(TryHandleRecord)}");

                    var parameters = new List<SqlParameterRepresentationBase>()
                                     {
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamName.Concern), Tables.Handling.Concern.DataType, concern),
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamName.ResourceDetails), Tables.Resource.Details.DataType, resourceDetails),
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamName.OrderRecordsStrategy), new StringSqlDataTypeRepresentation(false, 50), orderRecordsStrategy.ToString()),
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamName.IdentifierAssemblyQualifiedNameWithoutVersionQuery), Tables.TypeWithoutVersion.AssemblyQualifiedName.DataType, identifierType.WithoutVersion.BuildAssemblyQualifiedName()),
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamName.IdentifierAssemblyQualifiedNameWithVersionQuery), Tables.TypeWithVersion.AssemblyQualifiedName.DataType, identifierType.WithVersion.BuildAssemblyQualifiedName()),
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamName.ObjectAssemblyQualifiedNameWithoutVersionQuery), Tables.TypeWithoutVersion.AssemblyQualifiedName.DataType, objectType.WithoutVersion.BuildAssemblyQualifiedName()),
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamName.ObjectAssemblyQualifiedNameWithVersionQuery), Tables.TypeWithVersion.AssemblyQualifiedName.DataType, objectType.WithVersion.BuildAssemblyQualifiedName()),
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamName.TypeVersionMatchStrategy), new StringSqlDataTypeRepresentation(false, 50), typeVersionMatchStrategy.ToString()),
                                         new SqlOutputParameterRepresentation<int>(nameof(OutputParamName.ShouldHandle), new IntSqlDataTypeRepresentation()),
                                         new SqlOutputParameterRepresentation<long>(nameof(OutputParamName.Id), Tables.Handling.Id.DataType),
                                         new SqlOutputParameterRepresentation<long>(nameof(OutputParamName.InternalRecordId), Tables.Record.Id.DataType),
                                         new SqlOutputParameterRepresentation<string>(nameof(OutputParamName.SerializationConfigAssemblyQualifiedNameWithoutVersion), Tables.TypeWithoutVersion.AssemblyQualifiedName.DataType),
                                         new SqlOutputParameterRepresentation<SerializationKind>(nameof(OutputParamName.SerializationKind), Tables.SerializerRepresentation.SerializationKind.DataType),
                                         new SqlOutputParameterRepresentation<SerializationFormat>(nameof(OutputParamName.SerializationFormat), Tables.SerializerRepresentation.SerializationFormat.DataType),
                                         new SqlOutputParameterRepresentation<CompressionKind>(nameof(OutputParamName.CompressionKind), Tables.SerializerRepresentation.CompressionKind.DataType),
                                         new SqlOutputParameterRepresentation<string>(nameof(OutputParamName.IdentifierAssemblyQualifiedNameWithVersion), Tables.TypeWithVersion.AssemblyQualifiedName.DataType),
                                         new SqlOutputParameterRepresentation<string>(nameof(OutputParamName.ObjectAssemblyQualifiedNameWithVersion), Tables.TypeWithVersion.AssemblyQualifiedName.DataType),
                                         new SqlOutputParameterRepresentation<string>(nameof(OutputParamName.StringSerializedId), Tables.Record.StringSerializedId.DataType),
                                         new SqlOutputParameterRepresentation<string>(nameof(OutputParamName.StringSerializedObject), Tables.Record.StringSerializedObject.DataType),
                                         new SqlOutputParameterRepresentation<DateTime>(nameof(OutputParamName.RecordDateTime), Tables.Record.RecordCreatedUtc.DataType),
                                         new SqlOutputParameterRepresentation<DateTime?>(nameof(OutputParamName.ObjectDateTime), Tables.Record.ObjectDateTimeUtc.DataType),
                                         new SqlOutputParameterRepresentation<string>(nameof(OutputParamName.TagsXml), new StringSqlDataTypeRepresentation(true, -1)),
                                     };

                    var parameterNameToRepresentationMap = parameters.ToDictionary(k => k.Name, v => v);

                    var result = new ExecuteStoredProcedureOp(sprocName, parameterNameToRepresentationMap);

                    return result;
                }

                /// <summary>
                /// Builds the creation script for put sproc.
                /// </summary>
                /// <param name="streamName">Name of the stream.</param>
                /// <returns>System.String.</returns>
                [System.Diagnostics.CodeAnalysis.SuppressMessage(
                    "Microsoft.Naming",
                    "CA1702:CompoundWordsShouldBeCasedCorrectly",
                    MessageId = "ForGet",
                    Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
                [System.Diagnostics.CodeAnalysis.SuppressMessage(
                    "Microsoft.Naming",
                    "CA1704:IdentifiersShouldBeSpelledCorrectly",
                    MessageId = "Sproc",
                    Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
                public static string BuildCreationScript(
                    string streamName)
                {
                    const string serializerRepresentationId = "SerializerRepresentationId";
                    const string serializerConfigTypeId = "SerializerConfigTypeId";
                    const string identifierTypeWithVersionId = "IdentifierTypeWithVersionId";
                    const string objectTypeWithVersionId = "ObjectTypeWithVersionId";
                    const string objectTypeWithoutVersionIdQuery = "ObjectTypeWithoutVersionIdQuery";
                    const string objectTypeWithVersionIdQuery = "ObjectTypeWithVersionIdQuery";
                    const string recordToHandleId = "RecordToHandleId";
                    const string transaction = "Transaction";
                    const string resourceId = "ResourceId";
                    const string blockedStatus = "BlockedStatus";
                    var result = FormattableString.Invariant(
                        $@"
CREATE PROCEDURE [{streamName}].{TryHandleRecord.Name}(
  @{InputParamName.Concern} AS {Tables.Handling.Concern.DataType.DeclarationInSqlSyntax}
, @{InputParamName.ResourceDetails} AS {Tables.Resource.Details.DataType.DeclarationInSqlSyntax}
, @{InputParamName.OrderRecordsStrategy} AS {new StringSqlDataTypeRepresentation(false, 50).DeclarationInSqlSyntax}
, @{InputParamName.IdentifierAssemblyQualifiedNameWithoutVersionQuery} AS {Tables.TypeWithoutVersion.AssemblyQualifiedName.DataType.DeclarationInSqlSyntax}
, @{InputParamName.IdentifierAssemblyQualifiedNameWithVersionQuery} AS {Tables.TypeWithVersion.AssemblyQualifiedName.DataType.DeclarationInSqlSyntax}
, @{InputParamName.ObjectAssemblyQualifiedNameWithoutVersionQuery} AS {Tables.TypeWithoutVersion.AssemblyQualifiedName.DataType.DeclarationInSqlSyntax}
, @{InputParamName.ObjectAssemblyQualifiedNameWithVersionQuery} AS {Tables.TypeWithVersion.AssemblyQualifiedName.DataType.DeclarationInSqlSyntax}
, @{InputParamName.TypeVersionMatchStrategy} AS varchar(10)
, @{OutputParamName.Id} AS {Tables.Handling.Id.DataType.DeclarationInSqlSyntax} OUTPUT
, @{OutputParamName.InternalRecordId} AS {Tables.Record.Id.DataType.DeclarationInSqlSyntax} OUTPUT
, @{OutputParamName.SerializationConfigAssemblyQualifiedNameWithoutVersion} AS {Tables.TypeWithoutVersion.AssemblyQualifiedName.DataType.DeclarationInSqlSyntax} OUTPUT
, @{OutputParamName.SerializationKind} AS {Tables.SerializerRepresentation.SerializationKind.DataType.DeclarationInSqlSyntax} OUTPUT
, @{OutputParamName.SerializationFormat} AS {Tables.SerializerRepresentation.SerializationFormat.DataType.DeclarationInSqlSyntax} OUTPUT
, @{OutputParamName.CompressionKind} AS {Tables.SerializerRepresentation.CompressionKind.DataType.DeclarationInSqlSyntax} OUTPUT
, @{OutputParamName.IdentifierAssemblyQualifiedNameWithVersion} AS {Tables.TypeWithoutVersion.AssemblyQualifiedName.DataType.DeclarationInSqlSyntax} OUTPUT
, @{OutputParamName.ObjectAssemblyQualifiedNameWithVersion} AS {Tables.TypeWithoutVersion.AssemblyQualifiedName.DataType.DeclarationInSqlSyntax} OUTPUT
, @{OutputParamName.StringSerializedId} AS {Tables.Record.StringSerializedId.DataType.DeclarationInSqlSyntax} OUTPUT
, @{OutputParamName.StringSerializedObject} AS {Tables.Record.StringSerializedObject.DataType.DeclarationInSqlSyntax} OUTPUT
, @{OutputParamName.ObjectDateTime} AS {Tables.Record.ObjectDateTimeUtc.DataType.DeclarationInSqlSyntax} OUTPUT
, @{OutputParamName.RecordDateTime} AS {Tables.Record.RecordCreatedUtc.DataType.DeclarationInSqlSyntax} OUTPUT
, @{OutputParamName.TagsXml} AS {new StringSqlDataTypeRepresentation(true, -1).DeclarationInSqlSyntax} OUTPUT
, @{OutputParamName.ShouldHandle} AS {new IntSqlDataTypeRepresentation().DeclarationInSqlSyntax} OUTPUT
)
AS
BEGIN
    DECLARE @{blockedStatus} {Tables.Handling.Status.DataType.DeclarationInSqlSyntax}
	SELECT TOP 1 @{blockedStatus} = [{Tables.Handling.Status.Name}] FROM [{streamName}].[{Tables.Handling.Table.Name}]
	WHERE [{Tables.Handling.Concern.Name}] = '{Concerns.RecordHandlingConcern}'

	IF ((@{blockedStatus} IS NULL) OR (@{blockedStatus} <> '{HandlingStatus.Blocked}'))
	BEGIN
		DECLARE @{resourceId} {Tables.Resource.Id.DataType.DeclarationInSqlSyntax}
		EXEC [{streamName}].[{GetIdAddIfNecessaryResource.Name}] @{InputParamName.ResourceDetails}, @{resourceId} OUTPUT

		DECLARE @{objectTypeWithoutVersionIdQuery} {Tables.TypeWithoutVersion.Id.DataType.DeclarationInSqlSyntax}
		EXEC [{streamName}].[{GetIdAddIfNecessaryTypeWithoutVersion.Name}] @{InputParamName.ObjectAssemblyQualifiedNameWithoutVersionQuery}, @{objectTypeWithoutVersionIdQuery} OUTPUT
		DECLARE @{objectTypeWithVersionIdQuery} {Tables.TypeWithVersion.Id.DataType.DeclarationInSqlSyntax}
		EXEC [{streamName}].[{GetIdAddIfNecessaryTypeWithVersion.Name}] @{InputParamName.ObjectAssemblyQualifiedNameWithVersionQuery}, @{objectTypeWithVersionIdQuery} OUTPUT

		DECLARE @{recordToHandleId} {Tables.Record.Id.DataType.DeclarationInSqlSyntax}
		BEGIN TRANSACTION [{transaction}]
	  BEGIN TRY
	  IF (@{InputParamName.OrderRecordsStrategy} = '{OrderRecordsStrategy.ByInternalRecordIdAscending}')
          BEGIN
			  -- See if any reprocessing is needed
			  IF (@{recordToHandleId} IS NULL)
			  BEGIN
			      SELECT TOP 1 @{recordToHandleId} = h.[{Tables.Handling.RecordId.Name}]
				  FROM [{streamName}].[{Tables.Handling.Table.Name}] h
			      WHERE h.[{Tables.Handling.Concern.Name}] = @{InputParamName.Concern}
			        AND (h.[{Tables.Handling.Status.Name}] = '{HandlingStatus.RetryFailed}' OR h.[{Tables.Handling.Status.Name}] = '{HandlingStatus.CanceledRunning}' OR h.[{Tables.Handling.Status.Name}] = '{HandlingStatus.SelfCanceledRunning}')
					AND (SELECT TOP 1 [{Tables.Handling.Status.Name}] FROM [{streamName}].[{Tables.Handling.Table.Name}] i WHERE i.{Tables.Handling.RecordId.Name} = h.{Tables.Handling.RecordId.Name} ORDER BY i.{Tables.Handling.Id.Name} DESC) = h.{Tables.Handling.Status.Name}
				  ORDER BY h.[{Tables.Record.Id.Name}] ASC
			  END

			  -- See if any new records
			  IF (@{recordToHandleId} IS NULL)
			  BEGIN
			      SELECT TOP 1 @{recordToHandleId} = r.[{Tables.Record.Id.Name}]
			      FROM [{streamName}].[{Tables.Record.Table.Name}] r
				  LEFT JOIN [{streamName}].[{Tables.Handling.Table.Name}] h
			      ON r.[{Tables.Record.Id.Name}] = h.[{Tables.Handling.RecordId.Name}] AND h.[{Tables.Handling.Concern.Name}] = @{InputParamName.Concern}
			      WHERE r.[{Tables.Record.ObjectTypeWithoutVersionId.Name}] = @{objectTypeWithoutVersionIdQuery}
					AND h.[{Tables.Handling.Id.Name}] IS NULL
				  ORDER BY r.[{Tables.Record.Id.Name}] ASC
			  END
          END
      ELSE IF (@{InputParamName.OrderRecordsStrategy} = '{OrderRecordsStrategy.ByInternalRecordIdDescending}')
			  -- See if any new records
			  IF (@{recordToHandleId} IS NULL)
			  BEGIN
			      SELECT TOP 1 @{recordToHandleId} = r.[{Tables.Record.Id.Name}]
			      FROM [{streamName}].[{Tables.Record.Table.Name}] r
				  LEFT JOIN [{streamName}].[{Tables.Handling.Table.Name}] h
			      ON r.[{Tables.Record.Id.Name}] = h.[{Tables.Handling.RecordId.Name}] AND h.[{Tables.Handling.Concern.Name}] = @{InputParamName.Concern}
			      WHERE r.[{Tables.Record.ObjectTypeWithoutVersionId.Name}] = @{objectTypeWithoutVersionIdQuery}
					AND h.[{Tables.Handling.Id.Name}] IS NULL
				  ORDER BY r.[{Tables.Record.Id.Name}] DESC
			  END

			  -- See if any reprocessing is needed
			  IF (@{recordToHandleId} IS NULL)
			  BEGIN
			      SELECT TOP 1 @{recordToHandleId} = h.[{Tables.Handling.RecordId.Name}]
				  FROM [{streamName}].[{Tables.Handling.Table.Name}] h
			      WHERE h.[{Tables.Handling.Concern.Name}] = @{InputParamName.Concern}
			        AND (h.[{Tables.Handling.Status.Name}] = '{HandlingStatus.RetryFailed}' OR h.[{Tables.Handling.Status.Name}] = '{HandlingStatus.CanceledRunning}' OR h.[{Tables.Handling.Status.Name}] = '{HandlingStatus.SelfCanceledRunning}')
					AND (SELECT TOP 1 [{Tables.Handling.Status.Name}] FROM [{streamName}].[{Tables.Handling.Table.Name}] i WHERE i.{Tables.Handling.RecordId.Name} = h.{Tables.Handling.RecordId.Name} ORDER BY i.{Tables.Handling.Id.Name} DESC) = h.{Tables.Handling.Status.Name}
				  ORDER BY h.[{Tables.Record.Id.Name}] DESC
			  END
      ELSE
		   BEGIN
		      DECLARE @NotValidOrderRecordsStrategyErrorMessage nvarchar(max), 
		              @NotValidOrderRecordsStrategyErrorSeverity int, 
		              @NotValidOrderRecordsStrategyErrorState int

		      SELECT @NotValidOrderRecordsStrategyErrorMessage = 'Unsupported {nameof(OrderRecordsStrategy)}: ' + @{InputParamName.OrderRecordsStrategy} + ERROR_MESSAGE() + ' Line ' + cast(ERROR_LINE() as nvarchar(5)), @NotValidOrderRecordsStrategyErrorSeverity = ERROR_SEVERITY(), @NotValidOrderRecordsStrategyErrorState = ERROR_STATE()
		      RAISERROR (@NotValidOrderRecordsStrategyErrorMessage, @NotValidOrderRecordsStrategyErrorSeverity, @NotValidOrderRecordsStrategyErrorState)
		   END
	IF (@{recordToHandleId} IS NOT NULL)
	BEGIN
		INSERT INTO [{streamName}].[{Tables.Handling.Table.Name}] (
		    [{Tables.Handling.Concern.Name}]
		  , [{Tables.Handling.RecordId.Name}]
		  , [{Tables.Handling.ResourceId.Name}]
		  , [{Tables.Handling.Status.Name}]
		  , [{Tables.Handling.Details.Name}]
		  , [{Tables.Handling.RecordCreatedUtc.Name}]
		  ) VALUES (
	        @{InputParamName.Concern}
	      , @{recordToHandleId}
	      , @{resourceId}
	      , '{HandlingStatus.Running}'
	      , 'Created by {nameof(TryHandleRecord)}'
		  , GETUTCDATE()
		  )

	      SET @{OutputParamName.Id} = SCOPE_IDENTITY()
		  SET @{OutputParamName.ShouldHandle} = 1
		END

	      COMMIT TRANSACTION [{transaction}]
		  END TRY
	      BEGIN CATCH
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

    IF (@{OutputParamName.ShouldHandle} = 1)
	BEGIN
	    DECLARE @{serializerRepresentationId} int   
		DECLARE @{identifierTypeWithVersionId} int
		DECLARE @{objectTypeWithVersionId} int
	    SELECT TOP 1
		   @{serializerRepresentationId} = [{Tables.Record.SerializerRepresentationId.Name}]
		 , @{identifierTypeWithVersionId} = [{Tables.Record.IdentifierTypeWithVersionId.Name}]
		 , @{objectTypeWithVersionId} = [{Tables.Record.ObjectTypeWithVersionId.Name}]
		 , @{OutputParamName.StringSerializedId} = [{Tables.Record.StringSerializedId.Name}]
		 , @{OutputParamName.StringSerializedObject} = [{Tables.Record.StringSerializedObject.Name}]
		 , @{OutputParamName.InternalRecordId} = [{Tables.Record.Id.Name}]
		 , @{OutputParamName.RecordDateTime} = [{Tables.Record.RecordCreatedUtc.Name}]
		 , @{OutputParamName.ObjectDateTime} = [{Tables.Record.ObjectDateTimeUtc.Name}]
		FROM [{streamName}].[{Tables.Record.Table.Name}]
		WHERE [{Tables.Record.Id.Name}] = @{recordToHandleId}

		DECLARE @{serializerConfigTypeId} int
		SELECT 
			@{serializerConfigTypeId} = [{Tables.SerializerRepresentation.SerializationConfigurationTypeWithoutVersionId.Name}] 
		  , @{OutputParamName.SerializationKind} = [{Tables.SerializerRepresentation.SerializationKind.Name}]
		  , @{OutputParamName.SerializationFormat} = [{Tables.SerializerRepresentation.SerializationFormat.Name}]
		  , @{OutputParamName.CompressionKind} = [{Tables.SerializerRepresentation.CompressionKind.Name}]
		FROM [{streamName}].[{Tables.SerializerRepresentation.Table.Name}] WHERE [{Tables.SerializerRepresentation.Id.Name}] = @{serializerRepresentationId}

		SELECT @{OutputParamName.SerializationConfigAssemblyQualifiedNameWithoutVersion} = [{Tables.TypeWithoutVersion.AssemblyQualifiedName.Name}] FROM [{streamName}].[{nameof(Tables.TypeWithoutVersion)}] WHERE [{Tables.TypeWithoutVersion.Id.Name}] = @{serializerConfigTypeId}
		SELECT @{OutputParamName.IdentifierAssemblyQualifiedNameWithVersion} = [{Tables.TypeWithoutVersion.AssemblyQualifiedName.Name}] FROM [{streamName}].[{nameof(Tables.TypeWithVersion)}] WHERE [{Tables.TypeWithVersion.Id.Name}] = @{identifierTypeWithVersionId}
		SELECT @{OutputParamName.ObjectAssemblyQualifiedNameWithVersion} = [{Tables.TypeWithoutVersion.AssemblyQualifiedName.Name}] FROM [{streamName}].[{nameof(Tables.TypeWithVersion)}] WHERE [{Tables.TypeWithVersion.Id.Name}] = @{objectTypeWithVersionId}

	    SELECT @{OutputParamName.TagsXml} = (SELECT
			{Tables.Tag.TagKey.Name} AS [@{TagConversionTool.TagEntryKeyAttributeName}],
			{Tables.Tag.TagValue.Name} AS [@{TagConversionTool.TagEntryValueAttributeName}]
		FROM [{streamName}].[{Tables.Tag.Table.Name}]
		WHERE [{Tables.Tag.RecordId.Name}] = 1
		FOR XML PATH ('{TagConversionTool.TagEntryElementName}'), ROOT('{TagConversionTool.TagSetElementName}'))
	END
    ELSE
	BEGIN
		SET @{OutputParamName.ShouldHandle} = 0
		SET @{OutputParamName.Id} = -1
		SET @{OutputParamName.InternalRecordId} = -1
		SET @{OutputParamName.SerializationConfigAssemblyQualifiedNameWithoutVersion} = 'Fake'
		SET @{OutputParamName.SerializationKind} = 'Json'
		SET @{OutputParamName.SerializationFormat} = 'String'
		SET @{OutputParamName.CompressionKind} = 'None'
		SET @{OutputParamName.IdentifierAssemblyQualifiedNameWithVersion} = 'Fake'
		SET @{OutputParamName.ObjectAssemblyQualifiedNameWithVersion} = 'Fake'
		SET @{OutputParamName.StringSerializedId} = 'Fake'
		SET @{OutputParamName.StringSerializedObject} = 'Fake'
		SET @{OutputParamName.ObjectDateTime} = GETUTCDATE()
		SET @{OutputParamName.RecordDateTime} = GETUTCDATE()
		SET @{OutputParamName.TagsXml} = null
	END
END

			");

                    return result;
                }
            }
        }
    }
}
