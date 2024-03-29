﻿using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace Api.Domain.Storage.Post
{
    [ExcludeFromCodeCoverage]
    public class ResourceStoragePostRequest : IRequest<ResourceStoragePostResponse>
    {       
        public string Namespace { get; set; }
        public dynamic Content { get; set; }
        public string Keys { get; set; }
        public Guid OwnerId { get; set; }
        public Guid RequestId { get; set; }
    }
}
