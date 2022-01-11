// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlStream.StandardGetDistinctStringSerializedIdsOp.cs" company="Naos Project">
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
    using OBeautifulCode.Collection.Recipes;
    using OBeautifulCode.DateTime.Recipes;
    using OBeautifulCode.Serialization;
    using OBeautifulCode.String.Recipes;
    using OBeautifulCode.Type.Recipes;
    using static System.FormattableString;

    public partial class SqlStream
    {
        /// <inheritdoc />
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "internalRecordId", Justification = "Leaving for debugging and future use.")]
        public override IReadOnlyCollection<string> Execute(
            StandardGetDistinctStringSerializedIdsOp operation)
        {
            operation.MustForArg(nameof(operation)).NotBeNull();

            var locator = this.ResourceLocatorProtocols.Execute(new GetResourceLocatorForUniqueIdentifierOp());
            var sqlServerLocator = locator as SqlServerLocator
                                ?? throw new NotSupportedException(Invariant($"{nameof(GetResourceLocatorForUniqueIdentifierOp)} should return a {nameof(SqlServerLocator)} and returned {locator?.GetType().ToStringReadable()}."));

            var deprecatedIdTypeId = operation.DeprecatedIdentifierType == null ? null : this.GetIdsAddIfNecessaryType(sqlServerLocator, operation.DeprecatedIdentifierType.ToWithAndWithoutVersion());
            var identifierTypeId = operation.IdentifierType == null ? null : this.GetIdsAddIfNecessaryType(sqlServerLocator, operation.IdentifierType.ToWithAndWithoutVersion());
            var objectTypeId = operation.ObjectType == null ? null : this.GetIdsAddIfNecessaryType(sqlServerLocator, operation.ObjectType.ToWithAndWithoutVersion());
            var tagIdsCsv = operation.TagsToMatch == null
                ? null
                : this.GetIdsAddIfNecessaryTag(sqlServerLocator, operation.TagsToMatch).Select(_ => _.ToStringInvariantPreferred()).ToCsv();

            var storedProcOp = StreamSchema.Sprocs.GetDistinctStringSerializedIds.BuildExecuteStoredProcedureOp(
                this.Name,
                tagIdsCsv,
                deprecatedIdTypeId,
                identifierTypeId,
                objectTypeId,
                operation.VersionMatchStrategy);

            var sqlProtocol = this.BuildSqlOperationsProtocol(sqlServerLocator);
            var sprocResult = sqlProtocol.Execute(storedProcOp);

            var resultXml = sprocResult.OutputParameters[nameof(StreamSchema.Sprocs.GetDistinctStringSerializedIds.OutputParamName.StringIdentifiersXml)].GetValueOfType<string>();
            var resultList = TagConversionTool.GetTagsFromXmlString(resultXml);
            var result = resultList.Select(_ => _.Value).ToList();

            return result;
        }
    }
}
