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
        private readonly IResourceStorageActionValidator<ResourceStoragePutRequest, ResourceStoragePutResponse> _actionValidator;


        public ResourceStoragePutRequestHandler(IRepository<Resource> storage, 
            ResourceStoragePutRequestValidator requestValidator,
            IResourceStorageActionValidator<ResourceStoragePutRequest, ResourceStoragePutResponse> actionValidator)
        {
            _storage = storage;
            _requestValidator = requestValidator;
            _actionValidator = actionValidator;
        }

        public async Task<ResourceStoragePutResponse> Handle(ResourceStoragePutRequest request, CancellationToken cancellationToken)
        {

            var response = new ResourceStoragePutResponse() { };
            
            var validationResult = _requestValidator.Validate(request);

            if (!validationResult.IsValid)
            {
                response.StatusCode = HttpStatusCodes.BADREQUEST;
                response.RequestValidationErrors = validationResult.Errors.Select(x => $"{x.PropertyName}\t{x.ErrorCode}\t{x.ErrorMessage}").ToList();
                return response;
            }

            Resource? resource = (await _storage.GetAsync(r => r.Id == request.Id
                                                                && r.OwnerId == request.OwnerId
                                                                && r.Namespace == request.Namespace
                                                                , cancellationToken)).FirstOrDefault();
         
            (resource, response) = _actionValidator.Validate(resource,request, response);

            if (resource == null)
            {
                return response;
            }

            resource.Content = request.Content;
            resource.Modified = DateTimeOffset.UtcNow;
            resource.Metadata.Tags.Add(new Tag() { Name = MetadataPropertyNames.ChangeRequestIdentifier, Value = request.RequestId });
            resource.Metadata.Tags.Add(new Tag() { Name = MetadataPropertyNames.Updated, Value = resource.Modified });

            if (!string.IsNullOrWhiteSpace(request.MoveTo))
            {
                resource.Namespace = request.MoveTo;
                resource.Metadata.Tags.Add(new Tag() { Name = MetadataPropertyNames.NamespaceRename, Value = resource.Namespace });
            }
            
            response.Model = await _storage.UpdateAsync(resource, cancellationToken); ;
            response.StatusCode = HttpStatusCodes.OK;
            return response;
        }
    }
}
