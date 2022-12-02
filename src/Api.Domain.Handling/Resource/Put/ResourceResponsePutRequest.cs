using MediatR;
using System.Net;

namespace Api.Domain.Handling.Resource.Put
{
    /// <summary>
    /// Defines the PUT request data to fulfill a service response.
    /// </summary>
    public class ResourceResponsePutRequest : IRequest<ResourceResponse<Data.Model.Response.Resource>>
    {
        public Data.Model.Storage.Resource? Model { get; set; }
        public List<string> RequestValidationErrors { get; set; } = new List<string>();
        public HttpStatusCode StatusCode { get; set; }
        public string Scheme { get; set; } = string.Empty;
        public string Host { get; set; } = string.Empty;
        public string PathBase { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public string Keys { get; set; } = string.Empty;
        public string Namespace { get; set; } = string.Empty;
    }
}
