// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamSchema.Sprocs.CreateStreamUser.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System.Collections.Generic;
    using static System.FormattableString;

    public static partial class StreamSchema
    {
        public static partial class Sprocs
        {
            /// <summary>
            /// Stored procedure: CreateStreamUser.
            /// </summary>
            public static class CreateStreamUser
            {
                /// <summary>
                /// Gets the name.
                /// </summary>
                public static string Name => nameof(CreateStreamUser);

                /// <summary>
                /// Input parameter names.
                /// </summary>
                public enum InputParamName
                {
                    /// <summary>
                    /// The login name.
                    /// </summary>
                    LoginName,

                    /// <summary>
                    /// The username.
                    /// </summary>
                    Username,

                    /// <summary>
                    /// The password.
                    /// </summary>
                    ClearTextPassword,

                    /// <summary>
                    /// The roles as CSV.
                    /// </summary>
                    RoleCsv,

                    /// <summary>
                    /// The flag to indicate whether or not to create the login.
                    /// </summary>
                    ShouldCreateLogin,
                }

                /// <summary>
                /// Builds the execute stored procedure operation.
                /// </summary>
                /// <param name="streamName">Name of the stream.</param>
                /// <param name="loginName">The login name.</param>
                /// <param name="username">The username.</param>
                /// <param name="clearTextPassword">The password.</param>
                /// <param name="roles">The roles as CSV.</param>
                /// <param name="shouldCreateLogin">Indicates whether or not to create the login or look it up (must specify <paramref name="loginName"/> if this is TRUE).</param>
                /// <returns>Operation to execute stored procedure.</returns>
                public static ExecuteStoredProcedureOp BuildExecuteStoredProcedureOp(
                    string streamName,
                    string loginName,
                    string username,
                    string clearTextPassword,
                    string roles,
                    bool shouldCreateLogin)
                {
                    var sprocName = Invariant($"[{streamName}].{nameof(CreateStreamUser)}");

                    var parameters = new List<ParameterDefinitionBase>()
                                     {
                                         new InputParameterDefinition<string>(nameof(InputParamName.LoginName), new StringSqlDataTypeRepresentation(true, 128), loginName ?? username),
                                         new InputParameterDefinition<string>(nameof(InputParamName.Username), new StringSqlDataTypeRepresentation(true, 128), username),
                                         new InputParameterDefinition<string>(nameof(InputParamName.ClearTextPassword), new StringSqlDataTypeRepresentation(true, 128), clearTextPassword),
                                         new InputParameterDefinition<string>(nameof(InputParamName.RoleCsv), new StringSqlDataTypeRepresentation(true, StringSqlDataTypeRepresentation.MaxUnicodeLengthConstant), roles),
                                         new InputParameterDefinition<int>(nameof(InputParamName.ShouldCreateLogin), new IntSqlDataTypeRepresentation(), shouldCreateLogin ? 1 : 0),
                                     };

                    var result = new ExecuteStoredProcedureOp(sprocName, parameters);

                    return result;
                }

                /// <summary>
                /// Builds the creation script for put stored procedure.
                /// </summary>
                /// <param name="streamName">Name of the stream.</param>
                /// <param name="asAlter">An optional value indicating whether or not to generate a ALTER versus CREATE; DEFAULT is false and will generate a CREATE script.</param>
                /// <returns>Creation script for creating the stored procedure.</returns>
                public static string BuildCreationScript(
                    string streamName,
                    bool asAlter = false)
                {
                    var createOrModify = asAlter ? "CREATE OR ALTER" : "CREATE";
                    var result = Invariant(
                        $@"
{createOrModify} PROCEDURE [{streamName}].[{Name}](
  @{InputParamName.LoginName} AS {new StringSqlDataTypeRepresentation(true, 128).DeclarationInSqlSyntax}
, @{InputParamName.Username} AS {new StringSqlDataTypeRepresentation(true, 128).DeclarationInSqlSyntax}
, @{InputParamName.ClearTextPassword} AS {new StringSqlDataTypeRepresentation(true, 128).DeclarationInSqlSyntax}
, @{InputParamName.RoleCsv} AS {new StringSqlDataTypeRepresentation(true, StringSqlDataTypeRepresentation.MaxUnicodeLengthConstant).DeclarationInSqlSyntax}
, @{InputParamName.ShouldCreateLogin} AS {new IntSqlDataTypeRepresentation().DeclarationInSqlSyntax}
)
AS
BEGIN
    DECLARE @quotedLogin {new StringSqlDataTypeRepresentation(true, 128).DeclarationInSqlSyntax}
	SET @quotedLogin = QUOTENAME(@{InputParamName.LoginName})
    DECLARE @quotedUsername {new StringSqlDataTypeRepresentation(true, 128).DeclarationInSqlSyntax}
	SET @quotedUsername = QUOTENAME(@{InputParamName.Username})
	DECLARE @db {new StringSqlDataTypeRepresentation(true, 128).DeclarationInSqlSyntax}
	SET @db = QUOTENAME(DB_NAME())

    IF (@{InputParamName.ShouldCreateLogin} = 1)
    BEGIN
       DECLARE @quotedPassword {new StringSqlDataTypeRepresentation(true, 128).DeclarationInSqlSyntax}
	   SET @quotedPassword = QUOTENAME(@{InputParamName.ClearTextPassword}, '''')
       EXEC('CREATE LOGIN ' + @quotedLogin + ' WITH PASSWORD = ' + @quotedPassword + ', DEFAULT_DATABASE = ' + @db)
    END

    EXEC('CREATE USER ' + @quotedUsername + ' FOR LOGIN ' + @quotedLogin + ' WITH DEFAULT_SCHEMA=[{streamName}]')

    -- Iterate over all roles
    DECLARE @role {new StringSqlDataTypeRepresentation(false, StringSqlDataTypeRepresentation.MaxNonUnicodeLengthConstant).DeclarationInSqlSyntax}
    DECLARE cur CURSOR LOCAL FOR
    SELECT value
    FROM STRING_SPLIT(@{InputParamName.RoleCsv}, ',')
    OPEN cur
    FETCH NEXT FROM cur INTO @role

    WHILE @@FETCH_STATUS = 0
    BEGIN
	    DECLARE @quotedRole {new StringSqlDataTypeRepresentation(true, 128).DeclarationInSqlSyntax}
	    SET @quotedRole = QUOTENAME(@role)
        EXEC('EXEC sp_addrolemember @rolename =' + @quotedRole + ', @membername = ' + @quotedUsername)
        FETCH NEXT FROM cur INTO @role
    END

    CLOSE cur
    DEALLOCATE cur
END");

                    return result;
                }
            }
        }
    }
}
