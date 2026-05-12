// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidateSqlScriptProtocol.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Protocol.Validation
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Microsoft.SqlServer.TransactSql.ScriptDom;
    using Naos.SqlServer.Domain;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Type;
    using OBeautifulCode.Type.Recipes;
    using static System.FormattableString;

    /// <summary>
    /// Protocol to execute a <see cref="ValidateSqlScriptOp" />.
    /// </summary>
    public class ValidateSqlScriptProtocol : SyncSpecificReturningProtocolBase<ValidateSqlScriptOp, SqlScriptValidationResult>
    {
        /// <inheritdoc />
        public override SqlScriptValidationResult Execute(
            ValidateSqlScriptOp operation)
        {
            operation.MustForArg(nameof(operation)).NotBeNull();

            var sqlVersion = ToSqlVersion(operation.TargetSqlServerVersion);

            var parser = TSqlParser.CreateParser(sqlVersion, initialQuotedIdentifiers: true);

            TSqlFragment fragment;
            IList<ParseError> parseErrors;

            using (var reader = new StringReader(operation.Sql))
            {
                fragment = parser.Parse(reader, out parseErrors);
            }

            var sqlScriptParsingErrors = ToSqlScriptParsingErrors(parseErrors);

            SqlScriptValidationResult result;

            if (sqlScriptParsingErrors == null)
            {
                var ruleViolations = EvaluateRules(fragment, operation.Rules);

                result = new SqlScriptValidationResult(
                    operation.TargetSqlServerVersion,
                    null,
                    ruleViolations);
            }
            else
            {
                result = new SqlScriptValidationResult(
                    operation.TargetSqlServerVersion,
                    sqlScriptParsingErrors,
                    null);
            }

            return result;
        }

        private static SqlVersion ToSqlVersion(
            SqlServerVersion sqlServerVersion)
        {
            switch (sqlServerVersion)
            {
                case SqlServerVersion.SqlServer2000:
                    return SqlVersion.Sql80;
                case SqlServerVersion.SqlServer2005:
                    return SqlVersion.Sql90;
                case SqlServerVersion.SqlServer2008:
                    return SqlVersion.Sql100;
                case SqlServerVersion.SqlServer2012:
                    return SqlVersion.Sql110;
                case SqlServerVersion.SqlServer2014:
                    return SqlVersion.Sql120;
                case SqlServerVersion.SqlServer2016:
                    return SqlVersion.Sql130;
                case SqlServerVersion.SqlServer2017:
                    return SqlVersion.Sql140;
                case SqlServerVersion.SqlServer2019:
                    return SqlVersion.Sql150;
                case SqlServerVersion.SqlServer2022:
                    return SqlVersion.Sql160;
                default:
                    throw new NotSupportedException(Invariant($"This {nameof(SqlServerVersion)} is not supported: {sqlServerVersion}."));
            }
        }

        private static IReadOnlyList<SqlScriptParsingError> ToSqlScriptParsingErrors(
            IList<ParseError> parseErrors)
        {
            List<SqlScriptParsingError> result = null;

            if ((parseErrors != null) && (parseErrors.Count > 0))
            {
                result = new List<SqlScriptParsingError>(parseErrors.Count);

                foreach (var parseError in parseErrors)
                {
                    var parsingError = new SqlScriptParsingError(
                        parseError.Offset,
                        parseError.Message);

                    result.Add(parsingError);
                }
            }

            return result;
        }

        private static IReadOnlyList<SqlScriptValidationRuleViolation> EvaluateRules(
            TSqlFragment root,
            IReadOnlyList<SqlScriptValidationRuleBase> rules)
        {
            List<SqlScriptValidationRuleViolation> result = null;

            foreach (var rule in rules)
            {
                SqlScriptValidationRuleEvaluatorBase ruleEvaluator;

                if (rule is DisallowedSchemasSqlScriptValidationRule disallowedSchemasSqlScriptValidationRule)
                {
                    ruleEvaluator = new DisallowedSchemasSqlScriptValidationRuleEvaluator(disallowedSchemasSqlScriptValidationRule);
                }
                else if (rule is DisallowSystemSchemasSqlScriptValidationRule disallowSystemSchemasSqlScriptValidationRule)
                {
                    ruleEvaluator = new DisallowSystemSchemasSqlScriptValidationRuleEvaluator(disallowSystemSchemasSqlScriptValidationRule);
                }
                else if (rule is SanctionedSchemasSqlScriptValidationRule sanctionedSchemasSqlScriptValidationRule)
                {
                    ruleEvaluator = new SanctionedSchemasSqlScriptValidationRuleEvaluator(sanctionedSchemasSqlScriptValidationRule);
                }
                else if (rule is SingleSchemaSqlScriptValidationRule singleSchemaSqlScriptValidationRule)
                {
                    ruleEvaluator = new SingleSchemaSqlScriptValidationRuleEvaluator(singleSchemaSqlScriptValidationRule);
                }
                else if (rule is SchemaQualifiedTableReferencesSqlScriptValidationRule schemaQualifiedTableReferencesSqlScriptValidationRule)
                {
                    ruleEvaluator = new SchemaQualifiedTableReferencesSqlScriptValidationRuleEvaluator(schemaQualifiedTableReferencesSqlScriptValidationRule);
                }
                else if (rule is SingleStatementSqlScriptValidationRule singleStatementSqlScriptValidationRule)
                {
                    ruleEvaluator = new SingleStatementSqlScriptValidationRuleEvaluator(singleStatementSqlScriptValidationRule);
                }
                else if (rule is ReadOnlySelectSqlScriptValidationRule readOnlySelectSqlScriptValidationRule)
                {
                    ruleEvaluator = new ReadOnlySelectSqlScriptValidationRuleEvaluator(readOnlySelectSqlScriptValidationRule);
                }
                else if (rule is DisallowAdHocDistributedQueriesSqlScriptValidationRule disallowAdHocDistributedQueriesSqlScriptValidationRule)
                {
                    ruleEvaluator = new DisallowAdHocDistributedQueriesSqlScriptValidationRuleEvaluator(disallowAdHocDistributedQueriesSqlScriptValidationRule);
                }
                else if (rule is FlatQuerySqlScriptValidationRule flatQuerySqlScriptValidationRule)
                {
                    ruleEvaluator = new FlatQuerySqlScriptValidationRuleEvaluator(flatQuerySqlScriptValidationRule);
                }
                else
                {
                    throw new NotSupportedException(Invariant($"This type of {nameof(SqlScriptValidationRuleBase)} is not supported: {rule.GetType().ToStringReadable()}."));
                }

                root.Accept(ruleEvaluator);

                var violations = ruleEvaluator.GetViolations();

                if ((violations != null) && violations.Any())
                {
                    if (result == null)
                    {
                        result = new List<SqlScriptValidationRuleViolation>();
                    }

                    result.AddRange(violations);
                }
            }

            return result;
        }
    }
}