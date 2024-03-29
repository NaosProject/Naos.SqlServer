﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamSchema.Sprocs.GetNextUniqueLong.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System.Collections.Generic;
    using Naos.Database.Domain;
    using static System.FormattableString;

    public static partial class StreamSchema
    {
        public static partial class Sprocs
        {
            /// <summary>
            /// Stored procedure: GetNextUniqueLong.
            /// </summary>
            public static class GetNextUniqueLong
            {
                /// <summary>
                /// Gets the name of the stored procedure.
                /// </summary>
                public static string Name => nameof(GetNextUniqueLong);

                /// <summary>
                /// Input parameter names.
                /// </summary>
                public enum InputParamName
                {
                    /// <summary>
                    /// The details about the long request.
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
                    Value,
                }

                /// <summary>
                /// Builds the execute stored procedure operation.
                /// </summary>
                /// <param name="streamName">Name of the stream.</param>
                /// <param name="operation">The operation to use as inputs.</param>
                /// <returns>Operation to execute stored procedure.</returns>
                public static ExecuteStoredProcedureOp BuildExecuteStoredProcedureOp(
                    string streamName,
                    StandardGetNextUniqueLongOp operation)
                {
                    var sprocName = Invariant($"[{streamName}].[{nameof(GetNextUniqueLong)}]");

                    var parameters = new List<ParameterDefinitionBase>()
                                     {
                                         new InputParameterDefinition<string>(nameof(InputParamName.Details), Tables.NextUniqueLong.Details.SqlDataType, operation.Details),
                                         new OutputParameterDefinition<long>(nameof(OutputParamName.Value), Tables.NextUniqueLong.Id.SqlDataType),
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
                    var transaction = Invariant($"{nameof(GetNextUniqueLong)}Tran");

                    var createOrModify = asAlter ? "CREATE OR ALTER" : "CREATE";
                    var result = Invariant(
                        $@"
{createOrModify} PROCEDURE [{streamName}].[{GetNextUniqueLong.Name}](
  @{InputParamName.Details} {Tables.NextUniqueLong.Details.SqlDataType.DeclarationInSqlSyntax}
, @{OutputParamName.Value} {Tables.NextUniqueLong.Id.SqlDataType.DeclarationInSqlSyntax} OUTPUT
)
AS
BEGIN

BEGIN TRANSACTION [{transaction}]
  BEGIN TRY
	  BEGIN
	      INSERT INTO [{streamName}].[{Tables.NextUniqueLong.Table.Name}] WITH (TABLOCKX) (
		      [{Tables.NextUniqueLong.Details.Name}]
		    , [{Tables.NextUniqueLong.RecordCreatedUtc.Name}]
		  ) VALUES (
              @{InputParamName.Details}
		    , GETUTCDATE()
		  )

	      SET @{OutputParamName.Value} = SCOPE_IDENTITY()
	  END

      COMMIT TRANSACTION [{transaction}]

  END TRY

  BEGIN CATCH
      SET @{OutputParamName.Value} = NULL
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
