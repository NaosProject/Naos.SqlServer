// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GlobalSuppressions.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.
//
// To add a suppression to this file, right-click the message in the
// Code Analysis results, point to "Suppress Message", and click
// "In Suppression File".
// You do not need to add suppressions to this file manually.
using System.Diagnostics.CodeAnalysis;

using Naos.CodeAnalysis.Recipes;

[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "Naos.SqlServer.Domain", Justification = NaosSuppressBecause.CA1020_AvoidNamespacesWithFewTypes_OptimizeForLogicalGroupingOfTypes)]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Param", Scope = "type", Target = "Naos.SqlServer.Domain.StreamSchema+Sprocs+GetIdAddIfNecessarySerializerRepresentation+InputParamName", Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Param", Scope = "type", Target = "Naos.SqlServer.Domain.StreamSchema+Sprocs+GetIdAddIfNecessarySerializerRepresentation+OutputParamName", Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Param", Scope = "type", Target = "Naos.SqlServer.Domain.StreamSchema+Sprocs+GetIdAddIfNecessaryTypeWithoutVersion+InputParamNames", Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Param", Scope = "type", Target = "Naos.SqlServer.Domain.StreamSchema+Sprocs+GetIdAddIfNecessaryTypeWithoutVersion+OutputParamName", Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Param", Scope = "type", Target = "Naos.SqlServer.Domain.StreamSchema+Sprocs+GetIdAddIfNecessaryTypeWithVersion+InputParamNames", Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Param", Scope = "type", Target = "Naos.SqlServer.Domain.StreamSchema+Sprocs+GetIdAddIfNecessaryTypeWithVersion+OutputParamName", Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Param", Scope = "type", Target = "Naos.SqlServer.Domain.StreamSchema+Sprocs+PutObject+InputParamName", Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Param", Scope = "type", Target = "Naos.SqlServer.Domain.StreamSchema+Sprocs+PutObject+OutputParamNames", Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
[assembly: SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Object", Scope = "type", Target = "Naos.SqlServer.Domain.StreamSchema+Tables+Object", Justification = "Name makes sense in context.")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string", Scope = "member", Target = "Naos.SqlServer.Domain.StreamSchema+Sprocs+PutObject.#BuildExecuteStoredProcedureOp(System.String,System.String,System.String,System.Int32,System.String,System.String,System.Byte[],System.String)", Justification = "Name makes sense in context.")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Param", Scope = "type", Target = "Naos.SqlServer.Domain.StreamSchema+Sprocs+GetIdAddIfNecessaryTypeWithoutVersion+InputParamName")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Param", Scope = "type", Target = "Naos.SqlServer.Domain.StreamSchema+Sprocs+GetIdAddIfNecessaryTypeWithVersion+InputParamName")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Param", Scope = "type", Target = "Naos.SqlServer.Domain.StreamSchema+Sprocs+PutObject+OutputParamName")]
