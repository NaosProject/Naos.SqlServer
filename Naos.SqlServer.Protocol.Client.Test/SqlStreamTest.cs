// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlStreamTest.cs" company="Naos Project">
//     Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Protocol.Client.Test
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using FakeItEasy;
    using Naos.CodeAnalysis.Recipes;
    using Naos.Configuration.Domain;
    using Naos.Database.Domain;
    using Naos.SqlServer.Domain;
    using Naos.SqlServer.Serialization.Bson;
    using Naos.SqlServer.Serialization.Json;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Representation.System;
    using OBeautifulCode.Serialization;
    using OBeautifulCode.Serialization.Bson;
    using OBeautifulCode.Serialization.Json;
    using OBeautifulCode.Serialization.Recipes;
    using OBeautifulCode.String.Recipes;
    using OBeautifulCode.Type;
    using Xunit;
    using Xunit.Abstractions;
    using static System.FormattableString;

    /// <summary>
    /// Class SqlStreamTest.
    /// </summary>
    public partial class SqlStreamTest
    {
        private readonly string streamName = "Stream251";
        private readonly ITestOutputHelper testOutputHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlStreamTest" /> class.
        /// </summary>
        /// <param name="testOutputHelper">The test output helper.</param>
        public SqlStreamTest(
            ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        /// <summary>
        /// Defines the test method GetSprocCreationScript.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sproc", Justification = "Name is preferred in context.")]
        [SuppressMessage(
            "Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "Might use testHelper.")]
        [Fact]
        public void GetSprocCreationScript()
        {
            var script = StreamSchema.Sprocs.GetDistinctStringSerializedIds.BuildCreationScript(this.streamName);
            this.testOutputHelper.WriteLine(script);
        }

        /// <summary>
        /// Defines the test method TestBinary.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Might use testHelper.")]
        [Fact(Skip = "Local testing only.")]
        public void TestBinary()
        {
            // TODO: finish this
            throw new NotImplementedException();
        }

        /// <summary>
        /// Defines the test method TestRandom.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Might use testHelper.")]
        [Fact(Skip = "Local testing only.")]
        public void TestRandom()
        {
            // TODO: finish this
            throw new NotImplementedException();
        }

        /// <summary>
        /// Defines the test method CreateStreamsTestingDatabase.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Might use testHelper.")]
        [Fact(Skip = "Local testing only.")]
        public void CreateStreamsTestingDatabase()
        {
            var sqlServerLocator = GetSqlServerLocator();
            var configuration = SqlServerDatabaseDefinition.BuildDatabaseConfigurationUsingDefaultsAsNecessary("Streams", @"D:\SQL\");
            var createDatabaseOp = new CreateDatabaseOp(configuration, ExistingDatabaseStrategy.Throw);
            var protocol = new SqlOperationsProtocol(sqlServerLocator);
            protocol.Execute(createDatabaseOp);
        }

        /// <summary>
        /// Defines the test method CreateDatabase_ExistingDatabaseTest.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "ex", Justification = "Showing return value.")]
        [Fact]
        public static void CreateDatabase_ExistingDatabaseTest()
        {
            var sqlServerLocatorCopy = GetSqlServerLocator();
            var sqlServerLocator = sqlServerLocatorCopy.DeepCloneWithDatabaseName("Monkey");
            var protocol = new SqlOperationsProtocol(sqlServerLocator);
            var configuration = SqlServerDatabaseDefinition.BuildDatabaseConfigurationUsingDefaultsAsNecessary(sqlServerLocator.DatabaseName, @"D:\SQL\");
            
            var createDatabaseOpThrow = new CreateDatabaseOp(configuration, ExistingDatabaseStrategy.Throw);
            var createDatabaseOpSkip = new CreateDatabaseOp(configuration, ExistingDatabaseStrategy.Skip);

            protocol.Execute(createDatabaseOpThrow);
            protocol.Execute(createDatabaseOpSkip);
            var ex = Record.Exception(() => protocol.Execute(createDatabaseOpThrow));
        }

        /// <summary>
        /// Defines the test method TestConcurrent.
        /// </summary>
        [Fact]
        public void TestConcurrent()
        {
            var serializerRepresentation = GetSerializerRepresentation();
            var tags = new List<NamedValue<string>>
                       {
                           new NamedValue<string>("ChangeSet", Guid.NewGuid().ToString().ToUpperInvariant()),
                       };
            var timestampUtc = DateTime.UtcNow;
            var payloadType = typeof(byte[]).ToRepresentation();
            var metadata = new StreamRecordMetadata(
                Guid.NewGuid().ToString().ToUpperInvariant(),
                serializerRepresentation,
                typeof(string).ToRepresentation().ToWithAndWithoutVersion(),
                payloadType.ToWithAndWithoutVersion(),
                tags,
                timestampUtc,
                timestampUtc);

            var payload = new BinaryDescribedSerialization(payloadType, serializerRepresentation, new byte[3000000]);
            var putOp = new StandardPutRecordOp(metadata, payload);
            var commandTimeout = TimeSpan.FromSeconds(1000);
            var listOfStreams = Enumerable.Range(1, 10)
                                          .Select(
                                               _ => this.GetCreatedSqlStream(
                                                   commandTimeout,
                                                   RecordTagAssociationManagementStrategy.ExternallyManaged))
                                          .ToList();

            var times = new ConcurrentBag<TimeSpan>();
            Parallel.ForEach(listOfStreams, _ =>
                                            {
                                                var stopwatch = new Stopwatch();
                                                for (var idx = 0;
                                                    idx < 100;
                                                    idx++)
                                                {
                                                    stopwatch.Reset();
                                                    stopwatch.Start();
                                                    _.Execute(putOp);
                                                    stopwatch.Stop();
                                                    times.Add(stopwatch.Elapsed);
                                                }
                                            });

            var averageSeconds = times.Average(_ => _.TotalSeconds);
            var minSeconds = times.Min(_ => _.TotalSeconds);
            var maxSeconds = times.Max(_ => _.TotalSeconds);

            this.testOutputHelper.WriteLine(Invariant($"{nameof(averageSeconds)}: {averageSeconds}, {nameof(minSeconds)}: {minSeconds}, {nameof(maxSeconds)}: {maxSeconds}, "));
            foreach (var time in times)
            {
                this.testOutputHelper.WriteLine(time.TotalSeconds.ToString(CultureInfo.InvariantCulture));
            }
        }

        /// <summary>
        /// Defines the test method ExistingRecordStrategyTestForPutRecord.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Versionless", Justification = "Name is preferred in context.")]
        [Fact(Skip = "Local testing only.")]
        public void TestRecordFilterHonorsBothVersionAndVersionlessTypeRepresentations()
        {
            var stream = this.GetCreatedSqlStream();

            var key = Guid.NewGuid().ToString().ToUpperInvariant();
            var serializedKey = stream.SerializerFactory.BuildSerializer(stream.DefaultSerializerRepresentation).SerializeToString(key);
            var firstObject = new MyObject("test", "typeless");
            stream.PutWithId(key, firstObject);

            // With version
            var getWithVersionByIdType = stream.Execute(
                new StandardGetLatestRecordOp(
                    new RecordFilter(
                        idTypes: new[]
                                 {
                                     typeof(string).ToRepresentation(),
                                 },
                        versionMatchStrategy: VersionMatchStrategy.SpecifiedVersion)));
            getWithVersionByIdType.MustForTest().NotBeNull();

            var getWithVersionByObjectType = stream.Execute(
                new StandardGetLatestRecordOp(
                    new RecordFilter(
                        objectTypes: new[]
                                 {
                                     typeof(MyObject).ToRepresentation(),
                                 },
                        versionMatchStrategy: VersionMatchStrategy.SpecifiedVersion)));
            getWithVersionByObjectType.MustForTest().NotBeNull();

            var getWithVersionById = stream.Execute(
                new StandardGetLatestRecordOp(
                    new RecordFilter(
                        ids: new[]
                             {
                                 new StringSerializedIdentifier(serializedKey, typeof(string).ToRepresentation()),
                             },
                        versionMatchStrategy: VersionMatchStrategy.SpecifiedVersion)));
            getWithVersionById.MustForTest().NotBeNull();

            var getWithVersionWithDeprecated = stream.Execute(
                new StandardGetLatestRecordOp(
                    new RecordFilter(
                        deprecatedIdTypes: new[]
                                           {
                                               typeof(MyObject).ToRepresentation(),
                                           },
                        versionMatchStrategy: VersionMatchStrategy.SpecifiedVersion)));
            getWithVersionWithDeprecated.MustForTest().BeNull();

            // Without version
            var getWithoutVersionByIdType = stream.Execute(
                new StandardGetLatestRecordOp(
                    new RecordFilter(
                        idTypes: new[]
                                 {
                                     typeof(string).ToRepresentation().RemoveAssemblyVersions(),
                                 },
                        versionMatchStrategy: VersionMatchStrategy.Any)));
            getWithoutVersionByIdType.MustForTest().NotBeNull();

            var getWithoutVersionByObjectType = stream.Execute(
                new StandardGetLatestRecordOp(
                    new RecordFilter(
                        objectTypes: new[]
                                 {
                                     typeof(MyObject).ToRepresentation().RemoveAssemblyVersions(),
                                 },
                        versionMatchStrategy: VersionMatchStrategy.Any)));
            getWithoutVersionByObjectType.MustForTest().NotBeNull();

            var getWithoutVersionById = stream.Execute(
                new StandardGetLatestRecordOp(
                    new RecordFilter(
                        ids: new[]
                             {
                                 new StringSerializedIdentifier(serializedKey, typeof(string).ToRepresentation().RemoveAssemblyVersions()),
                             },
                        versionMatchStrategy: VersionMatchStrategy.Any)));
            getWithoutVersionById.MustForTest().NotBeNull();

            var getWithVersionWithoutDeprecated = stream.Execute(
                new StandardGetLatestRecordOp(
                    new RecordFilter(
                        deprecatedIdTypes: new[]
                                           {
                                               typeof(MyObject).ToRepresentation().RemoveAssemblyVersions(),
                                           },
                        versionMatchStrategy: VersionMatchStrategy.Any)));
            getWithVersionWithoutDeprecated.MustForTest().BeNull();
        }

        /// <summary>
        /// Defines the test method ExistingRecordStrategyTestForPutRecord.
        /// </summary>
        [Fact(Skip = "Local testing only.")]
        public void ExistingRecordStrategyTestForPutRecord()
        {
            var stream = this.GetCreatedSqlStream();

            stream.Execute(
                new CreateStreamUserOp(
                    this.streamName + "ReadOnly",
                    this.streamName + "ReadOnly-User",
                    "ReadMe",
                    StreamAccessKinds.Read,
                    true));

            var key = Guid.NewGuid().ToString().ToUpperInvariant();
            var firstValue = "Testing again.";
            var firstTags = new List<NamedValue<string>>
                            {
                                new NamedValue<string>(nameof(MyObject.Field), firstValue),
                            };

            var firstObject = new MyObject(key, firstValue);

            StreamRecordMetadata<string> result;
            result = stream.GetLatestRecordMetadataById(key);
            result.MustForTest().BeNull();
            stream.PutWithId(key, firstObject);
            result = stream.GetLatestRecordMetadataById(key);
            var firstRecordTimestamp = result.TimestampUtc;
            result.MustForTest().NotBeNull();
            result.TypeRepresentationOfObject.WithVersion.MustForTest().BeEqualTo(typeof(MyObject).ToRepresentation());

            stream.PutWithId(key, "hello", firstTags, ExistingRecordStrategy.DoNotWriteIfFoundById);
            result = stream.GetLatestRecordMetadataById(key);
            result.TimestampUtc.MustForTest().BeEqualTo(firstRecordTimestamp);

            stream.PutWithId(key, "hello", firstTags, ExistingRecordStrategy.DoNotWriteIfFoundByIdAndType);
            result = stream.GetLatestRecordMetadataById(key);
            var secondRecordTimestamp = result.TimestampUtc;
            result.TimestampUtc.MustForTest().NotBeEqualTo(firstRecordTimestamp);
            result.TypeRepresentationOfObject.WithVersion.MustForTest().BeEqualTo(typeof(string).ToRepresentation());

            stream.PutWithId(key, "hello", firstTags, ExistingRecordStrategy.DoNotWriteIfFoundByIdAndType);
            result = stream.GetLatestRecordMetadataById(key);
            result.TimestampUtc.MustForTest().BeEqualTo(secondRecordTimestamp);
            result.TypeRepresentationOfObject.WithVersion.MustForTest().BeEqualTo(typeof(string).ToRepresentation());

            stream.PutWithId(key, "hello", firstTags, ExistingRecordStrategy.DoNotWriteIfFoundByIdAndTypeAndContent);
            result = stream.GetLatestRecordMetadataById(key);
            result.TimestampUtc.MustForTest().BeEqualTo(secondRecordTimestamp);
            result.TypeRepresentationOfObject.WithVersion.MustForTest().BeEqualTo(typeof(string).ToRepresentation());

            stream.PutWithId(key, "goodbye", firstTags, ExistingRecordStrategy.DoNotWriteIfFoundByIdAndTypeAndContent);
            result = stream.GetLatestRecordMetadataById(key);
            result.TimestampUtc.MustForTest().NotBeEqualTo(secondRecordTimestamp);
            result.TypeRepresentationOfObject.WithVersion.MustForTest().BeEqualTo(typeof(string).ToRepresentation());
        }

        /// <summary>
        /// Defines the test method UpdateSprocs.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sprocs", Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
        [Fact(Skip = "Local testing only.")]
        public void UpdateSprocs()
        {
            var stream = this.GetCreatedSqlStream();

            var updateOp = new UpdateStreamStoredProceduresOp(
                RecordTagAssociationManagementStrategy.ExternallyManaged,
                5);
            var result = stream.Execute(updateOp);
            this.testOutputHelper.WriteLine("Version: " + (result.PriorVersion ?? "<null>"));
        }

        /// <summary>
        /// Defines the test method TagCachingTests.
        /// </summary>
        [Fact(Skip = "Local testing only.")]
        public void TagCachingTests()
        {
            var streamOne = this.GetCreatedSqlStream();
            var sqlServerLocator = GetSqlServerLocator();

            var tagsOne = new List<NamedValue<string>>
                       {
                           new NamedValue<string>("Key", "1" + Guid.NewGuid()),
                       };

            var tagIdsOne = streamOne.GetIdsAddIfNecessaryTag(sqlServerLocator, tagsOne);
            tagIdsOne.MustForTest().NotBeNullNorEmptyEnumerable();

            var tagsBackOne = streamOne.GetTagsByIds(sqlServerLocator, tagIdsOne);
            tagsBackOne.MustForTest().NotBeNullNorEmptyEnumerable();

            // get new tag caching...
            var streamOneAgain = this.GetCreatedSqlStream();

            var tagsBackOneAgain = streamOneAgain.GetTagsByIds(sqlServerLocator, tagIdsOne);
            tagsBackOneAgain.MustForTest().NotBeNullNorEmptyEnumerable();

            var streamTwo = this.GetCreatedSqlStream();

            var tagsTwo = new List<NamedValue<string>>
                       {
                           new NamedValue<string>(tagsOne.Single().Name, tagsOne.Single().Value),
                           new NamedValue<string>("Key2", "2" + Guid.NewGuid()),
                       };

            var tagIdsTwo = streamTwo.GetIdsAddIfNecessaryTag(sqlServerLocator, tagsTwo);
            tagIdsTwo.MustForTest().NotBeNullNorEmptyEnumerable();

            var tagsBackTwo = streamTwo.GetTagsByIds(sqlServerLocator, tagIdsTwo);
            tagsBackTwo.MustForTest().NotBeNullNorEmptyEnumerable();
        }

        /// <summary>
        /// Defines the test method ExistingRecordStrategyTestForPutRecord.
        /// </summary>
        [Fact(Skip = "Local testing only.")]
        public void GetDistinctIds_With_Depreciated_Types_Test()
        {
            // Arrange
            var stream = this.GetCreatedSqlStream();

            var item1 = new MyObject("1", "my-obj-1");
            var item2 = new MyObject("2", "my-obj-2");
            var item3 = new MyObject2("1", "my-obj-2");
            var item4 = new MyObject2("2", "my-obj-2");
            var item5 = new MyObject("3", "my-obj-1");
            var item6 = new MyObject2("4", "my-obj-2");

            var depreciated1 = new IdDeprecatedEvent<MyObject>(DateTime.UtcNow);
            var depreciated2 = new IdDeprecatedEvent<MyObject2>(DateTime.UtcNow);

            stream.PutWithId(item1.Id, item1);
            stream.PutWithId(item2.Id, item2);
            stream.PutWithId(item3.Id, item3);
            stream.PutWithId(item4.Id, item4);
            stream.PutWithId(item5.Id, item5);
            stream.PutWithId(item6.Id, item6);
            stream.PutWithId(item1.Id, depreciated1);
            stream.PutWithId(item4.Id, depreciated2);
            stream.PutWithId(item6.Id, depreciated2);
            stream.PutWithId(item6.Id, item6);

            // Act
            var actual1 = stream.GetDistinctIds<string>(new[] { typeof(MyObject).ToRepresentation() }, deprecatedIdTypes: new[] { depreciated1.GetType().ToRepresentation() });

            var actual2 = stream.GetDistinctIds<string>(new[] { typeof(MyObject2).ToRepresentation() }, deprecatedIdTypes: new[] { depreciated2.GetType().ToRepresentation() });

            // Assert
            actual1.AsTest().Must().BeEqualTo((IReadOnlyCollection<string>)new[] { item2.Id, item5.Id });
            actual2.AsTest().Must().BeEqualTo((IReadOnlyCollection<string>)new[] { item3.Id, item6.Id });
        }

        /// <summary>
        /// Defines the test method PutGetTests.
        /// </summary>
        [Fact]
        public void PutGetTests()
        {
            var stream = this.GetCreatedSqlStream();

            var key = stream.Name;
            var firstValue = "Testing again.";
            var secondValue = "Testing again latest.";
            var firstTags = new List<NamedValue<string>>
                            {
                                new NamedValue<string>(nameof(MyObject.Field), firstValue),
                                new NamedValue<string>(nameof(MyObject), secondValue),
                            };

            stream.PutWithId(key, new MyObject(key, firstValue), firstTags);

            var streamUncached = this.GetCreatedSqlStream();

            var firstTagsFirstOnly = new List<NamedValue<string>>
                                     {
                                         new NamedValue<string>(nameof(MyObject.Field), firstValue),
                                     };

            streamUncached.PutWithId(Guid.NewGuid().ToString().ToUpperInvariant(), new MyObject(key, secondValue), firstTagsFirstOnly);
            var item = streamUncached.GetLatestObjectById<string, MyObject>(key);
            item.Field.MustForTest().BeEqualTo(firstValue);
        }

        /// <summary>
        /// Defines the test method HandlingTests.
        /// </summary>
        [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = NaosSuppressBecause.CA1505_AvoidUnmaintainableCode_DisagreeWithAssessment)]
        [Fact]
        public void HandlingTests()
        {
            var stream = this.GetCreatedSqlStream();

            var x = stream.GetNextUniqueLong();
            x.MustForTest().BeGreaterThan(0L);

            var start = DateTime.UtcNow;
            for (int idx = 0;
                idx < 10;
                idx++)
            {
                var key = Invariant($"{stream.Name}Key{idx}");

                var firstValue = "Testing again.";
                var firstObject = new MyObject(key, firstValue);
                var firstConcern = "CanceledPickedBackUpScenario";
                var firstTags = new List<NamedValue<string>>
                                {
                                    new NamedValue<string>("Record", Guid.NewGuid().ToString().ToUpper(CultureInfo.InvariantCulture)),
                                };

                var handleTags = new List<NamedValue<string>>
                                {
                                    new NamedValue<string>("Handle", Guid.NewGuid().ToString().ToUpper(CultureInfo.InvariantCulture)),
                                };

                var firstRecordAndHandleTags = firstTags.Concat(handleTags).ToList();

                stream.PutWithId(firstObject.Id, firstObject, firstTags);
                var metadata = stream.GetLatestRecordMetadataById("monkeyAintThere");
                metadata.MustForTest().BeNull();

                var handlingStatusAvailableByDefault = stream.Execute(
                    new StandardGetHandlingStatusOp(
                        firstConcern,
                        new RecordFilter(tags: firstTags),
                        new HandlingFilter(
                            new[]
                            {
                                HandlingStatus.AvailableByDefault,
                            })));
                handlingStatusAvailableByDefault.MustForTest().NotBeEmptyDictionary();
                handlingStatusAvailableByDefault.Single().Value.MustForTest().BeEqualTo(HandlingStatus.AvailableByDefault);
                var first = stream.Execute(
                    new StandardTryHandleRecordOp(
                        firstConcern,
                        new RecordFilter(
                            idTypes: new[]
                                     {
                                         typeof(string).ToRepresentation(),
                                     },
                            objectTypes: new[]
                                         {
                                             typeof(MyObject).ToRepresentation(),
                                         }),
                        inheritRecordTags: true,
                        tags: handleTags));
                first.RecordToHandle.MustForTest().NotBeNull();

                var getFirstStatusByTagsOp = new StandardGetHandlingStatusOp(
                    firstConcern,
                    new RecordFilter(tags: firstTags),
                    new HandlingFilter());

                stream.Execute(getFirstStatusByTagsOp).OrderByDescending(_ => _.Key).First().Value.MustForTest().BeEqualTo(HandlingStatus.Running);

                var firstInternalRecordId = first.RecordToHandle.InternalRecordId;
                stream.Execute(
                    new CancelRunningHandleRecordOp(
                        firstInternalRecordId,
                        firstConcern,
                        "Resources unavailable; node out of disk space.",
                        tags: firstRecordAndHandleTags).Standardize());
                stream.Execute(getFirstStatusByTagsOp).OrderByDescending(_ => _.Key).First().Value.MustForTest().BeEqualTo(HandlingStatus.AvailableAfterExternalCancellation);

                stream.Execute(new DisableHandlingForStreamOp("Stop processing, fixing resource issue.").Standardize());
                stream.Execute(getFirstStatusByTagsOp).OrderByDescending(_ => _.Key).First().Value.MustForTest().BeEqualTo(HandlingStatus.DisabledForStream);
                first = stream.Execute(new StandardTryHandleRecordOp(
                    firstConcern,
                    new RecordFilter(
                        idTypes: new[]
                                 {
                                     typeof(string).ToRepresentation(),
                                 },
                        objectTypes: new[]
                                     {
                                         typeof(MyObject).ToRepresentation(),
                                     }),
                    inheritRecordTags: true,
                    tags: firstTags));
                first.RecordToHandle.MustForTest().BeNull();
                first.IsBlocked.MustForTest().BeTrue();
                stream.Execute(getFirstStatusByTagsOp).OrderByDescending(_ => _.Key).First().Value.MustForTest().BeEqualTo(HandlingStatus.DisabledForStream);
                var runningStatuses = stream.Execute(
                    new StandardGetHandlingStatusOp(
                        firstConcern,
                        getFirstStatusByTagsOp.RecordFilter,
                        new HandlingFilter(
                            new[]
                            {
                                HandlingStatus.Running,
                            })));
                runningStatuses.MustForTest().BeEmptyDictionary();

                stream.Execute(new EnableHandlingForStreamOp("Resume processing, fixed resource issue.").Standardize());
                stream.Execute(getFirstStatusByTagsOp).OrderByDescending(_ => _.Key).First().Value.MustForTest().BeEqualTo(HandlingStatus.AvailableAfterExternalCancellation);

                first = stream.Execute(new StandardTryHandleRecordOp(
                    firstConcern,
                    new RecordFilter(
                        idTypes: new[]
                                 {
                                     typeof(string).ToRepresentation(),
                                 },
                        objectTypes: new[]
                                     {
                                         typeof(MyObject).ToRepresentation(),
                                     }),
                    inheritRecordTags: true,
                    tags: handleTags));
                first.RecordToHandle.MustForTest().NotBeNull();
                stream.Execute(getFirstStatusByTagsOp).OrderByDescending(_ => _.Key).First().Value.MustForTest().BeEqualTo(HandlingStatus.Running);

                stream.Execute(
                    new SelfCancelRunningHandleRecordOp(
                        firstInternalRecordId,
                        firstConcern,
                        "Processing not finished, check later.",
                        tags: firstRecordAndHandleTags).Standardize());
                stream.Execute(getFirstStatusByTagsOp).OrderByDescending(_ => _.Key).First().Value.MustForTest().BeEqualTo(HandlingStatus.AvailableAfterSelfCancellation);
                first = stream.Execute(new StandardTryHandleRecordOp(
                    firstConcern,
                    new RecordFilter(
                        idTypes: new[]
                                 {
                                     typeof(string).ToRepresentation(),
                                 },
                        objectTypes: new[]
                                     {
                                         typeof(MyObject).ToRepresentation(),
                                     }),
                    inheritRecordTags: true,
                    tags: handleTags));
                first.RecordToHandle.MustForTest().NotBeNull();
                stream.Execute(getFirstStatusByTagsOp).OrderByDescending(_ => _.Key).First().Value.MustForTest().BeEqualTo(HandlingStatus.Running);

                stream.Execute(
                    new CompleteRunningHandleRecordOp(
                        firstInternalRecordId,
                        firstConcern,
                        "Processing not finished, check later.",
                        tags: firstRecordAndHandleTags).Standardize());
                stream.Execute(getFirstStatusByTagsOp).OrderByDescending(_ => _.Key).First().Value.MustForTest().BeEqualTo(HandlingStatus.Completed);
                first = stream.Execute(new StandardTryHandleRecordOp(
                    firstConcern,
                    new RecordFilter(
                        idTypes: new[]
                                 {
                                     typeof(string).ToRepresentation(),
                                 },
                        objectTypes: new[]
                                     {
                                         typeof(MyObject).ToRepresentation(),
                                     }),
                    inheritRecordTags: true,
                    tags: handleTags));
                first.RecordToHandle.MustForTest().BeNull();
                stream.Execute(getFirstStatusByTagsOp).OrderByDescending(_ => _.Key).First().Value.MustForTest().BeEqualTo(HandlingStatus.Completed);

                var firstHistory = stream.Execute(new StandardGetHandlingHistoryOp(firstInternalRecordId, firstConcern));
                firstHistory.MustForTest().HaveCount(7);
                foreach (var history in firstHistory)
                {
                    this.testOutputHelper.WriteLine(
                        Invariant(
                            $"{history.Concern}: {history.InternalHandlingEntryId}:{history.InternalRecordId} - {history.Status} - {history.Details ?? "<no details specified>"}"));
                }

                var secondConcern = "FailedRetriedScenario";
                var second = stream.Execute(
                    new StandardTryHandleRecordOp(
                        secondConcern,
                        new RecordFilter(
                            idTypes: new[]
                                     {
                                         typeof(string).ToRepresentation(),
                                     },
                            objectTypes: new[]
                                         {
                                             typeof(MyObject).ToRepresentation(),
                                         }),
                        inheritRecordTags: true,
                        tags: handleTags));
                second.RecordToHandle.MustForTest().NotBeNull();
                var secondInternalRecordId = second.RecordToHandle.InternalRecordId;
                var getSecondStatusByIdOp = new StandardGetHandlingStatusOp(
                    secondConcern,
                    new RecordFilter(
                        ids:
                        new[]
                        {
                            new StringSerializedIdentifier(
                                second.RecordToHandle.Metadata.StringSerializedId,
                                second.RecordToHandle.Metadata.TypeRepresentationOfId.WithVersion),
                        }),
                    new HandlingFilter());

                stream.Execute(getSecondStatusByIdOp).OrderByDescending(_ => _.Key).First().Value.MustForTest().BeEqualTo(HandlingStatus.Running);

                stream.Execute(
                    new FailRunningHandleRecordOp(
                        secondInternalRecordId,
                        secondConcern,
                        "NullReferenceException: Bot v1.0.1 doesn't work.",
                        tags: firstRecordAndHandleTags).Standardize());

                stream.Execute(getSecondStatusByIdOp).OrderByDescending(_ => _.Key).First().Value.MustForTest().BeEqualTo(HandlingStatus.Failed);
                second = stream.Execute(new StandardTryHandleRecordOp(
                    secondConcern,
                    new RecordFilter(
                        idTypes: new[]
                                 {
                                     typeof(string).ToRepresentation(),
                                 },
                        objectTypes: new[]
                                     {
                                         typeof(MyObject).ToRepresentation(),
                                     }),
                    inheritRecordTags: true,
                    tags: handleTags));
                second.RecordToHandle.MustForTest().BeNull();
                stream.Execute(getSecondStatusByIdOp).OrderByDescending(_ => _.Key).First().Value.MustForTest().BeEqualTo(HandlingStatus.Failed);

                stream.Execute(
                    new ResetFailedHandleRecordOp(secondInternalRecordId, secondConcern, "Redeployed Bot v1.0.1-hotfix, re-run.", tags: firstRecordAndHandleTags).Standardize());
                stream.Execute(getSecondStatusByIdOp).OrderByDescending(_ => _.Key).First().Value.MustForTest().BeEqualTo(HandlingStatus.AvailableAfterFailure);

                stream.GetStreamRecordHandlingProtocols().Execute(new DisableHandlingForStreamOp("Stop processing, need to confirm deployment."));
                stream.Execute(getSecondStatusByIdOp).OrderByDescending(_ => _.Key).First().Value.MustForTest().BeEqualTo(HandlingStatus.DisabledForStream);
                second = stream.Execute(
                    new StandardTryHandleRecordOp(
                        secondConcern,
                        new RecordFilter(
                            idTypes: new[]
                                     {
                                         typeof(string).ToRepresentation(),
                                     },
                            objectTypes: new[]
                                         {
                                             typeof(MyObject).ToRepresentation(),
                                         })));
                second.RecordToHandle.MustForTest().BeNull();
                stream.Execute(getSecondStatusByIdOp).OrderByDescending(_ => _.Key).First().Value.MustForTest().BeEqualTo(HandlingStatus.DisabledForStream);

                stream.GetStreamRecordHandlingProtocols().Execute(new EnableHandlingForStreamOp("Resume processing, confirmed deployment."));

                var secondConcernEmptyStatusCheck = stream.Execute(new StandardGetHandlingStatusOp(secondConcern, new RecordFilter(), new HandlingFilter(), null));
                secondConcernEmptyStatusCheck.MustForTest().NotBeEmptyDictionary();

                stream.Execute(getSecondStatusByIdOp).OrderByDescending(_ => _.Key).First().Value.MustForTest().BeEqualTo(HandlingStatus.AvailableAfterFailure);

                second = stream.Execute(new StandardTryHandleRecordOp(
                    secondConcern,
                    new RecordFilter(
                        idTypes: new[]
                                 {
                                     typeof(string).ToRepresentation(),
                                 },
                        objectTypes: new[]
                                     {
                                         typeof(MyObject).ToRepresentation(),
                                     }),
                    inheritRecordTags: true,
                    tags: handleTags));
                second.RecordToHandle.MustForTest().NotBeNull();
                stream.Execute(getSecondStatusByIdOp).OrderByDescending(_ => _.Key).First().Value.MustForTest().BeEqualTo(HandlingStatus.Running);

                stream.Execute(
                    new FailRunningHandleRecordOp(
                        secondInternalRecordId,
                        secondConcern,
                        "NullReferenceException: Bot v1.0.1-hotfix doesn't work.",
                        tags: firstRecordAndHandleTags).Standardize());
                stream.Execute(getSecondStatusByIdOp).OrderByDescending(_ => _.Key).First().Value.MustForTest().BeEqualTo(HandlingStatus.Failed);

                stream.Execute(new DisableHandlingForRecordOp(firstInternalRecordId, "Giving up.", tags: firstRecordAndHandleTags).Standardize());

                stream.Execute(getSecondStatusByIdOp).OrderByDescending(_ => _.Key).First().Value.MustForTest().BeEqualTo(HandlingStatus.DisabledForRecord);
                second = stream.Execute(new StandardTryHandleRecordOp(
                    secondConcern,
                    new RecordFilter(
                        idTypes: new[]
                                 {
                                     typeof(string).ToRepresentation(),
                                 },
                        objectTypes: new[]
                                     {
                                         typeof(MyObject).ToRepresentation(),
                                     }),
                    inheritRecordTags: true,
                    tags: handleTags));
                stream.Execute(getSecondStatusByIdOp).OrderByDescending(_ => _.Key).First().Value.MustForTest().BeEqualTo(HandlingStatus.DisabledForRecord);
                second.RecordToHandle.MustForTest().BeNull();

                var secondHistory = stream.Execute(new StandardGetHandlingHistoryOp(secondInternalRecordId, secondConcern));
                secondHistory.MustForTest().HaveCount(7);

                foreach (var history in secondHistory)
                {
                    this.testOutputHelper.WriteLine(
                        Invariant(
                            $"{history.Concern}: {history.InternalHandlingEntryId}:{history.InternalRecordId} - {history.Status} - {history.Details ?? "<no details specified>"}"));
                }

                var blockingHistory = stream.Execute(new StandardGetHandlingHistoryOp(0, Concerns.StreamHandlingDisabledConcern));

                foreach (var history in blockingHistory)
                {
                    this.testOutputHelper.WriteLine(
                        Invariant(
                            $"{history.Concern}: {history.InternalHandlingEntryId}:{history.InternalRecordId} - {history.Status} - {history.Details ?? "<no details specified>"}"));
                }
            }

            var stop = DateTime.UtcNow;
            this.testOutputHelper.WriteLine(Invariant($"TotalSeconds: {(stop - start).TotalSeconds}."));

            /*
            var allLocators = stream.ResourceLocatorProtocols.Execute(new GetAllResourceLocatorsOp()).ToList();
            var pruneDate = start.AddMilliseconds((stop - start).TotalMilliseconds / 2);
            allLocators.ForEach(_ => stream.Execute(new StandardPruneStreamOp(pruneDate, "Pruning by date.", _)));
            allLocators.ForEach(_ => stream.Execute(new PruneBeforeInternalRecordIdOp(7, "Pruning by id.", _)));
            */
        }

        /// <summary>
        /// Defines the test method HandlingEntryTagsUsedInStatusTests.
        /// </summary>
        [Fact]
        public void HandlingEntryTagsUsedInStatusTests()
        {
            var stream = this.GetCreatedSqlStream();

            var start = DateTime.UtcNow;

            var key = Invariant($"{stream.Name}Key");

            var firstValue = "Testing again.";
            var firstObject = new MyObject(key, firstValue);
            var firstConcern = "CanceledPickedBackUpScenario";
            var firstTags = new List<NamedValue<string>>
                            {
                                new NamedValue<string>("Record", Guid.NewGuid().ToString().ToUpper(CultureInfo.InvariantCulture)),
                            };

            var handleTags = new List<NamedValue<string>>
                            {
                                new NamedValue<string>("Handle", Guid.NewGuid().ToString().ToUpper(CultureInfo.InvariantCulture)),
                            };

            stream.PutWithId(firstObject.Id, firstObject, firstTags);
            var metadata = stream.GetLatestRecordMetadataById("monkeyAintThere");
            metadata.MustForTest().BeNull();
            var first = stream.Execute(
                new StandardTryHandleRecordOp(
                    firstConcern,
                    new RecordFilter(
                        idTypes: new[]
                                 {
                                     typeof(string).ToRepresentation(),
                                 },
                        objectTypes: new[]
                                     {
                                         typeof(MyObject).ToRepresentation(),
                                     }),
                    inheritRecordTags: true,
                    tags: handleTags));
            first.RecordToHandle.MustForTest().NotBeNull();

            var getFirstStatusByTagsOp = new StandardGetHandlingStatusOp(
                firstConcern,
                new RecordFilter(),
                new HandlingFilter(tags: handleTags));

            stream.Execute(getFirstStatusByTagsOp).OrderByDescending(_ => _.Key).First().Value.MustForTest().BeEqualTo(HandlingStatus.Running);

            var stop = DateTime.UtcNow;
            this.testOutputHelper.WriteLine(Invariant($"TotalSeconds: {(stop - start).TotalSeconds}."));
        }

        [Fact]
        public void HandlingCanBeCompletedWhileBlockedButNotNewHandlingTests()
        {
            var stream = this.GetCreatedSqlStream();

            var start = DateTime.UtcNow;

            var concern = Concerns.DefaultExecutionConcern;
            stream.PutWithId(1, "first");
            stream.PutWithId(2, "second");
            var first = stream.Execute(
                new StandardTryHandleRecordOp(
                    concern,
                    new RecordFilter(
                        objectTypes: new[]
                                     {
                                         typeof(string).ToRepresentation(),
                                     })));
            first.RecordToHandle.MustForTest().NotBeNull();

            var getHandlingStatusOp = new StandardGetHandlingStatusOp(
                concern,
                new RecordFilter(
                    ids: new[]
                         {
                             new StringSerializedIdentifier(
                                 stream.SerializerFactory.BuildSerializer(stream.DefaultSerializerRepresentation).SerializeToString(1),
                                 typeof(int).ToRepresentation()),
                         }),
                new HandlingFilter());

            stream.Execute(getHandlingStatusOp).OrderByDescending(_ => _.Key).Single().Value.MustForTest().BeEqualTo(HandlingStatus.Running);

            stream.IsRecordHandlingDisabled().MustForTest().BeFalse();

            stream.GetStreamRecordHandlingProtocols().Execute(new DisableHandlingForStreamOp("Blocking to test completion and status."));

            stream.IsRecordHandlingDisabled().MustForTest().BeTrue();

            var secondAttempted = stream.Execute(
                new StandardTryHandleRecordOp(
                    concern,
                    new RecordFilter(
                        objectTypes: new[]
                                     {
                                         typeof(string).ToRepresentation(),
                                     })));
            secondAttempted.RecordToHandle.MustForTest().BeNull();

            stream.Execute(getHandlingStatusOp).OrderByDescending(_ => _.Key).Single().Value.MustForTest().BeEqualTo(HandlingStatus.Running);

            stream.GetStreamRecordHandlingProtocols().Execute(new CompleteRunningHandleRecordOp(first.RecordToHandle.InternalRecordId, concern));

            stream.Execute(getHandlingStatusOp).OrderByDescending(_ => _.Key).Single().Value.MustForTest().BeEqualTo(HandlingStatus.Completed);

            stream.GetStreamRecordHandlingProtocols().Execute(new EnableHandlingForStreamOp("Unblocking to test completion and status."));

            stream.IsRecordHandlingDisabled().MustForTest().BeFalse();

            var second = stream.Execute(
                new StandardTryHandleRecordOp(
                    concern,
                    new RecordFilter(
                        objectTypes: new[]
                                     {
                                         typeof(string).ToRepresentation(),
                                     })));
            second.RecordToHandle.MustForTest().NotBeNull();

            var stop = DateTime.UtcNow;
            this.testOutputHelper.WriteLine(Invariant($"TotalSeconds: {(stop - start).TotalSeconds}."));
        }

        /// <summary>
        /// Defines the test method GetDistinctStringSerializedIds.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "internalRecordIdThree", Justification = "Showing return value.")]
        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "internalRecordIdTwo", Justification = "Showing return value.")]
        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "internalRecordIdOneOtherType", Justification = "Showing return value.")]
        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "internalRecordIdOneAgain", Justification = "Showing return value.")]
        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "internalRecordIdOne", Justification = "Showing return value.")]
        [Fact]
        public void GetDistinctStringSerializedIds()
        {
            var stream = this.GetCreatedSqlStream();

            var firstId = Guid.NewGuid().ToString().Replace("-", ",");
            var secondId = Guid.NewGuid().ToString().Replace("-", ",");

            var putOpOne = new PutWithIdAndReturnInternalRecordIdOp<string, string>(firstId, A.Dummy<string>());
            var internalRecordIdOne = stream.GetStreamWritingWithIdProtocols<string, string>().Execute(putOpOne);

            var putOpOneAgain = new PutWithIdAndReturnInternalRecordIdOp<string, string>(firstId, A.Dummy<string>());
            var internalRecordIdOneAgain = stream.GetStreamWritingWithIdProtocols<string, string>().Execute(putOpOneAgain);

            var putOpOneOtherType = new PutWithIdAndReturnInternalRecordIdOp<string, short>(secondId, A.Dummy<short>());
            var internalRecordIdOneOtherType = stream.GetStreamWritingWithIdProtocols<string, short>().Execute(putOpOneOtherType);

            var putOpTwo = new PutWithIdAndReturnInternalRecordIdOp<string, string>(secondId, A.Dummy<string>());
            var internalRecordIdTwo = stream.GetStreamWritingWithIdProtocols<string, string>().Execute(putOpTwo);

            var putOpTwoAgain = new PutWithIdAndReturnInternalRecordIdOp<string, IdDeprecatedEvent>(secondId, new IdDeprecatedEvent(DateTime.UtcNow));
            var internalRecordIdThree = stream.GetStreamWritingWithIdProtocols<string, IdDeprecatedEvent>().Execute(putOpTwoAgain);

            var distinctWrongType = stream.Execute(
                new StandardGetDistinctStringSerializedIdsOp(
                    new RecordFilter(
                        idTypes: new[]
                                 {
                                     typeof(string).ToRepresentation(),
                                 },
                        objectTypes: new[]
                                     {
                                         typeof(long).ToRepresentation(),
                                     },
                        deprecatedIdTypes: new[]
                                           {
                                               typeof(IdDeprecatedEvent).ToRepresentation(),
                                           })));
            distinctWrongType.MustForTest().BeEmptyEnumerable();

            var distinct = stream.Execute(
                new StandardGetDistinctStringSerializedIdsOp(
                    new RecordFilter(
                        idTypes: new[]
                                 {
                                     typeof(string).ToRepresentation(),
                                 },
                        deprecatedIdTypes: new[]
                                           {
                                               typeof(IdDeprecatedEvent).ToRepresentation(),
                                           })));
            distinct.MustForTest().NotBeNull();
            distinct.Single().StringSerializedId.MustForTest().BeEqualTo(Invariant($"\"{firstId}\""));
        }

        /// <summary>
        /// Defines the test method GetDistinctStringSerializedIds.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "internalRecordIdThreeLong", Justification = "Showing return value.")]
        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "internalRecordIdTwoLong", Justification = "Showing return value.")]
        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "internalRecordIdOneLong", Justification = "Showing return value.")]
        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "internalRecordIdThreeShort", Justification = "Showing return value.")]
        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "internalRecordIdTwoShort", Justification = "Showing return value.")]
        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "internalRecordIdOneShort", Justification = "Showing return value.")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Samed", Justification = "Name is preferred in context.")]
        [Fact]
        public void GetDistinctStringSerializedIds___SamedId___WithDifferentTypes()
        {
            var stream = this.GetCreatedSqlStream();

            var firstId = Guid.NewGuid().ToString().Replace("-", ",");
            var secondId = Guid.NewGuid().ToString().Replace("-", ",");
            var thirdId = Guid.NewGuid().ToString().Replace("-", ",");

            var putOneShortOp = new PutWithIdAndReturnInternalRecordIdOp<string, short>(firstId, A.Dummy<short>());
            var internalRecordIdOneShort = stream.GetStreamWritingWithIdProtocols<string, short>().Execute(putOneShortOp);

            var putTwoShortOp = new PutWithIdAndReturnInternalRecordIdOp<string, short>(secondId, A.Dummy<short>());
            var internalRecordIdTwoShort = stream.GetStreamWritingWithIdProtocols<string, short>().Execute(putTwoShortOp);

            var putThreeShortOp = new PutWithIdAndReturnInternalRecordIdOp<string, short>(thirdId, A.Dummy<short>());
            var internalRecordIdThreeShort = stream.GetStreamWritingWithIdProtocols<string, short>().Execute(putThreeShortOp);

            var putOneLongOp = new PutWithIdAndReturnInternalRecordIdOp<string, long>(firstId, A.Dummy<long>());
            var internalRecordIdOneLong = stream.GetStreamWritingWithIdProtocols<string, long>().Execute(putOneLongOp);

            var putTwoLongOp = new PutWithIdAndReturnInternalRecordIdOp<string, long>(secondId, A.Dummy<long>());
            var internalRecordIdTwoLong = stream.GetStreamWritingWithIdProtocols<string, long>().Execute(putTwoLongOp);

            var putThreeLongOp = new PutWithIdAndReturnInternalRecordIdOp<string, long>(thirdId, A.Dummy<long>());
            var internalRecordIdThreeLong = stream.GetStreamWritingWithIdProtocols<string, long>().Execute(putThreeLongOp);

            var distinct = stream.Execute(
                new StandardGetDistinctStringSerializedIdsOp(
                    new RecordFilter(
                        idTypes: new[]
                                 {
                                     typeof(string).ToRepresentation(),
                                 },
                        objectTypes: new[]
                                     {
                                        typeof(short).ToRepresentation(),
                                     },
                        deprecatedIdTypes: new[]
                                           {
                                               typeof(IdDeprecatedEvent).ToRepresentation(),
                                           })));
            distinct.MustForTest().NotBeNull();
            distinct.Select(_ => _.StringSerializedId)
                    .ToList()
                    .MustForTest()
                    .BeEqualTo(
                         new[]
                         {
                             Invariant($"\"{firstId}\""),
                             Invariant($"\"{secondId}\""),
                             Invariant($"\"{thirdId}\""),
                         }.ToList());
        }

        /// <summary>
        /// Defines the test method TagsCanBeNullTest.
        /// </summary>
        [Fact]
        public void TagsCanBeNullTest()
        {
            var stream = this.GetCreatedSqlStream();

            var id = A.Dummy<string>();

            var putOpTwo = new PutWithIdAndReturnInternalRecordIdOp<string, string>(id, A.Dummy<string>());
            var internalRecordIdTwo = stream.GetStreamWritingWithIdProtocols<string, string>().Execute(putOpTwo);
            var latestTwo = stream.Execute(
                new StandardGetLatestRecordOp(
                    new RecordFilter(
                        new[]
                        {
                            (long)internalRecordIdTwo,
                        })));
            latestTwo.InternalRecordId.MustForTest().BeEqualTo((long)internalRecordIdTwo);
            latestTwo.Metadata.Tags.MustForTest().BeNull();
        }

        /// <summary>
        /// Defines the test method TestUniqueLong.
        /// </summary>
        [Fact]
        public void TestUniqueLong()
        {
            var stream = this.GetCreatedSqlStream();

            var nextNoDetails = stream.Execute(new StandardGetNextUniqueLongOp());
            var nextDetails = stream.Execute(new StandardGetNextUniqueLongOp("Monkey."));

            nextDetails.MustForTest().BeGreaterThan(nextNoDetails);
        }

        /// <summary>
        /// Defines the test method TestGetDistinctEmptyStream.
        /// </summary>
        [Fact]
        public void TestGetDistinctEmptyStream()
        {
            var stream = this.GetCreatedSqlStream();

            var ids = stream.Execute(new StandardGetDistinctStringSerializedIdsOp(new RecordFilter()));
            ids.MustForTest().BeEmptyEnumerable();
        }

        /// <summary>
        /// Defines the test method TestNullStringSerializedId.
        /// </summary>
        [Fact]
        public void TestNullStringSerializedId()
        {
            const string id = null;

            var stream = this.GetCreatedSqlStream();
            var objectToPut = new MyObject("id", "test-null");
            stream.PutWithId(id, objectToPut);
            var ids = stream.Execute(new StandardGetDistinctStringSerializedIdsOp(new RecordFilter()));
            ids.Single().StringSerializedId.MustForTest().BeNull();
            var latestObjectById = stream.GetLatestObjectById<string, MyObject>(id);
            latestObjectById.Id.MustForTest().BeEqualTo(objectToPut.Id);
            latestObjectById.Field.MustForTest().BeEqualTo(objectToPut.Field);
            var latestObject = stream.GetLatestObject<MyObject>();
            latestObject.Id.MustForTest().BeEqualTo(objectToPut.Id);
            latestObject.Field.MustForTest().BeEqualTo(objectToPut.Field);
            var latestRecord = stream.GetLatestRecord<MyObject>();
            latestRecord.Metadata.StringSerializedId.MustForTest().BeNull();
            latestRecord.Payload.Id.MustForTest().BeEqualTo(objectToPut.Id);
            latestRecord.Payload.Field.MustForTest().BeEqualTo(objectToPut.Field);
        }

        /// <summary>
        /// Defines the test method TestGetDistinctEmptyRecordFilter.
        /// </summary>
        [Fact]
        public void TestGetDistinctEmptyRecordFilter()
        {
            var stream = this.GetCreatedSqlStream();

            var testId = Guid.NewGuid().ToStringInvariantPreferred().ToUpperInvariant();
            stream.PutWithId(testId, new MyObject("test", "empty-record-filter"));
            var ids = stream.Execute(new StandardGetDistinctStringSerializedIdsOp(new RecordFilter()));
            ids.MustForTest().NotBeEmptyEnumerable();
        }

        /// <summary>
        /// Defines the test method TestLatestStringSerializedObject.
        /// </summary>
        [Fact]
        public void TestLatestStringSerializedObject()
        {
            var stream = this.GetCreatedSqlStream(defaultSerializationFormat: SerializationFormat.String);

            var latestObject = A.Dummy<MyObject>();
            stream.PutWithId(latestObject.Id, latestObject);
            var latestStringSerializedObject = stream.GetStreamReadingWithIdProtocols<string>()
                                      .Execute(new GetLatestStringSerializedObjectByIdOp<string>(latestObject.Id));
            latestStringSerializedObject.MustForTest().StartWith("{");
            latestStringSerializedObject.MustForTest().ContainString(Invariant($"\"id\": \"{latestObject.Id}\""));
            latestStringSerializedObject.MustForTest().ContainString(Invariant($"\"field\": \"{latestObject.Field}\""));
            latestStringSerializedObject.MustForTest().EndWith("}");
        }

        /// <summary>
        /// Defines the test method TestConfigStreamBuilding.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "output", Justification = "Showing return value.")]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Keeping as is in case of future conversion.")]
        [Fact]
        public void TestConfigStreamBuilding()
        {
            var localStreamName = "MyStream";
            var x = new SqlServerStreamConfig(
                localStreamName,
                StreamAccessKinds.Read,
                TimeSpan.FromSeconds(30),
                TimeSpan.FromSeconds(30),
                new SerializerRepresentation(
                    SerializationKind.Bson,
                    typeof(SqlServerBsonSerializationConfiguration).ToRepresentation().RemoveAssemblyVersions()),
                SerializationFormat.String,
                new[]
                {
                    new SqlServerLocator("localhost", "MyDatabase", "user", "password"),
                });
            var serializer = new ObcJsonSerializer(typeof(SqlServerJsonSerializationConfiguration).ToJsonSerializationConfigurationType());
            var output = serializer.SerializeToString(x);

            var stream = Config.GetByName<SqlServerStreamConfig>(
                                         localStreamName,
                                         new SerializerRepresentation(
                                             SerializationKind.Json,
                                             typeof(SqlServerJsonSerializationConfiguration).ToRepresentation()),
                                         SerializerFactory.Instance)
                                    .ToStream(SerializerFactory.Instance);

            stream.MustForTest().NotBeNull().And().BeOfType<SqlStream>();
        }

        private static SqlServerLocator GetSqlServerLocator()
        {
            //var sqlServerLocator = new SqlServerLocator("localhost", "Streams", Invariant($"[{streamName}-read-only]"), "ReadMe", "SQLDEV2017");
            var sqlServerLocator = new SqlServerLocator("localhost", "Streams", "sa", "<password>", "SQLDEV2017");

            return sqlServerLocator;
        }

        private SqlStream GetCreatedSqlStream(
            TimeSpan? commandTimeout = null,
            RecordTagAssociationManagementStrategy recordTagAssociationManagementStrategy = RecordTagAssociationManagementStrategy.AssociatedDuringPutInSprocInTransaction,
            int? maxConcurrentHandlingCount = null,
            SerializationFormat defaultSerializationFormat = SerializationFormat.Binary)
        {
            var sqlServerLocator = GetSqlServerLocator();
            var resourceLocatorProtocol = new SingleResourceLocatorProtocols(sqlServerLocator);

            var defaultSerializerRepresentation = GetSerializerRepresentation();

            var stream = new SqlStream(
                this.streamName,
                TimeSpan.FromMinutes(1),
                commandTimeout ?? TimeSpan.FromMinutes(3),
                defaultSerializerRepresentation,
                defaultSerializationFormat,
                new ObcSimplifyingSerializerFactory(new JsonSerializerFactory()),
                resourceLocatorProtocol);

            stream.Execute(new StandardCreateStreamOp(stream.StreamRepresentation, ExistingStreamStrategy.Skip));
            stream.Execute(new UpdateStreamStoredProceduresOp(recordTagAssociationManagementStrategy, maxConcurrentHandlingCount));

            return stream;
        }

        private static SerializerRepresentation GetSerializerRepresentation()
        {
            SerializerRepresentation defaultSerializerRepresentation;
            var configurationTypeRepresentation =
                typeof(DependencyOnlyJsonSerializationConfiguration<SqlServerJsonSerializationConfiguration, TypesToRegisterJsonSerializationConfiguration<MyObject, MyObject2>>).ToRepresentation();

            defaultSerializerRepresentation = new SerializerRepresentation(
                SerializationKind.Json,
                configurationTypeRepresentation);
            return defaultSerializerRepresentation;
        }
    }

    /// <summary>
    /// Test object.
    /// Implements the <see cref="OBeautifulCode.Type.IHaveId{System.String}" />
    /// </summary>
    /// <seealso cref="OBeautifulCode.Type.IHaveId{System.String}" />
    public class MyObject : IHaveId<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MyObject" /> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="field">The field.</param>
        public MyObject(
            string id,
            string field)
        {
            this.Id = id;
            this.Field = field;
        }

        /// <summary>
        /// Gets the unique identifier.
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Gets the field.
        /// </summary>
        public string Field { get; private set; }

        /// <summary>
        /// Deeps the clone with new field.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns>MyObject.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "NewField", Justification = NaosSuppressBecause.CA1702_CompoundWordsShouldBeCasedCorrectly_AnalyzerIsIncorrectlyDetectingCompoundWords)]
        public MyObject DeepCloneWithNewField(string field)
        {
            var result = new MyObject(this.Id, field);
            return result;
        }
    }

    /// <summary>
    /// Test object.
    /// Implements the <see cref="OBeautifulCode.Type.IHaveId{System.String}" />
    /// </summary>
    /// <seealso cref="OBeautifulCode.Type.IHaveId{System.String}" />
    public class MyObject2 : IHaveId<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MyObject2" /> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="field">The field.</param>
        public MyObject2(
            string id,
            string field)
        {
            this.Id = id;
            this.Field = field;
        }

        /// <summary>
        /// Gets the unique identifier.
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Gets the field.
        /// </summary>
        public string Field { get; private set; }

        /// <summary>
        /// Deeps the clone with new field.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns>MyObject.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "NewField", Justification = NaosSuppressBecause.CA1702_CompoundWordsShouldBeCasedCorrectly_AnalyzerIsIncorrectlyDetectingCompoundWords)]
        public MyObject2 DeepCloneWithNewField(string field)
        {
            var result = new MyObject2(this.Id, field);
            return result;
        }
    }
}