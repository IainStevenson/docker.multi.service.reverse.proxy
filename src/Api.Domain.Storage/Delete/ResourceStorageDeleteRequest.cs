using MediatR;

namespace Api.Domain.Storage.Delete
{
    public class ResourceStorageDeleteRequest : IRequest<ResourceStorageDeleteResponse>
    {

        /// <summary>
        ///  Include teh resource if it has this identifier
        /// </summary>
        public Guid Id { get; set; } = Guid.Empty;
        /// <summary>
        /// Include the reosurce if it is owned by this owner identifier.
        /// </summary>
        public Guid OwnerId { get; set; } = Guid.Empty;
        /// <summary>
        /// Used for tracking the change.
        /// </summary>
        public Guid RequestId { get; set; } = Guid.Empty;
        /// <summary>
        /// Include the resource if it has this namespace.
        /// </summary>
        public string Namespace { get; set; } = "my";
        /// <summary>
        /// Include the resource if its current ETag is within this collection.
        /// </summary>
        public List<string> ETags { get; set; } = new List<string>();

        /// <summary>
        /// Include the resource if it remains unchanged since the specified time.
        /// </summary>
        public DateTimeOffset IsUnchangedSince { get; set; } 

    }
}
