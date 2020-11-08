﻿// --------------------------------------------------------------------------------------------------------------------
// <auto-generated>
//   Generated using OBeautifulCode.CodeGen.ModelObject (1.0.130.0)
// </auto-generated>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain.Test
{
    using global::System;
    using global::System.CodeDom.Compiler;
    using global::System.Collections.Concurrent;
    using global::System.Collections.Generic;
    using global::System.Collections.ObjectModel;
    using global::System.Diagnostics.CodeAnalysis;

    using global::FakeItEasy;

    using global::Naos.Database.Domain;
    using global::Naos.Protocol.Domain;
    using global::Naos.SqlServer.Domain;

    using global::OBeautifulCode.AutoFakeItEasy;
    using global::OBeautifulCode.Math.Recipes;
    using global::OBeautifulCode.Serialization;

    /// <summary>
    /// The default (code generated) Dummy Factory.
    /// Derive from this class to add any overriding or custom registrations.
    /// </summary>
    [ExcludeFromCodeCoverage]
    [GeneratedCode("OBeautifulCode.CodeGen.ModelObject", "1.0.130.0")]
#if !NaosSqlServerSolution
    internal
#else
    public
#endif
    abstract class DefaultSqlServerDummyFactory : IDummyFactory
    {
        public DefaultSqlServerDummyFactory()
        {
            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new SqlServerConnectionDefinition
                             {
                                 Server       = A.Dummy<string>(),
                                 InstanceName = A.Dummy<string>(),
                                 DatabaseName = A.Dummy<string>(),
                                 UserName     = A.Dummy<string>(),
                                 Password     = A.Dummy<string>(),
                             });

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new BackupSqlServerDatabaseDetails(A.Dummy<string>(), A.Dummy<string>(), A.Dummy<Device>(), A.Dummy<Uri>(), A.Dummy<string>(), A.Dummy<CompressionOption>(), A.Dummy<ChecksumOption>(), A.Dummy<ErrorHandling>(), A.Dummy<Cipher>(), A.Dummy<Encryptor>(), A.Dummy<string>()));

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new DatabaseConfiguration(
                                 A.Dummy<string>(),
                                 A.Dummy<DatabaseType>(),
                                 A.Dummy<RecoveryMode>(),
                                 A.Dummy<string>(),
                                 A.Dummy<string>(),
                                 A.Dummy<string>(),
                                 A.Dummy<long>(),
                                 A.Dummy<long>(),
                                 A.Dummy<long>(),
                                 A.Dummy<string>(),
                                 A.Dummy<long>(),
                                 A.Dummy<long>(),
                                 A.Dummy<long>()));

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new RestoreFile(
                                 A.Dummy<string>(),
                                 A.Dummy<string>(),
                                 A.Dummy<string>()));

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new RestoreSqlServerDatabaseDetails(
                                 A.Dummy<ChecksumOption>(),
                                 A.Dummy<string>(),
                                 A.Dummy<string>(),
                                 A.Dummy<Device>(),
                                 A.Dummy<ErrorHandling>(),
                                 A.Dummy<string>(),
                                 A.Dummy<RecoveryOption>(),
                                 A.Dummy<ReplaceOption>(),
                                 A.Dummy<Uri>(),
                                 A.Dummy<RestrictedUserOption>()));

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new BinarySqlDataTypeRepresentation(
                                 A.Dummy<int>()));

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new DecimalSqlDataTypeRepresentation(
                                 A.Dummy<byte>(),
                                 A.Dummy<byte>()));

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new IntSqlDataTypeRepresentation());

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () =>
                {
                    var availableTypes = new[]
                    {
                        typeof(BinarySqlDataTypeRepresentation),
                        typeof(DecimalSqlDataTypeRepresentation),
                        typeof(IntSqlDataTypeRepresentation),
                        typeof(StringSqlDataTypeRepresentation),
                        typeof(UtcDateTimeSqlDataTypeRepresentation)
                    };

                    var randomIndex = ThreadSafeRandom.Next(0, availableTypes.Length);

                    var randomType = availableTypes[randomIndex];

                    var result = (SqlDataTypeRepresentationBase)AD.ummy(randomType);

                    return result;
                });

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new StringSqlDataTypeRepresentation(
                                 A.Dummy<bool>(),
                                 A.Dummy<int>()));

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new UtcDateTimeSqlDataTypeRepresentation());

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new SqlInputParameterRepresentation<Version>(
                                 A.Dummy<string>(),
                                 A.Dummy<SqlDataTypeRepresentationBase>(),
                                 A.Dummy<Version>()));

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () =>
                {
                    var availableTypes = new[]
                    {
                        typeof(SqlOutputParameterRepresentationWithResult<Version>),
                        typeof(SqlOutputParameterRepresentation<Version>)
                    };

                    var randomIndex = ThreadSafeRandom.Next(0, availableTypes.Length);

                    var randomType = availableTypes[randomIndex];

                    var result = (SqlOutputParameterRepresentationBase)AD.ummy(randomType);

                    return result;
                });

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new SqlOutputParameterRepresentationWithResult<Version>(
                                 A.Dummy<string>(),
                                 A.Dummy<SqlDataTypeRepresentationBase>(),
                                 A.Dummy<Version>()));

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new SqlOutputParameterRepresentation<Version>(
                                 A.Dummy<string>(),
                                 A.Dummy<SqlDataTypeRepresentationBase>()));

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () =>
                {
                    var availableTypes = new[]
                    {
                        typeof(SqlInputParameterRepresentation<Version>),
                        typeof(SqlOutputParameterRepresentationWithResult<Version>),
                        typeof(SqlOutputParameterRepresentation<Version>)
                    };

                    var randomIndex = ThreadSafeRandom.Next(0, availableTypes.Length);

                    var randomType = availableTypes[randomIndex];

                    var result = (SqlParameterRepresentationBase)AD.ummy(randomType);

                    return result;
                });

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new ColumnDescription(
                                 A.Dummy<string>(),
                                 A.Dummy<int>(),
                                 A.Dummy<string>(),
                                 A.Dummy<bool>(),
                                 A.Dummy<string>()));

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new ColumnRepresentation(
                                 A.Dummy<string>(),
                                 A.Dummy<SqlDataTypeRepresentationBase>()));

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new TableDescription(
                                 A.Dummy<string>(),
                                 A.Dummy<string>(),
                                 A.Dummy<string>(),
                                 A.Dummy<IReadOnlyCollection<ColumnDescription>>()));

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new TableRepresentation(
                                 A.Dummy<string>(),
                                 A.Dummy<IReadOnlyDictionary<string, ColumnRepresentation>>()));

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new SqlServerLocator(
                                 A.Dummy<string>(),
                                 A.Dummy<string>(),
                                 A.Dummy<string>(),
                                 A.Dummy<string>(),
                                 A.Dummy<string>(),
                                 A.Dummy<int?>()));

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new StoredProcedureExecutionResult(
                                 A.Dummy<ExecuteStoredProcedureOp>(),
                                 A.Dummy<IReadOnlyDictionary<string, ISqlOutputParameterRepresentationWithResult>>()));

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new ExecuteStoredProcedureOp(
                                 A.Dummy<string>(),
                                 A.Dummy<IReadOnlyDictionary<string, SqlParameterRepresentationBase>>()));

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new GetIdAddIfNecessarySerializerRepresentationOp(
                                 A.Dummy<SqlServerLocator>(),
                                 A.Dummy<SerializerRepresentation>(),
                                 A.Dummy<SerializationFormat>()));
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