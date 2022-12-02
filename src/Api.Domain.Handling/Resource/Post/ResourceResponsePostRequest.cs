using MediatR;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace Api.Domain.Handling.Resource.Post
{
    /// <summary>
    /// Defines the POST request data to fulfill a service response.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ResourceResponsePostRequest : IRequest<ResourceResponse<Data.Model.Response.Resource>>
    {
        public Data.Model.Storage.Resource? Model { get; set; }
        public List<string> RequestValidationErrors { get; set; } = new List<string>();
        public string Scheme { get; set; } = string.Empty;
        public string Host { get; set; } = string.Empty;
        public string PathBase { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public string Namespace { get; set; } = string.Empty;
        public string Keys { get; set; } = string.Empty;
        public HttpStatusCode StatusCode { get; set; }
    }
}
