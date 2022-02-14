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
    using Naos.Database.Domain;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Collection.Recipes;
    using OBeautifulCode.Enum.Recipes;
    using OBeautifulCode.Type;
    using static System.FormattableString;

    /// <summary>
    /// Create a user with isolated permissions to the stream protocols set type specified.
    /// </summary>
    public partial class CreateStreamUserOp : VoidOperationBase
    {
        /// <summary>
        /// The <see cref="Database.Domain.StreamAccessKinds"/>'s that stream users can be created with access to.
        /// </summary>
        public static readonly IReadOnlyCollection<StreamAccessKinds> SupportedStreamAccessKinds =
            new[]
            {
                Database.Domain.StreamAccessKinds.Read,
                Database.Domain.StreamAccessKinds.Write,
                Database.Domain.StreamAccessKinds.Handle,
                Database.Domain.StreamAccessKinds.Manage,
            };

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateStreamUserOp"/> class.
        /// </summary>
        /// <param name="loginName">The login name of the user.</param>
        /// <param name="userName">The username of the user.</param>
        /// <param name="clearTextPassword">The clear text password.</param>
        /// <param name="streamAccessKinds"><see cref="Database.Domain.StreamAccessKinds"/> to grant access for.</param>
        /// <param name="shouldCreateLogin">A value indicating whether or not to create the login or look it up.</param>
        public CreateStreamUserOp(
            string loginName,
            string userName,
            string clearTextPassword,
            StreamAccessKinds streamAccessKinds,
            bool shouldCreateLogin)
        {
            userName.MustForArg(nameof(userName)).NotBeNullNorWhiteSpace().And().BeAlphanumeric(new[] { '-' });
            streamAccessKinds.MustForArg(nameof(streamAccessKinds)).NotBeEqualTo(Database.Domain.StreamAccessKinds.None);

            if (shouldCreateLogin)
            {
                clearTextPassword.MustForArg(nameof(clearTextPassword)).NotBeNullNorWhiteSpace();
            }
            else
            {
                loginName.MustForArg(nameof(loginName)).NotBeNullNorWhiteSpace();
            }

            if (!string.IsNullOrWhiteSpace(loginName))
            {
                loginName.MustForArg(nameof(loginName)).BeAlphanumeric(new[] { '-' });
            }

            var individualStreamAccessKinds = streamAccessKinds.GetIndividualFlags<StreamAccessKinds>();
            if (individualStreamAccessKinds.Except(CreateStreamUserOp.SupportedStreamAccessKinds).Any())
            {
                var supportedValuesString = SupportedStreamAccessKinds.Select(_ => _.ToString()).ToDelimitedString(",");

                var providedValuesString = individualStreamAccessKinds.Select(_ => _.ToString()).ToDelimitedString(",");

                throw new ArgumentException(Invariant($"Unsupported access type provided; supported: '{supportedValuesString}', provided: '{providedValuesString}'."), nameof(streamAccessKinds));
            }

            this.LoginName = loginName;
            this.UserName = userName;
            this.ClearTextPassword = clearTextPassword;
            this.StreamAccessKinds = streamAccessKinds;
            this.ShouldCreateLogin = shouldCreateLogin;
        }

        /// <summary>
        /// Gets the name of the login name.
        /// </summary>
        public string LoginName { get; private set; }

        /// <summary>
        /// Gets the username of the user.
        /// </summary>
        public string UserName { get; private set; }

        /// <summary>
        /// Gets the clear text password.
        /// </summary>
        public string ClearTextPassword { get; private set; }

        /// <summary>
        /// Gets the <see cref="StreamAccessKinds"/> to grant access for.
        /// </summary>
        public StreamAccessKinds StreamAccessKinds { get; private set; }

        /// <summary>
        /// Gets a value indicating whether a login should be created or looked up by name.
        /// </summary>
        public bool ShouldCreateLogin { get; private set; }
    }
}