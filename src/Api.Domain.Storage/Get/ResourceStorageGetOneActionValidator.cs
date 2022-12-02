using Data.Model.Storage;

namespace Api.Domain.Storage.Get
{
    /// <inheritdoc/>
    public class ResourceStorageGetOneActionValidator : IResourceStorageActionValidator<ResourceStorageGetOneRequest, ResourceStorageGetOneResponse>
    {
        /// <inheritdoc/>
        public (Resource?, ResourceStorageGetOneResponse) Validate(Resource? resource, ResourceStorageGetOneRequest request, ResourceStorageGetOneResponse response)
        {
            // to cope with concurrency the client can track either and or etag and last time modified to make a call for a single record
            // that has changed since that state.
            // so if the record retrieved by id/owner/namespace has moved on from that state then return it
            // otherwise return notmodified.

            // the since date and etag are to check if the resource remains nchanged and if so retur not modified
            // otherwise return the new resource.
            // if no resource found return not found
            // if since date provided then if resource has NOT changed since then return notmodified
            // if etag provided then if resource has that etag then return notmodified
            // else return OK

            if (resource == null)
            {
                response.StatusCode = ApiDomainStatusCodes.NOTFOUND;
                response.RequestValidationErrors.Add($"Get failed, because the record identified by {request.Id} was not found.");
                return (resource, response);
            }

            var timeOfLastChange = (resource.Modified.HasValue ?
                                            resource.Modified :
                                            resource.Created );
         
            if (timeOfLastChange <= request.IfModifiedSince)
            {
                response.StatusCode = ApiDomainStatusCodes.NOTMODIFIED;
                response.RequestValidationErrors.Add($"Get failed, because the record has not changed since {request.IfModifiedSince}.");
                resource = null;
                return (resource, response);
            }

            if (ETagIsIn(resource.Etag, request.IfNotETags))
            {
                response.StatusCode = ApiDomainStatusCodes.NOTMODIFIED;
                response.RequestValidationErrors.Add($"Get failed, because the record still has the eTag {request.IfNotETags.Aggregate(string.Empty, (current, next) => $"{current}{(current.Length == 0? "":",")}{next}")}.");
                resource = null;
                return (resource, response);
            }
            response.StatusCode = ApiDomainStatusCodes.OK;
            return (resource, response);
        }

        private bool ETagIsIn(string eTag, List<string> eTags)
        {
            return  eTags.Any() &&  eTags.Contains(eTag);   
        }
    }

}
