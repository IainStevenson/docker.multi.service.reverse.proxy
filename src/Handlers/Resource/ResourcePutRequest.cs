using MediatR;
using Microsoft.AspNetCore.Http;
using System;

namespace Handlers.Resource
{
    public class ResourcePutRequest : IRequest<ResourcePutResponse>
    {
        public Guid Id { get; set; }
        public string Namespace { get; set; }
        public IHeaderDictionary Headers { get; set; }
        public string Host { get; set; }
        public string Scheme { get; set; }
        public string Path { get; set; }
        public Guid RequestId { get; set; }
        public dynamic Model { get; set; }
        public Guid OwnerId { get; set; }
        public IQueryCollection Query { get; set; }
        public string Keys { get; set; }
        public string MoveTo { get;  set; }
        public string PathBase { get; set; }
    }
}
