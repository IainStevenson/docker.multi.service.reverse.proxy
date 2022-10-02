using MediatR;

namespace Api.Domain.Storage.Get
{
    public class ResourceStorageGetManyRequest : IRequest<ResourceStorageGetManyResponse>
    {
        public string Namespace { get; set; }
        public Guid OwnerId { get; set; }
        public Guid RequestId { get; set; }

        public DateTimeOffset? IfModifiedSince { get; internal set; }
        public List<string> ETags { get; internal set; }
        public object Scheme { get; internal set; }
        public string Host { get; internal set; }
        public string PathBase { get; internal set; }
        public string Path { get; internal set; }
    }

}
