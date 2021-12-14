// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateStreamUserOp.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Naos.CodeAnalysis.Recipes;
    using Naos.Database.Domain;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Collection.Recipes;
    using OBeautifulCode.Representation.System;
    using OBeautifulCode.Type;
    using static System.FormattableString;

    /// <summary>
    /// Create a user with isolated permissions to the stream protocols set type specified.
    /// </summary>
    public partial class CreateStreamUserOp : VoidOperationBase
    {
        /// <summary>
        /// The <see cref="TypeRepresentation"/>'s of the supported protocols sets that stream users can be created with.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = NaosSuppressBecause.CA2104_DoNotDeclareReadOnlyMutableReferenceTypes_TypeIsImmutable)]
        public static readonly IReadOnlyCollection<TypeRepresentation> VersionlessSupportedProtocolTypeRepresentations = new[]
        {
            typeof(IStreamReadProtocols).ToRepresentation().RemoveAssemblyVersions(),
            typeof(IStreamWriteProtocols).ToRepresentation().RemoveAssemblyVersions(),
            typeof(IStreamRecordHandlingProtocols).ToRepresentation().RemoveAssemblyVersions(),
            typeof(IStreamManagementProtocols).ToRepresentation().RemoveAssemblyVersions(),
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateStreamUserOp"/> class.
        /// </summary>
        /// <param name="userName">The username of the user.</param>
        /// <param name="clearTextPassword">The clear text password.</param>
        /// <param name="protocolsToGrantAccessFor">Support protocols to grant access for.</param>
        public CreateStreamUserOp(
            string userName,
            string clearTextPassword,
            IReadOnlyCollection<TypeRepresentation> protocolsToGrantAccessFor)
        {
            userName.MustForArg(nameof(userName)).NotBeNullNorWhiteSpace().And().BeAlphanumeric(new[] { '-' });
            clearTextPassword.MustForArg(nameof(clearTextPassword)).NotBeNullNorWhiteSpace();
            protocolsToGrantAccessFor.MustForArg(nameof(protocolsToGrantAccessFor)).NotBeNullNorEmptyEnumerableNorContainAnyNulls();

            if (protocolsToGrantAccessFor.Select(_ => _.RemoveAssemblyVersions()).Any(_ => !VersionlessSupportedProtocolTypeRepresentations.Contains(_)))
            {
                var supportedValuesString = VersionlessSupportedProtocolTypeRepresentations.Select(_ => _.ToString()).ToDelimitedString(",");

                var providedValuesString = protocolsToGrantAccessFor.Select(_ => _.ToString()).ToDelimitedString(",");

                throw new ArgumentException(Invariant($"Unsupported access type provided; supported: '{supportedValuesString}', provided: '{providedValuesString}'."), nameof(protocolsToGrantAccessFor));
            }

            this.UserName = userName;
            this.ClearTextPassword = clearTextPassword;
            this.ProtocolsToGrantAccessFor = protocolsToGrantAccessFor;
        }

        /// <summary>
        /// Gets the username of the user.
        /// </summary>
        public string UserName { get; private set; }

        /// <summary>
        /// Gets the clear text password.
        /// </summary>
        public string ClearTextPassword { get; private set; }

        /// <summary>
        /// Gets the protocols to grant access for.
        /// </summary>
        public IReadOnlyCollection<TypeRepresentation> ProtocolsToGrantAccessFor { get; private set; }
    }
}