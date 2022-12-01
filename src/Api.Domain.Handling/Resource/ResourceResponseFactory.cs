using Api.Domain.Handling.Resource.Delete;
using Api.Domain.Handling.Resource.Get;
using Api.Domain.Handling.Resource.Post;
using Api.Domain.Handling.Resource.Put;
using System.Net;

namespace Api.Domain.Handling.Resource
{
    public class ResourceResponseFactory : IResourceResponseFactory
    {
        /// <inheritdoc/>
        //public ResourceResponsePostRequest CreateResourceOutputPostRequest(Data.Model.Storage.Resource model, HttpStatusCode statusCode, string scheme, string host, string pathBase, string path, string keys)
        //{
        //    return new ResourceResponsePostRequest()
        //    {
        //        Model = model,
        //        StatusCode = statusCode,
        //        Scheme = scheme,
        //        Host = host,
        //        PathBase = pathBase,
        //        Path = path,
        //        Keys = keys
        //    };
        //}

        public ResourceResponseDeleteRequest CreateResourceResponseDeleteRequest(HttpStatusCode statusCode, List<string> requestValidationErrors)
        {
            return new ResourceResponseDeleteRequest()
            {
                StatusCode = statusCode,
                RequestValidationErrors = requestValidationErrors
            };
        }

        /// <inheritdoc/>
        public ResourceResponseGetManyRequest CreateResourceResponseGetManyRequest(
                IEnumerable<Data.Model.Storage.Resource> model,
                HttpStatusCode statusCode)
        {
            return new ResourceResponseGetManyRequest()
            {
                Model = model,
                StatusCode = statusCode,
            };
        }

        /// <inheritdoc/>
        public ResourceResponseGetOneRequest CreateResourceResponseGetOneRequest(Data.Model.Storage.Resource model, HttpStatusCode statusCode)
        {
            return new ResourceResponseGetOneRequest()
            {
                Model = model,
                StatusCode = statusCode,


            };
        }

        /// <inheritdoc/>
        public ResourceResponsePostRequest CreateResourceResponsePostRequest(Data.Model.Storage.Resource model, 
            HttpStatusCode statusCode, string scheme, string host, string pathBase, string path, string @namespace, string keys)
        {
            return new ResourceResponsePostRequest()
            {
                Model = model,
                StatusCode = statusCode,
                Scheme = scheme,
                Host = host,
                PathBase = pathBase.TrimEnd('/'),
                Path = path.TrimEnd('/'),
                Namespace= @namespace,
                Keys = keys
            };
        }
        /// <inheritdoc/>
        public ResourceResponsePutRequest CreateResourceResponsePutRequest(Data.Model.Storage.Resource model, HttpStatusCode statusCode, string @namespace, string scheme, string host, string pathBase, string path, string keys)
        {
            return new ResourceResponsePutRequest()
            {
                Model = model,
                Namespace = @namespace.ToLower(),
                StatusCode = statusCode,
                Scheme = scheme,
                Host = host,
                PathBase = pathBase.TrimEnd('/'),
                Path = path.TrimEnd('/'),
                Keys = keys
            };
        }
    }
}