using MediatR;
using Storage;

namespace Api.Domain.Storage.Get
{
    public class ResourceStorageGetManyRequestHandler : IRequestHandler<ResourceStorageGetManyRequest, ResourceStorageGetManyResponse>
    {
        private readonly IRepository<Data.Model.Storage.Resource> _storage;
        private readonly IResourceStorageActionMultiValidator<ResourceStorageGetManyRequest, ResourceStorageGetManyResponse> _validatePreConditions;

        public ResourceStorageGetManyRequestHandler(
            IRepository<Data.Model.Storage.Resource> storage,
            IResourceStorageActionMultiValidator<ResourceStorageGetManyRequest, ResourceStorageGetManyResponse> preconditionValidator)
        {
            _storage = storage;
            _validatePreConditions = preconditionValidator;
        }


        public async Task<ResourceStorageGetManyResponse> Handle(ResourceStorageGetManyRequest request, CancellationToken cancellationToken)
        {
            var response = new ResourceStorageGetManyResponse();

            IEnumerable<Data.Model.Storage.Resource> resources = new List<Data.Model.Storage.Resource>();

            resources = await _storage.GetAsync(
                    r => r.OwnerId == request.OwnerId
                    && r.Namespace == request.Namespace
                    );

            (resources, response) = _validatePreConditions.Validate(resources, request, response);

            if (response.StatusCode != StatusCodes.OK)
            {
                return response;
            }


            response.StatusCode = StatusCodes.OK;
            return response;

        }
    }
}
