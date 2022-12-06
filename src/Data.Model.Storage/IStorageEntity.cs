using System;

namespace Data.Model.Storage
{
    public interface IStorageEntity : IEntity
    {
        string Etag { get; set; }
        /// <summary>
        /// The date and time offset when the entity was soft deleted. 
        /// Obviously this is a default of null and set when the client deletes the content. 
        /// In soft deletion the content is nulled and this property set to when taht happened. 
        /// Other data may record other information about the delerion event.
        /// </summary>
        DateTimeOffset? Deleted { get; set; }
    }
}
