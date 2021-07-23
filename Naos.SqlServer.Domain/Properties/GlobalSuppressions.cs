﻿// --------------------------------------------------------------------------------------------------------------------
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
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Param", Scope = "type", Target = "Naos.SqlServer.Domain.StreamSchema+Sprocs+GetIdAddIfNecessaryTypeWithVersion+InputParamName", Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Param", Scope = "type", Target = "Naos.SqlServer.Domain.StreamSchema+Sprocs+PutObject+OutputParamName", Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Param", Scope = "type", Target = "Naos.SqlServer.Domain.StreamSchema+Sprocs+PutRecord+OutputParamName", Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Param", Scope = "type", Target = "Naos.SqlServer.Domain.StreamSchema+Sprocs+PutRecord+InputParamName", Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Param", Scope = "type", Target = "Naos.SqlServer.Domain.StreamSchema+Sprocs+AddHandlingEntry+InputParamName", Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Param", Scope = "type", Target = "Naos.SqlServer.Domain.StreamSchema+Sprocs+AddHandlingEntry+OutputParamName", Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Param", Scope = "type", Target = "Naos.SqlServer.Domain.StreamSchema+Sprocs+GetIdAddIfNecessaryResource+InputParamName", Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Param", Scope = "type", Target = "Naos.SqlServer.Domain.StreamSchema+Sprocs+GetIdAddIfNecessaryResource+OutputParamName", Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Param", Scope = "type", Target = "Naos.SqlServer.Domain.StreamSchema+Sprocs+GetNextUniqueLong+InputParamName", Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Param", Scope = "type", Target = "Naos.SqlServer.Domain.StreamSchema+Sprocs+GetNextUniqueLong+OutputParamName", Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Param", Scope = "type", Target = "Naos.SqlServer.Domain.StreamSchema+Sprocs+GetIdAddIfNecessaryTypeWithoutVersion+InputParamName", Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Param", Scope = "type", Target = "Naos.SqlServer.Domain.StreamSchema+Sprocs+PutHandling+OutputParamName", Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Param", Scope = "type", Target = "Naos.SqlServer.Domain.StreamSchema+Sprocs+PutHandling+InputParamName", Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Param", Scope = "type", Target = "Naos.SqlServer.Domain.StreamSchema+Sprocs+GetTypeFromId+OutputParamName", Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Param", Scope = "type", Target = "Naos.SqlServer.Domain.StreamSchema+Sprocs+GetTypeFromId+InputParamName", Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Param", Scope = "type", Target = "Naos.SqlServer.Domain.StreamSchema+Sprocs+GetSerializerRepresentationFromId+OutputParamName", Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Param", Scope = "type", Target = "Naos.SqlServer.Domain.StreamSchema+Sprocs+GetSerializerRepresentationFromId+InputParamName", Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Param", Scope = "type", Target = "Naos.SqlServer.Domain.StreamSchema+Sprocs+GetIdsAddIfNecessaryTagSet+OutputParamName", Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Param", Scope = "type", Target = "Naos.SqlServer.Domain.StreamSchema+Sprocs+GetIdsAddIfNecessaryTagSet+InputParamName", Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Param", Scope = "type", Target = "Naos.SqlServer.Domain.StreamSchema+Sprocs+GetCompositeHandlingStatus+OutputParamName", Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Param", Scope = "type", Target = "Naos.SqlServer.Domain.StreamSchema+Sprocs+GetCompositeHandlingStatus+InputParamName", Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Param", Scope = "type", Target = "Naos.SqlServer.Domain.StreamSchema+Funcs+GetTagsTableVariableFromTagsXml+InputParamName", Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Param", Scope = "type", Target = "Naos.SqlServer.Domain.StreamSchema+Funcs+GetTagsTableVariableFromTagIdsXml+InputParamName", Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Param", Scope = "type", Target = "Naos.SqlServer.Domain.StreamSchema+Sprocs+CreateStreamUser+InputParamName", Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
[assembly: SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Object", Scope = "type", Target = "Naos.SqlServer.Domain.StreamSchema+Tables+Object", Justification = "Name makes sense in context.")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string", Scope = "member", Target = "Naos.SqlServer.Domain.StreamSchema+Sprocs+PutObject.#BuildExecuteStoredProcedureOp(System.String,System.String,System.String,System.Int32,System.String,System.String,System.Byte[],System.String)", Justification = "Name makes sense in context.")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string", Scope = "member", Target = "Naos.SqlServer.Domain.StreamSchema+Sprocs+GetLatestRecordById.#BuildExecuteStoredProcedureOp(System.String,System.String,Naos.Database.Domain.TypeRepresentationWithAndWithoutVersion,Naos.Database.Domain.TypeRepresentationWithAndWithoutVersion,Naos.Protocol.Domain.VersionMatchStrategy)", Justification = NaosSuppressBecause.CA1720_IdentifiersShouldNotContainTypeNames_TypeNameAddsClarityToIdentifierAndAlternativesDegradeClarity)]
[assembly: SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string", Scope = "member", Target = "Naos.SqlServer.Domain.StreamSchema+Sprocs+PutRecord.#BuildExecuteStoredProcedureOp(System.String,System.String,System.String,System.String,System.String,System.Int32,System.String,System.String,System.Nullable`1<System.DateTime>,System.String)", Justification = NaosSuppressBecause.CA1720_IdentifiersShouldNotContainTypeNames_TypeNameAddsClarityToIdentifierAndAlternativesDegradeClarity)]
[assembly: SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string", Scope = "member", Target = "Naos.SqlServer.Domain.StreamSchema+Sprocs+GetLatestRecordMetadataById.#BuildExecuteStoredProcedureOp(System.String,System.String,Naos.Database.Domain.TypeRepresentationWithAndWithoutVersion,Naos.Database.Domain.TypeRepresentationWithAndWithoutVersion,Naos.Protocol.Domain.VersionMatchStrategy)", Justification = NaosSuppressBecause.CA1720_IdentifiersShouldNotContainTypeNames_TypeNameAddsClarityToIdentifierAndAlternativesDegradeClarity)]
[assembly: SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string", Scope = "member", Target = "Naos.SqlServer.Domain.StreamSchema+Sprocs+GetLatestRecordById.#BuildExecuteStoredProcedureOp(System.String,System.String,Naos.SqlServer.Domain.IdentifiedType,Naos.SqlServer.Domain.IdentifiedType,Naos.Protocol.Domain.VersionMatchStrategy,Naos.Database.Domain.ExistingRecordNotEncounteredStrategy)", Justification = NaosSuppressBecause.CA1720_IdentifiersShouldNotContainTypeNames_TypeNameAddsClarityToIdentifierAndAlternativesDegradeClarity)]
[assembly: SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string", Scope = "member", Target = "Naos.SqlServer.Domain.StreamSchema+Sprocs+GetLatestRecordMetadataById.#BuildExecuteStoredProcedureOp(System.String,System.String,Naos.SqlServer.Domain.IdentifiedType,Naos.SqlServer.Domain.IdentifiedType,Naos.Protocol.Domain.VersionMatchStrategy,Naos.Database.Domain.ExistingRecordNotEncounteredStrategy)", Justification = NaosSuppressBecause.CA1720_IdentifiersShouldNotContainTypeNames_TypeNameAddsClarityToIdentifierAndAlternativesDegradeClarity)]
[assembly: SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string", Scope = "member", Target = "Naos.SqlServer.Domain.StreamSchema+Sprocs+PutRecord.#BuildExecuteStoredProcedureOp(System.String,Naos.SqlServer.Domain.IdentifiedSerializerRepresentation,Naos.SqlServer.Domain.IdentifiedType,Naos.SqlServer.Domain.IdentifiedType,System.String,System.String,System.Nullable`1<System.DateTime>,System.String,Naos.Database.Domain.ExistingRecordEncounteredStrategy)", Justification = NaosSuppressBecause.CA1720_IdentifiersShouldNotContainTypeNames_TypeNameAddsClarityToIdentifierAndAlternativesDegradeClarity)]
[assembly: SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string", Scope = "member", Target = "Naos.SqlServer.Domain.StreamSchema+Sprocs+PutRecord.#BuildExecuteStoredProcedureOp(System.String,Naos.SqlServer.Domain.IdentifiedSerializerRepresentation,Naos.SqlServer.Domain.IdentifiedType,Naos.SqlServer.Domain.IdentifiedType,System.String,System.String,System.Nullable`1<System.DateTime>,System.String,Naos.Database.Domain.ExistingRecordEncounteredStrategy,Naos.Protocol.Domain.VersionMatchStrategy)", Justification = NaosSuppressBecause.CA1720_IdentifiersShouldNotContainTypeNames_TypeNameAddsClarityToIdentifierAndAlternativesDegradeClarity)]
[assembly: SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string", Scope = "member", Target = "Naos.SqlServer.Domain.StreamSchema+Sprocs+PutRecord.#BuildExecuteStoredProcedureOp(System.String,Naos.SqlServer.Domain.IdentifiedSerializerRepresentation,Naos.SqlServer.Domain.IdentifiedType,Naos.SqlServer.Domain.IdentifiedType,System.String,System.String,System.Byte[],System.Nullable`1<System.DateTime>,System.String,Naos.Database.Domain.ExistingRecordEncounteredStrategy,System.Nullable`1<System.Int32>,Naos.Protocol.Domain.VersionMatchStrategy)", Justification = NaosSuppressBecause.CA1720_IdentifiersShouldNotContainTypeNames_TypeNameAddsClarityToIdentifierAndAlternativesDegradeClarity)]
[assembly: SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "bytes", Scope = "member", Target = "Naos.SqlServer.Domain.StreamSchema+Sprocs+PutRecord.#BuildExecuteStoredProcedureOp(System.String,Naos.SqlServer.Domain.IdentifiedSerializerRepresentation,Naos.SqlServer.Domain.IdentifiedType,Naos.SqlServer.Domain.IdentifiedType,System.String,System.String,System.Byte[],System.Nullable`1<System.DateTime>,System.String,Naos.Database.Domain.ExistingRecordEncounteredStrategy,System.Nullable`1<System.Int32>,Naos.Protocol.Domain.VersionMatchStrategy)", Justification = NaosSuppressBecause.CA1720_IdentifiersShouldNotContainTypeNames_TypeNameAddsClarityToIdentifierAndAlternativesDegradeClarity)]
