// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlStreamObjectOperationsProtocol.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Protocol.Client
{
    using System.Collections.Generic;
    using Naos.Database.Domain;
    using Naos.Protocol.Domain;
    using Naos.SqlServer.Protocol.Client;
    using OBeautifulCode.Assertion.Recipes;

#pragma warning disable CS1710 // XML comment has a duplicate typeparam tag
#pragma warning disable CS1710 // XML comment has a duplicate typeparam tag
    /// <summary>
    /// SQL Server implementation of <see cref="IProtocolFactoryStreamObjectReadOperations{TId}" /> and <see cref="IProtocolFactoryStreamObjectWriteOperations{TId}" />.
    /// </summary>
    /// <typeparam name="TId">The type of the key.</typeparam>
    /// <typeparam name="TObject">The type of the object.</typeparam>
    public partial class SqlStreamObjectOperationsProtocol<TId, TObject> : IProtocolStreamObjectReadOperations<TId, TObject>, IProtocolStreamObjectWriteOperations<TId, TObject>
#pragma warning restore CS1710 // XML comment has a duplicate typeparam tag
#pragma warning restore CS1710 // XML comment has a duplicate typeparam tag
    {
        private readonly SqlStream<TId> stream;
        private readonly IReturningProtocol<GetIdFromObjectOp<TId, TObject>, TId> getIdFromObjectProtocol;
        private readonly IReturningProtocol<GetTagsFromObjectOp<TObject>, IReadOnlyDictionary<string, string>> getTagsFromObjectProtocol;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlStreamObjectOperationsProtocol{TId,TObject}"/> class.
        /// </summary>
        /// <param name="stream">The stream to operation against.</param>
        public SqlStreamObjectOperationsProtocol(
            SqlStream<TId> stream)
        {
            stream.MustForArg(nameof(stream)).NotBeNull();

            this.stream = stream;
            this.getIdFromObjectProtocol = this.stream.BuildGetIdFromObjectProtocol<TObject>();
            this.getTagsFromObjectProtocol = this.stream.BuildGetTagsFromObjectProtocol<TObject>();
        }
    }
}