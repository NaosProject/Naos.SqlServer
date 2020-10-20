// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamSchema.Sprocs.GetIdAddIfNecessaryTypeWithVersion.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Object table schema.
    /// </summary>
    public static partial class StreamSchema
    {
        /// <summary>
        /// Stored procedures.
        /// </summary>
        public static partial class Sprocs
        {
            /// <summary>
            /// Class TypeWithVersion.
            /// </summary>
            public static class GetIdAddIfNecessaryTypeWithVersion
            {
                /// <summary>
                /// Input parameter names.
                /// </summary>
                public enum InputParamNames
                {
                    /// <summary>
                    /// The object assembly qualified name With version.
                    /// </summary>
                    AssemblyQualifiedNameWithVersion,
                }

                /// <summary>
                /// Output parameter names.
                /// </summary>
                public enum OutputParamNames
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
                /// <param name="assemblyQualifiedNameWithVersion">The assembly qualified name.</param>
                /// <returns>ExecuteStoredProcedureOp.</returns>
                public static ExecuteStoredProcedureOp BuildExecuteStoredProcedureOp(
                    string streamName,
                    string assemblyQualifiedNameWithVersion)
                {
                    var sprocName = FormattableString.Invariant($"[{streamName}].{nameof(GetIdAddIfNecessaryTypeWithVersion)}");

                    var parameters = new List<SqlParameterRepresentationBase>()
                                     {
                                         new SqlInputParameterRepresentation<string>(
                                             nameof(InputParamNames.AssemblyQualifiedNameWithVersion),
                                             Tables.TypeWithVersion.AssemblyQualifiedName.DataType,
                                             assemblyQualifiedNameWithVersion),
                                         new SqlOutputParameterRepresentation<int>(nameof(OutputParamNames.Id), Tables.Object.Id.DataType),
                                     };

                    var parameterNameToDetailsMap = parameters.ToDictionary(k => k.Name, v => v);

                    var result = new ExecuteStoredProcedureOp(sprocName, parameterNameToDetailsMap);

                    return result;
                }

                /// <summary>
                /// Builds the name of the put stored procedure.
                /// </summary>
                /// <param name="streamName">Name of the stream.</param>
                /// <returns>Name of the put stored procedure.</returns>
                public static string BuildCreationScript(
                    string streamName)
                {
                    return FormattableString.Invariant(
                        $@"
CREATE PROCEDURE [{streamName}].{nameof(GetIdAddIfNecessaryTypeWithVersion)}(
  @{nameof(InputParamNames.AssemblyQualifiedNameWithVersion)} {Tables.TypeWithVersion.AssemblyQualifiedName.DataType.DeclarationInSqlSyntax},
  @{nameof(OutputParamNames.Id)} {Tables.TypeWithVersion.Id.DataType.DeclarationInSqlSyntax} OUTPUT
  )
AS
BEGIN

BEGIN TRANSACTION [GetIdAddTypeWithVersion]
  BEGIN TRY
      SELECT @{nameof(OutputParamNames.Id)} = [{nameof(Tables.TypeWithVersion.Id)}] FROM [{streamName}].[{nameof(Tables.TypeWithVersion)}]
        WHERE [{nameof(Tables.TypeWithVersion.AssemblyQualifiedName)}] = @{nameof(InputParamNames.AssemblyQualifiedNameWithVersion)}

	  IF (@{nameof(OutputParamNames.Id)} IS NULL)
	  BEGIN
	      
	      INSERT INTO [{streamName}].[{nameof(Tables.TypeWithVersion)}] ([{nameof(Tables.TypeWithVersion.AssemblyQualifiedName)}], [{nameof(Tables.TypeWithVersion.RecordCreatedUtc)}]) VALUES (@{nameof(InputParamNames.AssemblyQualifiedNameWithVersion)}, GETUTCDATE())
		  SET @{nameof(OutputParamNames.Id)} = SCOPE_IDENTITY()
	  END

      COMMIT TRANSACTION [GetIdAddTypeWithVersion]

  END TRY
  BEGIN CATCH
      SET @{nameof(OutputParamNames.Id)} = NULL
      DECLARE @ErrorMessage nvarchar(max), 
              @ErrorSeverity int, 
              @ErrorState int

      SELECT @ErrorMessage = ERROR_MESSAGE() + ' Line ' + cast(ERROR_LINE() as nvarchar(5)), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE()

      IF (@@trancount > 0)
      BEGIN
         ROLLBACK TRANSACTION [GetIdAddTypeWithVersion]
      END
    RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
  END CATCH
END");
                }
            }
        }
    }
}