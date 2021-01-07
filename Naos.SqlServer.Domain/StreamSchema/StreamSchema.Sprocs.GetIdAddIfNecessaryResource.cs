// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamSchema.Sprocs.GetIdAddIfNecessaryResource.cs" company="Naos Project">
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
            /// Stored procedure: GetIdAddIfNecessaryResource.
            /// </summary>
            public static class GetIdAddIfNecessaryResource
            {
                /// <summary>
                /// Gets the name of the stored procedure.
                /// </summary>
                /// <value>The name of the stored procedure.</value>
                public static string Name => nameof(GetIdAddIfNecessaryResource);

                /// <summary>
                /// Input parameter names.
                /// </summary>
                public enum InputParamName
                {
                    /// <summary>
                    /// The details about the resource.
                    /// </summary>
                    Details,
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
                /// <param name="details">Details about the resource.</param>
                /// <returns>ExecuteStoredProcedureOp.</returns>
                public static ExecuteStoredProcedureOp BuildExecuteStoredProcedureOp(
                    string streamName,
                    string details)
                {
                    var sprocName = FormattableString.Invariant($"[{streamName}].{nameof(GetIdAddIfNecessaryResource)}");

                    var parameters = new List<SqlParameterRepresentationBase>()
                                     {
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamName.Details), Tables.Resource.Details.DataType, details),
                                         new SqlOutputParameterRepresentation<int>(nameof(OutputParamName.Id), Tables.Resource.Id.DataType),
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
CREATE PROCEDURE [{streamName}].{GetIdAddIfNecessaryResource.Name}(
  @{InputParamName.Details} AS {Tables.TypeWithoutVersion.AssemblyQualifiedName.DataType.DeclarationInSqlSyntax}
, @{OutputParamName.Id} {Tables.Resource.Id.DataType.DeclarationInSqlSyntax} OUTPUT
)
AS
BEGIN

BEGIN TRANSACTION [GetIdAddResource]
  BEGIN TRY
      SELECT @{OutputParamName.Id} = [{Tables.Resource.Id.Name}] FROM [{streamName}].[{Tables.Resource.Table.Name}]
        WHERE [{Tables.Resource.Details.Name}] = @{InputParamName.Details}

	  IF (@{nameof(OutputParamName.Id)} IS NULL)
	  BEGIN
	      INSERT INTO [{streamName}].[{Tables.Resource.Table.Name}] (
		    [{Tables.Resource.Details.Name}]
		  , [{Tables.Resource.RecordCreatedUtc.Name}]
		  ) VALUES (
	        @{InputParamName.Details}
		  , GETUTCDATE()
		  )

	      SET @{OutputParamName.Id} = SCOPE_IDENTITY()
	  END

      COMMIT TRANSACTION [GetIdAddResource]

  END TRY

  BEGIN CATCH
      SET @{OutputParamName.Id} = NULL
      DECLARE @ErrorMessage nvarchar(max), 
              @ErrorSeverity int, 
              @ErrorState int

      SELECT @ErrorMessage = ERROR_MESSAGE() + ' Line ' + cast(ERROR_LINE() as nvarchar(5)), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE()

      IF (@@trancount > 0)
      BEGIN
         ROLLBACK TRANSACTION [GetIdAddResource]
      END
    RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
  END CATCH
END");
                }
            }
        }
    }
}
