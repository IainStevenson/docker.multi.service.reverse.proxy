using MediatR;
using Storage;

namespace Api.Domain.Storage.Get
{
    public class ResourceStorageGetOneRequestHandler : IRequestHandler<ResourceStorageGetOneRequest, ResourceStorageGetOneResponse>
    {
        private readonly IRepository<Data.Model.Storage.Resource> _storage;


        public ResourceStorageGetOneRequestHandler(
            IRepository<Data.Model.Storage.Resource> storage

            )
        {
            _storage = storage;

        }

        public async Task<ResourceStorageGetOneResponse> Handle(ResourceStorageGetOneRequest request, CancellationToken cancellationToken)
        {
            var response = new ResourceStorageGetOneResponse();

            Data.Model.Storage.Resource? resource = (await _storage.GetAsync(
                        r => r.Id == request.Id
                        && r.OwnerId == request.OwnerId
                        && r.Namespace == request.Namespace
                )).SingleOrDefault();


            if (resource == null)
            {
                response.StatusCode = 404; // System.Net.HttpStatusCode.NotFound;
                return response;
            }

            if (request.ETags.Any() && request.ETags.Contains(resource.Etag))
            {
                response.StatusCode = 304; //System.Net.HttpStatusCode.NotModified;
                return response;
            }

            if (request.IfModifiedSince.HasValue)
            {
                var resourceHasNotBeenModifiedSince = !(resource.Modified.HasValue ?
                                                            resource.Modified > request.IfModifiedSince.Value :
                                                            resource.Created > request.IfModifiedSince.Value);
                if (resourceHasNotBeenModifiedSince)
                {
                    response.StatusCode = 304; // System.Net.HttpStatusCode.NotModified;
                    return response;
                }
            }

            response.Model = resource;
            response.StatusCode = 200; //System.Net.HttpStatusCode.OK;
            return response;
        }
    }

}
