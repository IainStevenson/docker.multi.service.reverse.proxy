using MediatR;
using Storage;

namespace Api.Domain.Storage.Delete
{
    public class ResourceStorageDeleteRequestHandler : IRequestHandler<ResourceStorageDeleteRequest, ResourceStorageDeleteResponse>
    {
        private readonly IRepository<Data.Model.Storage.Resource> _storage;
        public ResourceStorageDeleteRequestHandler(IRepository<Data.Model.Storage.Resource> storage)
        {
            _storage = storage;
        }

        public async Task<ResourceStorageDeleteResponse> Handle(ResourceStorageDeleteRequest request, CancellationToken cancellationToken)
        {

            var response = new ResourceStorageDeleteResponse() { };

            Data.Model.Storage.Resource? resource = (await _storage.GetAsync(r =>
                               r.Id == request.Id
                               && r.OwnerId == request.OwnerId
                               && r.Namespace == request.Namespace
                               )).FirstOrDefault();


            if (resource == null)
            {
                response.StatusCode = 404; // HttpStatusCode.NotFound;
                return response;
            }


            // only proceed if resource is unmodified since or is one of the etags
            if (
                    (resource.Modified.HasValue ? resource.Modified.Value <= request.UnmodifiedSince : resource.Created <= request.UnmodifiedSince) ||
                    request.Etags.Contains(resource.Etag)
                    )
            {

                var count = await _storage.DeleteAsync(request.Id);
                if (count == 1)
                {
                    response.StatusCode = 204; // HttpStatusCode.NoContent;
                    return response;
                }
                response.RequestValidationErrors.Add($"The resource deletion was attempted but did not happen. This indicates that it has gone already.");

                response.StatusCode = 401; //HttpStatusCode.Gone;
                return response;
            }
            else
            {

                if (request.Etags.Any())
                {
                    response.RequestValidationErrors.Add($"The resource has None of the specified ETags {string.Join(',', request.Etags)}/r/n");
                }
                if (request.UnmodifiedSince != DateTimeOffset.MinValue)
                {
                    response.RequestValidationErrors.Add($"The resource has been modified since {request.UnmodifiedSince}");
                }
                response.StatusCode = 412; //HttpStatusCode.PreconditionFailed;
                return response;
            }
        }




    }

}
