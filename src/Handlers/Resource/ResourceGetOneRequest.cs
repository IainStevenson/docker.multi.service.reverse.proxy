using MediatR;
using Microsoft.AspNetCore.Http;
using System;

namespace Handlers.Resource
{
    public class ResourceGetOneRequest : IRequest<ResourceGetOneResponse>
    {
        public Guid Id { get; set; }

        public IHeaderDictionary Headers { get; set; }
    }
}
