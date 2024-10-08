﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamSchema.Sprocs.PutHandling.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System.Collections.Generic;
    using System.Linq;
    using Naos.Database.Domain;
    using OBeautifulCode.Collection.Recipes;
    using static System.FormattableString;

    public static partial class StreamSchema
    {
        public static partial class Sprocs
        {
            /// <summary>
            /// Stored procedure: PutHandling.
            /// </summary>
            public static class PutHandling
            {
                /// <summary>
                /// Gets the name of the stored procedure.
                /// </summary>
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
                    /// The acceptable current statuses as CSV.
                    /// </summary>
                    AcceptableCurrentStatusesCsv,

                    /// <summary>
                    /// The tag identifiers as CSV.
                    /// </summary>
                    TagIdsForEntryCsv,

                    /// <summary>
                    /// Inherit the record's tags in handling.
                    /// </summary>
                    InheritRecordTags,

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
                /// <param name="tagIdsForEntryCsv">The tag identifiers as CSV.</param>
                /// <param name="inheritRecordTags">The tags on the record should also be on the handling entry.</param>
                /// <returns>Operation to execute stored procedure.</returns>
                public static ExecuteStoredProcedureOp BuildExecuteStoredProcedureOp(
                    string streamName,
                    string concern,
                    string details,
                    long recordId,
                    HandlingStatus newStatus,
                    IReadOnlyCollection<HandlingStatus> acceptableCurrentStatuses,
                    string tagIdsForEntryCsv,
                    bool inheritRecordTags)
                {
                    var sprocName = Invariant($"[{streamName}].{nameof(PutHandling)}");

                    var acceptableCurrentStatusesCsv = acceptableCurrentStatuses.Select(_ => _.ToString()).ToCsv();

                    var parameters = new List<ParameterDefinitionBase>()
                                     {
                                         new InputParameterDefinition<string>(nameof(InputParamName.Concern), Tables.Handling.Concern.SqlDataType, concern),
                                         new InputParameterDefinition<string>(nameof(InputParamName.Details), Tables.Handling.Details.SqlDataType, details),
                                         new InputParameterDefinition<long>(nameof(InputParamName.RecordId), Tables.Handling.RecordId.SqlDataType, recordId),
                                         new InputParameterDefinition<string>(nameof(InputParamName.NewStatus), Tables.Handling.Status.SqlDataType, newStatus.ToString()),
                                         new InputParameterDefinition<string>(nameof(InputParamName.AcceptableCurrentStatusesCsv), new StringSqlDataTypeRepresentation(false, StringSqlDataTypeRepresentation.MaxNonUnicodeLengthConstant), acceptableCurrentStatusesCsv),
                                         new InputParameterDefinition<string>(nameof(InputParamName.TagIdsForEntryCsv), Tables.Record.TagIdsCsv.SqlDataType, tagIdsForEntryCsv),
                                         new InputParameterDefinition<int>(nameof(InputParamName.InheritRecordTags), new IntSqlDataTypeRepresentation(), inheritRecordTags ? 1 : 0),
                                         new InputParameterDefinition<int>(nameof(InputParamName.IsUnHandledRecord), new IntSqlDataTypeRepresentation(), 0),
                                         new InputParameterDefinition<int>(nameof(InputParamName.IsClaimingRecordId), new IntSqlDataTypeRepresentation(), 0),
                                         new OutputParameterDefinition<long?>(nameof(OutputParamName.Id), Tables.Handling.Id.SqlDataType),
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
                    var recordCreatedUtc = "RecordCreatedUtc";
                    var unionedIfNecessaryTagIdsCsv = "UnionedIfNecessaryTagIdsCsv";
                    var transaction = Invariant($"{nameof(PutHandling)}Transaction");
                    var currentStatus = "CurrentStatus";
                    var currentStatusAccepted = "CurrentStatusAccepted";
                    var acceptableCurrentStatusesTable = "AcceptableCurrentStatuses";
                    var createOrModify = asAlter ? "CREATE OR ALTER" : "CREATE";
                    var result = Invariant($@"
{createOrModify} PROCEDURE [{streamName}].[{PutHandling.Name}](
  @{InputParamName.Concern} AS {Tables.Handling.Concern.SqlDataType.DeclarationInSqlSyntax}
, @{InputParamName.Details} AS {Tables.Handling.Details.SqlDataType.DeclarationInSqlSyntax}
, @{InputParamName.RecordId} AS {Tables.Handling.RecordId.SqlDataType.DeclarationInSqlSyntax}
, @{InputParamName.NewStatus} AS {Tables.Handling.Status.SqlDataType.DeclarationInSqlSyntax}
, @{InputParamName.AcceptableCurrentStatusesCsv} AS {new StringSqlDataTypeRepresentation(false, StringSqlDataTypeRepresentation.MaxNonUnicodeLengthConstant).DeclarationInSqlSyntax}
, @{InputParamName.TagIdsForEntryCsv} AS {Tables.Record.TagIdsCsv.SqlDataType.DeclarationInSqlSyntax}
, @{InputParamName.InheritRecordTags} AS {new IntSqlDataTypeRepresentation().DeclarationInSqlSyntax}
, @{InputParamName.IsUnHandledRecord} AS {new IntSqlDataTypeRepresentation().DeclarationInSqlSyntax} -- This indicates whether to expect no handling history or not
, @{InputParamName.IsClaimingRecordId} AS {new IntSqlDataTypeRepresentation().DeclarationInSqlSyntax} -- This indicates whether this sproc is being called from TryHandleRecord or directly to make a status change (e.g. Running => Completed)
, @{OutputParamName.Id} AS {Tables.Handling.Id.SqlDataType.DeclarationInSqlSyntax} OUTPUT
)
AS
BEGIN
-- DOES NOT check stream record handling blocked status which allows running records to bleed out and complete/cancel/fail/etc while stopping new handling.
DECLARE @{currentStatus} {Tables.Handling.Status.SqlDataType.DeclarationInSqlSyntax}
SELECT TOP 1 @{currentStatus} = {Tables.Handling.Status.Name}
    FROM [{streamName}].[{Tables.Handling.Table.Name}]
    WHERE [{Tables.Handling.Concern.Name}] = @{InputParamName.Concern}
      AND [{Tables.Handling.RecordId.Name}] = @{InputParamName.RecordId}
    ORDER BY [{Tables.Handling.Id.Name}] DESC

IF @{currentStatus} IS NULL
BEGIN
    SET @{currentStatus} = '{HandlingStatus.AvailableByDefault}'
END

DECLARE @{currentStatusAccepted} BIT

DECLARE @{acceptableCurrentStatusesTable} TABLE
(
   {Tables.Handling.Status.Name} VARCHAR(50)
)
INSERT INTO @{acceptableCurrentStatusesTable} SELECT Value FROM STRING_SPLIT(@{InputParamName.AcceptableCurrentStatusesCsv}, ',')

SELECT @{currentStatusAccepted} = 1 FROM @{acceptableCurrentStatusesTable}
WHERE [{Tables.Handling.Status.Name}] = @{currentStatus}

IF (@{currentStatusAccepted} IS NULL)
BEGIN
	IF (@{InputParamName.IsClaimingRecordId} = 1)
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
		THROW {GeneralPurposeErrorNumberForThrowStatement}, @NotValidStatusErrorMessage, {GeneralPurposeErrorStateForThrowStatement}
	END
END

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
BEGIN TRANSACTION [{transaction}]
  BEGIN TRY
	      DECLARE @{recordCreatedUtc} {Tables.Record.RecordCreatedUtc.SqlDataType.DeclarationInSqlSyntax}
	      SET @{recordCreatedUtc} = GETUTCDATE()

          IF (@{InputParamName.IsClaimingRecordId} = 0)
          BEGIN
		      -- We are not doing similar hardening here (secondary status check and table lock like in the code below
		      -- to ensure that the state transition is valid).
		      -- The status transitions that follow this code path (not TryHandle originating/claiming a record)
		      -- tend (see exceptions below) to be consequence driven (e.g. a bot finished or failed and is updating the status) and thus not competing
		      -- with other tenants who are potentially simultaneously trying to make the same state transition and also
		      -- not constantly trying to make the transition (e.g. a bot calling TryHandle in an infinite loop).
		      -- Exceptions to consequence driven tendency:
		      -- * Someone could be trying to externally cancel a record's handling at the same time that the tenant is trying to fail,
		      --   complete, or self cancel the record, which would create similar contention as TryHandle.
		      -- * A tenant could attempt to complete or fail handling for a record that has already been externally cancelled.
		      --   Which could create an invalid state transition due to the lack of secondary valid existing status checking.
		      --   We have not yet witnessed this scenario like the Completed => Running scenario.
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
			  IF (@{InputParamName.IsUnHandledRecord} = 1)
		      BEGIN
			  	  -- For Claiming a Record we'll need to confirm another tenant has not already claimed
				  -- (two tenants may be competing for the same unhandled record and we need to ensure that only one wins).
			      -- Here we believe that the record has no handling history, but we confirm that and only
				  -- insert if that's true (look at the WHERE clause below).
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
				  -- For Claiming a Record we'll need to confirm another tenant has not already claimed
				  -- (two tenants may be competing for the same record with prior handling history and we need to ensure that only one wins).
			      -- Here we believe that the record has history, but we confirm that the state transition is valid (has not already been changed to Running state)
				  -- (look at INNER JOIN @AcceptableCurrentStatuses below).
			      -- Previously, we only checked that the status is != Running, but after seeing records moving from Completed to Running
				  -- while a maintenance job was running, we are now checking that the current status is still a valid status holistically, not just that it's in a Running status
				  -- inside of a transaction and not exclusively relying on the valid existing status check above (which is out of the transaction).
				  -- We have witnessed this invalid transition while a maintenance job was running, twice after tens of millions of handling attempts across multiple years.
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
					  ON e.[{Tables.Handling.RecordId.Name}] = h2.[{Tables.Handling.RecordId.Name}]
                      AND e.[{Tables.Handling.Concern.Name}] = h2.[{Tables.Handling.Concern.Name}]
                      AND e.[{Tables.Handling.Id.Name}] < h2.[{Tables.Handling.Id.Name}]
                  INNER JOIN @{acceptableCurrentStatusesTable} a
				      ON a.[{Tables.Handling.Status.Name}] = e.[{Tables.Handling.Status.Name}]
			      WHERE h2.[{Tables.Handling.Id.Name}] IS NULL
			   END
          END

		  IF (@@ROWCOUNT > 0)
		  BEGIN
		      SET @{OutputParamName.Id} = SCOPE_IDENTITY()

		      DECLARE @{unionedIfNecessaryTagIdsCsv} {Tables.Record.TagIdsCsv.SqlDataType.DeclarationInSqlSyntax}

	          SELECT @{unionedIfNecessaryTagIdsCsv} = STRING_AGG([{Tables.Tag.Id.Name}], ',')
	          FROM
		      	(
	                  SELECT DISTINCT [{Tables.Tag.Id.Name}] FROM
		      		(
		      			SELECT value AS [{Tables.Tag.Id.Name}]
		      		    FROM STRING_SPLIT(@{InputParamName.TagIdsForEntryCsv}, ',')
		      	        UNION ALL
		      			SELECT value AS [{Tables.Tag.Id.Name}]
                        FROM STRING_SPLIT(
                        (
                          SELECT [{Tables.Record.TagIdsCsv.Name}]
	                      FROM [{streamName}].[{Tables.Record.Table.Name}]
		      			  WHERE @{InputParamName.InheritRecordTags} = 1 AND [{Tables.Record.Id.Name}] = @{InputParamName.RecordId}
                       ), ',')
		      		) AS u
		      	) AS d

		      INSERT INTO [{streamName}].[{Tables.HandlingTag.Table.Name}] (
			    [{Tables.HandlingTag.HandlingId.Name}]
			  , [{Tables.HandlingTag.TagId.Name}]
			  , [{Tables.HandlingTag.RecordCreatedUtc.Name}]
			  )
	         SELECT
  			    @{OutputParamName.Id}
			  , value AS [{Tables.Tag.Id.Name}]
			  , @{recordCreatedUtc}
	         FROM STRING_SPLIT(@{unionedIfNecessaryTagIdsCsv}, ',')
		  END

      COMMIT TRANSACTION [{transaction}]

  END TRY

  BEGIN CATCH
      SET @{OutputParamName.Id} = NULL
      DECLARE @ThrowMessage nvarchar(max),
              @ErrorMessage nvarchar(max),
              @ErrorSeverity int,
              @ErrorState int

      SELECT @ErrorMessage = ERROR_MESSAGE() + ' Line ' + cast(ERROR_LINE() as nvarchar(5)), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE()
      SELECT @ThrowMessage = @ErrorMessage + '; ErrorSeverity=' + cast(@ErrorSeverity as nvarchar(20)) + '; ErrorState=' + cast(@ErrorState as nvarchar(20))

      IF (@@trancount > 0)
      BEGIN
         ROLLBACK TRANSACTION [{transaction}]
      END;

      THROW {GeneralPurposeErrorNumberForThrowStatement}, @ThrowMessage, {GeneralPurposeErrorStateForThrowStatement}
  END CATCH
END");

                    return result;
                }
            }
        }
    }
}
