using System;

namespace Storage
{

    /// <summary>
    /// Defines the minimum structure of a storage item class 
    /// </summary>
    public interface IStorageItem
    {
        /// <summary>
        /// The unique identifier of the item, should be set by the class contrstructor
        /// </summary>
        Guid Id { get; set; }
        /// <summary>
        /// The Utc date and time it was created, should be set by the class constructor
        /// </summary>
        DateTimeOffset Created { get; set; }
        /// <summary>
        /// The Utc time it was modified - if any, will be set by the storage system
        /// </summary>
        DateTimeOffset? Modified { get; set; }
        /// <summary>
        /// The system assigned etag to provide concurrency control, will be set by the storage system
        /// </summary>
        string ETag { get; set; }
    }
}
