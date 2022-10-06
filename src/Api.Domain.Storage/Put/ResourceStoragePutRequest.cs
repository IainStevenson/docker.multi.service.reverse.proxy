using MediatR;

namespace Api.Domain.Storage.Put
{
    public class ResourceStoragePutRequest : IRequest<ResourceStoragePutResponse>
    {        
        public Guid Id { get; set; }
        public string Namespace { get; set; }
        public Guid RequestId { get; set; }
        public Guid OwnerId { get; set; }
        public string Keys { get; set; }
        public string MoveTo { get; set; }
        public dynamic Content { get; set; }
        public List<string> ETags {  get;set; } = new List<string>();
        public DateTimeOffset UnmodifiedSince { get; set; }
    }
}
