// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlStream.StandardGetHandlingHistoryOp.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Protocol.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Naos.CodeAnalysis.Recipes;
    using Naos.Database.Domain;
    using Naos.SqlServer.Domain;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Collection.Recipes;
    using OBeautifulCode.Enum.Recipes;
    using OBeautifulCode.String.Recipes;
    using OBeautifulCode.Type.Recipes;
    using static System.FormattableString;

    public partial class SqlStream
    {
        /// <inheritdoc />
        public override IReadOnlyList<StreamRecordHandlingEntry> Execute(
            StandardGetHandlingHistoryOp operation)
        {
            operation.MustForArg(nameof(operation)).NotBeNull();

            throw new NotImplementedException();
        }
    }
}
