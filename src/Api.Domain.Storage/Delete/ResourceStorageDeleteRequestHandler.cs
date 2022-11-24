using FluentValidation;
using MediatR;
using Storage;

namespace Api.Domain.Storage.Delete
{
    public class ResourceStorageDeleteRequestHandler : IRequestHandler<ResourceStorageDeleteRequest, ResourceStorageDeleteResponse>
    {
        private readonly IRepository<Data.Model.Storage.Resource> _storage;
        private readonly AbstractValidator<ResourceStorageDeleteRequest> _validator;
        private readonly ResourceStorageDeleteValidator _validatePreConditions;
       


        public ResourceStorageDeleteRequestHandler(IRepository<Data.Model.Storage.Resource> storage, ResourceStorageDeleteRequestValidator requestValidator, ResourceStorageDeleteValidator validatePreConditions)
        {
            _storage = storage;
            _validator = requestValidator;
            _validatePreConditions = validatePreConditions;
        }

        public async Task<ResourceStorageDeleteResponse> Handle(ResourceStorageDeleteRequest request, CancellationToken cancellationToken)
        {

            var response = new ResourceStorageDeleteResponse() { };

            var validationResult = _validator.Validate(request);

            if (!validationResult.IsValid)
            {
                response.RequestValidationErrors.AddRange(validationResult.Errors.Select(x => x.ErrorMessage));
                response.StatusCode = StatusCodes.BADREQUEST;
            }


            Data.Model.Storage.Resource? resource = (await _storage.GetAsync(r => r.Id == request.Id
                                                                                   && r.OwnerId == request.OwnerId
                                                                                   && r.Namespace == request.Namespace
                                                                                   )).FirstOrDefault();

            (resource,response) = _validatePreConditions.Validate(resource, request, response);

            if (response.StatusCode != StatusCodes.OK)
            {
                return response;
            }

            var count = await _storage.DeleteAsync(request.Id);
            response.StatusCode = StatusCodes.NOCONTENT;
            return response;

        }
    }
}

