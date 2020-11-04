﻿// --------------------------------------------------------------------------------------------------------------------
// <auto-generated>
//   Generated using OBeautifulCode.CodeGen.ModelObject (1.0.116.0)
// </auto-generated>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Protocol.Domain.Test
{
    using global::System;
    using global::System.CodeDom.Compiler;
    using global::System.Collections.Concurrent;
    using global::System.Collections.Generic;
    using global::System.Collections.ObjectModel;
    using global::System.Diagnostics.CodeAnalysis;

    using global::FakeItEasy;

    using global::Naos.Protocol.Domain;

    using global::OBeautifulCode.AutoFakeItEasy;
    using global::OBeautifulCode.Math.Recipes;
    using global::OBeautifulCode.Representation.System;
    using global::OBeautifulCode.Type;

    /// <summary>
    /// The default (code generated) Dummy Factory.
    /// Derive from this class to add any overriding or custom registrations.
    /// </summary>
    [ExcludeFromCodeCoverage]
    [GeneratedCode("OBeautifulCode.CodeGen.ModelObject", "1.0.116.0")]
#if !NaosProtocolSolution
    internal
#else
    public
#endif
    abstract class DefaultProtocolDummyFactory : IDummyFactory
    {
        public DefaultProtocolDummyFactory()
        {
            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new ExecutedOpEvent<Version, HandleEventOp<ExecutedOpEvent<Version, GetIdFromObjectOp<Version, Version>>, Version>>(
                                 A.Dummy<Version>(),
                                 A.Dummy<DateTime>(),
                                 A.Dummy<HandleEventOp<ExecutedOpEvent<Version, GetIdFromObjectOp<Version, Version>>, Version>>(),
                                 A.Dummy<IReadOnlyDictionary<string, string>>()));

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new ExecutingOpEvent<Version, HandleEventOp<ExecutedOpEvent<Version, GetIdFromObjectOp<Version, Version>>, Version>>(
                                 A.Dummy<Version>(),
                                 A.Dummy<DateTime>(),
                                 A.Dummy<HandleEventOp<ExecutedOpEvent<Version, GetIdFromObjectOp<Version, Version>>, Version>>(),
                                 A.Dummy<IReadOnlyDictionary<string, string>>()));

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new HandledEventEvent<Version, ExecutedOpEvent<Version, HandleEventOp<ExecutingOpEvent<Version, GetIdFromObjectOp<Version, Version>>, Version>>, Version>(
                                 A.Dummy<Version>(),
                                 A.Dummy<DateTime>(),
                                 A.Dummy<ExecutedOpEvent<Version, HandleEventOp<ExecutingOpEvent<Version, GetIdFromObjectOp<Version, Version>>, Version>>>(),
                                 A.Dummy<IReadOnlyDictionary<string, string>>()));

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new HandleEventOp<ExecutedOpEvent<Version, HandleEventOp<ExecutingOpEvent<Version, GetIdFromObjectOp<Version, Version>>, Version>>, Version>(
                                 A.Dummy<ExecutedOpEvent<Version, HandleEventOp<ExecutingOpEvent<Version, GetIdFromObjectOp<Version, Version>>, Version>>>()));

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new HandlingEventEvent<Version, ExecutedOpEvent<Version, HandleEventOp<ExecutingOpEvent<Version, GetIdFromObjectOp<Version, Version>>, Version>>, Version>(
                                 A.Dummy<Version>(),
                                 A.Dummy<DateTime>(),
                                 A.Dummy<ExecutedOpEvent<Version, HandleEventOp<ExecutingOpEvent<Version, GetIdFromObjectOp<Version, Version>>, Version>>>(),
                                 A.Dummy<IReadOnlyDictionary<string, string>>()));

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new RecordedObjectEvent<Version, Version>(
                                 A.Dummy<Version>(),
                                 A.Dummy<DateTime>(),
                                 A.Dummy<Version>(),
                                 A.Dummy<IReadOnlyDictionary<string, string>>()));

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new GetIdFromObjectOp<Version, Version>(
                                 A.Dummy<Version>()));

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new GetTagsFromObjectOp<Version>(
                                 A.Dummy<Version>()));

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new GetProtocolByTypeOp(
                                 A.Dummy<TypeRepresentation>(),
                                 A.Dummy<MissingProtocolStrategy>()));

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new GetReturningProtocolOp<GetIdFromObjectOp<Version, Version>, Version>());

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new GetVoidProtocolOp<HandleEventOp<ExecutedOpEvent<Version, GetIdFromObjectOp<Version, Version>>, Version>>());

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new CacheResult<Version, Version>(
                                 A.Dummy<Version>(),
                                 A.Dummy<Version>(),
                                 A.Dummy<DateTime>()));

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new CacheStatusResult(
                                 A.Dummy<long>(),
                                 A.Dummy<UtcDateTimeRangeInclusive>()));

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new ClearCacheOp());

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new GetCacheStatusOp());

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new GetOrAddCachedItemOp<GetIdFromObjectOp<Version, Version>, Version>(
                                 A.Dummy<GetIdFromObjectOp<Version, Version>>()));

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new GetAllResourceLocatorsOp());

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new GetResourceLocatorByIdOp<Version>(
                                 A.Dummy<Version>()));

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new NullResourceLocator());

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new ThrowIfResourceUnavailableOp(
                                 A.Dummy<ResourceLocatorBase>()));

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () =>
                {
                    var availableTypes = new[]
                    {
                        typeof(ExecutedOpEvent<Version, HandleEventOp<ExecutedOpEvent<Version, GetIdFromObjectOp<Version, Version>>, Version>>),
                        typeof(ExecutingOpEvent<Version, HandleEventOp<ExecutedOpEvent<Version, GetIdFromObjectOp<Version, Version>>, Version>>),
                        typeof(HandledEventEvent<Version, ExecutedOpEvent<Version, HandleEventOp<ExecutingOpEvent<Version, GetIdFromObjectOp<Version, Version>>, Version>>, Version>),
                        typeof(HandlingEventEvent<Version, ExecutedOpEvent<Version, HandleEventOp<ExecutingOpEvent<Version, GetIdFromObjectOp<Version, Version>>, Version>>, Version>),
                        typeof(RecordedObjectEvent<Version, Version>)
                    };

                    var randomIndex = ThreadSafeRandom.Next(0, availableTypes.Length);

                    var randomType = availableTypes[randomIndex];

                    var result = (EventBase<Version>)AD.ummy(randomType);

                    return result;
                });

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () =>
                {
                    var availableTypes = new[]
                    {
                        typeof(HandleEventOp<ExecutedOpEvent<Version, HandleEventOp<ExecutingOpEvent<Version, GetIdFromObjectOp<Version, Version>>, Version>>, Version>),
                        typeof(GetIdFromObjectOp<Version, Version>),
                        typeof(GetTagsFromObjectOp<Version>),
                        typeof(GetProtocolByTypeOp),
                        typeof(GetReturningProtocolOp<GetIdFromObjectOp<Version, Version>, Version>),
                        typeof(GetVoidProtocolOp<HandleEventOp<ExecutedOpEvent<Version, GetIdFromObjectOp<Version, Version>>, Version>>),
                        typeof(ClearCacheOp),
                        typeof(GetCacheStatusOp),
                        typeof(GetOrAddCachedItemOp<GetIdFromObjectOp<Version, Version>, Version>),
                        typeof(GetAllResourceLocatorsOp),
                        typeof(GetResourceLocatorByIdOp<Version>),
                        typeof(ThrowIfResourceUnavailableOp)
                    };

                    var randomIndex = ThreadSafeRandom.Next(0, availableTypes.Length);

                    var randomType = availableTypes[randomIndex];

                    var result = (OperationBase)AD.ummy(randomType);

                    return result;
                });

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () =>
                {
                    var availableTypes = new[]
                    {
                        typeof(GetIdFromObjectOp<Version, Version>)
                    };

                    var randomIndex = ThreadSafeRandom.Next(0, availableTypes.Length);

                    var randomType = availableTypes[randomIndex];

                    var result = (ReturningOperationBase<Version>)AD.ummy(randomType);

                    return result;
                });

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () =>
                {
                    var availableTypes = new[]
                    {
                        typeof(HandleEventOp<ExecutedOpEvent<Version, HandleEventOp<ExecutingOpEvent<Version, GetIdFromObjectOp<Version, Version>>, Version>>, Version>),
                        typeof(ClearCacheOp),
                        typeof(ThrowIfResourceUnavailableOp)
                    };

                    var randomIndex = ThreadSafeRandom.Next(0, availableTypes.Length);

                    var randomType = availableTypes[randomIndex];

                    var result = (VoidOperationBase)AD.ummy(randomType);

                    return result;
                });

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new NamedResourceLocator(
                                 A.Dummy<string>()));

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () =>
                {
                    var availableTypes = new[]
                    {
                        typeof(NullResourceLocator),
                        typeof(NamedResourceLocator)
                    };

                    var randomIndex = ThreadSafeRandom.Next(0, availableTypes.Length);

                    var randomType = availableTypes[randomIndex];

                    var result = (ResourceLocatorBase)AD.ummy(randomType);

                    return result;
                });
        }

        /// <inheritdoc />
        public Priority Priority => new FakeItEasy.Priority(1);

        /// <inheritdoc />
        public bool CanCreate(Type type)
        {
            return false;
        }

        /// <inheritdoc />
        public object Create(Type type)
        {
            return null;
        }
    }
}