// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BackupSqlServerDatabaseDetailsTest.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain.Test
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using FakeItEasy;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.AutoFakeItEasy;
    using OBeautifulCode.CodeAnalysis.Recipes;
    using OBeautifulCode.CodeGen.ModelObject.Recipes;
    using Xunit;

    using static System.FormattableString;

    public static partial class BackupSqlServerDatabaseDetailsTest
    {
        [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = ObcSuppressBecause.CA1505_AvoidUnmaintainableCode_DisagreeWithAssessment)]
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = ObcSuppressBecause.CA1810_InitializeReferenceTypeStaticFieldsInline_FieldsDeclaredInCodeGeneratedPartialTestClass)]
        static BackupSqlServerDatabaseDetailsTest()
        {
            ConstructorArgumentValidationTestScenarios
               .RemoveAllScenarios()

               .AddScenario(
                    () =>
                        new ConstructorArgumentValidationTestScenario<BackupSqlServerDatabaseDetails>
                        {
                            Name = "constructor should throw ArgumentException when parameter 'name' has invalid chars scenario",
                            ConstructionFunc = () =>
                                               {
                                                   var referenceObject = A.Dummy<BackupSqlServerDatabaseDetails>();

                                                   var result = new BackupSqlServerDatabaseDetails(
                                                       "not-valid",
                                                       referenceObject.Description,
                                                       referenceObject.Device,
                                                       referenceObject.BackupTo,
                                                       referenceObject.Credential,
                                                       referenceObject.CompressionOption,
                                                       referenceObject.ChecksumOption,
                                                       referenceObject.ErrorHandling,
                                                       referenceObject.Cipher,
                                                       referenceObject.Encryptor,
                                                       referenceObject.EncryptorName);

                                                   return result;
                                               },
                            ExpectedExceptionType = typeof(ArgumentException),
                            ExpectedExceptionMessageContains = new[]
                                                               {
                                                                   "name",
                                                                   "not alphanumeric",
                                                               },
                        })
               .AddScenario(
                    () =>
                        new ConstructorArgumentValidationTestScenario<BackupSqlServerDatabaseDetails>
                        {
                            Name = "constructor should throw ArgumentException when parameter 'name' is longer than 128 chars scenario",
                            ConstructionFunc = () =>
                                               {
                                                   var referenceObject = A.Dummy<BackupSqlServerDatabaseDetails>();

                                                   var result = new BackupSqlServerDatabaseDetails(
                                                       new string('a', 129),
                                                       referenceObject.Description,
                                                       referenceObject.Device,
                                                       referenceObject.BackupTo,
                                                       referenceObject.Credential,
                                                       referenceObject.CompressionOption,
                                                       referenceObject.ChecksumOption,
                                                       referenceObject.ErrorHandling,
                                                       referenceObject.Cipher,
                                                       referenceObject.Encryptor,
                                                       referenceObject.EncryptorName);

                                                   return result;
                                               },
                            ExpectedExceptionType = typeof(ArgumentException),
                            ExpectedExceptionMessageContains = new[]
                                                               {
                                                                   "Name",
                                                                   "128 characters",
                                                               },
                        })
               .AddScenario(
                    () =>
                        new ConstructorArgumentValidationTestScenario<BackupSqlServerDatabaseDetails>
                        {
                            Name = "constructor should throw ArgumentException when parameter 'description' is longer than 255 chars scenario",
                            ConstructionFunc = () =>
                                               {
                                                   var referenceObject = A.Dummy<BackupSqlServerDatabaseDetails>();

                                                   var result = new BackupSqlServerDatabaseDetails(
                                                       referenceObject.Name,
                                                       new string('a', 256),
                                                       referenceObject.Device,
                                                       referenceObject.BackupTo,
                                                       referenceObject.Credential,
                                                       referenceObject.CompressionOption,
                                                       referenceObject.ChecksumOption,
                                                       referenceObject.ErrorHandling,
                                                       referenceObject.Cipher,
                                                       referenceObject.Encryptor,
                                                       referenceObject.EncryptorName);

                                                   return result;
                                               },
                            ExpectedExceptionType = typeof(ArgumentException),
                            ExpectedExceptionMessageContains = new[]
                                                               {
                                                                   "Description",
                                                                   "255 characters",
                                                               },
                        })
               .AddScenario(
                    () =>
                        new ConstructorArgumentValidationTestScenario<BackupSqlServerDatabaseDetails>
                        {
                            Name = "constructor should throw ArgumentException when parameter 'description' has invalid chars scenario",
                            ConstructionFunc = () =>
                                               {
                                                   var referenceObject = A.Dummy<BackupSqlServerDatabaseDetails>();

                                                   var result = new BackupSqlServerDatabaseDetails(
                                                       referenceObject.Name,
                                                       "not-valid",
                                                       referenceObject.Device,
                                                       referenceObject.BackupTo,
                                                       referenceObject.Credential,
                                                       referenceObject.CompressionOption,
                                                       referenceObject.ChecksumOption,
                                                       referenceObject.ErrorHandling,
                                                       referenceObject.Cipher,
                                                       referenceObject.Encryptor,
                                                       referenceObject.EncryptorName);

                                                   return result;
                                               },
                            ExpectedExceptionType = typeof(ArgumentException),
                            ExpectedExceptionMessageContains = new[]
                                                               {
                                                                   "description",
                                                                   "not alphanumeric",
                                                               },
                        })
               .AddScenario(
                    () =>
                        new ConstructorArgumentValidationTestScenario<BackupSqlServerDatabaseDetails>
                        {
                            Name = "constructor should throw ArgumentNullException when parameter 'backupTo' is null scenario",
                            ConstructionFunc = () =>
                                               {
                                                   var referenceObject = A.Dummy<BackupSqlServerDatabaseDetails>();

                                                   var result = new BackupSqlServerDatabaseDetails(
                                                       referenceObject.Name,
                                                       referenceObject.Description,
                                                       referenceObject.Device,
                                                       null,
                                                       referenceObject.Credential,
                                                       referenceObject.CompressionOption,
                                                       referenceObject.ChecksumOption,
                                                       referenceObject.ErrorHandling,
                                                       referenceObject.Cipher,
                                                       referenceObject.Encryptor,
                                                       referenceObject.EncryptorName);

                                                   return result;
                                               },
                            ExpectedExceptionType = typeof(ArgumentNullException),
                            ExpectedExceptionMessageContains = new[]
                                                               {
                                                                   "backupTo",
                                                               },
                        })
               .AddScenario(
                    () =>
                        new ConstructorArgumentValidationTestScenario<BackupSqlServerDatabaseDetails>
                        {
                            Name = "constructor should throw ArgumentNullException when parameter 'credential' is null and Device is 'Url' scenario",
                            ConstructionFunc = () =>
                                               {
                                                   var referenceObject = A.Dummy<BackupSqlServerDatabaseDetails>();

                                                   var result = new BackupSqlServerDatabaseDetails(
                                                       referenceObject.Name,
                                                       referenceObject.Description,
                                                       Device.Url,
                                                       referenceObject.BackupTo,
                                                       null,
                                                       referenceObject.CompressionOption,
                                                       referenceObject.ChecksumOption,
                                                       referenceObject.ErrorHandling,
                                                       referenceObject.Cipher,
                                                       referenceObject.Encryptor,
                                                       referenceObject.EncryptorName);

                                                   return result;
                                               },
                            ExpectedExceptionType = typeof(ArgumentNullException),
                            ExpectedExceptionMessageContains = new[]
                                                               {
                                                                   "credential",
                                                               },
                        })
               .AddScenario(
                    () =>
                        new ConstructorArgumentValidationTestScenario<BackupSqlServerDatabaseDetails>
                        {
                            Name =
                                "constructor should throw ArgumentException when parameter 'credential' is white space and Device is 'Url' scenario",
                            ConstructionFunc = () =>
                                               {
                                                   var referenceObject = A.Dummy<BackupSqlServerDatabaseDetails>();

                                                   var result = new BackupSqlServerDatabaseDetails(
                                                       referenceObject.Name,
                                                       referenceObject.Description,
                                                       Device.Url,
                                                       referenceObject.BackupTo,
                                                       Invariant($"  {Environment.NewLine}  "),
                                                       referenceObject.CompressionOption,
                                                       referenceObject.ChecksumOption,
                                                       referenceObject.ErrorHandling,
                                                       referenceObject.Cipher,
                                                       referenceObject.Encryptor,
                                                       referenceObject.EncryptorName);

                                                   return result;
                                               },
                            ExpectedExceptionType = typeof(ArgumentException),
                            ExpectedExceptionMessageContains = new[]
                                                               {
                                                                   "credential",
                                                                   "white space",
                                                               },
                        })
               .AddScenario(
                    () =>
                        new ConstructorArgumentValidationTestScenario<BackupSqlServerDatabaseDetails>
                        {
                            Name =
                                "constructor should throw ArgumentException when parameter 'credential' is not-alphanumeric and Device is 'Url' scenario",
                            ConstructionFunc = () =>
                                               {
                                                   var referenceObject = A.Dummy<BackupSqlServerDatabaseDetails>();

                                                   var result = new BackupSqlServerDatabaseDetails(
                                                       referenceObject.Name,
                                                       referenceObject.Description,
                                                       Device.Url,
                                                       referenceObject.BackupTo,
                                                       Invariant($"not-alpha-numeric"),
                                                       referenceObject.CompressionOption,
                                                       referenceObject.ChecksumOption,
                                                       referenceObject.ErrorHandling,
                                                       referenceObject.Cipher,
                                                       referenceObject.Encryptor,
                                                       referenceObject.EncryptorName);

                                                   return result;
                                               },
                            ExpectedExceptionType = typeof(ArgumentException),
                            ExpectedExceptionMessageContains = new[]
                                                               {
                                                                   "credential",
                                                                   "alphanumeric",
                                                               },
                        })
               .AddScenario(
                    () =>
                        new ConstructorArgumentValidationTestScenario<BackupSqlServerDatabaseDetails>
                        {
                            Name =
                                "constructor should throw ArgumentException when parameter 'errorHandling' is 'None' and ChecksumOption is 'Checksum' scenario",
                            ConstructionFunc = () =>
                                               {
                                                   var referenceObject = A.Dummy<BackupSqlServerDatabaseDetails>();

                                                   var result = new BackupSqlServerDatabaseDetails(
                                                       referenceObject.Name,
                                                       referenceObject.Description,
                                                       Device.Url,
                                                       referenceObject.BackupTo,
                                                       referenceObject.Credential,
                                                       referenceObject.CompressionOption,
                                                       ChecksumOption.Checksum,
                                                       ErrorHandling.None,
                                                       referenceObject.Cipher,
                                                       referenceObject.Encryptor,
                                                       referenceObject.EncryptorName);

                                                   return result;
                                               },
                            ExpectedExceptionType = typeof(ArgumentException),
                            ExpectedExceptionMessageContains = new[]
                                                               {
                                                                   "ErrorHandling cannot be None when using checksum.",
                                                               },
                        })
               .AddScenario(
                    () =>
                        new ConstructorArgumentValidationTestScenario<BackupSqlServerDatabaseDetails>
                        {
                            Name =
                                "constructor should throw ArgumentException when parameter 'encryptor' is 'None' and Cipher is not 'NoEncryption' scenario",
                            ConstructionFunc = () =>
                                               {
                                                   var referenceObject = A.Dummy<BackupSqlServerDatabaseDetails>();

                                                   var cipher = A.Dummy<Cipher>().ThatIsNot(Cipher.NoEncryption);
                                                   var result = new BackupSqlServerDatabaseDetails(
                                                       referenceObject.Name,
                                                       referenceObject.Description,
                                                       referenceObject.Device,
                                                       referenceObject.BackupTo,
                                                       referenceObject.Credential,
                                                       referenceObject.CompressionOption,
                                                       referenceObject.ChecksumOption,
                                                       referenceObject.ErrorHandling,
                                                       cipher,
                                                       Encryptor.None,
                                                       referenceObject.EncryptorName);

                                                   return result;
                                               },
                            ExpectedExceptionType = typeof(ArgumentException),
                            ExpectedExceptionMessageContains = new[]
                                                               {
                                                                   "Encryptor is required when any Cipher != NoEncryption",
                                                               },
                        })
               .AddScenario(
                    () =>
                        new ConstructorArgumentValidationTestScenario<BackupSqlServerDatabaseDetails>
                        {
                            Name =
                                "constructor should throw ArgumentException when parameter 'encryptorName' is 'null' and Cipher is not 'NoEncryption' scenario",
                            ConstructionFunc = () =>
                                               {
                                                   var referenceObject = A.Dummy<BackupSqlServerDatabaseDetails>();

                                                   var cipher = A.Dummy<Cipher>().ThatIsNot(Cipher.NoEncryption);
                                                   var encryptor = A.Dummy<Encryptor>().ThatIsNot(Encryptor.None);
                                                   var result = new BackupSqlServerDatabaseDetails(
                                                       referenceObject.Name,
                                                       referenceObject.Description,
                                                       referenceObject.Device,
                                                       referenceObject.BackupTo,
                                                       referenceObject.Credential,
                                                       referenceObject.CompressionOption,
                                                       referenceObject.ChecksumOption,
                                                       referenceObject.ErrorHandling,
                                                       cipher,
                                                       encryptor,
                                                       null);

                                                   return result;
                                               },
                            ExpectedExceptionType = typeof(ArgumentNullException),
                            ExpectedExceptionMessageContains = new[]
                                                               {
                                                                   "encryptorName",
                                                               },
                        })
               .AddScenario(
                    () =>
                        new ConstructorArgumentValidationTestScenario<BackupSqlServerDatabaseDetails>
                        {
                            Name =
                                "constructor should throw ArgumentException when parameter 'encryptorName' is 'empty string' and Cipher is not 'NoEncryption' scenario",
                            ConstructionFunc = () =>
                                               {
                                                   var referenceObject = A.Dummy<BackupSqlServerDatabaseDetails>();

                                                   var cipher = A.Dummy<Cipher>().ThatIsNot(Cipher.NoEncryption);
                                                   var encryptor = A.Dummy<Encryptor>().ThatIsNot(Encryptor.None);
                                                   var result = new BackupSqlServerDatabaseDetails(
                                                       referenceObject.Name,
                                                       referenceObject.Description,
                                                       referenceObject.Device,
                                                       referenceObject.BackupTo,
                                                       referenceObject.Credential,
                                                       referenceObject.CompressionOption,
                                                       referenceObject.ChecksumOption,
                                                       referenceObject.ErrorHandling,
                                                       cipher,
                                                       encryptor,
                                                       string.Empty);

                                                   return result;
                                               },
                            ExpectedExceptionType = typeof(ArgumentException),
                            ExpectedExceptionMessageContains = new[]
                                                               {
                                                                   "encryptorName",
                                                               },
                        })
               .AddScenario(
                    () =>
                        new ConstructorArgumentValidationTestScenario<BackupSqlServerDatabaseDetails>
                        {
                            Name =
                                "constructor should throw ArgumentException when parameter 'encryptorName' is 'whitespace' and Cipher is not 'NoEncryption' scenario",
                            ConstructionFunc = () =>
                                               {
                                                   var referenceObject = A.Dummy<BackupSqlServerDatabaseDetails>();

                                                   var cipher = A.Dummy<Cipher>().ThatIsNot(Cipher.NoEncryption);
                                                   var encryptor = A.Dummy<Encryptor>().ThatIsNot(Encryptor.None);
                                                   var result = new BackupSqlServerDatabaseDetails(
                                                       referenceObject.Name,
                                                       referenceObject.Description,
                                                       referenceObject.Device,
                                                       referenceObject.BackupTo,
                                                       referenceObject.Credential,
                                                       referenceObject.CompressionOption,
                                                       referenceObject.ChecksumOption,
                                                       referenceObject.ErrorHandling,
                                                       cipher,
                                                       encryptor,
                                                       Invariant($"   {Environment.NewLine}   "));

                                                   return result;
                                               },
                            ExpectedExceptionType = typeof(ArgumentException),
                            ExpectedExceptionMessageContains = new[]
                                                               {
                                                                   "encryptorName",
                                                               },
                        })
               .AddScenario(
                    () =>
                        new ConstructorArgumentValidationTestScenario<BackupSqlServerDatabaseDetails>
                        {
                            Name =
                                "constructor should throw ArgumentException when parameter 'encryptorName' is 'invalid' and Cipher is not 'NoEncryption' scenario",
                            ConstructionFunc = () =>
                                               {
                                                   var referenceObject = A.Dummy<BackupSqlServerDatabaseDetails>();

                                                   var cipher = A.Dummy<Cipher>().ThatIsNot(Cipher.NoEncryption);
                                                   var encryptor = A.Dummy<Encryptor>().ThatIsNot(Encryptor.None);
                                                   var result = new BackupSqlServerDatabaseDetails(
                                                       referenceObject.Name,
                                                       referenceObject.Description,
                                                       referenceObject.Device,
                                                       referenceObject.BackupTo,
                                                       referenceObject.Credential,
                                                       referenceObject.CompressionOption,
                                                       referenceObject.ChecksumOption,
                                                       referenceObject.ErrorHandling,
                                                       cipher,
                                                       encryptor,
                                                       Invariant($"not-valid"));

                                                   return result;
                                               },
                            ExpectedExceptionType = typeof(ArgumentException),
                            ExpectedExceptionMessageContains = new[]
                                                               {
                                                                   "encryptorName",
                                                               },
                        });
        }

        [Fact]
        public static void Constructor___Should_succeed___If_name_is_null()
        {
            var referenceObject = A.Dummy<BackupSqlServerDatabaseDetails>();

            var result = new BackupSqlServerDatabaseDetails(
                null,
                referenceObject.Description,
                referenceObject.Device,
                referenceObject.BackupTo,
                referenceObject.Credential,
                referenceObject.CompressionOption,
                referenceObject.ChecksumOption,
                referenceObject.ErrorHandling,
                referenceObject.Cipher,
                referenceObject.Encryptor,
                referenceObject.EncryptorName);

            result.MustForTest().NotBeNull();
        }

        [Fact]
        public static void Constructor___Should_succeed___If_name_has_whitespace()
        {
            var referenceObject = A.Dummy<BackupSqlServerDatabaseDetails>();

            var result = new BackupSqlServerDatabaseDetails(
                "some white space",
                referenceObject.Description,
                referenceObject.Device,
                referenceObject.BackupTo,
                referenceObject.Credential,
                referenceObject.CompressionOption,
                referenceObject.ChecksumOption,
                referenceObject.ErrorHandling,
                referenceObject.Cipher,
                referenceObject.Encryptor,
                referenceObject.EncryptorName);

            result.MustForTest().NotBeNull();
        }

        [Fact]
        public static void Constructor___Should_succeed___If_name_has_underscores()
        {
            var referenceObject = A.Dummy<BackupSqlServerDatabaseDetails>();

            var result = new BackupSqlServerDatabaseDetails(
                "some_under_scores",
                referenceObject.Description,
                referenceObject.Device,
                referenceObject.BackupTo,
                referenceObject.Credential,
                referenceObject.CompressionOption,
                referenceObject.ChecksumOption,
                referenceObject.ErrorHandling,
                referenceObject.Cipher,
                referenceObject.Encryptor,
                referenceObject.EncryptorName);

            result.MustForTest().NotBeNull();
        }

        [Fact]
        public static void Constructor___Should_succeed___If_description_is_null()
        {
            var referenceObject = A.Dummy<BackupSqlServerDatabaseDetails>();

            var result = new BackupSqlServerDatabaseDetails(
                referenceObject.Name,
                null,
                referenceObject.Device,
                referenceObject.BackupTo,
                referenceObject.Credential,
                referenceObject.CompressionOption,
                referenceObject.ChecksumOption,
                referenceObject.ErrorHandling,
                referenceObject.Cipher,
                referenceObject.Encryptor,
                referenceObject.EncryptorName);

            result.MustForTest().NotBeNull();
        }

        [Fact]
        public static void Constructor___Should_succeed___If_description_has_whitespace()
        {
            var referenceObject = A.Dummy<BackupSqlServerDatabaseDetails>();

            var result = new BackupSqlServerDatabaseDetails(
                referenceObject.Name,
                "some white space",
                referenceObject.Device,
                referenceObject.BackupTo,
                referenceObject.Credential,
                referenceObject.CompressionOption,
                referenceObject.ChecksumOption,
                referenceObject.ErrorHandling,
                referenceObject.Cipher,
                referenceObject.Encryptor,
                referenceObject.EncryptorName);

            result.MustForTest().NotBeNull();
        }

        [Fact]
        public static void Constructor___Should_succeed___If_description_has_underscores()
        {
            var referenceObject = A.Dummy<BackupSqlServerDatabaseDetails>();

            var result = new BackupSqlServerDatabaseDetails(
                referenceObject.Name,
                "some_under_scores",
                referenceObject.Device,
                referenceObject.BackupTo,
                referenceObject.Credential,
                referenceObject.CompressionOption,
                referenceObject.ChecksumOption,
                referenceObject.ErrorHandling,
                referenceObject.Cipher,
                referenceObject.Encryptor,
                referenceObject.EncryptorName);

            result.MustForTest().NotBeNull();
        }
    }
}