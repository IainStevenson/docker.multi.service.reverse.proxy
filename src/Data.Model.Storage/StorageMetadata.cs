using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace Data.Model.Storage
{
    public class StorageMetadata : IStorageMetadata
    {

        public StorageMetadata()
        {   
        }
        
        [BsonRepresentation(BsonType.String)] public Guid RequestId { get; set; }      
        public IDictionary<string, object> Tags { get; set; } = new Dictionary<string, object>();
        public long Index { get; set; }
    }
}