using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace Api.Domain.Storage.Put
{
    [ExcludeFromCodeCoverage]
    public class ResourceStoragePutRequest : IRequest<ResourceStoragePutResponse>
    {        
        public Guid Id { get; set; } = Guid.Empty;
        public string ContentNamespace { get; set; } = string.Empty;
        public string ContentKeys { get; set; } = string.Empty;
        public Guid RequestId { get; set; } = Guid.Empty;
        public Guid OwnerId { get; set; } = Guid.Empty;
        public string MoveTo { get; set; } = "my";
        public dynamic? Content { get; set; }
        public List<string> ETags {  get;set; } = new List<string>();
        public DateTimeOffset UnmodifiedSince { get; set; } = DateTimeOffset.MaxValue;
        public bool IfIsDeleted { get; internal set; }
    }
}
