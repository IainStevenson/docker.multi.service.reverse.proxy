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

            Data.Model.Storage.Resource? resource = (await _storage.GetAsync(r => r.Id == request.Id
                                                                                   && r.OwnerId == request.OwnerId
                                                                                   && r.Namespace == request.Namespace
                                                                                  // && (r.Modified?? DateTimeOffset.MaxValue.AddTicks(-20)) < request.IsUnchangedSince
                                                                                   )).FirstOrDefault();


            if (resource == null)
            {
                response.StatusCode = 404; // HttpStatusCode.NotFound;
                return response;
            }


            // only proceed if resource is unmodified since or is one of the etags
            if (
                    (resource.Modified.HasValue ? resource.Modified.Value <= request.IsUnchangedSince : resource.Created <= request.IsUnchangedSince) ||
                    request.ETags.Contains(resource.Etag)
                    )
            {

                var count = await _storage.DeleteAsync(request.Id);
                if (count == 1)
                {
                    response.StatusCode = 204; // HttpStatusCode.NoContent;
                    return response;
                }
                response.RequestValidationErrors.Add($"The resource deletion was attempted but did not happen. This indicates that it has gone already.");

                response.StatusCode = 410; //HttpStatusCode.Gone;
                return response;
            }
            else
            {

                if (request.ETags.Any())
                {
                    response.RequestValidationErrors.Add($"The resource has None of the specified ETags {string.Join(',', request.ETags)}/r/n");
                }
                if (request.IsUnchangedSince != DateTimeOffset.MinValue)
                {
                    response.RequestValidationErrors.Add($"The resource has been modified since {request.IsUnchangedSince}");
                }
                response.StatusCode = 412; //HttpStatusCode.PreconditionFailed;
                return response;
            }
        }




    }

}
