using Api.Domain.Handling.Resource.Delete;
using Api.Domain.Handling.Resource.Get;
using Api.Domain.Handling.Resource.Post;
using Api.Domain.Handling.Resource.Put;
using System.Net;

namespace Api.Domain.Handling.Resource
{
    public class ResourceResponseFactory : IResourceResponseFactory
    {
        
        public ResourceResponseDeleteRequest CreateResourceResponseDeleteRequest(
            HttpStatusCode statusCode, 
            List<string> responseValidationErrors)
        {
            return new ResourceResponseDeleteRequest()
            {
                StatusCode = statusCode,
                RequestValidationErrors = responseValidationErrors
            };
        }

        /// <inheritdoc/>
        public ResourceResponseGetManyRequest CreateResourceResponseGetManyRequest(
                IEnumerable<Data.Model.Storage.Resource> model,
                HttpStatusCode statusCode, 
                List<string> responseValidationErrors)
        {
            return new ResourceResponseGetManyRequest()
            {
                Model = model,
                StatusCode = statusCode,
            };
        }

        /// <inheritdoc/>
        public ResourceResponseGetOneRequest CreateResourceResponseGetOneRequest(
            Data.Model.Storage.Resource model, 
            HttpStatusCode statusCode, 
            List<string> responseValidationErrors)
        {
            return new ResourceResponseGetOneRequest()
            {
                Model = model,
                StatusCode = statusCode,
                RequestValidationErrors= responseValidationErrors
            };
        }

        /// <inheritdoc/>
        public ResourceResponsePostRequest CreateResourceResponsePostRequest(
            Data.Model.Storage.Resource model,
            string contentNamespace,
            string contentKeys,
            HttpStatusCode statusCode,
            List<string> responseValidationErrors,
            string scheme,
            string host,
            string pathBase,
            string path)
        {
            return new ResourceResponsePostRequest()
            {
                Model = model,
                ContentNamespace= contentNamespace,
                ContentKeys = contentKeys,
                StatusCode = statusCode,
                Scheme = scheme,
                Host = host,
                PathBase = pathBase.TrimEnd('/'),
                Path = path.TrimEnd('/')
            };
        }
        /// <inheritdoc/>
        public ResourceResponsePutRequest CreateResourceResponsePutRequest(
            Data.Model.Storage.Resource model,
            string contentNamespace,
            string contentKeys
,
            HttpStatusCode statusCode,
            List<string> responseValidationErrors,
            string scheme,
            string host,
            string pathBase,
            string path)
        {
            return new ResourceResponsePutRequest()
            {
                Model = model,
                ContentNamespace = contentNamespace.ToLower(),
                ContentKeys = contentKeys,
                StatusCode = statusCode,
                Scheme = scheme,
                Host = host,
                PathBase = pathBase.TrimEnd('/'),
                Path = path.TrimEnd('/')
            };
        }
    }
}