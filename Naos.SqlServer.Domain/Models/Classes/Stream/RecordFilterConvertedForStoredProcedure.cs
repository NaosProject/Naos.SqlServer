// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RecordFilterConvertedForStoredProcedure.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using Naos.Database.Domain;
    using OBeautifulCode.Type;

    /// <summary>
    /// Model to contain the information in a <see cref="Naos.Database.Domain.RecordFilter"/> that is converted to a form that can be passed to a stored procedure.
    /// </summary>
    public partial class RecordFilterConvertedForStoredProcedure : IModelViaCodeGen
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RecordFilterConvertedForStoredProcedure"/> class.
        /// </summary>
        /// <param name="internalRecordIdsCsv">The internal record identifiers as CSV.</param>
        /// <param name="identifierTypeIdsCsv">The identifier type identifiers as CSV.</param>
        /// <param name="objectTypeIdsCsv">The object type identifiers as CSV.</param>
        /// <param name="stringIdsToMatchXml">The string identifiers to match as XML (key is string identifier and value is the appropriate type identifier per the <see cref="OBeautifulCode.Type.VersionMatchStrategy"/>).</param>
        /// <param name="tagIdsCsv">The tag identifiers as CSV.</param>
        /// <param name="tagMatchStrategy">The <see cref="Naos.Database.Domain.TagMatchStrategy"/>.</param>
        /// <param name="versionMatchStrategy">The <see cref="OBeautifulCode.Type.VersionMatchStrategy"/>.</param>
        /// <param name="deprecatedIdEventTypeIdsCsv">The deprecated identifier event type identifiers as CSV.</param>
        public RecordFilterConvertedForStoredProcedure(
            string internalRecordIdsCsv,
            string identifierTypeIdsCsv,
            string objectTypeIdsCsv,
            string stringIdsToMatchXml,
            string tagIdsCsv,
            TagMatchStrategy tagMatchStrategy,
            VersionMatchStrategy versionMatchStrategy,
            string deprecatedIdEventTypeIdsCsv)
        {
            this.InternalRecordIdsCsv = internalRecordIdsCsv;
            this.IdentifierTypeIdsCsv = identifierTypeIdsCsv;
            this.ObjectTypeIdsCsv = objectTypeIdsCsv;
            this.StringIdsToMatchXml = stringIdsToMatchXml;
            this.TagIdsCsv = tagIdsCsv;
            this.TagMatchStrategy = tagMatchStrategy;
            this.VersionMatchStrategy = versionMatchStrategy;
            this.DeprecatedIdEventTypeIdsCsv = deprecatedIdEventTypeIdsCsv;
        }

        /// <summary>
        /// Gets the internal record identifiers as CSV.
        /// </summary>
        public string InternalRecordIdsCsv { get; private set; }

        /// <summary>
        /// Gets the identifier type identifiers as CSV.
        /// </summary>
        public string IdentifierTypeIdsCsv { get; private set; }

        /// <summary>
        /// Gets the object type identifiers as CSV.
        /// </summary>
        public string ObjectTypeIdsCsv { get; private set; }

        /// <summary>
        /// Gets the string identifiers to match as XML (key is string identifier and value is the appropriate type identifier per the <see cref="OBeautifulCode.Type.VersionMatchStrategy"/>).
        /// </summary>
        public string StringIdsToMatchXml { get; private set; }

        /// <summary>
        /// Gets the tag identifiers as CSV.
        /// </summary>
        public string TagIdsCsv { get; private set; }

        /// <summary>
        /// Gets the <see cref="Naos.Database.Domain.TagMatchStrategy"/>.
        /// </summary>
        public TagMatchStrategy TagMatchStrategy { get; private set; }

        /// <summary>
        /// Gets the <see cref="OBeautifulCode.Type.VersionMatchStrategy"/>.
        /// </summary>
        public VersionMatchStrategy VersionMatchStrategy { get; private set; }

        /// <summary>
        /// Gets the deprecated identifier event type identifiers as CSV.
        /// </summary>
        public string DeprecatedIdEventTypeIdsCsv { get; private set; }
    }
}
