// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateStreamUserOp.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Naos.CodeAnalysis.Recipes;
    using Naos.Database.Domain;
    using Naos.SqlServer.Domain;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Collection.Recipes;
    using OBeautifulCode.Representation.System;
    using OBeautifulCode.Serialization;
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = NaosSuppressBecause.CA2104_DoNotDeclareReadOnlyMutableReferenceTypes_TypeIsImmutable)]
        public static readonly IReadOnlyCollection<TypeRepresentation> SupportedProtocolTypeRepresentations = new[]
                                                                                              {
                                                                                                  typeof(IStreamReadProtocols).ToRepresentation(),
                                                                                                  typeof(IStreamWriteProtocols).ToRepresentation(),
                                                                                                  typeof(IStreamRecordHandlingProtocols).ToRepresentation(),
                                                                                                  typeof(IStreamManagementProtocols).ToRepresentation(),
                                                                                              };

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateStreamUserOp"/> class.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="clearTextPassword">The clear text password.</param>
        /// <param name="protocolsToGrantAccessFor">Support protocols to grant access for.</param>
        public CreateStreamUserOp(string userName, string clearTextPassword, IReadOnlyCollection<TypeRepresentation> protocolsToGrantAccessFor)
        {
            userName.MustForArg(nameof(userName))
                    .NotBeNullNorWhiteSpace()
                    .And()
                    .BeAlphanumeric(
                         new[]
                         {
                             '-',
                         });

            clearTextPassword.MustForArg(clearTextPassword).NotBeNull();
            protocolsToGrantAccessFor.MustForArg(nameof(protocolsToGrantAccessFor)).NotBeNullNorEmptyEnumerableNorContainAnyNulls();
            if (protocolsToGrantAccessFor.Any(_ => !SupportedProtocolTypeRepresentations.Contains(_)))
            {
                var supportedValuesString = SupportedProtocolTypeRepresentations.Select(_ => _.ToString()).ToDelimitedString(",");
                var providedValuesString = protocolsToGrantAccessFor.Select(_ => _.ToString()).ToDelimitedString(",");
                throw new ArgumentException(Invariant($"Unsupported access type provided; supported: '{supportedValuesString}', provided: '{providedValuesString}'."), nameof(protocolsToGrantAccessFor));
            }

            this.UserName = userName;
            this.ClearTextPassword = clearTextPassword;
            this.ProtocolsToGrantAccessFor = protocolsToGrantAccessFor;
        }

        /// <summary>
        /// Gets the name of the user.
        /// </summary>
        /// <value>The name of the user.</value>
        public string UserName { get; private set; }

        /// <summary>
        /// Gets the clear text password.
        /// </summary>
        /// <value>The clear text password.</value>
        public string ClearTextPassword { get; private set; }

        /// <summary>
        /// Gets the protocols to grant access for.
        /// </summary>
        /// <value>The protocols to grant access for.</value>
        public IReadOnlyCollection<TypeRepresentation> ProtocolsToGrantAccessFor { get; private set; }
    }
}