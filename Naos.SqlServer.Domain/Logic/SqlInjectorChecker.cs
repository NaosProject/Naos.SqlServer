// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlInjectorChecker.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System;
    using System.IO;
    using OBeautifulCode.Assertion.Recipes;

    /// <summary>
    /// Utility methods to guard against SQL Injection, this is not ideal and should not be a substitute for traditional guards, it's intention is for when you must concatenate for dynamic SQL.
    /// </summary>
    public static class SqlInjectorChecker
    {
        /// <summary>
        /// Throws an ArgumentException if input is not a valid path.
        /// </summary>
        /// <param name="pathToCheck">Path to check.</param>
        /// <param name="customErrorMessage">Optional additional text for exception (like a parameter name).</param>
        public static void ThrowIfNotValidPath(
            string pathToCheck,
            string customErrorMessage = null)
        {
            new { pathToCheck }.AsArg().Must().NotBeNullNorWhiteSpace(customErrorMessage);

            try
            {
                var fileInfoToCheck = new FileInfo(pathToCheck);

                new { fileInfoToCheck }.AsOp().Must().NotBeNull(customErrorMessage);
            }
            catch (Exception)
            {
                var customErrorMessageAddIn = string.IsNullOrWhiteSpace(customErrorMessage)
                    ? string.Empty
                    : FormattableString.Invariant($"({customErrorMessage})");

                throw new ArgumentException("The provided path: " + pathToCheck + " is invalid " + customErrorMessageAddIn + ".");
            }

            if (pathToCheck.Contains("'") || pathToCheck.Contains("\"") || pathToCheck.Contains(";"))
            {
                var customErrorMessageAddIn = string.IsNullOrWhiteSpace(customErrorMessage)
                    ? string.Empty
                    : FormattableString.Invariant($"({customErrorMessage})");

                throw new ArgumentException("The provided path: " + pathToCheck + " contains either quotes or single quotes or semicolons which are not allowed " + customErrorMessageAddIn + ".");
            }
        }
    }
}
