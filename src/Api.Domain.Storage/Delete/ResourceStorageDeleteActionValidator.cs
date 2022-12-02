using Data.Model.Storage;

namespace Api.Domain.Storage.Delete
{

    /// <inheritdoc/>
    public class ResourceStorageDeleteActionValidator : 
        IResourceStorageActionValidator<ResourceStorageDeleteRequest, ResourceStorageDeleteResponse>
    {
        /// <inheritdoc/>
        public (Resource?, ResourceStorageDeleteResponse) Validate(Resource? resource, ResourceStorageDeleteRequest request, ResourceStorageDeleteResponse response)
        {
            if (resource == null)
            {
                response.StatusCode = ApiDomainStatusCodes.NOTFOUND;
                response.RequestValidationErrors.Add($"Deletion failed, because the record identified by {request.Id} was not found.");
                return (resource, response);
            }


            if ((resource.Modified.HasValue ?
                        resource.Modified > request.IsUnchangedSince :
                        resource.Created > request.IsUnchangedSince)
                    )
            {
                response.RequestValidationErrors.Add($"Deletion failed, because the resource has been modified since {request.IsUnchangedSince}");
                response.StatusCode = ApiDomainStatusCodes.PRECONDITIONFAILED;
                resource = null;
                return (resource, response);
            }

            if (request.IsNotETags.Any() && !request.IsNotETags.Contains(resource.Etag))
            {
                response.RequestValidationErrors.Add($"Deletion failed, as the resource has None of the specified ETags {string.Join(',', request.IsNotETags)}/r/n");
                response.StatusCode = ApiDomainStatusCodes.PRECONDITIONFAILED;
                resource = null;
                return (resource, response);

            }
            response.StatusCode = ApiDomainStatusCodes.OK; //assume ok to be modified as needed
            return (resource, response);
        }

    }
}
