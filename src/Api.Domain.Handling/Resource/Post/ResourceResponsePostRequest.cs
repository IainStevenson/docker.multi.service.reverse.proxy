using MediatR;
using System.Net;

namespace Api.Domain.Handling.Resource.Post
{
    public class ResourceResponsePostRequest : IRequest<ResourceResponse<Data.Model.Response.Resource>>
    {
        public Data.Model.Storage.Resource Model { get; set; }
        public string Scheme { get; set; }
        public string Host { get; set; }
        public string PathBase { get; set; }
        public string Path { get; set; }
        public string Keys { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}
