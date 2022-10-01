using MediatR;
using System.Net;

namespace Api.Domain.Handling.Resource.Get
{
    public class ResourceResponseGetOneRequest : IRequest<ResourceResponse<Data.Model.Response.Resource>> { 
    
        public Data.Model.Storage.Resource Model { get; set; }
        public HttpStatusCode StatusCode { get; internal set; }
        public string Scheme { get; internal set; }
        public string Host { get; internal set; }
        public string PathBase { get; internal set; }
        public string Path { get; internal set; }
    }


}
