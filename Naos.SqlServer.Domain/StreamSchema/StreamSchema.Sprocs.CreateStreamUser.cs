﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamSchema.Sprocs.CreateStreamUser.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Naos.Database.Domain;
    using Naos.Protocol.Domain;
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
            /// Stored procedure: CreateStreamUser.
            /// </summary>
            public static class CreateStreamUser
            {
                /// <summary>
                /// Gets the name.
                /// </summary>
                /// <value>The name.</value>
                public static string Name => nameof(CreateStreamUser);

                /// <summary>
                /// Input parameter names.
                /// </summary>
                public enum InputParamName
                {
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
                }

                /// <summary>
                /// Builds the execute stored procedure operation.
                /// </summary>
                /// <param name="streamName">Name of the stream.</param>
                /// <param name="username">The username.</param>
                /// <param name="clearTextPassword">The password.</param>
                /// <param name="roles">The roles as CSV.</param>
                /// <returns>Operation to execute stored procedure.</returns>
                public static ExecuteStoredProcedureOp BuildExecuteStoredProcedureOp(
                    string streamName,
                    string username,
                    string clearTextPassword,
                    string roles)
                {
                    var sprocName = Invariant($"[{streamName}].{nameof(CreateStreamUser)}");

                    var parameters = new List<SqlParameterRepresentationBase>()
                                     {
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamName.Username), new StringSqlDataTypeRepresentation(true, 128), username),
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamName.ClearTextPassword), new StringSqlDataTypeRepresentation(true, 128), clearTextPassword),
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamName.RoleCsv), new StringSqlDataTypeRepresentation(true, StringSqlDataTypeRepresentation.MaxLengthConstant), roles),
                                     };

                    var parameterNameToDetailsMap = parameters.ToDictionary(k => k.Name, v => v);

                    var result = new ExecuteStoredProcedureOp(sprocName, parameterNameToDetailsMap);

                    return result;
                }

                /// <summary>
                /// Builds the creation script for put stored procedure.
                /// </summary>
                /// <param name="streamName">Name of the stream.</param>
                /// <param name="asAlter">An optional value indicating whether or not to generate a ALTER versus CREATE; DEFAULT is false and will generate a CREATE script.</param>
                /// <returns>System.String.</returns>
                public static string BuildCreationScript(
                    string streamName,
                    bool asAlter = false)
                {
                    var createOrModify = asAlter ? "ALTER" : "CREATE";
                    var result = Invariant(
                        $@"
{createOrModify} PROCEDURE [{streamName}].[{CreateStreamUser.Name}](
  @{InputParamName.Username} AS {new StringSqlDataTypeRepresentation(true, 128).DeclarationInSqlSyntax}
, @{InputParamName.ClearTextPassword} AS {new StringSqlDataTypeRepresentation(true, 128).DeclarationInSqlSyntax}
, @{InputParamName.RoleCsv} AS {new StringSqlDataTypeRepresentation(true, StringSqlDataTypeRepresentation.MaxLengthConstant).DeclarationInSqlSyntax}
)
AS
BEGIN
    DECLARE @quotedLogin {new StringSqlDataTypeRepresentation(true, 128).DeclarationInSqlSyntax}
	SET @quotedLogin = QUOTENAME('{streamName}-' + @{InputParamName.Username})
    DECLARE @quotedUsername {new StringSqlDataTypeRepresentation(true, 128).DeclarationInSqlSyntax}
	SET @quotedUsername = QUOTENAME('{streamName}-' + @{InputParamName.Username})
    DECLARE @quotedPassword {new StringSqlDataTypeRepresentation(true, 128).DeclarationInSqlSyntax}
	SET @quotedPassword = QUOTENAME(@{InputParamName.ClearTextPassword}, '''')
	DECLARE @db {new StringSqlDataTypeRepresentation(true, 128).DeclarationInSqlSyntax}
	SET @db = QUOTENAME(DB_NAME())

    EXEC('CREATE LOGIN ' + @quotedLogin + ' WITH PASSWORD = ' + @quotedPassword + ', DEFAULT_DATABASE = ' + @db)
    EXEC('CREATE USER ' + @quotedUsername + ' FOR LOGIN ' + @quotedLogin + ' WITH DEFAULT_SCHEMA=[{streamName}]')

    -- Iterate over all roles
    DECLARE @role {new StringSqlDataTypeRepresentation(false, StringSqlDataTypeRepresentation.MaxLengthConstant).DeclarationInSqlSyntax}
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
