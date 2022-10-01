using MediatR;

namespace Api.Domain.Storage.Post
{
    public class ResourceStoragePostRequest : IRequest<ResourceStoragePostResponse>
    {
        public string Namespace { get; set; }
        public dynamic Content { get; set; }
        public string Keys { get; set; }
        public Guid OwnerId { get; set; }
        public Guid RequestId { get; set; }
    }
}
