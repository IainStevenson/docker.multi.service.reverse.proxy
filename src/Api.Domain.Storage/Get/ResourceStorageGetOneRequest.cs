using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace Api.Domain.Storage.Get
{
    [ExcludeFromCodeCoverage]
    public class ResourceStorageGetOneRequest : IRequest<ResourceStorageGetOneResponse>
    {
        public Guid Id { get; set; } = Guid.Empty;
        public string ContentNamespace { get; set; } = "my";
        public Guid OwnerId { get; set; } = Guid.Empty;
        public Guid RequestId { get; set; } = Guid.Empty;
        public List<string> IfNotETags { get; set; } = new List<string>();
        /// <summary>
        /// Note: the default value of <see cref="DateTimeOffset.MinValue"/> resutls in getting everything ever that matches other constraints.
        /// To exclude a get based on near past changes set this value to an appropriate point in time.
        /// </summary>
        public DateTimeOffset IfModifiedSince { get; set; }
        public bool IfIsDeleted { get; internal set; }
    }
}
