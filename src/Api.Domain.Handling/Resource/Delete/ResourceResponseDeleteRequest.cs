using MediatR;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace Api.Domain.Handling.Resource.Delete
{
    public class ResourceResponseDeleteRequest : IRequest<ResourceResponse>
    {
        public HttpStatusCode StatusCode { get; set; }
        public List<string> RequestValidationErrors { get; set; } = new List<string>();
    }


}
