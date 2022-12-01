using FluentValidation;
using MediatR;
using Storage;

namespace Api.Domain.Storage.Get
{
    public class ResourceStorageGetOneRequestHandler : IRequestHandler<ResourceStorageGetOneRequest, ResourceStorageGetOneResponse>
    {
        private readonly IRepository<Data.Model.Storage.Resource> _storage;
        private readonly AbstractValidator<ResourceStorageGetOneRequest> _requestValidator;
        private readonly IResourceStorageActionValidator<ResourceStorageGetOneRequest, ResourceStorageGetOneResponse> _validatePreConditions;

        public ResourceStorageGetOneRequestHandler(IRepository<Data.Model.Storage.Resource> storage, 
                ResourceStorageGetOneRequestValidator requestValidator, 
                IResourceStorageActionValidator<ResourceStorageGetOneRequest, ResourceStorageGetOneResponse> validatePreConditions)
        {
            _storage = storage;
            _requestValidator = requestValidator;
            _validatePreConditions = validatePreConditions;
        }

        public async Task<ResourceStorageGetOneResponse> Handle(ResourceStorageGetOneRequest request, CancellationToken cancellationToken)
        {
            var response = new ResourceStorageGetOneResponse();

            var validationResult = _requestValidator.Validate(request);

            if (!validationResult.IsValid)
            {
                response.RequestValidationErrors = validationResult.Errors.Select(x => $"{x.PropertyName}\t{x.ErrorCode}\t{x.ErrorMessage}").ToList();
                response.StatusCode = HttpStatusCodes.BADREQUEST;
            }
            Data.Model.Storage.Resource? resource = null;
            if (string.IsNullOrWhiteSpace(request.Namespace) )
            {
                resource = (await _storage.GetAsync(r => r.Id == request.Id
                                                                                    && r.OwnerId == request.OwnerId
                                                                                    , cancellationToken)).SingleOrDefault();

            }
            else
            {
                resource = (await _storage.GetAsync(r => r.Id == request.Id
                                                                                    && r.OwnerId == request.OwnerId
                                                                                    && r.Namespace == request.Namespace 
                                                                                    , cancellationToken)).SingleOrDefault();

            }


            (resource, response) = _validatePreConditions.Validate(resource, request, response);

            if (resource == null)
            {
                return response;
            }


            response.Model = resource;
            response.StatusCode = HttpStatusCodes.OK;
            return response;
        }
    }

}
