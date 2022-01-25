// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlStream.StandardGetNextUniqueLongOp.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Protocol.Client
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using Naos.CodeAnalysis.Recipes;
    using Naos.Database.Domain;
    using Naos.SqlServer.Domain;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.DateTime.Recipes;
    using OBeautifulCode.Serialization;
    using OBeautifulCode.String.Recipes;
    using OBeautifulCode.Type.Recipes;
    using static System.FormattableString;

    public partial class SqlStream
    {
        /// <inheritdoc />
        public override long Execute(
            StandardGetNextUniqueLongOp operation)
        {
            operation.MustForArg(nameof(operation)).NotBeNull();

            var locator = this.ResourceLocatorProtocols.Execute(new GetResourceLocatorForUniqueIdentifierOp());
            var sqlServerLocator = locator as SqlServerLocator
                                ?? throw new NotSupportedException(Invariant($"{nameof(GetResourceLocatorForUniqueIdentifierOp)} should return a {nameof(SqlServerLocator)} and returned {locator?.GetType().ToStringReadable()}."));

            var storedProcOp = StreamSchema.Sprocs.GetNextUniqueLong.BuildExecuteStoredProcedureOp(this.Name, operation);

            var sqlProtocol = this.BuildSqlOperationsProtocol(sqlServerLocator);
            var sprocResult = sqlProtocol.Execute(storedProcOp);

            long result = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.GetNextUniqueLong.OutputParamName.Value)].GetValueOfType<long>();

            return result;
        }
    }
}
