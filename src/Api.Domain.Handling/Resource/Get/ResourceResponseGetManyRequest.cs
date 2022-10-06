using MediatR;
using System.Net;

namespace Api.Domain.Handling.Resource.Get
{
    public class ResourceResponseGetManyRequest : IRequest<ResourceResponse<IEnumerable<Data.Model.Response.Resource>>>
    {
        public IEnumerable<Data.Model.Storage.Resource> Model { get;  set; }
        public HttpStatusCode StatusCode { get;  set; }
        public string Scheme { get;  set; }
        public string Host { get;  set; }
        public string PathBase { get;  set; }
        public string Path { get;  set; }
        public DateTimeOffset IfModifiedSince { get;  set; }
        public string Namespace { get; internal set; }
    }


}
