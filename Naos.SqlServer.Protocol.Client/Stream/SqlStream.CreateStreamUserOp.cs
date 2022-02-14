// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlStream.CreateStreamUserOp.cs" company="Naos Project">
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
    using System.Threading.Tasks;
    using Naos.CodeAnalysis.Recipes;
    using Naos.Database.Domain;
    using Naos.SqlServer.Domain;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Collection.Recipes;
    using OBeautifulCode.DateTime.Recipes;
    using OBeautifulCode.Enum.Recipes;
    using OBeautifulCode.Serialization;
    using OBeautifulCode.String.Recipes;
    using static System.FormattableString;

    public partial class SqlStream
    {
        /// <inheritdoc />
        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "sprocResult", Justification = "Part of contract, could be a output parameter in future.")]
        public void Execute(
            CreateStreamUserOp operation)
        {
            operation.MustForArg(nameof(operation)).NotBeNull();

            var allLocators = this.ResourceLocatorProtocols.Execute(new GetAllResourceLocatorsOp());
            foreach (var resourceLocator in allLocators)
            {
                var sqlServerLocator = resourceLocator.ConfirmAndConvert<SqlServerLocator>();

                var roles = operation
                           .StreamAccessKinds
                           .GetIndividualFlags<StreamAccessKinds>()
                           .Select(_ => StreamSchema.GetRoleNameFromStreamAccessKind(_, this.Name))
                           .ToList();

                var rolesCsv = roles.ToCsv();
                var storedProcOp = StreamSchema.Sprocs.CreateStreamUser.BuildExecuteStoredProcedureOp(
                    this.Name,
                    operation.LoginName,
                    operation.UserName,
                    operation.ClearTextPassword,
                    rolesCsv,
                    operation.ShouldCreateLogin);

                var sqlProtocol = this.BuildSqlOperationsProtocol(sqlServerLocator);
                var sprocResult = sqlProtocol.Execute(storedProcOp);
            }
        }

        /// <inheritdoc />
        public async Task ExecuteAsync(
            CreateStreamUserOp operation)
        {
            this.Execute(operation);
            await Task.FromResult(true); // just for await
        }
    }
}
