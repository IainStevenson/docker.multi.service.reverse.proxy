using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace Api.Domain.Storage.Delete
{
    [ExcludeFromCodeCoverage]
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
        public List<string> IsNotETags { get; set; } = new List<string>();

        /// <summary>
        /// Include the resource if it remains unchanged since the specified time.
        /// The default of <see cref="DateTimeOffset.MaxValue"/> indicates that it is not specified and 
        /// therefore any record has been unmodified since that time.
        /// </summary>
        public DateTimeOffset IsUnchangedSince { get; set; }  = DateTimeOffset.MaxValue;

    }
}
