using MediatR;
using System.Net;

namespace Resource.Handling
{
    public class PostResourceOutputRequest : IRequest<ResourceOutputResponse<Data.Model.Response.Resource>>
    {
        public int StatusCode { get; set; }
        public Data.Model.Response.Resource Model { get; set; }
        public string Scheme { get; set; }
        public string Host { get; set; }
        public string PathBase { get; set; }
        public string Path { get; set; }
        public string Keys { get; set; }
    }
}