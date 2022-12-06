using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;

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
        [Description("The DATETIMEOFFSET when created")]
        OriginallyCreated,
        [Description("The content keys noinated by the client")]
        OriginalDataKeys,
        [Description("Change in sort value")]
        SortValue,
        [Description("The identifier of the change request")]
        ChangeRequestIdentifier,
        [Description("The DATETIMEOFFSET when updated")]
        Updated,
        [Description("The new namesapce")]
        NamespaceRename,
        [Description("The DATETIMEOFFSET when deleted")]
        Deleted

    }
}