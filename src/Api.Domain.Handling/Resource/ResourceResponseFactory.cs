using System.Net;

namespace Api.Domain.Handling.Resource
{
    public class ResourceResponseFactory : IResourceResponseFactory
    {
        /// <inheritdoc/>
        public ResourceResponsePostRequest CreateResourceOutputPostRequest(Resource model, HttpStatusCode statusCode, string scheme, string host, string pathBase, string path, string keys)
        {
            return new ResourceResponsePostRequest()
            {
                Model = model,
                StatusCode = statusCode,
                Scheme = scheme,
                Host = host,
                PathBase = pathBase,
                Path = path,
                Keys = keys
            };
        }
    }
}