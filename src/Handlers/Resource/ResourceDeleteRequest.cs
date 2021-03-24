using MediatR;
using Microsoft.AspNetCore.Http;
using System;

namespace Handlers.Resource
{
    public class ResourceDeleteRequest : IRequest<ResourceDeleteResponse>
    {
        public Guid Id { get; set; }
        public Guid OwnerId { get; set; }
        public Guid RequestId { get; set; }
        public IHeaderDictionary Headers { get; set; }
        public string Namespace { get; set; }
    }
}
