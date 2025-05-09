﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LegacySqlStreamTest.cs" company="Naos Project">
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
    using OBeautifulCode.Serialization.Json;
    using OBeautifulCode.Serialization.Recipes;
    using OBeautifulCode.String.Recipes;
    using OBeautifulCode.Type;
    using Xunit;
    using Xunit.Abstractions;
    using static System.FormattableString;

    public class LegacySqlStreamTest
    {
        private const string DatabaseName = "master";  // change to locally created database name for local testing
        private const string InstanceName = "SQL2017"; // set to null for local testing

        private readonly ITestOutputHelper testOutputHelper;

        public LegacySqlStreamTest(
            ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sproc", Justification = "Name is preferred in context.")]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Might use testHelper.")]
        [Fact]
        public void GetSprocCreationScript()
        {
            var script = StreamSchema.Sprocs.GetDistinctStringSerializedIds.BuildCreationScript(nameof(this.GetSprocCreationScript));
            this.testOutputHelper.WriteLine(script);
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Might use testHelper.")]
        [Fact(Skip = "Local testing only.")]
        public void TestBinary()
        {
            // TODO: finish this
            throw new NotImplementedException();
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Might use testHelper.")]
        [Fact(Skip = "Local testing only.")]
        public void TestRandom()
        {
            // TODO: finish this
            throw new NotImplementedException();
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Might use testHelper.")]
        [Fact(Skip = "For creating a new test database")]
        public static void CreateStreamsTestingDatabase()
        {
            var sqlServerLocator = GetSqlServerLocator();
            var configuration = SqlServerDatabaseDefinition.BuildDatabaseConfigurationUsingDefaultsAsNecessary("Streams1", @"D:\SQL\");
            var createDatabaseOp = new CreateDatabaseOp(configuration, ExistingDatabaseStrategy.Throw);
            var protocol = new SqlOperationsProtocol(sqlServerLocator);
            protocol.Execute(createDatabaseOp);
        }

        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "ex", Justification = "Showing return value.")]
        [Fact(Skip = "For testing creating a new database")]
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

        [Fact(Skip = "Deadlocks every time.")]
        public void TestConcurrent()
        {
            var streamName = nameof(this.TestConcurrent) + Guid.NewGuid().ToStringInvariantPreferred().Substring(0, 5);

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
                                               _ => GetCreatedSqlStream(
                                                   streamName,
                                                   commandTimeout,
                                                   RecordTagAssociationManagementStrategy.ExternallyManaged))
                                          .ToList();

            var times = new ConcurrentBag<TimeSpan>();
            Parallel.ForEach(
                listOfStreams,
                new ParallelOptions
                {
                    MaxDegreeOfParallelism = 2,
                },
                _ =>
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

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Versionless", Justification = "Name is preferred in context.")]
        [Fact(Skip = "Local testing only.")]
        public static void TestRecordFilterHonorsBothVersionAndVersionlessTypeRepresentations()
        {
            var streamName = nameof(TestRecordFilterHonorsBothVersionAndVersionlessTypeRepresentations) + Guid.NewGuid().ToStringInvariantPreferred().Substring(0, 5);
            var stream = GetCreatedSqlStream(streamName);

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

        [Fact(Skip = "Local testing only.")]
        public static void ExistingRecordStrategyTestForPutRecord()
        {
            var streamName = nameof(ExistingRecordStrategyTestForPutRecord) + Guid.NewGuid().ToStringInvariantPreferred().Substring(0, 5);
            var stream = GetCreatedSqlStream(streamName);

            stream.Execute(
                new CreateStreamUserOp(
                    stream.Name + "ReadOnly",
                    stream.Name + "ReadOnly-User",
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

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sprocs", Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
        [Fact(Skip = "Local testing only.")]
        public void UpdateSprocs()
        {
            var streamName = nameof(this.UpdateSprocs) + Guid.NewGuid().ToStringInvariantPreferred().Substring(0, 5);
            var stream = GetCreatedSqlStream(streamName);

            var updateOp = new UpdateStreamStoredProceduresOp(
                RecordTagAssociationManagementStrategy.ExternallyManaged,
                5);
            var result = stream.Execute(updateOp);
            this.testOutputHelper.WriteLine("Version: " + (result.PriorVersion ?? "<null>"));
        }

        [Fact(Skip = "Local testing only.")]
        public static void TagCachingTests()
        {
            var streamName = nameof(TagCachingTests) + Guid.NewGuid().ToStringInvariantPreferred().Substring(0, 5);
            var streamOne = GetCreatedSqlStream(streamName);
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
            var streamOneAgain = GetCreatedSqlStream(streamName);

            var tagsBackOneAgain = streamOneAgain.GetTagsByIds(sqlServerLocator, tagIdsOne);
            tagsBackOneAgain.MustForTest().NotBeNullNorEmptyEnumerable();

            var streamTwo = GetCreatedSqlStream(streamName);

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

        [Fact]
        public static void PutGetTests()
        {
            var streamName = nameof(PutGetTests) + Guid.NewGuid().ToStringInvariantPreferred().Substring(0, 5);
            var stream = GetCreatedSqlStream(streamName);

            var key = stream.Name;
            var firstValue = "Testing again.";
            var secondValue = "Testing again latest.";
            var firstTags = new List<NamedValue<string>>
                            {
                                new NamedValue<string>(nameof(MyObject.Field), firstValue),
                                new NamedValue<string>(nameof(MyObject), secondValue),
                            };

            stream.PutWithId(key, new MyObject(key, firstValue), firstTags);

            var streamUncached = GetCreatedSqlStream(streamName);

            var firstTagsFirstOnly = new List<NamedValue<string>>
                                     {
                                         new NamedValue<string>(nameof(MyObject.Field), firstValue),
                                     };

            streamUncached.PutWithId(Guid.NewGuid().ToString().ToUpperInvariant(), new MyObject(key, secondValue), firstTagsFirstOnly);
            var item = streamUncached.GetLatestObjectById<string, MyObject>(key);
            item.Field.MustForTest().BeEqualTo(firstValue);
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = NaosSuppressBecause.CA1505_AvoidUnmaintainableCode_DisagreeWithAssessment)]
        [Fact]
        public void HandlingTests()
        {
            var streamName = nameof(this.HandlingTests) + Guid.NewGuid().ToStringInvariantPreferred().Substring(0, 5);
            var stream = GetCreatedSqlStream(streamName);

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
                secondHistory.MustForTest().HaveCount(6);

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

        [Fact]
        public void HandlingEntryTagsUsedInStatusTests()
        {
            var streamName = nameof(this.HandlingEntryTagsUsedInStatusTests) + Guid.NewGuid().ToStringInvariantPreferred().Substring(0, 5);
            var stream = GetCreatedSqlStream(streamName);

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
            var streamName = nameof(this.HandlingCanBeCompletedWhileBlockedButNotNewHandlingTests) + Guid.NewGuid().ToStringInvariantPreferred().Substring(0, 5);
            var stream = GetCreatedSqlStream(streamName);

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

        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "internalRecordIdThreeLong", Justification = "Showing return value.")]
        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "internalRecordIdTwoLong", Justification = "Showing return value.")]
        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "internalRecordIdOneLong", Justification = "Showing return value.")]
        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "internalRecordIdThreeShort", Justification = "Showing return value.")]
        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "internalRecordIdTwoShort", Justification = "Showing return value.")]
        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "internalRecordIdOneShort", Justification = "Showing return value.")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Samed", Justification = "Name is preferred in context.")]
        [Fact]
        public static void GetDistinctStringSerializedIds___SamedId___WithDifferentTypes()
        {
            var streamName = nameof(GetDistinctStringSerializedIds___SamedId___WithDifferentTypes) + Guid.NewGuid().ToStringInvariantPreferred().Substring(0, 5);
            var stream = GetCreatedSqlStream(streamName);

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
                    .OrderBy(_ => _)
                    .ToList()
                    .MustForTest()
                    .BeEqualTo(
                         new[]
                             {
                                 firstId,
                                 secondId,
                                 thirdId,
                             }.OrderBy(_ => _)
                              .ToList());
        }

        [Fact]
        public static void TagsCanBeNullTest()
        {
            var streamName = nameof(TagsCanBeNullTest) + Guid.NewGuid().ToStringInvariantPreferred().Substring(0, 5);
            var stream = GetCreatedSqlStream(streamName);

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

        [Fact]
        public static void TestUniqueLong()
        {
            var streamName = nameof(TestUniqueLong) + Guid.NewGuid().ToStringInvariantPreferred().Substring(0, 5);
            var stream = GetCreatedSqlStream(streamName);

            var nextNoDetails = stream.Execute(new StandardGetNextUniqueLongOp());
            var nextDetails = stream.Execute(new StandardGetNextUniqueLongOp("Monkey."));

            nextDetails.MustForTest().BeGreaterThan(nextNoDetails);
        }

        [Fact]
        public static void TestGetDistinctEmptyStream()
        {
            var streamName = nameof(TestGetDistinctEmptyStream) + Guid.NewGuid().ToStringInvariantPreferred().Substring(0, 5);
            var stream = GetCreatedSqlStream(streamName);

            var ids = stream.Execute(new StandardGetDistinctStringSerializedIdsOp(new RecordFilter()));
            ids.MustForTest().BeEmptyEnumerable();
        }

        [Fact]
        public static void TestNullStringSerializedId()
        {
            const string id = null;

            var streamName = nameof(TestNullStringSerializedId) + Guid.NewGuid().ToStringInvariantPreferred().Substring(0, 5);
            var stream = GetCreatedSqlStream(streamName);
            var objectToPut = new MyObject("id", "test-null");
            stream.PutWithId(id, objectToPut, typeSelectionStrategy: TypeSelectionStrategy.UseDeclaredType);
            var ids = stream.Execute(new StandardGetDistinctStringSerializedIdsOp(new RecordFilter()));
            ids.Single().StringSerializedId.MustForTest().BeNull();
            var latestObjectById = stream.GetLatestObjectById<string, MyObject>(id, typeSelectionStrategy: TypeSelectionStrategy.UseDeclaredType);
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

        [Fact]
        public static void TestGetDistinctEmptyRecordFilter()
        {
            var streamName = nameof(TestGetDistinctEmptyRecordFilter) + Guid.NewGuid().ToStringInvariantPreferred().Substring(0, 5);
            var stream = GetCreatedSqlStream(streamName);

            var testId = Guid.NewGuid().ToStringInvariantPreferred().ToUpperInvariant();
            stream.PutWithId(testId, new MyObject("test", "empty-record-filter"));
            var ids = stream.Execute(new StandardGetDistinctStringSerializedIdsOp(new RecordFilter()));
            ids.MustForTest().NotBeEmptyEnumerable();
        }

        [Fact]
        public static void TestLatestStringSerializedObject()
        {
            var streamName = nameof(TestLatestStringSerializedObject) + Guid.NewGuid().ToStringInvariantPreferred().Substring(0, 5);
            var stream = GetCreatedSqlStream(streamName, defaultSerializationFormat: SerializationFormat.String);

            var latestObject = A.Dummy<MyObject>();
            stream.PutWithId(latestObject.Id, latestObject);
            var latestStringSerializedObject = stream.GetStreamReadingWithIdProtocols<string>()
                                      .Execute(new GetLatestStringSerializedObjectByIdOp<string>(latestObject.Id));
            latestStringSerializedObject.MustForTest().StartWith("{");
            latestStringSerializedObject.MustForTest().ContainString(Invariant($"\"id\": \"{latestObject.Id}\""));
            latestStringSerializedObject.MustForTest().ContainString(Invariant($"\"field\": \"{latestObject.Field}\""));
            latestStringSerializedObject.MustForTest().EndWith("}");
        }

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

            Config.Precedence = new[]
                                {
                                    "Local",
                                };

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
            var sqlServerLocator = new SqlServerLocator("(local)", DatabaseName, "sa", "Password12!", InstanceName);

            return sqlServerLocator;
        }

        private static SqlStream GetCreatedSqlStream(
            string streamName,
            TimeSpan? commandTimeout = null,
            RecordTagAssociationManagementStrategy recordTagAssociationManagementStrategy = RecordTagAssociationManagementStrategy.AssociatedDuringPutInSprocInTransaction,
            int? maxConcurrentHandlingCount = null,
            SerializationFormat defaultSerializationFormat = SerializationFormat.Binary)
        {
            var sqlServerLocator = GetSqlServerLocator();
            var resourceLocatorProtocol = new SingleResourceLocatorProtocols(sqlServerLocator);

            var defaultSerializerRepresentation = GetSerializerRepresentation();

            var stream = new SqlStream(
                streamName,
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
                typeof(DependencyOnlyJsonSerializationConfiguration<SqlServerJsonSerializationConfiguration, TypesToRegisterJsonSerializationConfiguration<MyObject>>).ToRepresentation();

            defaultSerializerRepresentation = new SerializerRepresentation(
                SerializationKind.Json,
                configurationTypeRepresentation);
            return defaultSerializerRepresentation;
        }

        private class MyObject : IHaveId<string>
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
}