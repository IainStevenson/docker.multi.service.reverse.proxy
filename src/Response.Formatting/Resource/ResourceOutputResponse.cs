using Data.Model.Response;
using System.Collections.Generic;
using System.Net;

namespace Response.Formatting
{

    public class ResourceOutputResponse<T> : IApiLinks
    {
        public HttpStatusCode StatusCode { get; set; }
        public T Model { get; set; }
        public IEnumerable<IApiLink> Links { get; set; }
    }
}