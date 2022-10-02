using MediatR;
using System.Net;

namespace Api.Domain.Handling.Resource.Get
{
    public class ResourceResponseGetOneRequest : IRequest<ResourceResponse<Data.Model.Response.Resource>> { 
    
        public Data.Model.Storage.Resource Model { get; set; }
        public HttpStatusCode StatusCode { get;  set; }
        public string Scheme { get;  set; }
        public string Host { get;  set; }
        public string PathBase { get;  set; }
        public string Path { get;  set; }
    }


}
