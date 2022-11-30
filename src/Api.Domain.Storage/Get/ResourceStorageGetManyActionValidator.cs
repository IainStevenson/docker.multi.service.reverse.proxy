using Data.Model.Storage;

namespace Api.Domain.Storage.Get
{
    /// <inheritdoc/>
    public class ResourceStorageGetManyActionValidator : IResourceStorageActionMultiValidator<ResourceStorageGetManyRequest, ResourceStorageGetManyResponse>
    {
        /// <inheritdoc/>
        public (IEnumerable<Resource>, ResourceStorageGetManyResponse) Validate(IEnumerable<Resource> resources, ResourceStorageGetManyRequest request, ResourceStorageGetManyResponse response)
        {
            if (resources.Any())
            {
                // if all of them are unmodified since then return none
                var unmodifiedItems = resources.Where(r =>
                                            r.Modified.HasValue ? r.Modified <= request.IfModifiedSince :
                                            r.Created <= request.IfModifiedSince);
                if (unmodifiedItems.Count() == resources.Count())
                {
                    response.StatusCode = HttpStatusCodes.NOTMODIFIED;
                    response.RequestValidationErrors.Add($"Get failed, because no records were not found modified since {request.IfModifiedSince:o}.");
                    return (new List<Resource>(), response);
                }

                var modifiedItems = resources.Where(r =>
                            r.Modified.HasValue ? r.Modified > request.IfModifiedSince :
                            r.Created > request.IfModifiedSince).ToList();
                response.StatusCode = HttpStatusCodes.OK;
                
                return (modifiedItems, response);
            }

            response.StatusCode = HttpStatusCodes.NOTFOUND;
            response.RequestValidationErrors.Add($"Get failed, because no records were not found.");
            return (resources, response);
        }
    }

}
