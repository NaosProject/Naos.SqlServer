// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlStreamObjectOperationsProtocol.Put.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Protocol.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Naos.Database.Domain;
    using Naos.Protocol.Domain;
    using Naos.SqlServer.Domain;
    using SerializationFormat = OBeautifulCode.Serialization.SerializationFormat;

#pragma warning disable CS1710 // XML comment has a duplicate typeparam tag
#pragma warning disable CS1710 // XML comment has a duplicate typeparam tag
    /// <summary>
    /// SQL Server implementation of <see cref="IProtocolFactoryStreamObjectReadOperations{TId}" /> and <see cref="IProtocolFactoryStreamObjectWriteOperations{TId}" />.
    /// </summary>
    /// <typeparam name="TId">The type of the key.</typeparam>
    /// <typeparam name="TObject">The type of the object.</typeparam>
    public partial class SqlStreamObjectOperationsProtocol<TId, TObject>
#pragma warning restore CS1710 // XML comment has a duplicate typeparam tag
#pragma warning restore CS1710 // XML comment has a duplicate typeparam tag
    {
        /// <inheritdoc />
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "sprocResult", Justification = "Contains the ID but this is a void operation.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Should dispose correctly.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "Name is built internally.")]
        public void Execute(PutOp<TObject> operation)
        {
            var id = this.getIdFromObjectProtocol.Execute(new GetIdFromObjectOp<TId, TObject>(operation.ObjectToPut));
            var resourceLocator = this.stream.ResourceLocatorProtocol.Execute(new GetResourceLocatorByIdOp<TId>(id));
            if (resourceLocator is SqlServerLocator sqlServerLocator)
            {
                var objectType = operation.ObjectToPut?.GetType() ?? typeof(TObject);
                var objectAssemblyQualifiedNameWithoutVersion = objectType.AssemblyQualifiedName;
                var objectAssemblyQualifiedNameWithVersion = objectType.AssemblyQualifiedName;
                var describedSerializer = this.stream.GetDescribedSerializer(sqlServerLocator);
                var tagsXml = this.GetTagsXmlString(operation.ObjectToPut);

                var serializedObjectString = describedSerializer.SerializationFormat != SerializationFormat.String
                    ? null
                    : describedSerializer.Serializer.SerializeToString(operation.ObjectToPut);
                var serializedObjectBinary = describedSerializer.SerializationFormat != SerializationFormat.Binary
                    ? null
                    : describedSerializer.Serializer.SerializeToBytes(operation.ObjectToPut);

                var serializedObjectId = (id is string stringKey) ? stringKey : describedSerializer.Serializer.SerializeToString(id);

                var storedProcOp = StreamSchema.Sprocs.PutObject.BuildExecuteStoredProcedureOp(
                    this.stream.Name,
                    objectAssemblyQualifiedNameWithoutVersion,
                    objectAssemblyQualifiedNameWithVersion,
                    describedSerializer.SerializerRepresentationId,
                    serializedObjectId,
                    serializedObjectString,
                    serializedObjectBinary,
                    tagsXml);

                var sqlProtocol = this.stream.BuildSqlOperationsProtocol(sqlServerLocator);
                var sprocResult = sqlProtocol.Execute(storedProcOp); // should this be returning with the ID??? Dangerous b/c it blurs the contract, opens avenues for coupling and misuse...
            }
            else
            {
                throw SqlServerLocator.BuildInvalidLocatorException(resourceLocator.GetType());
            }
        }

        private string GetTagsXmlString(
            TObject objectToPut)
        {
            var tags = this.getTagsFromObjectProtocol.Execute(new GetTagsFromObjectOp<TObject>(objectToPut));
            if (!tags.Any())
            {
                return null;
            }

            var tagsXmlBuilder = new StringBuilder();
            tagsXmlBuilder.Append("<Tags>");
            foreach (var tag in tags ?? new Dictionary<string, string>())
            {
                var escapedKey = new XElement("ForEscapingOnly", tag.Key).LastNode.ToString();
                var escapedValue = tag.Value == null ? null : new XElement("ForEscapingOnly", tag.Value).LastNode.ToString();
                tagsXmlBuilder.Append("<Tag ");
                if (escapedValue == null)
                {
                    tagsXmlBuilder.Append(FormattableString.Invariant($"Key=\"{escapedKey}\" Value=null"));
                }
                else
                {
                    tagsXmlBuilder.Append(FormattableString.Invariant($"Key=\"{escapedKey}\" Value=\"{escapedValue}\""));
                }

                tagsXmlBuilder.Append("/>");
            }

            tagsXmlBuilder.Append("</Tags>");
            var result = tagsXmlBuilder.ToString();
            return result;
        }

        /// <inheritdoc />
        public Task ExecuteAsync(
            PutOp<TObject> operation)
        {
            throw new NotImplementedException();
        }
    }
}