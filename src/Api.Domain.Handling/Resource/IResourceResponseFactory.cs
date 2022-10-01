using System.Net;

namespace Api.Domain.Handling.Resource
{
    public interface IResourceResponseFactory
    {
        /// <summary>
        /// Create and return a new instance of <see cref="ResourceResponsePostRequest"/> from the provided variables.
        /// </summary>
        /// <param name="model">The data model.</param>
        /// <param name="statusCode">The process status code.</param>
        /// <param name="scheme">The transport scheme.</param>
        /// <param name="host">The processing host.</param>
        /// <param name="pathBase">The path base of the host.</param>
        /// <param name="path">The path element of the call.</param>
        /// <param name="keys">The client provided keys</param>
        /// <returns>A new instance of <see cref="ResourceResponsePostRequest"/></returns>
        ResourceResponsePostRequest CreateResourceOutputPostRequest(
            Resource model,
            HttpStatusCode statusCode,
            string scheme,
            string host,
            string pathBase,
            string path,
            string keys);
    }
}