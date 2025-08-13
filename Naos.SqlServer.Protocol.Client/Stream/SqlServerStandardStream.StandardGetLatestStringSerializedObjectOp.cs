// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlServerStandardStream.StandardGetLatestStringSerializedObjectOp.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Protocol.Client
{
    using Naos.Database.Domain;
    using Naos.SqlServer.Domain;
    using OBeautifulCode.Assertion.Recipes;

    public partial class SqlServerStandardStream
    {
        /// <inheritdoc />
        public override string Execute(
            StandardGetLatestStringSerializedObjectOp operation)
        {
            operation.MustForArg(nameof(operation)).NotBeNull();

            var sqlServerLocator = this.TryGetLocator(operation);

            var convertedRecordFilter = this.ConvertRecordFilter(
                operation.RecordFilter,
                null,
                sqlServerLocator);

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
