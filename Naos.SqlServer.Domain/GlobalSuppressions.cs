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

[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Param", Scope = "type", Target = "Naos.SqlServer.Domain.StreamSchema+Sprocs+GetStreamDetails+OutputParamName", Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
[assembly: SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "UnHandled", Scope = "member", Target = "Naos.SqlServer.Domain.StreamSchema+Sprocs+PutHandling+InputParamName.#IsUnHandledRecord", Justification = NaosSuppressBecause.CA1702_CompoundWordsShouldBeCasedCorrectly_AnalyzerIsIncorrectlyDetectingCompoundWords)]
[assembly: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Un", Scope = "member", Target = "Naos.SqlServer.Domain.StreamSchema+Sprocs+PutHandling+InputParamName.#IsUnHandledRecord", Justification = NaosSuppressBecause.CA1709_IdentifiersShouldBeCasedCorrectly_CasingIsAsPreferred)]
[assembly: SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string", Scope = "member", Target = "Naos.SqlServer.Domain.StreamSchema+Sprocs+GetLatestRecordById.#BuildExecuteStoredProcedureOp(System.String,System.String,Naos.SqlServer.Domain.IdentifiedType,Naos.SqlServer.Domain.IdentifiedType,OBeautifulCode.Type.VersionMatchStrategy,Naos.Database.Domain.RecordNotFoundStrategy)", Justification = NaosSuppressBecause.CA1720_IdentifiersShouldNotContainTypeNames_TypeNameAddedToIdentifierForTestsWhereTypeIsPrimaryConcern)]
[assembly: SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string", Scope = "member", Target = "Naos.SqlServer.Domain.StreamSchema+Sprocs+GetLatestRecordMetadataById.#BuildExecuteStoredProcedureOp(System.String,System.String,Naos.SqlServer.Domain.IdentifiedType,Naos.SqlServer.Domain.IdentifiedType,OBeautifulCode.Type.VersionMatchStrategy,Naos.Database.Domain.RecordNotFoundStrategy)", Justification = NaosSuppressBecause.CA1720_IdentifiersShouldNotContainTypeNames_TypeNameAddedToIdentifierForTestsWhereTypeIsPrimaryConcern)]
[assembly: SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "bytes", Scope = "member", Target = "Naos.SqlServer.Domain.StreamSchema+Sprocs+PutRecord.#BuildExecuteStoredProcedureOp(System.String,Naos.SqlServer.Domain.IdentifiedSerializerRepresentation,Naos.SqlServer.Domain.IdentifiedType,Naos.SqlServer.Domain.IdentifiedType,System.Nullable`1<System.Int64>,System.String,System.String,System.Byte[],System.Nullable`1<System.DateTime>,System.String,Naos.Database.Domain.ExistingRecordStrategy,System.Nullable`1<System.Int32>,OBeautifulCode.Type.VersionMatchStrategy)", Justification = NaosSuppressBecause.CA1720_IdentifiersShouldNotContainTypeNames_TypeNameAddedToIdentifierForTestsWhereTypeIsPrimaryConcern)]
[assembly: SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string", Scope = "member", Target = "Naos.SqlServer.Domain.StreamSchema+Sprocs+PutRecord.#BuildExecuteStoredProcedureOp(System.String,Naos.SqlServer.Domain.IdentifiedSerializerRepresentation,Naos.SqlServer.Domain.IdentifiedType,Naos.SqlServer.Domain.IdentifiedType,System.Nullable`1<System.Int64>,System.String,System.String,System.Byte[],System.Nullable`1<System.DateTime>,System.String,Naos.Database.Domain.ExistingRecordStrategy,System.Nullable`1<System.Int32>,OBeautifulCode.Type.VersionMatchStrategy)", Justification = NaosSuppressBecause.CA1720_IdentifiersShouldNotContainTypeNames_TypeNameAddedToIdentifierForTestsWhereTypeIsPrimaryConcern)]