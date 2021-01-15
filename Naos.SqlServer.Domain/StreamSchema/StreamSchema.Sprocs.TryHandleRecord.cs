// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamSchema.Sprocs.TryHandleRecord.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
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
                    /// The details.
                    /// </summary>
                    Details,

                    /// <summary>
                    /// The identifier assembly qualified name without version
                    /// </summary>
                    IdentifierTypeWithoutVersionIdQuery,

                    /// <summary>
                    /// The identifier assembly qualified name with version
                    /// </summary>
                    IdentifierTypeWithVersionIdQuery,

                    /// <summary>
                    /// The object assembly qualified name without version
                    /// </summary>
                    ObjectTypeWithoutVersionIdQuery,

                    /// <summary>
                    /// The object assembly qualified name with version
                    /// </summary>
                    ObjectTypeWithVersionIdQuery,

                    /// <summary>
                    /// The order record strategy
                    /// </summary>
                    OrderRecordsStrategy,

                    /// <summary>
                    /// The type version match strategy
                    /// </summary>
                    TypeVersionMatchStrategy,

                    /// <summary>
                    /// The tag identifiers as XML.
                    /// </summary>
                    TagIdsForEntryXml,

                    /// <summary>
                    /// Inherit the record's tags in handling.
                    /// </summary>
                    InheritRecordTags,
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
                    /// The serialization configuration assembly qualified name without version
                    /// </summary>
                    SerializerRepresentationId,

                    /// <summary>
                    /// The identifier assembly qualified name with version
                    /// </summary>
                    IdentifierTypeWithVersionId,

                    /// <summary>
                    /// The object assembly qualified name with version
                    /// </summary>
                    ObjectTypeWithVersionId,

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
                    TagIdsXml,

                    /// <summary>
                    /// An indicator of whether or not to handle.
                    /// </summary>
                    ShouldHandle,
                }

                /// <summary>
                /// Builds the execute stored procedure op.
                /// </summary>
                /// <param name="streamName">Name of the stream.</param>
                /// <param name="concern">The concern.</param>
                /// <param name="details">The details.</param>
                /// <param name="identifierType">Type of the identifier.</param>
                /// <param name="objectType">Type of the object.</param>
                /// <param name="orderRecordsStrategy">The order records strategy.</param>
                /// <param name="typeVersionMatchStrategy">The type version match strategy.</param>
                /// <param name="tagIdsXml">The tag identifiers as XML.</param>
                /// <returns>Operation to execute stored procedure.</returns>
                public static ExecuteStoredProcedureOp BuildExecuteStoredProcedureOp(
                    string streamName,
                    string concern,
                    string details,
                    IdentifiedType identifierType,
                    IdentifiedType objectType,
                    OrderRecordsStrategy orderRecordsStrategy,
                    TypeVersionMatchStrategy typeVersionMatchStrategy,
                    string tagIdsXml)
                {
                    var sprocName = FormattableString.Invariant($"[{streamName}].{nameof(TryHandleRecord)}");

                    var parameters = new List<SqlParameterRepresentationBase>()
                                     {
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamName.Concern), Tables.Handling.Concern.DataType, concern),
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamName.Details), Tables.Handling.Details.DataType, details),
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamName.OrderRecordsStrategy), new StringSqlDataTypeRepresentation(false, 50), orderRecordsStrategy.ToString()),
                                         new SqlInputParameterRepresentation<int?>(nameof(InputParamName.IdentifierTypeWithoutVersionIdQuery), Tables.TypeWithoutVersion.Id.DataType, identifierType?.IdWithoutVersion),
                                         new SqlInputParameterRepresentation<int?>(nameof(InputParamName.IdentifierTypeWithVersionIdQuery), Tables.TypeWithVersion.Id.DataType, identifierType?.IdWithVersion),
                                         new SqlInputParameterRepresentation<int?>(nameof(InputParamName.ObjectTypeWithoutVersionIdQuery), Tables.TypeWithoutVersion.Id.DataType, objectType?.IdWithoutVersion),
                                         new SqlInputParameterRepresentation<int?>(nameof(InputParamName.ObjectTypeWithVersionIdQuery), Tables.TypeWithVersion.Id.DataType, objectType?.IdWithVersion),
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamName.TypeVersionMatchStrategy), new StringSqlDataTypeRepresentation(false, 50), typeVersionMatchStrategy.ToString()),
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamName.TagIdsForEntryXml), new StringSqlDataTypeRepresentation(true, -1), tagIdsXml),
                                         new SqlInputParameterRepresentation<int>(nameof(InputParamName.InheritRecordTags), new IntSqlDataTypeRepresentation(), 1),
                                         new SqlOutputParameterRepresentation<int>(nameof(OutputParamName.ShouldHandle), new IntSqlDataTypeRepresentation()),
                                         new SqlOutputParameterRepresentation<long>(nameof(OutputParamName.Id), Tables.Handling.Id.DataType),
                                         new SqlOutputParameterRepresentation<long>(nameof(OutputParamName.InternalRecordId), Tables.Record.Id.DataType),
                                         new SqlOutputParameterRepresentation<int>(nameof(OutputParamName.SerializerRepresentationId), Tables.SerializerRepresentation.Id.DataType),
                                         new SqlOutputParameterRepresentation<int>(nameof(OutputParamName.IdentifierTypeWithVersionId), Tables.TypeWithVersion.Id.DataType),
                                         new SqlOutputParameterRepresentation<int>(nameof(OutputParamName.ObjectTypeWithVersionId), Tables.TypeWithVersion.Id.DataType),
                                         new SqlOutputParameterRepresentation<string>(nameof(OutputParamName.StringSerializedId), Tables.Record.StringSerializedId.DataType),
                                         new SqlOutputParameterRepresentation<string>(nameof(OutputParamName.StringSerializedObject), Tables.Record.StringSerializedObject.DataType),
                                         new SqlOutputParameterRepresentation<DateTime>(nameof(OutputParamName.RecordDateTime), Tables.Record.RecordCreatedUtc.DataType),
                                         new SqlOutputParameterRepresentation<DateTime?>(nameof(OutputParamName.ObjectDateTime), Tables.Record.ObjectDateTimeUtc.DataType),
                                         new SqlOutputParameterRepresentation<string>(nameof(OutputParamName.TagIdsXml), new StringSqlDataTypeRepresentation(true, -1)),
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
                    const string recordToHandleId = "RecordToHandleId";
                    const string transaction = "Transaction";
                    const string blockedStatus = "BlockedStatus";
                    const string unionedIfNecessaryTagIdsXml = "UnionedIfNecessaryTagIdsXml";
                    string acceptableNoneStatusXml = TagConversionTool.GetTagsXmlString(
                        new Dictionary<string, string>
                        {
                            { 1.ToString(CultureInfo.InvariantCulture), HandlingStatus.None.ToString() },
                        });

                    var result = FormattableString.Invariant(
                        $@"
CREATE PROCEDURE [{streamName}].[{TryHandleRecord.Name}](
  @{InputParamName.Concern} AS {Tables.Handling.Concern.DataType.DeclarationInSqlSyntax}
, @{InputParamName.Details} AS {Tables.Handling.Details.DataType.DeclarationInSqlSyntax}
, @{InputParamName.OrderRecordsStrategy} AS {new StringSqlDataTypeRepresentation(false, 50).DeclarationInSqlSyntax}
, @{InputParamName.IdentifierTypeWithoutVersionIdQuery} AS {Tables.TypeWithoutVersion.Id.DataType.DeclarationInSqlSyntax}
, @{InputParamName.IdentifierTypeWithVersionIdQuery} AS {Tables.TypeWithVersion.Id.DataType.DeclarationInSqlSyntax}
, @{InputParamName.ObjectTypeWithoutVersionIdQuery} AS {Tables.TypeWithoutVersion.Id.DataType.DeclarationInSqlSyntax}
, @{InputParamName.ObjectTypeWithVersionIdQuery} AS {Tables.TypeWithVersion.Id.DataType.DeclarationInSqlSyntax}
, @{InputParamName.TypeVersionMatchStrategy} AS varchar(10)
, @{InputParamName.TagIdsForEntryXml} AS {new StringSqlDataTypeRepresentation(true, -1).DeclarationInSqlSyntax}
, @{InputParamName.InheritRecordTags} AS {new IntSqlDataTypeRepresentation().DeclarationInSqlSyntax}
, @{OutputParamName.Id} AS {Tables.Handling.Id.DataType.DeclarationInSqlSyntax} OUTPUT
, @{OutputParamName.InternalRecordId} AS {Tables.Record.Id.DataType.DeclarationInSqlSyntax} OUTPUT
, @{OutputParamName.SerializerRepresentationId} AS {Tables.SerializerRepresentation.Id.DataType.DeclarationInSqlSyntax} OUTPUT
, @{OutputParamName.IdentifierTypeWithVersionId} AS {Tables.TypeWithoutVersion.Id.DataType.DeclarationInSqlSyntax} OUTPUT
, @{OutputParamName.ObjectTypeWithVersionId} AS {Tables.TypeWithoutVersion.Id.DataType.DeclarationInSqlSyntax} OUTPUT
, @{OutputParamName.StringSerializedId} AS {Tables.Record.StringSerializedId.DataType.DeclarationInSqlSyntax} OUTPUT
, @{OutputParamName.StringSerializedObject} AS {Tables.Record.StringSerializedObject.DataType.DeclarationInSqlSyntax} OUTPUT
, @{OutputParamName.ObjectDateTime} AS {Tables.Record.ObjectDateTimeUtc.DataType.DeclarationInSqlSyntax} OUTPUT
, @{OutputParamName.RecordDateTime} AS {Tables.Record.RecordCreatedUtc.DataType.DeclarationInSqlSyntax} OUTPUT
, @{OutputParamName.TagIdsXml} AS {new StringSqlDataTypeRepresentation(true, -1).DeclarationInSqlSyntax} OUTPUT
, @{OutputParamName.ShouldHandle} AS {new IntSqlDataTypeRepresentation().DeclarationInSqlSyntax} OUTPUT
)
AS
BEGIN
    DECLARE @{blockedStatus} {Tables.Handling.Status.DataType.DeclarationInSqlSyntax}
	SELECT TOP 1 @{blockedStatus} = [{Tables.Handling.Status.Name}] FROM [{streamName}].[{Tables.Handling.Table.Name}]
	WHERE [{Tables.Handling.Concern.Name}] = '{Concerns.RecordHandlingConcern}'

	IF ((@{blockedStatus} IS NULL) OR (@{blockedStatus} <> '{HandlingStatus.Blocked}'))
	BEGIN
		DECLARE @{recordToHandleId} {Tables.Record.Id.DataType.DeclarationInSqlSyntax}
        SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
		BEGIN TRANSACTION [{transaction}]
	  BEGIN TRY
	  IF (@{InputParamName.OrderRecordsStrategy} = '{OrderRecordsStrategy.ByInternalRecordIdAscending}')
      BEGIN
		  -- See if any reprocessing is needed
		  IF (@{recordToHandleId} IS NULL)
		  BEGIN
		      SELECT TOP 1 @{recordToHandleId} = h.[{Tables.Handling.RecordId.Name}]
			  FROM [{streamName}].[{Tables.Handling.Table.Name}] h
              INNER JOIN [{streamName}].[{Tables.Record.Table.Name}] r ON r.[{Tables.Record.Id.Name}] = h.[{Tables.Handling.RecordId.Name}]
		      WHERE h.[{Tables.Handling.Concern.Name}] = @{InputParamName.Concern}
		        AND (h.[{Tables.Handling.Status.Name}] = '{HandlingStatus.RetryFailed}' OR h.[{Tables.Handling.Status.Name}] = '{HandlingStatus.CanceledRunning}' OR h.[{Tables.Handling.Status.Name}] = '{HandlingStatus.SelfCanceledRunning}')
				AND (SELECT TOP 1 [{Tables.Handling.Status.Name}] FROM [{streamName}].[{Tables.Handling.Table.Name}] i WHERE i.{Tables.Handling.RecordId.Name} = h.{Tables.Handling.RecordId.Name} ORDER BY i.{Tables.Handling.Id.Name} DESC) = h.{Tables.Handling.Status.Name}
			    AND (
			            -- No Type filter at all
			            (@{InputParamName.IdentifierTypeWithoutVersionIdQuery} IS NULL AND @{InputParamName.IdentifierTypeWithVersionIdQuery} IS NULL AND @{InputParamName.ObjectTypeWithoutVersionIdQuery} IS NULL AND @{InputParamName.ObjectTypeWithVersionIdQuery} IS NULL)
			            OR
			            -- Specific Only Id
			            (@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Specific}' AND @{InputParamName.IdentifierTypeWithVersionIdQuery} IS NOT NULL AND @{InputParamName.ObjectTypeWithVersionIdQuery} IS NULL AND [{Tables.Record.IdentifierTypeWithVersionId.Name}] = @{InputParamName.IdentifierTypeWithVersionIdQuery})
			            OR
			            -- Specific Only Object
			            (@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Specific}' AND @{InputParamName.ObjectTypeWithVersionIdQuery} IS NOT NULL AND @{InputParamName.IdentifierTypeWithVersionIdQuery} IS NULL AND [{Tables.Record.ObjectTypeWithVersionId.Name}] = @{InputParamName.ObjectTypeWithVersionIdQuery})
			            OR
			            -- Specific Both
			            (@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Specific}' AND @{InputParamName.IdentifierTypeWithVersionIdQuery} IS NOT NULL AND @{InputParamName.ObjectTypeWithVersionIdQuery} IS NOT NULL AND [{Tables.Record.IdentifierTypeWithVersionId.Name}] = @{InputParamName.IdentifierTypeWithVersionIdQuery} AND [{Tables.Record.ObjectTypeWithVersionId.Name}] = @{InputParamName.ObjectTypeWithVersionIdQuery})
			            OR
			            -- Any Only Id
			            (@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Any}' AND @{InputParamName.IdentifierTypeWithoutVersionIdQuery} IS NOT NULL AND @{InputParamName.ObjectTypeWithoutVersionIdQuery} IS NULL AND [{Tables.Record.IdentifierTypeWithoutVersionId.Name}] = @{InputParamName.IdentifierTypeWithoutVersionIdQuery})
			            OR
			            -- Any Only Object
			            (@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Any}' AND @{InputParamName.ObjectTypeWithoutVersionIdQuery} IS NOT NULL AND @{InputParamName.IdentifierTypeWithoutVersionIdQuery} IS NULL AND [{Tables.Record.ObjectTypeWithoutVersionId.Name}] = @{InputParamName.ObjectTypeWithoutVersionIdQuery})
			            OR
			            -- Any Both
			            (@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Any}' AND @{InputParamName.IdentifierTypeWithoutVersionIdQuery} IS NOT NULL AND @{InputParamName.ObjectTypeWithoutVersionIdQuery} IS NOT NULL AND [{Tables.Record.IdentifierTypeWithoutVersionId.Name}] = @{InputParamName.IdentifierTypeWithoutVersionIdQuery} AND [{Tables.Record.ObjectTypeWithoutVersionId.Name}] = @{InputParamName.ObjectTypeWithoutVersionIdQuery})
			        )
			  ORDER BY h.[{Tables.Record.Id.Name}] ASC
		  END

		  -- See if any new records
		  IF (@{recordToHandleId} IS NULL)
		  BEGIN
		      SELECT TOP 1 @{recordToHandleId} = r.[{Tables.Record.Id.Name}]
		      FROM [{streamName}].[{Tables.Record.Table.Name}] r
			  LEFT JOIN [{streamName}].[{Tables.Handling.Table.Name}] h
		      ON r.[{Tables.Record.Id.Name}] = h.[{Tables.Handling.RecordId.Name}] AND h.[{Tables.Handling.Concern.Name}] = @{InputParamName.Concern}
		      WHERE h.[{Tables.Handling.Id.Name}] IS NULL
			    AND (
			            -- No Type filter at all
			            (@{InputParamName.IdentifierTypeWithoutVersionIdQuery} IS NULL AND @{InputParamName.IdentifierTypeWithVersionIdQuery} IS NULL AND @{InputParamName.ObjectTypeWithoutVersionIdQuery} IS NULL AND @{InputParamName.ObjectTypeWithVersionIdQuery} IS NULL)
			            OR
			            -- Specific Only Id
			            (@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Specific}' AND @{InputParamName.IdentifierTypeWithVersionIdQuery} IS NOT NULL AND @{InputParamName.ObjectTypeWithVersionIdQuery} IS NULL AND [{Tables.Record.IdentifierTypeWithVersionId.Name}] = @{InputParamName.IdentifierTypeWithVersionIdQuery})
			            OR
			            -- Specific Only Object
			            (@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Specific}' AND @{InputParamName.ObjectTypeWithVersionIdQuery} IS NOT NULL AND @{InputParamName.IdentifierTypeWithVersionIdQuery} IS NULL AND [{Tables.Record.ObjectTypeWithVersionId.Name}] = @{InputParamName.ObjectTypeWithVersionIdQuery})
			            OR
			            -- Specific Both
			            (@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Specific}' AND @{InputParamName.IdentifierTypeWithVersionIdQuery} IS NOT NULL AND @{InputParamName.ObjectTypeWithVersionIdQuery} IS NOT NULL AND [{Tables.Record.IdentifierTypeWithVersionId.Name}] = @{InputParamName.IdentifierTypeWithVersionIdQuery} AND [{Tables.Record.ObjectTypeWithVersionId.Name}] = @{InputParamName.ObjectTypeWithVersionIdQuery})
			            OR
			            -- Any Only Id
			            (@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Any}' AND @{InputParamName.IdentifierTypeWithoutVersionIdQuery} IS NOT NULL AND @{InputParamName.ObjectTypeWithoutVersionIdQuery} IS NULL AND [{Tables.Record.IdentifierTypeWithoutVersionId.Name}] = @{InputParamName.IdentifierTypeWithoutVersionIdQuery})
			            OR
			            -- Any Only Object
			            (@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Any}' AND @{InputParamName.ObjectTypeWithoutVersionIdQuery} IS NOT NULL AND @{InputParamName.IdentifierTypeWithoutVersionIdQuery} IS NULL AND [{Tables.Record.ObjectTypeWithoutVersionId.Name}] = @{InputParamName.ObjectTypeWithoutVersionIdQuery})
			            OR
			            -- Any Both
			            (@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Any}' AND @{InputParamName.IdentifierTypeWithoutVersionIdQuery} IS NOT NULL AND @{InputParamName.ObjectTypeWithoutVersionIdQuery} IS NOT NULL AND [{Tables.Record.IdentifierTypeWithoutVersionId.Name}] = @{InputParamName.IdentifierTypeWithoutVersionIdQuery} AND [{Tables.Record.ObjectTypeWithoutVersionId.Name}] = @{InputParamName.ObjectTypeWithoutVersionIdQuery})
			        )
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
		      WHERE h.[{Tables.Handling.Id.Name}] IS NULL
			    AND (
			            -- No Type filter at all
			            (@{InputParamName.IdentifierTypeWithoutVersionIdQuery} IS NULL AND @{InputParamName.IdentifierTypeWithVersionIdQuery} IS NULL AND @{InputParamName.ObjectTypeWithoutVersionIdQuery} IS NULL AND @{InputParamName.ObjectTypeWithVersionIdQuery} IS NULL)
			            OR
			            -- Specific Only Id
			            (@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Specific}' AND @{InputParamName.IdentifierTypeWithVersionIdQuery} IS NOT NULL AND @{InputParamName.ObjectTypeWithVersionIdQuery} IS NULL AND [{Tables.Record.IdentifierTypeWithVersionId.Name}] = @{InputParamName.IdentifierTypeWithVersionIdQuery})
			            OR
			            -- Specific Only Object
			            (@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Specific}' AND @{InputParamName.ObjectTypeWithVersionIdQuery} IS NOT NULL AND @{InputParamName.IdentifierTypeWithVersionIdQuery} IS NULL AND [{Tables.Record.ObjectTypeWithVersionId.Name}] = @{InputParamName.ObjectTypeWithVersionIdQuery})
			            OR
			            -- Specific Both
			            (@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Specific}' AND @{InputParamName.IdentifierTypeWithVersionIdQuery} IS NOT NULL AND @{InputParamName.ObjectTypeWithVersionIdQuery} IS NOT NULL AND [{Tables.Record.IdentifierTypeWithVersionId.Name}] = @{InputParamName.IdentifierTypeWithVersionIdQuery} AND [{Tables.Record.ObjectTypeWithVersionId.Name}] = @{InputParamName.ObjectTypeWithVersionIdQuery})
			            OR
			            -- Any Only Id
			            (@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Any}' AND @{InputParamName.IdentifierTypeWithoutVersionIdQuery} IS NOT NULL AND @{InputParamName.ObjectTypeWithoutVersionIdQuery} IS NULL AND [{Tables.Record.IdentifierTypeWithoutVersionId.Name}] = @{InputParamName.IdentifierTypeWithoutVersionIdQuery})
			            OR
			            -- Any Only Object
			            (@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Any}' AND @{InputParamName.ObjectTypeWithoutVersionIdQuery} IS NOT NULL AND @{InputParamName.IdentifierTypeWithoutVersionIdQuery} IS NULL AND [{Tables.Record.ObjectTypeWithoutVersionId.Name}] = @{InputParamName.ObjectTypeWithoutVersionIdQuery})
			            OR
			            -- Any Both
			            (@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Any}' AND @{InputParamName.IdentifierTypeWithoutVersionIdQuery} IS NOT NULL AND @{InputParamName.ObjectTypeWithoutVersionIdQuery} IS NOT NULL AND [{Tables.Record.IdentifierTypeWithoutVersionId.Name}] = @{InputParamName.IdentifierTypeWithoutVersionIdQuery} AND [{Tables.Record.ObjectTypeWithoutVersionId.Name}] = @{InputParamName.ObjectTypeWithoutVersionIdQuery})
			        )
			  ORDER BY r.[{Tables.Record.Id.Name}] ASC
		  END

		  -- See if any reprocessing is needed
		  IF (@{recordToHandleId} IS NULL)
		  BEGIN
		      SELECT TOP 1 @{recordToHandleId} = h.[{Tables.Handling.RecordId.Name}]
			  FROM [{streamName}].[{Tables.Handling.Table.Name}] h
              INNER JOIN [{streamName}].[{Tables.Record.Table.Name}] r ON r.[{Tables.Record.Id.Name}] = h.[{Tables.Handling.RecordId.Name}]
		      WHERE h.[{Tables.Handling.Concern.Name}] = @{InputParamName.Concern}
		        AND (h.[{Tables.Handling.Status.Name}] = '{HandlingStatus.RetryFailed}' OR h.[{Tables.Handling.Status.Name}] = '{HandlingStatus.CanceledRunning}' OR h.[{Tables.Handling.Status.Name}] = '{HandlingStatus.SelfCanceledRunning}')
				AND (SELECT TOP 1 [{Tables.Handling.Status.Name}] FROM [{streamName}].[{Tables.Handling.Table.Name}] i WHERE i.{Tables.Handling.RecordId.Name} = h.{Tables.Handling.RecordId.Name} ORDER BY i.{Tables.Handling.Id.Name} DESC) = h.{Tables.Handling.Status.Name}
			    AND (
			            -- No Type filter at all
			            (@{InputParamName.IdentifierTypeWithoutVersionIdQuery} IS NULL AND @{InputParamName.IdentifierTypeWithVersionIdQuery} IS NULL AND @{InputParamName.ObjectTypeWithoutVersionIdQuery} IS NULL AND @{InputParamName.ObjectTypeWithVersionIdQuery} IS NULL)
			            OR
			            -- Specific Only Id
			            (@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Specific}' AND @{InputParamName.IdentifierTypeWithVersionIdQuery} IS NOT NULL AND @{InputParamName.ObjectTypeWithVersionIdQuery} IS NULL AND [{Tables.Record.IdentifierTypeWithVersionId.Name}] = @{InputParamName.IdentifierTypeWithVersionIdQuery})
			            OR
			            -- Specific Only Object
			            (@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Specific}' AND @{InputParamName.ObjectTypeWithVersionIdQuery} IS NOT NULL AND @{InputParamName.IdentifierTypeWithVersionIdQuery} IS NULL AND [{Tables.Record.ObjectTypeWithVersionId.Name}] = @{InputParamName.ObjectTypeWithVersionIdQuery})
			            OR
			            -- Specific Both
			            (@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Specific}' AND @{InputParamName.IdentifierTypeWithVersionIdQuery} IS NOT NULL AND @{InputParamName.ObjectTypeWithVersionIdQuery} IS NOT NULL AND [{Tables.Record.IdentifierTypeWithVersionId.Name}] = @{InputParamName.IdentifierTypeWithVersionIdQuery} AND [{Tables.Record.ObjectTypeWithVersionId.Name}] = @{InputParamName.ObjectTypeWithVersionIdQuery})
			            OR
			            -- Any Only Id
			            (@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Any}' AND @{InputParamName.IdentifierTypeWithoutVersionIdQuery} IS NOT NULL AND @{InputParamName.ObjectTypeWithoutVersionIdQuery} IS NULL AND [{Tables.Record.IdentifierTypeWithoutVersionId.Name}] = @{InputParamName.IdentifierTypeWithoutVersionIdQuery})
			            OR
			            -- Any Only Object
			            (@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Any}' AND @{InputParamName.ObjectTypeWithoutVersionIdQuery} IS NOT NULL AND @{InputParamName.IdentifierTypeWithoutVersionIdQuery} IS NULL AND [{Tables.Record.ObjectTypeWithoutVersionId.Name}] = @{InputParamName.ObjectTypeWithoutVersionIdQuery})
			            OR
			            -- Any Both
			            (@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Any}' AND @{InputParamName.IdentifierTypeWithoutVersionIdQuery} IS NOT NULL AND @{InputParamName.ObjectTypeWithoutVersionIdQuery} IS NOT NULL AND [{Tables.Record.IdentifierTypeWithoutVersionId.Name}] = @{InputParamName.IdentifierTypeWithoutVersionIdQuery} AND [{Tables.Record.ObjectTypeWithoutVersionId.Name}] = @{InputParamName.ObjectTypeWithoutVersionIdQuery})
			        )
			  ORDER BY h.[{Tables.Record.Id.Name}] ASC
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
		DECLARE @{unionedIfNecessaryTagIdsXml} {new XmlSqlDataTypeRepresentation().DeclarationInSqlSyntax}
		
        SELECT @{unionedIfNecessaryTagIdsXml} = (
        SELECT
				  ROW_NUMBER() OVER (ORDER BY [{Tables.Tag.Id.Name}]) AS [@{TagConversionTool.TagEntryKeyAttributeName}]
				, [{Tables.Tag.Id.Name}] AS [@{TagConversionTool.TagEntryValueAttributeName}]
        FROM
			(
                SELECT DISTINCT [{Tables.Tag.Id.Name}] FROM
				(
					SELECT [{Tables.Tag.TagValue.Name}] AS [{Tables.Tag.Id.Name}]
				    FROM [{streamName}].[{Funcs.GetTagsTableVariableFromTagIdsXml.Name}](@{InputParamName.TagIdsForEntryXml})
			        UNION ALL
					SELECT [{Tables.RecordTag.TagId.Name}] AS [{Tables.Tag.Id.Name}]
                    FROM [{streamName}].[{Tables.RecordTag.Table.Name}]
					WHERE @{InputParamName.InheritRecordTags} = 1 AND [{Tables.RecordTag.RecordId.Name}] = @{recordToHandleId}
				) AS u
			) AS d
	    FOR XML PATH ('{TagConversionTool.TagEntryElementName}'), ROOT('{TagConversionTool.TagSetElementName}'))

		EXEC [{streamName}].[{PutHandling.Name}] 
		@{PutHandling.InputParamName.Concern} = @{InputParamName.Concern}, 
		@{PutHandling.InputParamName.Details} = @{InputParamName.Details}, 
		@{PutHandling.InputParamName.RecordId} = @{recordToHandleId}, 
		@{PutHandling.InputParamName.NewStatus} = '{HandlingStatus.Running}', 
		@{PutHandling.InputParamName.AcceptableCurrentStatusesXml} = '{acceptableNoneStatusXml}', 
		@{PutHandling.InputParamName.TagIdsXml} = @{unionedIfNecessaryTagIdsXml}, 
		@{PutHandling.OutputParamName.Id} = @{OutputParamName.Id} OUTPUT

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
	    SELECT TOP 1
		   @{OutputParamName.SerializerRepresentationId} = [{Tables.Record.SerializerRepresentationId.Name}]
		 , @{OutputParamName.IdentifierTypeWithVersionId} = [{Tables.Record.IdentifierTypeWithVersionId.Name}]
		 , @{OutputParamName.ObjectTypeWithVersionId} = [{Tables.Record.ObjectTypeWithVersionId.Name}]
		 , @{OutputParamName.StringSerializedId} = [{Tables.Record.StringSerializedId.Name}]
		 , @{OutputParamName.StringSerializedObject} = [{Tables.Record.StringSerializedObject.Name}]
		 , @{OutputParamName.InternalRecordId} = [{Tables.Record.Id.Name}]
		 , @{OutputParamName.RecordDateTime} = [{Tables.Record.RecordCreatedUtc.Name}]
		 , @{OutputParamName.ObjectDateTime} = [{Tables.Record.ObjectDateTimeUtc.Name}]
		FROM [{streamName}].[{Tables.Record.Table.Name}]
		WHERE [{Tables.Record.Id.Name}] = @{recordToHandleId}

	    SELECT @{OutputParamName.TagIdsXml} = (SELECT
			ROW_NUMBER() OVER (ORDER BY [{Tables.Tag.Id.Name}]) AS [@{TagConversionTool.TagEntryKeyAttributeName}],
			{Tables.Tag.Id.Name} AS [@{TagConversionTool.TagEntryValueAttributeName}]
		FROM [{streamName}].[{Tables.RecordTag.Table.Name}]
		WHERE [{Tables.RecordTag.RecordId.Name}] = @{recordToHandleId}
		FOR XML PATH ('{TagConversionTool.TagEntryElementName}'), ROOT('{TagConversionTool.TagSetElementName}'))
	END
    ELSE
	BEGIN
		SET @{OutputParamName.ShouldHandle} = 0
		SET @{OutputParamName.Id} = -1
		SET @{OutputParamName.InternalRecordId} = -1
		SET @{OutputParamName.SerializerRepresentationId} = -1
		SET @{OutputParamName.IdentifierTypeWithVersionId} = -1
		SET @{OutputParamName.ObjectTypeWithVersionId} = -1
		SET @{OutputParamName.StringSerializedId} = 'Fake'
		SET @{OutputParamName.StringSerializedObject} = 'Fake'
		SET @{OutputParamName.ObjectDateTime} = GETUTCDATE()
		SET @{OutputParamName.RecordDateTime} = GETUTCDATE()
		SET @{OutputParamName.TagIdsXml} = null
	END
END

			");

                    return result;
                }
            }
        }
    }
}
