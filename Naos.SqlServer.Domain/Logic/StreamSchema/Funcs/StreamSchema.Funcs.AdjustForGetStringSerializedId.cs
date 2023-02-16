// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamSchema.Funcs.AdjustForGetStringSerializedId.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using static System.FormattableString;

    /// <summary>
    /// Container for schema.
    /// </summary>
    public static partial class StreamSchema
    {
        public static partial class Funcs
        {
            /// <summary>
            /// Function: AdjustForGetStringSerializedId.
            /// </summary>
            public static class AdjustForGetStringSerializedId
            {
                /// <summary>
                /// Gets the name of the function.
                /// </summary>
                public static string Name => nameof(AdjustForGetStringSerializedId);

                /// <summary>
                /// Input parameter names.
                /// </summary>
                public enum InputParamName
                {
                    /// <summary>
                    /// The string serialized identifier.
                    /// </summary>
                    StringSerializedId,
                }

                /// <summary>
                /// Builds the creation script.
                /// </summary>
                /// <param name="streamName">Name of the stream.</param>
                /// <param name="asAlter">An optional value indicating whether or not to generate a ALTER versus CREATE; DEFAULT is false and will generate a CREATE script.</param>
                /// <returns>The creation script to create the func.</returns>
                public static string BuildCreationScript(
                    string streamName,
                    bool asAlter = false)
                {
                    var createOrModify = asAlter ? "CREATE OR ALTER" : "CREATE";
                    var result = Invariant(
                        $@"
{createOrModify} FUNCTION [{streamName}].[{Name}] (
      @{InputParamName.StringSerializedId} {Tables.Record.StringSerializedId.SqlDataType.DeclarationInSqlSyntax}
)
RETURNS {Tables.Record.StringSerializedId.SqlDataType.DeclarationInSqlSyntax}
AS
BEGIN
    RETURN
        CASE
            WHEN @{InputParamName.StringSerializedId} = '{Tables.Record.NullStringSerializedId}' THEN NULL
            ELSE @{InputParamName.StringSerializedId}
        END
END
");

                    return result;
                }
            }
        }
    }
}