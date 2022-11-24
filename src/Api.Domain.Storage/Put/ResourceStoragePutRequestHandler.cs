using Data.Model.Storage;
using FluentValidation;
using MediatR;
using Storage;

namespace Api.Domain.Storage.Put
{
    public class ResourceStoragePutRequestHandler : IRequestHandler<ResourceStoragePutRequest, ResourceStoragePutResponse>
    {
        private readonly IRepository<Resource> _storage;
        private readonly AbstractValidator<ResourceStoragePutRequest> _requestValidator;
        private readonly ResourceStoragePutValidator _validatePreConditions;


        public ResourceStoragePutRequestHandler(IRepository<Resource> storage, ResourceStoragePutRequestValidator requestValidator, ResourceStoragePutValidator validatePreConditions)
        {
            _storage = storage;
            _requestValidator = requestValidator;
            _validatePreConditions = validatePreConditions;
        }

        public async Task<ResourceStoragePutResponse> Handle(ResourceStoragePutRequest request, CancellationToken cancellationToken)
        {

            var response = new ResourceStoragePutResponse() { };
            var validationResult = _requestValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                response.StatusCode = StatusCodes.BADREQUEST;
                response.RequestValidationErrors = validationResult.Errors.Select(x => $"{x.PropertyName}\t{x.ErrorCode}\t{x.ErrorMessage}").ToList();
                return response;
            }

            Resource? existingResource = (await _storage.GetAsync(r => r.Id == request.Id
                                                                                && r.OwnerId == request.OwnerId
                                                                                && r.Namespace == request.Namespace
                                                                                )).FirstOrDefault();
         
            (existingResource, response) = _validatePreConditions.Validate(existingResource, response);

            if (response.StatusCode != StatusCodes.OK)
            {
                return response;
            }

            existingResource.Content = request.Content;
            existingResource.Modified = DateTimeOffset.UtcNow;
            existingResource.Metadata.Tags.Add(new Tag() { Name = MetadataPropertyNames.ChangeRequestIdentifier, Value = request.RequestId });
            existingResource.Metadata.Tags.Add(new Tag() { Name = MetadataPropertyNames.Updated, Value = existingResource.Modified });

            if (!string.IsNullOrWhiteSpace(request.MoveTo))
            {
                existingResource.Namespace = request.MoveTo;
                existingResource.Metadata.Tags.Add(new Tag() { Name = MetadataPropertyNames.NamespaceRename, Value = existingResource.Namespace });
            }
            
            response.Model = await _storage.UpdateAsync(existingResource); ;
            response.StatusCode = StatusCodes.OK;
            return response;
        }



    }
}
