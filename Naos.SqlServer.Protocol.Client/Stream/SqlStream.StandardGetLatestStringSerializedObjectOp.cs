// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlStream.StandardGetLatestStringSerializedObjectOp.cs" company="Naos Project">
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
    using static System.FormattableString;

    public partial class SqlStream
    {
        /// <inheritdoc />
        public override string Execute(
            StandardGetLatestStringSerializedObjectOp operation)
        {
            operation.MustForArg(nameof(operation)).NotBeNull();

            var sqlServerLocator = this.TryGetLocator(operation);

            var convertedRecordFilter = this.ConvertRecordFilter(operation.RecordFilter, sqlServerLocator);

            var storedProcOp = StreamSchema.Sprocs.GetLatestStringSerializedObject.BuildExecuteStoredProcedureOp(
                this.Name,
                convertedRecordFilter);

            var sqlProtocol = this.BuildSqlOperationsProtocol(sqlServerLocator);
            var sprocResult = sqlProtocol.Execute(storedProcOp);

            var result = sprocResult
                              .OutputParameters[nameof(StreamSchema.Sprocs.GetLatestStringSerializedObject.OutputParamName.StringSerializedObject)]
                              .GetValueOfType<string>();

            return result;
        }
    }
}
