// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamSchema.Sprocs.PutHandling.cs" company="Naos Project">
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

    using static System.FormattableString;

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
            /// Stored procedure: AddHandlingEntry.
            /// </summary>
            public static class PutHandling
            {
                /// <summary>
                /// Gets the name of the stored procedure.
                /// </summary>
                /// <value>The name of the stored procedure.</value>
                public static string Name => nameof(PutHandling);

                /// <summary>
                /// Input parameter names.
                /// </summary>
                public enum InputParamName
                {
                    /// <summary>
                    /// The concern.
                    /// </summary>
                    Concern,

                    /// <summary>
                    /// The record identifier.
                    /// </summary>
                    RecordId,

                    /// <summary>
                    /// The details about the resource.
                    /// </summary>
                    Details,

                    /// <summary>
                    /// Creates new status.
                    /// </summary>
                    NewStatus,

                    /// <summary>
                    /// The acceptable current statuses XML.
                    /// </summary>
                    AcceptableCurrentStatusesXml,

                    /// <summary>
                    /// The tag identifiers XML.
                    /// </summary>
                    TagIdsXml,

                    /// <summary>
                    /// Whether or not the record is unhandled.
                    /// </summary>
                    IsUnHandledRecord,

                    /// <summary>
                    /// Whether or not this is a claiming entry.
                    /// </summary>
                    IsClaimingRecordId,
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
                /// <param name="concern">The concern.</param>
                /// <param name="details">The details.</param>
                /// <param name="recordId">The record identifier.</param>
                /// <param name="newStatus">The new status.</param>
                /// <param name="acceptableCurrentStatuses">The acceptable current statuses.</param>
                /// <param name="tagIdsXml">The tag identifiers as XML.</param>
                /// <returns>Operation to execute stored procedure.</returns>
                public static ExecuteStoredProcedureOp BuildExecuteStoredProcedureOp(
                    string streamName,
                    string concern,
                    string details,
                    long recordId,
                    HandlingStatus newStatus,
                    IReadOnlyCollection<HandlingStatus> acceptableCurrentStatuses,
                    string tagIdsXml)
                {
                    var sprocName = FormattableString.Invariant($"[{streamName}].{nameof(PutHandling)}");

                    if (tagIdsXml == null)
                    {
                        var tagIds = new Dictionary<string, string>();
                        tagIdsXml = TagConversionTool.GetTagsXmlString(tagIds);
                    }

                    var acceptableCurrentStatusesDictionary = acceptableCurrentStatuses.ToList().ToOrdinalDictionary();

                    var acceptableCurrentStatusesXml = TagConversionTool.GetTagsXmlString(acceptableCurrentStatusesDictionary);

                    var parameters = new List<SqlParameterRepresentationBase>()
                                     {
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamName.Concern), Tables.Handling.Concern.DataType, concern),
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamName.Details), Tables.Handling.Details.DataType, details),
                                         new SqlInputParameterRepresentation<long>(nameof(InputParamName.RecordId), Tables.Handling.RecordId.DataType, recordId),
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamName.NewStatus), Tables.Handling.Status.DataType, newStatus.ToString()),
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamName.AcceptableCurrentStatusesXml), new XmlSqlDataTypeRepresentation(), acceptableCurrentStatusesXml),
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamName.TagIdsXml), new XmlSqlDataTypeRepresentation(), tagIdsXml),
                                         new SqlInputParameterRepresentation<int>(nameof(InputParamName.IsUnHandledRecord), new IntSqlDataTypeRepresentation(), 0),
                                         new SqlInputParameterRepresentation<int>(nameof(InputParamName.IsClaimingRecordId), new IntSqlDataTypeRepresentation(), 0),
                                         new SqlOutputParameterRepresentation<long>(nameof(OutputParamName.Id), Tables.Handling.Id.DataType),
                                     };

                    var parameterNameToRepresentationMap = parameters.ToDictionary(k => k.Name, v => v);

                    var result = new ExecuteStoredProcedureOp(sprocName, parameterNameToRepresentationMap);

                    return result;
                }

                /// <summary>
                /// Builds the name of the put stored procedure.
                /// </summary>
                /// <param name="streamName">Name of the stream.</param>
                /// <param name="asAlter">An optional value indicating whether or not to generate a ALTER versus CREATE; DEFAULT is false and will generate a CREATE script.</param>
                /// <returns>Name of the put stored procedure.</returns>
                [System.Diagnostics.CodeAnalysis.SuppressMessage(
                    "Microsoft.Naming",
                    "CA1704:IdentifiersShouldBeSpelledCorrectly",
                    MessageId = "Sproc",
                    Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
                public static string BuildCreationScript(
                    string streamName,
                    bool asAlter = false)
                {
                    var recordCreatedUtc = "RecordCreatedUtc";
                    const string tagIdsTable = "TagIdsTable";
                    var transaction = Invariant($"{nameof(PutHandling)}Transaction");
                    var currentStatus = "CurrentStatus";
                    var currentStatusAccepted = "CurrentStatusAccepted";
                    var createOrModify = asAlter ? "ALTER" : "CREATE";
                    var result = Invariant($@"
{createOrModify} PROCEDURE [{streamName}].[{PutHandling.Name}](
  @{InputParamName.Concern} AS {Tables.Handling.Concern.DataType.DeclarationInSqlSyntax}
, @{InputParamName.Details} AS {Tables.Handling.Details.DataType.DeclarationInSqlSyntax}
, @{InputParamName.RecordId} AS {Tables.Handling.RecordId.DataType.DeclarationInSqlSyntax}
, @{InputParamName.NewStatus} AS {Tables.Handling.Status.DataType.DeclarationInSqlSyntax}
, @{InputParamName.AcceptableCurrentStatusesXml} AS {new XmlSqlDataTypeRepresentation().DeclarationInSqlSyntax}
, @{InputParamName.TagIdsXml} AS {new XmlSqlDataTypeRepresentation().DeclarationInSqlSyntax}
, @{InputParamName.IsUnHandledRecord} AS {new IntSqlDataTypeRepresentation().DeclarationInSqlSyntax}
, @{InputParamName.IsClaimingRecordId} AS {new IntSqlDataTypeRepresentation().DeclarationInSqlSyntax}
, @{OutputParamName.Id} AS {Tables.Handling.Id.DataType.DeclarationInSqlSyntax} OUTPUT
)
AS
BEGIN

DECLARE @{currentStatus} {Tables.Handling.Status.DataType.DeclarationInSqlSyntax}
SELECT TOP 1 @{currentStatus} = {Tables.Handling.Status.Name}
    FROM [{streamName}].[{Tables.Handling.Table.Name}]
    WHERE [{Tables.Handling.Concern.Name}] = @{InputParamName.Concern} AND [{Tables.Handling.RecordId.Name}] = @{InputParamName.RecordId}
    ORDER BY [{Tables.Handling.Id.Name}] DESC

IF @{currentStatus} IS NULL
BEGIN
    SET @{currentStatus} = '{HandlingStatus.None}'
END
--TODO: should we guard against this changing while inserting? (exclusive table lock for a time to live, et al)
DECLARE @{currentStatusAccepted} BIT

SELECT @{currentStatusAccepted} = 1 FROM
[{streamName}].[{Funcs.GetTagsTableVariableFromTagsXml.Name}](@{InputParamName.AcceptableCurrentStatusesXml}) t
WHERE [{Tables.Tag.TagValue.Name}] = @{currentStatus}

IF (@{currentStatusAccepted} IS NULL)
BEGIN
	IF (@IsClaimingRecordId = 1)
	BEGIN
		-- If this is an attempt to claim a record it might have an invalid status due to the record
		--      already being claimed...this should only be invoked by {Sprocs.TryHandleRecord.Name} which
		--      filters to valid statuses only.
		SET @{OutputParamName.Id} = NULL
	END
	ELSE
	BEGIN
		-- In the event we are NOT claiming a record then this is being considered an invalid state change...
		DECLARE @NotValidStatusErrorMessage varchar(100)
		SET @NotValidStatusErrorMessage =  CONCAT('Invalid current status: ', @{currentStatus}, '.');
		THROW 60000, @NotValidStatusErrorMessage, 1
	END
END

{Funcs.GetTagsTableVariableFromTagsXml.BuildTagsTableWithOnlyIdDeclarationSyntax(tagIdsTable)}
INSERT INTO @{tagIdsTable} ([{Tables.Tag.Id.Name}])
    SELECT
	  [{Tables.Tag.TagValue.Name}]
	FROM [{streamName}].[{Funcs.GetTagsTableVariableFromTagIdsXml.Name}](@{InputParamName.TagIdsXml})

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
BEGIN TRANSACTION [{transaction}]
  BEGIN TRY
	      DECLARE @{recordCreatedUtc} {Tables.Record.RecordCreatedUtc.DataType.DeclarationInSqlSyntax}
	      SET @{recordCreatedUtc} = GETUTCDATE()
          IF (@{InputParamName.IsClaimingRecordId} = 0)
          BEGIN
	          INSERT INTO [{streamName}].[{Tables.Handling.Table.Name}]
               (
		        [{Tables.Handling.Concern.Name}]
		      , [{Tables.Handling.RecordId.Name}]
		      , [{Tables.Handling.Status.Name}]
		      , [{Tables.Handling.Details.Name}]
		      , [{Tables.Handling.RecordCreatedUtc.Name}]
		      ) VALUES (
	            @{InputParamName.Concern}
	          , @{InputParamName.RecordId}
	          , @{InputParamName.NewStatus}
	          , @{InputParamName.Details}
		      , @{recordCreatedUtc}
		      )
          END
          ELSE
          BEGIN
			  -- For Claiming a Record we'll need to confirm another tenant has not already claimed...
			  IF (@{InputParamName.IsUnHandledRecord} = 1)
		      BEGIN
		          INSERT INTO [{streamName}].[{Tables.Handling.Table.Name}] WITH (TABLOCKX)
	               (
			        [{Tables.Handling.Concern.Name}]
			      , [{Tables.Handling.RecordId.Name}]
			      , [{Tables.Handling.Status.Name}]
			      , [{Tables.Handling.Details.Name}]
			      , [{Tables.Handling.RecordCreatedUtc.Name}]
			      )
	              SELECT
				        n.[{Tables.Handling.Concern.Name}]
				      , n.[{Tables.Handling.RecordId.Name}]
				      , n.[{Tables.Handling.Status.Name}]
				      , n.[{Tables.Handling.Details.Name}]
				      , n.[{Tables.Handling.RecordCreatedUtc.Name}]
				  FROM
					(
						SELECT 
				            @{InputParamName.Concern} AS [{Tables.Handling.Concern.Name}]
				          , @{InputParamName.RecordId} AS [{Tables.Handling.RecordId.Name}]
				          , @{InputParamName.NewStatus} AS [{Tables.Handling.Status.Name}]
				          , @{InputParamName.Details} AS [{Tables.Handling.Details.Name}]
					      , @{recordCreatedUtc} AS [{Tables.Handling.RecordCreatedUtc.Name}]
					) AS n
				  LEFT JOIN [{streamName}].[{Tables.Handling.Table.Name}] e
					  ON n.[{Tables.Handling.RecordId.Name}] = e.[{Tables.Handling.RecordId.Name}]
					  AND n.[{Tables.Handling.Concern.Name}] = e.[{Tables.Handling.Concern.Name}]
			      WHERE e.[{Tables.Handling.RecordId.Name}] IS NULL
			   END
			   ELSE
			   BEGIN
		          INSERT INTO [{streamName}].[{Tables.Handling.Table.Name}] WITH (TABLOCKX)
	               (
			        [{Tables.Handling.Concern.Name}]
			      , [{Tables.Handling.RecordId.Name}]
			      , [{Tables.Handling.Status.Name}]
			      , [{Tables.Handling.Details.Name}]
			      , [{Tables.Handling.RecordCreatedUtc.Name}]
			      )
	              SELECT
				        n.[{Tables.Handling.Concern.Name}]
				      , n.[{Tables.Handling.RecordId.Name}]
				      , n.[{Tables.Handling.Status.Name}]
				      , n.[{Tables.Handling.Details.Name}]
				      , n.[{Tables.Handling.RecordCreatedUtc.Name}]
				  FROM
					(
						SELECT 
				            @{InputParamName.Concern} AS [{Tables.Handling.Concern.Name}]
				          , @{InputParamName.RecordId} AS [{Tables.Handling.RecordId.Name}]
				          , @{InputParamName.NewStatus} AS [{Tables.Handling.Status.Name}]
				          , @{InputParamName.Details} AS [{Tables.Handling.Details.Name}]
					      , @{recordCreatedUtc} AS [{Tables.Handling.RecordCreatedUtc.Name}]
					) AS n
				  INNER JOIN [{streamName}].[{Tables.Handling.Table.Name}] e
					  ON n.[{Tables.Handling.RecordId.Name}] = e.[{Tables.Handling.RecordId.Name}]
					  AND n.[{Tables.Handling.Concern.Name}] = e.[{Tables.Handling.Concern.Name}]
				  LEFT OUTER JOIN [{streamName}].[{Tables.Handling.Table.Name}] h2
					ON e.[{Tables.Handling.RecordId.Name}] = h2.[{Tables.Handling.RecordId.Name}] AND e.[{Tables.Handling.Id.Name}] < h2.[{Tables.Handling.Id.Name}]
			      WHERE h2.[{Tables.Handling.Id.Name}] IS NULL AND e.[{Tables.Handling.Status.Name}] <> '{HandlingStatus.Running}'
			   END
          END

		  IF (@@ROWCOUNT > 0)
		  BEGIN
		      SET @{OutputParamName.Id} = SCOPE_IDENTITY()
	          
		      INSERT INTO [{streamName}].[{Tables.HandlingTag.Table.Name}] (
			    [{Tables.HandlingTag.HandlingId.Name}]
			  , [{Tables.HandlingTag.TagId.Name}]
			  , [{Tables.HandlingTag.RecordCreatedUtc.Name}]
			  )
	         SELECT 
  			    @{OutputParamName.Id}
			  , t.[{Tables.Tag.Id.Name}]
			  , @{recordCreatedUtc}
	         FROM @{tagIdsTable} t
		  END

      COMMIT TRANSACTION [{transaction}]

  END TRY

  BEGIN CATCH
      SET @{OutputParamName.Id} = NULL
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
END");

                    return result;
                }
            }
        }
    }
}
