// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlStreamTest.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Protocol.Management.Test
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using Naos.Database.Domain;
    using Naos.SqlServer.Domain;
    using Naos.SqlServer.Protocol.Client;
    using Naos.SqlServer.Protocol.Management;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Representation.System;
    using OBeautifulCode.Serialization;
    using OBeautifulCode.Serialization.Json;
    using Xunit;
    using Xunit.Abstractions;
    using static System.FormattableString;

    /// <summary>
    /// Tests for <see cref="SqlStream"/>.
    /// </summary>
    public partial class SqlStreamTest
    {
        private readonly ITestOutputHelper testOutputHelper;

        public SqlStreamTest(
            ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        [Fact (Skip = "Local testing only")]
        public void CreateStreamExampleDatabaseAndScripts()
        {
            var sqlServerLocator = new SqlServerLocator("localhost", "StreamExample3", "sa", "<password>", "SQLDEV2017");
            var configuration = DatabaseConfiguration.BuildDatabaseConfigurationUsingDefaultsAsNecessary(sqlServerLocator.DatabaseName, @"D:\SQL\");
            var createDatabaseOp = new CreateDatabaseOp(configuration);
            var protocol = new SqlOperationsProtocol(sqlServerLocator);
            protocol.Execute(createDatabaseOp);

            var resourceLocatorProtocol = new SingleResourceLocatorProtocol(sqlServerLocator);

            var configurationTypeRepresentation = typeof(NullJsonSerializationConfiguration).ToRepresentation();

            SerializerRepresentation defaultSerializerRepresentation = new SerializerRepresentation(
                SerializationKind.Json,
                configurationTypeRepresentation);

            var defaultSerializationFormat = SerializationFormat.String;
            var stream = new SqlStream(
                "Example",
                TimeSpan.FromMinutes(1),
                TimeSpan.FromMinutes(3),
                defaultSerializerRepresentation,
                defaultSerializationFormat,
                new JsonSerializerFactory(),
                resourceLocatorProtocol);

            stream.Execute(new CreateStreamOp(stream.StreamRepresentation, ExistingStreamEncounteredStrategy.Skip));
            
            var path = "D:/Temp/Example";
            var connectionString = sqlServerLocator.BuildConnectionString(TimeSpan.FromSeconds(20));
//            Scripter.ScriptDatabaseToFilePath(connectionString, path, this.testOutputHelper.WriteLine, false);
        }
    }
}