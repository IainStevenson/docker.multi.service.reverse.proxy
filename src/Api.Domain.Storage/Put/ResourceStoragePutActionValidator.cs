using Data.Model.Storage;

namespace Api.Domain.Storage.Put
{
    /// <inheritdoc/>
    public class ResourceStoragePutActionValidator : IResourceStorageActionValidator<ResourceStoragePutRequest, ResourceStoragePutResponse>
    {
        /// <inheritdoc/>
        public (Resource?, ResourceStoragePutResponse) Validate(Resource? resource, ResourceStoragePutRequest request, ResourceStoragePutResponse response)
        {
            if (resource == null)
            {
                response.StatusCode = StatusCodes.NOTFOUND;
                return (resource, response);
            }

            if (
                    resource.Modified.HasValue ?
                        resource.Modified.Value > request.UnmodifiedSince :
                        resource.Created > request.UnmodifiedSince

                    )
            {
                response.RequestValidationErrors.Add($"The resource has been modified since {request.UnmodifiedSince} and therefore has not been updated.");
                response.StatusCode = StatusCodes.PRECONDITIONFAILED;
                resource = null;
                return (resource, response);
            }

            if (
                    request.ETags.Any() && !request.ETags.Contains(resource.Etag)
                )
            {
                response.RequestValidationErrors.Add($"The resource has None of the specified ETags {string.Join(',', request.ETags)} and therefore has not been updated./r/n");
                response.StatusCode = StatusCodes.PRECONDITIONFAILED;
                resource = null;
                return (resource, response);
            }

            response.StatusCode = StatusCodes.OK;
            return (resource, response);
        }
    }
}
