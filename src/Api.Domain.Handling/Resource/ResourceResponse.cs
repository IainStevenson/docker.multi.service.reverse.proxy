using Data.Model.Response;
using Microsoft.Extensions.Primitives;
using System.Net;

namespace Api.Domain.Handling.Resource
{
    public class ResourceResponse : IResponseItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        /// <summary>
        /// The resulting status code from the procesing.
        /// </summary>
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.BadRequest;
        public DateTimeOffset Created { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? Modified { get; set; }

        /// <summary>
        /// The HATEOS link information for this resource
        /// </summary>        
        public IEnumerable<IApiLink> Links { get; set; } = new List<IApiLink>();

        public IDictionary<string, StringValues> Headers { get; set; } = new Dictionary<string, StringValues>();
        public string Etag { get; set; } = string.Empty;
        public List<string> RequestValidationErrors { get; set; } = new List<string>();
    }
}
