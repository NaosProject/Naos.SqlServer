﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializationConfigurationTypes.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// <auto-generated>
//   Sourced from NuGet package Naos.Build.Conventions.VisualStudioProjectTemplates.Domain.Test (1.55.45)
// </auto-generated>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain.Test
{
    using System;
    using System.CodeDom.Compiler;
    using System.Diagnostics.CodeAnalysis;

    using OBeautifulCode.Serialization.Bson;
    using OBeautifulCode.Serialization.Json;

    using Naos.SqlServer.Serialization.Bson;
    using Naos.SqlServer.Serialization.Json;

    [ExcludeFromCodeCoverage]
    [GeneratedCode("Naos.Build.Conventions.VisualStudioProjectTemplates.Domain.Test", "1.55.45")]
    public static class SerializationConfigurationTypes
    {
        public static BsonSerializationConfigurationType BsonSerializationConfigurationType => typeof(SqlServerBsonSerializationConfiguration).ToBsonSerializationConfigurationType();

        public static JsonSerializationConfigurationType JsonSerializationConfigurationType => typeof(SqlServerJsonSerializationConfiguration).ToJsonSerializationConfigurationType();
    }
}
