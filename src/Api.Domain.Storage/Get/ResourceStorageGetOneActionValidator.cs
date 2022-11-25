using Data.Model.Storage;

namespace Api.Domain.Storage.Get
{
    /// <inheritdoc/>
    public class ResourceStorageGetOneActionValidator : IResourceStorageActionValidator<ResourceStorageGetOneRequest, ResourceStorageGetOneResponse>
    {
        /// <inheritdoc/>
        public (Resource?, ResourceStorageGetOneResponse) Validate(Resource? resource, ResourceStorageGetOneRequest request, ResourceStorageGetOneResponse response)
        {
            if (resource == null)
            {
                response.StatusCode = StatusCodes.NOTFOUND;
                return (resource, response);
            }

            var resourceHasNotBeenModifiedSince = !(resource.Modified.HasValue ?
                                                        resource.Modified > request.IfModifiedSince :
                                                        resource.Created > request.IfModifiedSince);
            if (resourceHasNotBeenModifiedSince)
            {
                response.StatusCode = StatusCodes.NOTMODIFIED;
                resource = null;
                return (resource, response);
            }

            if (request.IfNotETags.Any() && request.IfNotETags.Contains(resource.Etag))
            {
                response.StatusCode = StatusCodes.NOTMODIFIED;
                resource = null;
                return (resource, response);
            }
            response.StatusCode = StatusCodes.OK;
            return (resource, response);
        }
    }

}
