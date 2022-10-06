using MediatR;

namespace Api.Domain.Storage.Get
{
    public class ResourceStorageGetOneRequest : IRequest<ResourceStorageGetOneResponse>
    {
        public Guid Id { get; set; }      
        public string Namespace { get; set; }
        public Guid OwnerId { get; set; }
        public Guid RequestId { get; set; }        
        public List<string> IfNotETags { get; set; }  = new List<string>();
        public DateTimeOffset IfModifiedSince { get;  set; }
    }
}
