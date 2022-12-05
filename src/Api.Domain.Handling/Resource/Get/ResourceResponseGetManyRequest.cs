using MediatR;
using System.Net;

namespace Api.Domain.Handling.Resource.Get
{
    public class ResourceResponseGetManyRequest : IRequest<ResourceResponse<IEnumerable<Data.Model.Response.Resource>>>
    {
        public IEnumerable<Data.Model.Storage.Resource> Model { get;  set; } = new List<Data.Model.Storage.Resource>();
        public List<string> RequestValidationErrors { get; set; } = new List<string>();
        public string ContentNamespace { get; set; } = string.Empty;
        public DateTimeOffset IfModifiedSince { get;  set; } = DateTimeOffset.MinValue;
        public HttpStatusCode StatusCode { get;  set; } = HttpStatusCode.OK;
        public string Scheme { get;  set; } = string.Empty;
        public string Host { get;  set; } = string.Empty;
        public string PathBase { get;  set; }= string.Empty;
        public string Path { get;  set; }= string.Empty;
        
    }


}
