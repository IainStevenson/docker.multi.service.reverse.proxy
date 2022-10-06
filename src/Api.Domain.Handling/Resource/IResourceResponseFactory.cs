using Api.Domain.Handling.Resource.Delete;
using Api.Domain.Handling.Resource.Get;
using Api.Domain.Handling.Resource.Post;
using Api.Domain.Handling.Resource.Put;
using System.Net;

namespace Api.Domain.Handling.Resource
{
    public interface IResourceResponseFactory
    {
        ResourceResponseDeleteRequest CreateResourceResponseDeleteRequest(HttpStatusCode statusCode, List<string> requestValidationErrors);

        /// <summary>
        /// Create and return a new instance of <see cref="ResourceResponseGetManyRequest"/> from the provided variables.
        /// </summary>
        /// <param name="model">The data model.</param>
        /// <param name="statusCode">The process status code.</param>
        /// <param name="scheme">The transport scheme.</param>
        /// <param name="host">The processing host.</param>
        /// <param name="pathBase">The path base of the host.</param>
        /// <param name="path">The path element of the call.</param>
        /// <returns></returns>
        ResourceResponseGetManyRequest CreateResourceResponseGetManyRequest(IEnumerable<Data.Model.Storage.Resource> model, HttpStatusCode statusCode);
        /// <summary>
        /// Create and return a new instance of <see cref="ResourceResponseGetOneRequest"/> from the provided variables.
        /// </summary>
        /// <param name="model">The data model.</param>
        /// <param name="statusCode">The process status code.</param>
        /// <param name="scheme">The transport scheme.</param>
        /// <param name="host">The processing host.</param>
        /// <param name="pathBase">The path base of the host.</param>
        /// <param name="path">The path element of the call.</param>
        /// <returns></returns>
        ResourceResponseGetOneRequest CreateResourceResponseGetOneRequest(Data.Model.Storage.Resource model, HttpStatusCode statusCode);

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
        ResourceResponsePostRequest CreateResourceResponsePostRequest(
            Data.Model.Storage.Resource model,
            HttpStatusCode statusCode,
            string scheme,
            string host,
            string pathBase,
            string path,
            string keys);
        /// <summary>
        /// Create and return a new instance of <see cref="ResourceResponsePutRequest"/> from the provided variables.
        /// </summary>
        /// <param name="model">The data model.</param>
        /// <param name="statusCode">The process status code.</param>
        /// <param name="scheme">The transport scheme.</param>
        /// <param name="host">The processing host.</param>
        /// <param name="pathBase">The path base of the host.</param>
        /// <param name="path">The path element of the call.</param>
        /// <param name="keys">The client provided keys</param>
        /// <returns>A new instance of <see cref="ResourceResponsePutRequest"/></returns>
        ResourceResponsePutRequest CreateResourceResponsePutRequest(
            Data.Model.Storage.Resource model,
            HttpStatusCode statusCode,
            string @namespace,
            string scheme,
            string host,
            string pathBase,
            string path,
            string keys);
    }
}