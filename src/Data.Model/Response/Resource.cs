using CSharpVitamins;
using System;
using System.Collections.Generic;

namespace Data.Model.Response
{
    public class Resource : IResource, IResponseItem
    {
        public Resource()
        {
            Id = Guid.NewGuid();
            Created = DateTimeOffset.UtcNow;
            Etag = (ShortGuid)Guid.NewGuid().ToString();
        }
        public string Etag { get; set; }
        public Guid Id { get; set; }
        public List<IApiLink> Links { get; set; } 
        public dynamic Content { get; set; }
        public string Namespace { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Modified { get; set; }
    }
}
