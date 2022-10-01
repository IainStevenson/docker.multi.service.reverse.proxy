using MediatR;
using Microsoft.Extensions.Primitives;

namespace Api.Domain.Storage.Put
{
    public class ResourceStoragePutRequest : IRequest<ResourceStoragePutResponse>
    {        public Guid Id { get; set; }
        public string Namespace { get; set; }
        public IDictionary<string, StringValues> Headers { get; set; }
        public string Host { get; set; }
        public string Scheme { get; set; }
        public string Path { get; set; }
        public Guid RequestId { get; set; }
        public dynamic Model { get; set; }
        public Guid OwnerId { get; set; }
        public IEnumerable<KeyValuePair<string, StringValues>> Query { get; set; }
        public string Keys { get; set; }
        public string MoveTo { get; set; }
        public string PathBase { get; set; }
    }

}
