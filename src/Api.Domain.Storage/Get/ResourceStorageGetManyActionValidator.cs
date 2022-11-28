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
                                            r.Modified.HasValue ? r.Modified < request.IfModifiedSince :
                                            r.Created < request.IfModifiedSince);
                if (unmodifiedItems.Count() == resources.Count())
                {
                    response.StatusCode = HttpStatusCodes.NOTMODIFIED;
                    return (resources, response);
                }

                var modifiedItems = resources.Where(r =>
                            r.Modified.HasValue ? r.Modified >= request.IfModifiedSince :
                            r.Created > request.IfModifiedSince);
                response.StatusCode = HttpStatusCodes.OK;
                response.Model = modifiedItems;
                return (resources, response);
            }

            response.StatusCode = HttpStatusCodes.OK;
            return (resources, response);
        }
    }

}
