using MediatR;
using Storage;

namespace Api.Domain.Storage.Get
{
    public class ResourceStorageGetManyRequestHandler : IRequestHandler<ResourceStorageGetManyRequest, ResourceStorageGetManyResponse>
    {
        private readonly IRepository<Data.Model.Storage.Resource> _storage;


        public ResourceStorageGetManyRequestHandler(
            IRepository<Data.Model.Storage.Resource> storage
            )
        {
            _storage = storage;
        }


        public async Task<ResourceStorageGetManyResponse> Handle(ResourceStorageGetManyRequest request, CancellationToken cancellationToken)
        {
            var response = new ResourceStorageGetManyResponse();

            IEnumerable<Data.Model.Storage.Resource> resources = new List<Data.Model.Storage.Resource>();

            resources = await _storage.GetAsync(
                    r => r.OwnerId == request.OwnerId
                    && r.Namespace == request.Namespace
                    );

            if (resources.Any())
            {
                // if all of them are unmodified since then return none
                var unmodifiedItems = resources.Where(r =>
                                            r.Modified.HasValue ? r.Modified < request.IfModifiedSince :
                                            r.Created < request.IfModifiedSince);
                if (unmodifiedItems.Count() == resources.Count())
                {
                    response.StatusCode = 304; //System.Net.HttpStatusCode.NotModified;
                    return response;
                }

                var modifiedItems = resources.Where(r =>
                            r.Modified.HasValue ? r.Modified >= request.IfModifiedSince :
                            r.Created > request.IfModifiedSince);
                response.StatusCode = 200; // System.Net.HttpStatusCode.OK;
                response.Model = modifiedItems;
                return response;
            }

            response.StatusCode = 404; // System.Net.HttpStatusCode.NotFound;
            return response;

        }
    }
}
