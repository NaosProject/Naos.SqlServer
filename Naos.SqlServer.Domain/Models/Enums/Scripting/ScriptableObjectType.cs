// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScriptableObjectType.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    /// <summary>
    /// Enumeration of scriptable object types.
    /// </summary>
    public enum ScriptableObjectType
    {
        /// <summary>
        /// Invalid default state.
        /// </summary>
        Invalid,

        /// <summary>
        /// Database table.
        /// </summary>
        Table,

        /// <summary>
        /// Table foreign key.
        /// </summary>
        ForeignKey,

        /// <summary>
        /// Table or view index.
        /// </summary>
        Index,

        /// <summary>
        /// Database view.
        /// </summary>
        View,

        /// <summary>
        /// Stored procedure.
        /// </summary>
        StoredProcedure,

        /// <summary>
        /// User defined function.
        /// </summary>
        UserDefinedFunction,

        /// <summary>
        /// User defined data type.
        /// </summary>
        UserDefinedDataType,

        /// <summary>
        /// Database role.
        /// </summary>
        DatabaseRole,

        /// <summary>
        /// Database user.
        /// </summary>
        User,
    }
}
