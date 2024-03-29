﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlServerDummyFactory.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// <auto-generated>
//   Sourced from NuGet package Naos.Build.Conventions.VisualStudioProjectTemplates.Domain.Test (1.55.45)
// </auto-generated>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain.Test
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using FakeItEasy;
    using Naos.CodeAnalysis.Recipes;
    using Naos.Database.Domain;
    using OBeautifulCode.AutoFakeItEasy;
    using OBeautifulCode.Math.Recipes;
    using OBeautifulCode.Representation.System;
    using OBeautifulCode.Serialization;

    /// <summary>
    /// A Dummy Factory for types in <see cref="Domain"/>.
    /// </summary>
#if !NaosSqlServerSolution
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [System.CodeDom.Compiler.GeneratedCode("Naos.SqlServer.Domain.Test", "See package version number")]
    internal
#else
    [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = NaosSuppressBecause.CA1505_AvoidUnmaintainableCode_DisagreeWithAssessment)]
    public
#endif
    class SqlServerDummyFactory : DefaultSqlServerDummyFactory
    {
        [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = NaosSuppressBecause.CA1505_AvoidUnmaintainableCode_DisagreeWithAssessment)]
        public SqlServerDummyFactory()
        {
            // --------------------------- Enums -----------------------------
            AutoFixtureBackedDummyFactory.ConstrainDummyToExclude(ScriptableObjectType.Invalid);
            AutoFixtureBackedDummyFactory.ConstrainDummyToExclude(JobStatus.Invalid);

            // --------------------------- Interfaces ------------------------
            AutoFixtureBackedDummyFactory.AddDummyCreator(() =>
                (ISqlOutputParameterResult)new SqlOutputParameterResult<int?>(A.Dummy<OutputParameterDefinition<int?>>(), A.Dummy<int?>()));

            AutoFixtureBackedDummyFactory.AddDummyCreator(() =>
                (IResourceLocator)A.Dummy<SqlServerLocator>());

            AutoFixtureBackedDummyFactory.AddDummyCreator(() =>
                (IDatabaseDefinition)A.Dummy<SqlServerDatabaseDefinition>());

            // --------------------------- Data Type Representation -----------------------------
            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () =>
                {
                    var supportedLength = A.Dummy<PositiveInteger>().ThatIs(_ => (_ >= 1) && (_ <= BinarySqlDataTypeRepresentation.MaxLengthConstant));

                    var result = new BinarySqlDataTypeRepresentation(supportedLength);

                    return result;
                });

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () =>
                {
                    var precision = A.Dummy<byte>().ThatIs(_ => (_ >= 1) && (_ <= 38));

                    var scale = A.Dummy<byte>().ThatIs(_ => (_ <= precision));

                    var result = new DecimalSqlDataTypeRepresentation(precision, scale);

                    return result;
                });

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () =>
                {
                    var supportUnicode = A.Dummy<bool>();

                    var supportedLength = supportUnicode
                        ? A.Dummy<PositiveInteger>().ThatIs(_ => (_ >= 1) && (_ <= StringSqlDataTypeRepresentation.MaxUnicodeLengthConstant))
                        : A.Dummy<PositiveInteger>().ThatIs(_ => (_ >= 1) && (_ <= StringSqlDataTypeRepresentation.MaxNonUnicodeLengthConstant));

                    var result = new StringSqlDataTypeRepresentation(supportUnicode, supportedLength);

                    return result;
                });

            // ------------------------ Definition ---------------------------------
            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => ThreadSafeRandom.Next(0, 2) == 0 
                    ? (ParameterDefinitionBase)A.Dummy<InputParameterDefinition<int?>>()
                    : A.Dummy<OutputParameterDefinition<int?>>());

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => (InputParameterDefinitionBase)A.Dummy<InputParameterDefinition<int?>>());

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => (OutputParameterDefinitionBase)A.Dummy<OutputParameterDefinition<int?>>());

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new InputParameterDefinition<int?>(
                    A.Dummy<string>().Replace("-", string.Empty),
                    A.Dummy<IntSqlDataTypeRepresentation>(),
                    A.Dummy<int?>()));

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new InputParameterDefinition<Version>(
                    A.Dummy<string>().Replace("-", string.Empty),
                    A.Dummy<VersionSqlDataTypeRepresentation>(),
                    A.Dummy<Version>()));

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new OutputParameterDefinition<Version>(
                    A.Dummy<string>().Replace("-", string.Empty),
                    A.Dummy<VersionSqlDataTypeRepresentation>()));

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new OutputParameterDefinition<int?>(
                    A.Dummy<string>().Replace("-", string.Empty),
                    A.Dummy<IntSqlDataTypeRepresentation>()));

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new ColumnDefinition(
                    A.Dummy<string>().Replace("-", string.Empty),
                    A.Dummy<SqlDataTypeRepresentationBase>()));

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new SqlServerDatabaseDefinition(
                    A.Dummy<string>().Replace("-", string.Empty),
                    A.Dummy<DatabaseType>(),
                    A.Dummy<RecoveryMode>(),
                    A.Dummy<string>().Replace("-", string.Empty),
                    A.Dummy<string>().Replace("-", string.Empty),
                    A.Dummy<long>(),
                    A.Dummy<long>(),
                    A.Dummy<long>(),
                    A.Dummy<string>().Replace("-", string.Empty),
                    A.Dummy<string>().Replace("-", string.Empty),
                    A.Dummy<long>(),
                    A.Dummy<long>(),
                    A.Dummy<long>()));

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new TableDefinition(
                    A.Dummy<string>().Replace("-", string.Empty),
                    A.Dummy<IReadOnlyList<ColumnDefinition>>()));

            // ------------------------ Management ---------------------------------
            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () =>
                {
                    var checksumOption = A.Dummy<ChecksumOption>();
                    var cipher = A.Dummy<Cipher>();
                    var compressionOption = A.Dummy<CompressionOption>();
                    var device = A.Dummy<Device>();
                    var encryptor = A.Dummy<Encryptor>();
                    var errorHandling = A.Dummy<ErrorHandling>();

                    if (cipher == Cipher.NoEncryption && encryptor != Encryptor.None)
                    {
                        encryptor = Encryptor.None;
                    }

                    if (cipher != Cipher.NoEncryption && encryptor == Encryptor.None)
                    {
                        encryptor = A.Dummy<Encryptor>().ThatIsNot(Encryptor.None);
                    }

                    if (checksumOption == ChecksumOption.Checksum && errorHandling == ErrorHandling.None)
                    {
                        errorHandling = A.Dummy<ErrorHandling>().ThatIsNot(ErrorHandling.None);
                    }

                    var backupSqlServerDatabaseDetails = new BackupSqlServerDatabaseDetails(
                        A.Dummy<string>().Replace("-", string.Empty), 
                        A.Dummy<string>().Replace("-", string.Empty), 
                        device, 
                        A.Dummy<Uri>(),
                        A.Dummy<string>().Replace("-", string.Empty),
                        compressionOption,
                        checksumOption,
                        errorHandling,
                        cipher,
                        encryptor,
                        A.Dummy<string>().Replace("-", string.Empty));

                    return backupSqlServerDatabaseDetails;
                });

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () =>
                {
                    var checksumOption = A.Dummy<ChecksumOption>();
                    var device = A.Dummy<Device>();
                    var recoveryOption = A.Dummy<RecoveryOption>();
                    var replaceOption = A.Dummy<ReplaceOption>();
                    var restrictedUserOption = A.Dummy<RestrictedUserOption>();
                    var errorHandling = A.Dummy<ErrorHandling>();

                    if (checksumOption == ChecksumOption.Checksum && errorHandling == ErrorHandling.None)
                    {
                        errorHandling = A.Dummy<ErrorHandling>().ThatIsNot(ErrorHandling.None);
                    }

                    var logFilePath = FormattableString.Invariant($"C:\\directory\\{A.Dummy<string>()}.dat");
                    var dataFilePath = FormattableString.Invariant($"C:\\directory\\{A.Dummy<string>()}.ldf");
                    return new RestoreSqlServerDatabaseDetails(
                        dataFilePath,
                        logFilePath,
                        device,
                        A.Dummy<Uri>(),
                        A.Dummy<string>().Replace("-", string.Empty),
                        checksumOption,
                        errorHandling,
                        recoveryOption,
                        replaceOption,
                        restrictedUserOption);
                });

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new TableDescription(
                    A.Dummy<string>().Replace("-", string.Empty),
                    A.Dummy<string>().Replace("-", string.Empty),
                    A.Dummy<string>().Replace("-", string.Empty),
                    A.Dummy<IReadOnlyCollection<ColumnDescription>>()));

            // ------------------------ Resource Location ---------------------------------

            // ------------------------ Stream ---------------------------------
            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new SqlOutputParameterResult<int?>(
                    A.Dummy<OutputParameterDefinition<int?>>(),
                    A.Dummy<int?>()));

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new SqlServerStreamConfig(
                    A.Dummy<string>(),
                    A.Dummy<StreamAccessKinds>().Whose(_ => _ != StreamAccessKinds.None),
                    TimeSpan.FromSeconds(A.Dummy<PositiveDouble>()),
                    TimeSpan.FromSeconds(A.Dummy<PositiveDouble>()),
                    A.Dummy<SerializerRepresentation>(),
                    A.Dummy<SerializationFormat>(),
                    Some.ReadOnlyDummies<SqlServerLocator>().ToList()));

            // ------------------------ Operations ---------------------------------
            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new CreateStreamUserOp(
                    A.Dummy<Guid>().ToString(),
                    A.Dummy<Guid>().ToString(),
                    A.Dummy<Guid>().ToString(),
                    CreateStreamUserOp.SupportedStreamAccessKinds.ElementAt(
                        ThreadSafeRandom.Next(0, CreateStreamUserOp.SupportedStreamAccessKinds.Count - 1)),
                    A.Dummy<bool>()));

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new DeleteDatabaseOp(
                    A.Dummy<string>().Replace("-", string.Empty)));

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new UpdateStreamStoredProceduresOp(A.Dummy<RecordTagAssociationManagementStrategy>(), A.Dummy<PositiveInteger>()));
        }
    }
}
