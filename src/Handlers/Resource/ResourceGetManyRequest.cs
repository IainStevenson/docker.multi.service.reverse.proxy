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
    }
}
