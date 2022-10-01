using Api.Domain.Handling.Resource.Get;
using Api.Domain.Handling.Resource.Post;
using Api.Domain.Handling.Resource.Put;
using System.Net;

namespace Api.Domain.Handling.Resource
{
    public class ResourceResponseFactory : IResourceResponseFactory
    {
        /// <inheritdoc/>
        public ResourceResponsePostRequest CreateResourceOutputPostRequest(Data.Model.Storage.Resource model, HttpStatusCode statusCode, string scheme, string host, string pathBase, string path, string keys)
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

        /// <inheritdoc/>
        public ResourceResponseGetManyRequest CreateResourceResponseGetManyRequest(IEnumerable<Data.Model.Storage.Resource> model, HttpStatusCode statusCode, string scheme, string host, string pathBase, string path)
        {
            return new ResourceResponseGetManyRequest()
            {
                Model = model,
                StatusCode = statusCode,
                Scheme = scheme,
                Host = host,
                PathBase = pathBase,
                Path = path,

            };
        }

        /// <inheritdoc/>
        public ResourceResponseGetOneRequest CreateResourceResponseGetOneRequest(Data.Model.Storage.Resource model, HttpStatusCode statusCode, string scheme, string host, string pathBase, string path)
        {
            return new ResourceResponseGetOneRequest()
            {
                Model = model,
                StatusCode = statusCode,
                Scheme = scheme,
                Host = host,
                PathBase = pathBase,
                Path = path,
            };
        }

        /// <inheritdoc/>
        public ResourceResponsePostRequest CreateResourceResponsePostRequest(Data.Model.Storage.Resource model, HttpStatusCode statusCode, string scheme, string host, string pathBase, string path, string keys)
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
        /// <inheritdoc/>
        public ResourceResponsePutRequest CreateResourceResponsePutRequest(Data.Model.Storage.Resource model, HttpStatusCode statusCode, string scheme, string host, string pathBase, string path, string keys)
        {
            return new ResourceResponsePutRequest()
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