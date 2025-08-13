// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlServerStandardStreamTest.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Protocol.Management.Test
{
    using System;
    using Naos.Database.Domain;
    using Naos.SqlServer.Domain;
    using Naos.SqlServer.Protocol.Client;
    using OBeautifulCode.Representation.System;
    using OBeautifulCode.Serialization;
    using OBeautifulCode.Serialization.Json;
    using Xunit;
    using Xunit.Abstractions;

    public partial class SqlServerStandardStreamTest
    {
        private readonly ITestOutputHelper testOutputHelper;

        public SqlServerStandardStreamTest(
            ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        [Fact(Skip = "Local testing only")]
        public void CreateStreamExampleDatabaseAndScripts()
        {
            var sqlServerLocator = new SqlServerLocator("localhost", "StreamExample3", "sa", "<password>", "SQLDEV2017");
            var configuration = SqlServerDatabaseDefinition.BuildDatabaseConfigurationUsingDefaultsAsNecessary(sqlServerLocator.DatabaseName, @"D:\SQL\");
            var createDatabaseOp = new CreateDatabaseOp(configuration, ExistingDatabaseStrategy.Throw);
            var protocol = new SqlOperationsProtocol(sqlServerLocator);
            protocol.Execute(createDatabaseOp);

            var resourceLocatorProtocol = new SingleResourceLocatorProtocols(sqlServerLocator);

            var configurationTypeRepresentation = typeof(NullJsonSerializationConfiguration).ToRepresentation();

            var defaultSerializerRepresentation = new SerializerRepresentation(
                SerializationKind.Json,
                configurationTypeRepresentation);

            var defaultSerializationFormat = SerializationFormat.String;
            var stream = new SqlServerStandardStream(
                "Example",
                TimeSpan.FromMinutes(1),
                TimeSpan.FromMinutes(3),
                defaultSerializerRepresentation,
                defaultSerializationFormat,
                new JsonSerializerFactory(),
                resourceLocatorProtocol);

            stream.Execute(new StandardCreateStreamOp(stream.StreamRepresentation, ExistingStreamStrategy.Skip));

            this.testOutputHelper.WriteLine("Created database.");
            /*
            var path = "D:/Temp/Example";
            var connectionString = sqlServerLocator.BuildConnectionString(TimeSpan.FromSeconds(20));
            Scripter.ScriptDatabaseToFilePath(connectionString, path, this.testOutputHelper.WriteLine, false);
            */
        }
    }
}