// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlDatabaseManagerTests.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Protocol.Client.Test
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Naos.Database.Domain;
    using Naos.Protocol.Domain;
    using Naos.Protocol.Serialization.Json;
    using Naos.SqlServer.Domain;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Representation.System;
    using OBeautifulCode.Serialization;
    using OBeautifulCode.Serialization.Json;
    using OBeautifulCode.Type;
    using Xunit;
    using Xunit.Abstractions;

    /// <summary>
    /// Tests for <see cref="SqlStream{TKey}"/>.
    /// </summary>
    public partial class SqlDatabaseManagerTests
    {
        private readonly ITestOutputHelper testOutputHelper;

        public SqlDatabaseManagerTests(
            ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        [Fact(Skip = "Local testing only.")]
        public void Method___Should_do_something___When_called()
        {
            var sqlServerLocator = new SqlServerLocator("localhost", "Streams", "sa", "password", "SQLDEV2017");
            var connectionString = sqlServerLocator.BuildConnectionString(TimeSpan.FromSeconds(100));
            var output = SqlServerDatabaseManager.GetTableDescription(connectionString, "Streams", "Object", "StreamName1");
            output.MustForTest().NotBeNull();
        }
    }
}