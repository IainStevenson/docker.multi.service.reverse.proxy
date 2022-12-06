using Data.Model.Storage;
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
                response.StatusCode = ApiDomainStatusCodes.BADREQUEST;
            }


            Data.Model.Storage.Resource? resource = (await _storage.GetAsync(r => r.Id == request.Id
                                                                                   && r.OwnerId == request.OwnerId
                                                                                   && r.Namespace == request.ContentNamespace
                                                                                   && r.Deleted == null
                                                                                  , cancellationToken)).FirstOrDefault();

            (resource,response) = _actionValidator.Validate(resource, request, response);

            if (resource == null)
            {
                return response;
            }

            // perform soft delete
            // resource.Content = null; job will set content to null after grace period has expired.
            // based on Deleted date
            resource.Deleted = DateTimeOffset.UtcNow; 
            // mark even in metadata
            resource.Metadata.Tags.Add(new Tag() { Name = MetadataPropertyNames.ChangeRequestIdentifier, Value = request.RequestId });
            resource.Metadata.Tags.Add(new Tag() { Name = MetadataPropertyNames.Deleted, Value = resource.Deleted });

            await _storage.UpdateAsync(resource, cancellationToken);
            // do not hard delete
            //var count = await _storage.DeleteAsync(request.Id, cancellationToken);
            response.StatusCode = ApiDomainStatusCodes.NOCONTENT;
            return response;

        }
    }
}

