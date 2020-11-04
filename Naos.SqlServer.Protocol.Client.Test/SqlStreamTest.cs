// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlStreamTest.cs" company="Naos Project">
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
    public partial class SqlStreamTest
    {
        private readonly ITestOutputHelper testOutputHelper;

        public SqlStreamTest(
            ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        [Fact(Skip = "Local testing only.")]
        public void Method___Should_do_something___When_called()
        {
            var streamName = "StreamName32";

            var sqlServerLocator = new SqlServerLocator("localhost", "Streams", "sa", "password", "SQLDEV2017");
            var resourceLocatorProtocol = new SingleResourceLocatorProtocol<string>(sqlServerLocator);

            var configurationTypeRepresentation =
                typeof(DependencyOnlyJsonSerializationConfiguration<
                    TypesToRegisterJsonSerializationConfiguration<MyObject>,
                    ProtocolJsonSerializationConfiguration>).ToRepresentation();

            SerializerRepresentation defaultSerializerRepresentation = new SerializerRepresentation(
                SerializationKind.Json,
                configurationTypeRepresentation);

            var defaultSerializationFormat = SerializationFormat.String;

            var tagExtractor = new LambdaReturningProtocol<GetTagsFromObjectOp<MyObject>, IReadOnlyDictionary<string, string>>(
                _ => new Dictionary<string, string>
                     {
                         { nameof(MyObject.Field), _.ObjectToDetermineTagsFrom.Field },
                     });

            var stream = new SqlStream<string>(
                streamName,
                TimeSpan.FromMinutes(1),
                TimeSpan.FromMinutes(3),
                defaultSerializerRepresentation,
                defaultSerializationFormat,
                new JsonSerializerFactory(),
                resourceLocatorProtocol,
                new ProtocolFactory(new Dictionary<Type, Func<IProtocol>>()),
                new ProtocolFactory(
                    new Dictionary<Type, Func<IProtocol>>
                    {
                        { typeof(ISyncAndAsyncReturningProtocol<GetTagsFromObjectOp<MyObject>, IReadOnlyDictionary<string, string>>), () => tagExtractor },
                    }));

            stream.Execute(new CreateStreamOp<string>(stream.StreamRepresentation, ExistingStreamEncounteredStrategy.Skip));
            var key = stream.Name;
            var firstValue = "Testing again.";
            var secondValue = "Testing again latest.";
            stream.BuildPutProtocol<MyObject>().Execute(new PutOp<MyObject>(new MyObject(key, firstValue)));
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            stream.BuildPutProtocol<MyObject>().Execute(new PutOp<MyObject>(new MyObject(key, secondValue)));
            stopwatch.Stop();
            this.testOutputHelper.WriteLine(FormattableString.Invariant($"Put: {stopwatch.Elapsed.TotalMilliseconds} ms"));
            stopwatch.Reset();
            stopwatch.Start();
            var my = stream.BuildGetLatestByIdAndTypeProtocol<MyObject>().Execute(new GetLatestByIdAndTypeOp<string, MyObject>(key));
            this.testOutputHelper.WriteLine(FormattableString.Invariant($"Get: {stopwatch.Elapsed.TotalMilliseconds} ms"));
            // this.testOutputHelper.WriteLine(FormattableString.Invariant($"Get: {SqlStream<string>.Stopwatch.Elapsed.TotalMilliseconds} ms"));
            this.testOutputHelper.WriteLine(FormattableString.Invariant($"Key={my.Id}, Field={my.Field}"));
            my.Id.MustForTest().BeEqualTo(key);
        }
    }

    public class MyObject : IIdentifiableBy<string>
    {
        public MyObject(
            string id,
            string field)
        {
            this.Id = id;
            this.Field = field;
        }

        public string Id { get; private set; }

        public string Field { get; private set; }
    }
}