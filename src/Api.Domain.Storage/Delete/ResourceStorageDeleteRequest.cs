using MediatR;

namespace Api.Domain.Storage.Delete
{
    public class ResourceStorageDeleteRequest : IRequest<ResourceStorageDeleteResponse>
    {


        public List<string> Etags { get; set; } = new List<string>();
        public DateTimeOffset UnmodifiedSince { get; set; } = DateTimeOffset.MaxValue;

        public Guid Id { get; set; } = Guid.Empty;
        public Guid OwnerId { get; set; } = Guid.Empty;
        public Guid RequestId { get; set; } = Guid.Empty;
        
        public string Namespace { get; set; } = "my";

    }

}
