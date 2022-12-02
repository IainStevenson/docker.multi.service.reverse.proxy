using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace Api.Domain.Storage.Post
{
    [ExcludeFromCodeCoverage]
    public class ResourceStoragePostRequest : IRequest<ResourceStoragePostResponse>
    {       
        public Guid OwnerId { get; set; } = Guid.Empty;
        public Guid RequestId { get; set; } = Guid.Empty;
        public string ContentNamespace { get; set; } = "my";
        public dynamic? Content { get; set; }
        public string ContentKeys { get; set; } = string.Empty;
    }
}
