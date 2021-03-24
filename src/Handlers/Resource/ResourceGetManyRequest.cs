using MediatR;
using Microsoft.AspNetCore.Http;
using System;

namespace Handlers.Resource
{
    public class ResourceGetManyRequest : IRequest<ResourceGetManyResponse>
    {        
        public string Namespace { get; set; }
        public IHeaderDictionary Headers { get; set; }
        public Guid OwnerId { get; set; }
        public Guid RequestId { get; set; }
        public string Scheme { get; set; }
        public string Host { get; set; }
        public string Path { get; set; }
        public string PathBase { get; set; }
    }
}
