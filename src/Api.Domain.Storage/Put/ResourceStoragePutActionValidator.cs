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
                response.StatusCode = ApiDomainStatusCodes.NOTFOUND;
                response.RequestValidationErrors.Add($"Modification failed, because the record identified by {request.Id} was not found.");
                return (resource, response);
            }
            // Msut not have changed since
            if (
                    resource.Modified.HasValue ?
                        resource.Modified.Value > request.UnmodifiedSince :
                        resource.Created > request.UnmodifiedSince

                    )
            {
                response.RequestValidationErrors.Add($"The resource has been modified since {request.UnmodifiedSince} and therefore has not been updated.");
                response.StatusCode = ApiDomainStatusCodes.PRECONDITIONFAILED;
                resource = null;
                return (resource, response);
            }
            // must have the ETAG
            if (
                    request.ETags.Any()
                    && !request.ETags.Contains(resource.Etag)
                )
            {
                response.RequestValidationErrors.Add($"The resource has None of the specified ETags {string.Join(',', request.ETags)} and therefore has not been updated./r/n");
                response.StatusCode = ApiDomainStatusCodes.PRECONDITIONFAILED;
                resource = null;
                return (resource, response);
            }
            // has since and etag
            response.StatusCode = ApiDomainStatusCodes.OK;
            return (resource, response);
        }
    }
}
