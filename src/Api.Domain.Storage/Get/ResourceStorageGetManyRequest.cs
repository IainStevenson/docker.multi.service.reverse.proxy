using MediatR;

namespace Api.Domain.Storage.Get
{
    public class ResourceStorageGetManyRequest : IRequest<ResourceStorageGetManyResponse>
    {
        public string Namespace { get; set; }
        public Guid OwnerId { get; set; }
        public Guid RequestId { get; set; }
        public DateTimeOffset IfModifiedSince { get;  set; }
        public List<string> IfNotETags { get;  set; }
        public object Scheme { get;  set; }
        public string Host { get;  set; }
        public string PathBase { get;  set; }
        public string Path { get;  set; }
    }
}
