using MediatR;

namespace Api.Domain.Storage.Get
{
    public class ResourceStorageGetOneRequest : IRequest<ResourceStorageGetOneResponse>
    {
        public Guid Id { get; set; }
        //public IDictionary<string , StringValues> Headers { get; set; }
        //public string Scheme { get; set; }
        //public string Host { get; set; }
        //public string Path { get; set; }
        public string Namespace { get; set; }
        public Guid OwnerId { get; set; }
        public Guid RequestId { get; set; }
        //public string PathBase { get; set; }
        public List<string> IfNoneMatch { get; set; }  = new List<string>();
        public DateTimeOffset? IfModifiedSince { get; internal set; }
    }
}
