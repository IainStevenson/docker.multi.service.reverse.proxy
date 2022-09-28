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
        public List< Tuple<MetadataPropertyNames, object>> Tags { get; set; } = new List<Tuple<MetadataPropertyNames, object>>();
        public long Index { get; set; }
    }

    public enum MetadataPropertyNames
    {
        OriginallyCreated ,
        OriginalDataKeys,
        SortValue,
        ChangeRequestIdentifier,
        Updated,
        NamespaceRename
    }
}