using MediatR;
using System.Net;

namespace Api.Domain.Handling.Resource.Get
{
    public class ResourceResponseGetManyRequest : IRequest<ResourceResponse<IEnumerable<Data.Model.Response.Resource>>>
    {
        public IEnumerable<Data.Model.Storage.Resource> Model { get;  set; } = new List<Data.Model.Storage.Resource>();
        public string Namespace { get; set; } = "my";
        public DateTimeOffset IfModifiedSince { get;  set; } = DateTimeOffset.MinValue;
        public HttpStatusCode StatusCode { get;  set; } = HttpStatusCode.OK;
        public string Scheme { get;  set; } = "https";
        public string Host { get;  set; } = "127.0.0.1";
        public string PathBase { get;  set; }= "/";
        public string Path { get;  set; }=  "";
    }


}
