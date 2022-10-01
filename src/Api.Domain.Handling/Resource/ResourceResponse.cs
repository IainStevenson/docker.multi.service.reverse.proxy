using Data.Model.Response;
using System.Net;

namespace Api.Domain.Handling.Resource
{
    public class ResourceResponse : IResponseItem
    {
        public Guid Id { get; set; }
        /// <summary>
        /// The resulting status code from the procesing.
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Modified { get; set; }

        /// <summary>
        /// The HATEOS link information for this resource
        /// </summary>        
        public IEnumerable<IApiLink>? Links { get; set; }

        public string Etag { get; set; }
    }
}
