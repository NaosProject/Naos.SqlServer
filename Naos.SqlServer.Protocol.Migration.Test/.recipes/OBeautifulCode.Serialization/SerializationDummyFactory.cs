// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializationDummyFactory.cs" company="OBeautifulCode">
//   Copyright (c) OBeautifulCode 2018. All rights reserved.
// </copyright>
// <auto-generated>
//   Sourced from NuGet package. Will be overwritten with package update except in OBeautifulCode.Serialization.Test source.
// </auto-generated>
// --------------------------------------------------------------------------------------------------------------------

namespace OBeautifulCode.Serialization.Test
{
    using OBeautifulCode.AutoFakeItEasy;

    using OBeautifulCode.Serialization;

    /// <summary>
    /// A Dummy Factory for types in <see cref="OBeautifulCode.Serialization"/>.
    /// </summary>
#if !OBeautifulCodeSerializationSolution
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [System.CodeDom.Compiler.GeneratedCode("OBeautifulCode.Serialization.Test", "See package version number")]
    internal
#else
    public
#endif
    class SerializationDummyFactory : DefaultSerializationDummyFactory
    {
        public SerializationDummyFactory()
        {
            AutoFixtureBackedDummyFactory.ConstrainDummyToExclude(SerializationKind.Invalid, SerializationKind.Proprietary);
            AutoFixtureBackedDummyFactory.ConstrainDummyToExclude(SerializationFormat.Invalid);

            #if OBeautifulCodeSerializationSolution
            AutoFixtureBackedDummyFactory.UseRandomConcreteSubclassForDummy<KeyOrValueObjectHierarchyBase>();
            AutoFixtureBackedDummyFactory.UseRandomConcreteSubclassForDummy<TestBase>();
            #endif
        }
    }
}