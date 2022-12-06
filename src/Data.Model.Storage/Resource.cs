using CSharpVitamins;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Data.Model.Storage
{
    public class Resource : IResource, IStorageItem
    {        
        /// <summary>
        /// The resource identifier.
        /// </summary>
        [BsonElement("_id")] [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; } = Guid.NewGuid();


        /// <summary>
        /// The server handled tag for concurrency management.
        /// </summary>
        public string Etag { get; set; }= (ShortGuid)Guid.NewGuid().ToString();
    
        /// <summary>
        /// Metadata about the history of the resource.
        /// </summary>
        public IStorageMetadata Metadata { get; set; } = new StorageMetadata();

        /// <summary>
        /// The client specified storage namespace for this resource. Note: resources can me moved wrt namespaces via put.
        /// </summary>
        [BsonSerializer(typeof(DynamicSerializer))] public dynamic Content { get; set; }

        /// <summary>
        /// The soft storage classification provided by the client.
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// The orignal resource creation time stamp.
        /// </summary>
        [BsonRepresentation(BsonType.String)] public DateTimeOffset Created { get; set; } = DateTimeOffset.UtcNow;
        /// <summary>
        /// The last mofified time stamp for the resource.
        /// </summary>
        [BsonRepresentation(BsonType.String)] public DateTimeOffset? Modified { get; set; }
        /// <summary>
        /// The resource owner identifier.
        /// </summary>
        [BsonRepresentation(BsonType.String)] public Guid OwnerId { get; set; }
        /// <inheritdoc/>
        [BsonRepresentation(BsonType.String)] public DateTimeOffset? Deleted { get; set; }
    }

}
