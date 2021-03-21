using CSharpVitamins;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Data.Model.Storage
{
    public class Resource : IResource, IStorageItem
    {
        public Resource()
        {
            Id = Guid.NewGuid();
            Created = DateTimeOffset.UtcNow;
            Etag = (ShortGuid)Guid.NewGuid().ToString();
        }

         [BsonElement("_id")] [BsonRepresentation(BsonType.String)] public Guid Id { get; set; }

        public string Etag { get; set; }
    

        public IStorageMetadata Metadata { get; set; } = new StorageMetadata();

        [BsonSerializer(typeof(DynamicSerializer))] public dynamic Content { get; set; }
        public string Namespace { get; set; }
        [BsonRepresentation(BsonType.String)] public DateTimeOffset Created { get; set; } = DateTimeOffset.UtcNow;
        [BsonRepresentation(BsonType.String)] public DateTimeOffset? Modified { get; set; }
        [BsonRepresentation(BsonType.String)] public Guid OwnerId { get; set; }
        
    }

}
