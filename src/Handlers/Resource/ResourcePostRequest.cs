using MediatR;
using System;

namespace Handlers.Resource
{
    public class ResourcePostRequest : IRequest<ResourcePostResponse>
    {
        public string Namespace { get; set; }
        public dynamic Content { get; set; }
        public string Keys { get; set; }
        public Guid OwnerId { get; set; }
        public Guid RequestId { get; set; }


        public string Scheme { get;  set; }
        public string Host { get;  set; }
        public string Path { get;  set; }
    }
}
