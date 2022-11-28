using FluentValidation;
using MediatR;
using Storage;

namespace Api.Domain.Storage.Delete
{
    public class ResourceStorageDeleteRequestHandler : IRequestHandler<ResourceStorageDeleteRequest, ResourceStorageDeleteResponse>
    {
        private readonly IRepository<Data.Model.Storage.Resource> _storage;
        private readonly AbstractValidator<ResourceStorageDeleteRequest> _requestValidator;
        private readonly IResourceStorageActionValidator<ResourceStorageDeleteRequest, ResourceStorageDeleteResponse> _actionValidator;
       


        public ResourceStorageDeleteRequestHandler(IRepository<Data.Model.Storage.Resource> storage, 
            ResourceStorageDeleteRequestValidator requestValidator, 
            IResourceStorageActionValidator<ResourceStorageDeleteRequest, ResourceStorageDeleteResponse> actionValidator)
        {
            _storage = storage;
            _requestValidator = requestValidator;
            _actionValidator = actionValidator;
        }

        public async Task<ResourceStorageDeleteResponse> Handle(ResourceStorageDeleteRequest request, CancellationToken cancellationToken)
        {

            var response = new ResourceStorageDeleteResponse() { };

            var validationResult = _requestValidator.Validate(request);

            if (!validationResult.IsValid)
            {
                response.RequestValidationErrors.AddRange(validationResult.Errors.Select(x => x.ErrorMessage));
                response.StatusCode = HttpStatusCodes.BADREQUEST;
            }


            Data.Model.Storage.Resource? resource = (await _storage.GetAsync(r => r.Id == request.Id
                                                                                   && r.OwnerId == request.OwnerId
                                                                                   && r.Namespace == request.Namespace
                                                                                   )).FirstOrDefault();

            (resource,response) = _actionValidator.Validate(resource, request, response);

            if (resource == null)
            {
                return response;
            }

            var count = await _storage.DeleteAsync(request.Id);
            response.StatusCode = HttpStatusCodes.NOCONTENT;
            return response;

        }
    }
}

