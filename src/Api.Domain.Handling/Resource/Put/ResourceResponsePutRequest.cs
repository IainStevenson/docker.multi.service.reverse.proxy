using MediatR;
using System.Net;

namespace Api.Domain.Handling.Resource.Put
{
    public class ResourceResponsePutRequest : IRequest<ResourceResponse<Data.Model.Response.Resource>>
    {
        public Data.Model.Storage.Resource Model { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string Scheme { get;  set; }
        public string Host { get;  set; }
        public string PathBase { get;  set; }
        public string Path { get;  set; }
        public string Keys { get;  set; }
        public string Namespace { get; set; }
    }
}
