﻿using Data.Model.Response;
using System.Net;

namespace Api.Domain.Handling.Resource
{
    public class ResourceResponse : IResponseItem
    {
        public HttpStatusCode StatusCode { get; set; }
        public IEnumerable<IApiLink> Links { get; set; }
        public string Etag { get; set; }
        public Guid Id { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Modified { get; set; }
    }
}