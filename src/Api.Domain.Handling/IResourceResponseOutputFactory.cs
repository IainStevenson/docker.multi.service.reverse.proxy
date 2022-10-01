using Api.Domain.Handling.Post;
using Data.Model.Storage;
using System.Net;

namespace Api.Domain.Handling
{
    public interface IResourceResponseOutputFactory
    {
        /// <summary>
        /// Create and return a new instance of <see cref="ResourceOutputPostRequest"/> from the provided variables.
        /// </summary>
        /// <param name="model">The data model.</param>
        /// <param name="statusCode">The process status code.</param>
        /// <param name="scheme">The transport scheme.</param>
        /// <param name="host">The processing host.</param>
        /// <param name="pathBase">The path base of the host.</param>
        /// <param name="path">The path element of the call.</param>
        /// <param name="keys">The client provided keys</param>
        /// <returns>A new instance of <see cref="ResourceOutputPostRequest"/></returns>
        ResourceOutputPostRequest CreateResourceOutputPostRequest(
            Resource model, 
            HttpStatusCode statusCode, 
            string scheme, 
            string host, 
            string pathBase, 
            string path, 
            string keys);
    }

    public class ResourceResponseOutputFactory : IResourceResponseOutputFactory
    {
        /// <inheritdoc/>
        public ResourceOutputPostRequest CreateResourceOutputPostRequest(Resource model, HttpStatusCode statusCode, string scheme, string host, string pathBase, string path, string keys)
        {
            return new ResourceOutputPostRequest() {
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