using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace Api.Domain.Storage.Get
{
    [ExcludeFromCodeCoverage]
    public class ResourceStorageGetManyRequest : IRequest<ResourceStorageGetManyResponse>
    {
        public string ContentNamespace { get; set; } = "my";
        public Guid OwnerId { get; set; }
        public Guid RequestId { get; set; }
        public DateTimeOffset IfModifiedSince { get;  set; }
        public List<string> IfNotETags { get;  set; } = new List<string>();
        public string Scheme { get;  set; } = "https";
        public string Host { get;  set; } = "127.0.0.1";
        public string PathBase { get;  set; } = "";
        public string Path { get;  set; } = "";
        public bool IfIsDelted { get; internal set; }
    }
}
