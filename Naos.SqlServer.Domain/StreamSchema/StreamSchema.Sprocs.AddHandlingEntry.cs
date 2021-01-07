// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamSchema.Sprocs.AddHandlingEntry.cs" company="Naos Project">
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
            public static class AddHandlingEntry
            {
                /// <summary>
                /// Gets the name of the stored procedure.
                /// </summary>
                /// <value>The name of the stored procedure.</value>
                public static string Name => nameof(AddHandlingEntry);

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
                    /// The resource details
                    /// </summary>
                    ResourceDetails,

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
                /// <param name="recordId">The record identifier.</param>
                /// <param name="resourceDetails">The resource details.</param>
                /// <param name="newStatus">The new status.</param>
                /// <param name="acceptableCurrentStatuses">The acceptable current statuses.</param>
                /// <param name="details">The details.</param>
                /// <returns>ExecuteStoredProcedureOp.</returns>
                public static ExecuteStoredProcedureOp BuildExecuteStoredProcedureOp(
                    string streamName,
                    string concern,
                    long recordId,
                    string resourceDetails,
                    HandlingStatus newStatus,
                    IReadOnlyCollection<HandlingStatus> acceptableCurrentStatuses,
                    string details)
                {
                    var sprocName = FormattableString.Invariant($"[{streamName}].{nameof(AddHandlingEntry)}");

                    var acceptableCurrentStatusesDictionary = new Dictionary<string, string>();
                    for (var idx = 0; idx < acceptableCurrentStatuses.Count; idx++)
                    {
                        acceptableCurrentStatusesDictionary.Add(idx.ToString(CultureInfo.InvariantCulture), acceptableCurrentStatuses.ElementAt(idx).ToString());
                    }

                    var acceptableCurrentStatusesXml = TagConversionTool.GetTagsXmlString(acceptableCurrentStatusesDictionary);

                    var parameters = new List<SqlParameterRepresentationBase>()
                                     {
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamName.Concern), Tables.Handling.Details.DataType, concern),
                                         new SqlInputParameterRepresentation<long>(nameof(InputParamName.RecordId), Tables.Handling.RecordId.DataType, recordId),
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamName.Details), Tables.Handling.Details.DataType, details),
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamName.ResourceDetails), Tables.Resource.Details.DataType, resourceDetails),
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamName.NewStatus), Tables.Handling.Status.DataType, newStatus.ToString()),
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamName.AcceptableCurrentStatusesXml), new StringSqlDataTypeRepresentation(true, -1), acceptableCurrentStatusesXml),
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
                /// <returns>Name of the put stored procedure.</returns>
                [System.Diagnostics.CodeAnalysis.SuppressMessage(
                    "Microsoft.Naming",
                    "CA1704:IdentifiersShouldBeSpelledCorrectly",
                    MessageId = "Sproc",
                    Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
                public static string BuildCreationScript(
                    string streamName)
                {
                    var resourceId = "ResourceId";
                    var transaction = Invariant($"{nameof(AddHandlingEntry)}Transaction");
                    var currentStatus = "CurrentStatus";
                    var currentStatusAccepted = "CurrentStatusAccepted";
                    return Invariant(
                        $@"
CREATE PROCEDURE [{streamName}].{AddHandlingEntry.Name}(
  @{InputParamName.Concern} AS {Tables.Handling.Concern.DataType.DeclarationInSqlSyntax}
, @{InputParamName.RecordId} AS {Tables.Handling.RecordId.DataType.DeclarationInSqlSyntax}
, @{InputParamName.Details} AS {Tables.Handling.Details.DataType.DeclarationInSqlSyntax}
, @{InputParamName.ResourceDetails} AS {Tables.Resource.Details.DataType.DeclarationInSqlSyntax}
, @{InputParamName.NewStatus} AS {Tables.Handling.Status.DataType.DeclarationInSqlSyntax}
, @{InputParamName.AcceptableCurrentStatusesXml} AS [Xml]
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
    SET @{currentStatus} = '{HandlingStatus.Requested}'
END

DECLARE @{currentStatusAccepted} BIT
SELECT @{currentStatusAccepted} = 1
    FROM @{nameof(InputParamName.AcceptableCurrentStatusesXml)}.nodes('/{TagConversionTool.TagSetElementName}') AS T(C)
    WHERE C.value('({TagConversionTool.TagEntryElementName}/@{TagConversionTool.TagEntryValueAttributeName})[1]', '{Tables.Handling.Status.DataType.DeclarationInSqlSyntax}') = @{currentStatus}

IF (@{currentStatusAccepted} = 0)
BEGIN
      DECLARE @NotValidStatusErrorMessage nvarchar(max), 
              @NotValidStatusErrorSeverity int, 
              @NotValidStatusErrorState int

      SELECT @NotValidStatusErrorMessage = 'Invalid current status: ' + @{currentStatus} + ERROR_MESSAGE() + ' Line ' + cast(ERROR_LINE() as nvarchar(5)), @NotValidStatusErrorSeverity = ERROR_SEVERITY(), @NotValidStatusErrorState = ERROR_STATE()
      RAISERROR (@NotValidStatusErrorMessage, @NotValidStatusErrorSeverity, @NotValidStatusErrorState)
END

DECLARE @{resourceId} {Tables.Resource.Id.DataType.DeclarationInSqlSyntax}
EXEC [{streamName}].[{GetIdAddIfNecessaryResource.Name}] @{InputParamName.ResourceDetails}, @{resourceId} OUTPUT
 
BEGIN TRANSACTION [{transaction}]
  BEGIN TRY
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
	      , @{InputParamName.RecordId}
	      , @{resourceId}
	      , @{InputParamName.NewStatus}
	      , @{InputParamName.Details}
		  , GETUTCDATE()
		  )

	      SET @{OutputParamName.Id} = SCOPE_IDENTITY()
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
