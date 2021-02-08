// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateStreamStoredProceduresOp.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Naos.CodeAnalysis.Recipes;
    using Naos.Database.Domain;
    using Naos.Protocol.Domain;
    using Naos.SqlServer.Domain;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Collection.Recipes;
    using OBeautifulCode.Representation.System;
    using OBeautifulCode.Serialization;
    using OBeautifulCode.Type;
    using static System.FormattableString;

    /// <summary>
    /// Alters the stored procedures of the stream to the schema version being executed.
    /// </summary>
    public partial class UpdateStreamStoredProceduresOp : ReturningOperationBase<UpdateStreamStoredProceduresResult>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateStreamStoredProceduresOp"/> class.
        /// </summary>
        public UpdateStreamStoredProceduresOp()
        {
        }
    }
}