﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace Data.Model.Storage
{
    /// <summary>
    /// This data is for internal system use, and can form a raw set of data for auditing.
    /// </summary>
    public class StorageMetadata : IStorageMetadata
    {

        public StorageMetadata()
        {   
        }
        
        [BsonRepresentation(BsonType.String)] 
        public Guid RequestId { get; set; }      
        public List< Tag> Tags { get; set; } = new List<Tag>();
        public long Index { get; set; }
    }

    public class Tag
    {
        public MetadataPropertyNames Name { get; set; }
        public object Value { get; set; }
    }

    public enum MetadataPropertyNames
    {
        OriginallyCreated ,
        OriginalDataKeys,
        SortValue,
        ChangeRequestIdentifier,
        Updated,
        NamespaceRename,
        
    }
}