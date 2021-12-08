// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Cipher.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Specifies the algorithm used to encrypt/decrypt backups.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces", Justification = "Keeping this for now as it's not used.")]
    public enum Cipher
    {
        /// <summary>
        /// No encryption.
        /// </summary>
        NoEncryption,

        /// <summary>
        /// Use AES 128.
        /// </summary>
        Aes128,

        /// <summary>
        /// Use AES 192.
        /// </summary>
        Aes192,

        /// <summary>
        /// Use AES 256.
        /// </summary>
        Aes256,

        /// <summary>
        /// Use triple DES.
        /// </summary>
        TripleDes3Key,
    }
}
