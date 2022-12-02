using FluentValidation;
using MediatR;
using Storage;

namespace Api.Domain.Storage.Get
{
    public class ResourceStorageGetManyRequestHandler : IRequestHandler<ResourceStorageGetManyRequest, ResourceStorageGetManyResponse>
    {
        private readonly IRepository<Data.Model.Storage.Resource> _storage;
        private readonly IResourceStorageActionMultiValidator<ResourceStorageGetManyRequest, ResourceStorageGetManyResponse> _validatePreConditions;
        private readonly ResourceStorageGetManyRequestValidator _requestValidator;
        public ResourceStorageGetManyRequestHandler(
            IRepository<Data.Model.Storage.Resource> storage,
            IResourceStorageActionMultiValidator<ResourceStorageGetManyRequest, ResourceStorageGetManyResponse> preconditionValidator,
            ResourceStorageGetManyRequestValidator requestValidator)
        {
            _storage = storage;
            _validatePreConditions = preconditionValidator;
            _requestValidator = requestValidator;
        }


        public async Task<ResourceStorageGetManyResponse> Handle(ResourceStorageGetManyRequest request, CancellationToken cancellationToken)
        {
            var response = new ResourceStorageGetManyResponse();

            var validationResult = _requestValidator.Validate(request);

            if (!validationResult.IsValid)
            {
                response.RequestValidationErrors.AddRange(validationResult.Errors.Select(x => x.ErrorMessage));
                response.StatusCode = ApiDomainStatusCodes.BADREQUEST;
            }

            IEnumerable<Data.Model.Storage.Resource> resources = new List<Data.Model.Storage.Resource>();

            resources = await _storage.GetAsync(
                                                    r => r.OwnerId == request.OwnerId
                                                    && r.Namespace == request.ContentNamespace
                                                    , cancellationToken);

            (resources, response) = _validatePreConditions.Validate(resources, request, response);

            if (response.StatusCode != ApiDomainStatusCodes.OK)
            {
                return response;
            }


            response.StatusCode = ApiDomainStatusCodes.OK;
            response.Model = resources;
            return response;

        }
    }
}
