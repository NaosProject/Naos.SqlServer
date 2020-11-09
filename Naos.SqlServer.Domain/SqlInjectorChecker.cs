// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlInjectorChecker.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System;
    using System.IO;
    using System.Text.RegularExpressions;

    using OBeautifulCode.Assertion.Recipes;

    /// <summary>
    /// Utility methods to guard against SQL Injection, this is not ideal and should not be a substitute for traditional guards, it's intention is for when you must concatenate for dynamic SQL.
    /// </summary>
    public static class SqlInjectorChecker
    {
        /// <summary>
        /// Throws an <see cref="ArgumentException"/> if input has any characters that are not alpha-numeric nor the space character nor the underscore character.
        /// </summary>
        /// <param name="textToCheck">Text to check.</param>
        /// <param name="customErrorMessage">Optional additional text for exception (like a parameter name).</param>
        public static void ThrowIfNotAlphanumericOrSpaceOrUnderscore(string textToCheck, string customErrorMessage = null)
        {
            new { textToCheck }.AsArg().Must().NotBeNull();

            const string pattern = @"[a-zA-Z0-9 _]*";
            Match match = Regex.Match(textToCheck, pattern);
            if (match.Value != textToCheck)
            {
                var customMessageAddIn = string.IsNullOrWhiteSpace(customErrorMessage) ? string.Empty : FormattableString.Invariant($" ({customErrorMessage})");
                throw new ArgumentException("The provided input: " + textToCheck + " is not alphanumeric and is not valid " + customMessageAddIn + ".");
            }
        }

        /// <summary>
        /// Throws an ArgumentException if input is not a valid path.
        /// </summary>
        /// <param name="pathToCheck">Path to check.</param>
        /// <param name="customErrorMessage">Optional additional text for exception (like a parameter name).</param>
        public static void ThrowIfNotValidPath(string pathToCheck, string customErrorMessage = null)
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
