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
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamName.Concern), Tables.HandlingHistory.Concern.DataType, concern),
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamName.Details), Tables.HandlingHistory.Details.DataType, details),
                                         new SqlInputParameterRepresentation<long>(nameof(InputParamName.RecordId), Tables.HandlingHistory.RecordId.DataType, recordId),
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamName.NewStatus), Tables.HandlingHistory.Status.DataType, newStatus.ToString()),
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamName.AcceptableCurrentStatusesXml), new XmlSqlDataTypeRepresentation(), acceptableCurrentStatusesXml),
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamName.TagIdsXml), new XmlSqlDataTypeRepresentation(), tagIdsXml),
                                         new SqlInputParameterRepresentation<int>(nameof(InputParamName.IsUnHandledRecord), new IntSqlDataTypeRepresentation(), 0),
                                         new SqlInputParameterRepresentation<int>(nameof(InputParamName.IsClaimingRecordId), new IntSqlDataTypeRepresentation(), 0),
                                         new SqlOutputParameterRepresentation<long>(nameof(OutputParamName.Id), Tables.HandlingHistory.Id.DataType),
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
                    var recordCreatedUtc = "RecordCreatedUtc";
                    const string tagIdsTable = "TagIdsTable";
                    var transaction = Invariant($"{nameof(PutHandling)}Transaction");
                    var currentStatus = "CurrentStatus";
                    var currentStatusAccepted = "CurrentStatusAccepted";
                    return Invariant(
                        $@"
CREATE PROCEDURE [{streamName}].[{PutHandling.Name}](
  @{InputParamName.Concern} AS {Tables.HandlingHistory.Concern.DataType.DeclarationInSqlSyntax}
, @{InputParamName.Details} AS {Tables.HandlingHistory.Details.DataType.DeclarationInSqlSyntax}
, @{InputParamName.RecordId} AS {Tables.HandlingHistory.RecordId.DataType.DeclarationInSqlSyntax}
, @{InputParamName.NewStatus} AS {Tables.HandlingHistory.Status.DataType.DeclarationInSqlSyntax}
, @{InputParamName.AcceptableCurrentStatusesXml} AS {new XmlSqlDataTypeRepresentation().DeclarationInSqlSyntax}
, @{InputParamName.TagIdsXml} AS {new XmlSqlDataTypeRepresentation().DeclarationInSqlSyntax}
, @{InputParamName.IsUnHandledRecord} AS {new IntSqlDataTypeRepresentation().DeclarationInSqlSyntax}
, @{InputParamName.IsClaimingRecordId} AS {new IntSqlDataTypeRepresentation().DeclarationInSqlSyntax}
, @{OutputParamName.Id} AS {Tables.HandlingHistory.Id.DataType.DeclarationInSqlSyntax} OUTPUT
)
AS
BEGIN

DECLARE @{currentStatus} {Tables.HandlingHistory.Status.DataType.DeclarationInSqlSyntax}
SELECT TOP 1 @{currentStatus} = {Tables.HandlingHistory.Status.Name}
    FROM [{streamName}].[{Tables.HandlingHistory.Table.Name}]
    WHERE [{Tables.HandlingHistory.Concern.Name}] = @{InputParamName.Concern} AND [{Tables.HandlingHistory.RecordId.Name}] = @{InputParamName.RecordId}
    ORDER BY [{Tables.HandlingHistory.Id.Name}] DESC

IF @{currentStatus} IS NULL
BEGIN
    SET @{currentStatus} = '{HandlingStatus.Requested}'
END
--TODO: should we guard against this changing while inserting? (exclusive table lock for a time to live, et al)
DECLARE @{currentStatusAccepted} BIT

SELECT @{currentStatusAccepted} = 1 FROM
[{streamName}].[{Funcs.GetTagsTableVariableFromTagsXml.Name}](@{InputParamName.AcceptableCurrentStatusesXml}) t
WHERE [{Tables.Tag.TagValue.Name}] = @{currentStatus}

IF (@{currentStatusAccepted} = 0)
BEGIN
      SET @{OutputParamName.Id} = NULL
END

{Funcs.GetTagsTableVariableFromTagsXml.BuildTagsTableWithOnlyIdDeclarationSyntax(tagIdsTable)}
INSERT INTO @{tagIdsTable} ([{Tables.Tag.Id.Name}])
    SELECT
	  [{Tables.Tag.TagValue.Name}]
	FROM [{streamName}].[{Funcs.GetTagsTableVariableFromTagIdsXml.Name}](@{InputParamName.TagIdsXml})

BEGIN TRANSACTION [{transaction}]
  BEGIN TRY
	      DECLARE @{recordCreatedUtc} {Tables.Record.RecordCreatedUtc.DataType.DeclarationInSqlSyntax}
	      SET @{recordCreatedUtc} = GETUTCDATE()
          IF (@{InputParamName.IsClaimingRecordId} = 0)
          BEGIN
	          INSERT INTO [{streamName}].[{Tables.HandlingHistory.Table.Name}]
               (
		        [{Tables.HandlingHistory.Concern.Name}]
		      , [{Tables.HandlingHistory.RecordId.Name}]
		      , [{Tables.HandlingHistory.Status.Name}]
		      , [{Tables.HandlingHistory.Details.Name}]
		      , [{Tables.HandlingHistory.RecordCreatedUtc.Name}]
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
		          INSERT INTO [{streamName}].[{Tables.HandlingHistory.Table.Name}]
	               (
			        [{Tables.HandlingHistory.Concern.Name}]
			      , [{Tables.HandlingHistory.RecordId.Name}]
			      , [{Tables.HandlingHistory.Status.Name}]
			      , [{Tables.HandlingHistory.Details.Name}]
			      , [{Tables.HandlingHistory.RecordCreatedUtc.Name}]
			      )
	              SELECT
				        n.[{Tables.HandlingHistory.Concern.Name}]
				      , n.[{Tables.HandlingHistory.RecordId.Name}]
				      , n.[{Tables.HandlingHistory.Status.Name}]
				      , n.[{Tables.HandlingHistory.Details.Name}]
				      , n.[{Tables.HandlingHistory.RecordCreatedUtc.Name}]
				  FROM
					(
						SELECT 
				            @{InputParamName.Concern} AS [{Tables.HandlingHistory.Concern.Name}]
				          , @{InputParamName.RecordId} AS [{Tables.HandlingHistory.RecordId.Name}]
				          , @{InputParamName.NewStatus} AS [{Tables.HandlingHistory.Status.Name}]
				          , @{InputParamName.Details} AS [{Tables.HandlingHistory.Details.Name}]
					      , @{recordCreatedUtc} AS [{Tables.HandlingHistory.RecordCreatedUtc.Name}]
					) AS n
				  LEFT JOIN [{streamName}].[{Tables.HandlingHistory.Table.Name}] e
					  ON n.[{Tables.HandlingHistory.RecordId.Name}] = e.[{Tables.HandlingHistory.RecordId.Name}]
					  AND n.[{Tables.HandlingHistory.Concern.Name}] = e.[{Tables.HandlingHistory.Concern.Name}]
			      WHERE e.[{Tables.HandlingHistory.RecordId.Name}] IS NULL
			   END
			   ELSE
			   BEGIN
		          INSERT INTO [{streamName}].[{Tables.HandlingHistory.Table.Name}]
	               (
			        [{Tables.HandlingHistory.Concern.Name}]
			      , [{Tables.HandlingHistory.RecordId.Name}]
			      , [{Tables.HandlingHistory.Status.Name}]
			      , [{Tables.HandlingHistory.Details.Name}]
			      , [{Tables.HandlingHistory.RecordCreatedUtc.Name}]
			      )
	              SELECT
				        n.[{Tables.HandlingHistory.Concern.Name}]
				      , n.[{Tables.HandlingHistory.RecordId.Name}]
				      , n.[{Tables.HandlingHistory.Status.Name}]
				      , n.[{Tables.HandlingHistory.Details.Name}]
				      , n.[{Tables.HandlingHistory.RecordCreatedUtc.Name}]
				  FROM
					(
						SELECT 
				            @{InputParamName.Concern} AS [{Tables.HandlingHistory.Concern.Name}]
				          , @{InputParamName.RecordId} AS [{Tables.HandlingHistory.RecordId.Name}]
				          , @{InputParamName.NewStatus} AS [{Tables.HandlingHistory.Status.Name}]
				          , @{InputParamName.Details} AS [{Tables.HandlingHistory.Details.Name}]
					      , @{recordCreatedUtc} AS [{Tables.HandlingHistory.RecordCreatedUtc.Name}]
					) AS n
				  INNER JOIN [{streamName}].[{Tables.HandlingHistory.Table.Name}] e
					  ON n.[{Tables.HandlingHistory.RecordId.Name}] = e.[{Tables.HandlingHistory.RecordId.Name}]
					  AND n.[{Tables.HandlingHistory.Concern.Name}] = e.[{Tables.HandlingHistory.Concern.Name}]
				  LEFT OUTER JOIN [{streamName}].[{Tables.HandlingHistory.Table.Name}] h2
					ON e.[{Tables.HandlingHistory.RecordId.Name}] = h2.[{Tables.HandlingHistory.RecordId.Name}] AND e.[{Tables.HandlingHistory.Id.Name}] < h2.[{Tables.HandlingHistory.Id.Name}]
			      WHERE h2.[{Tables.HandlingHistory.Id.Name}] IS NULL AND e.[{Tables.HandlingHistory.Status.Name}] <> '{HandlingStatus.Running}'
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
                }
            }
        }
    }
}
