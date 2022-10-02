using FluentValidation;
using MediatR;
using Storage;

namespace Api.Domain.Storage.Post
{
    public class ResourceStoragePostRequestHandler : IRequestHandler<ResourceStoragePostRequest, ResourceStoragePostResponse>
    {
        private readonly IRepository<Data.Model.Storage.Resource> _storage;
       
        private readonly AbstractValidator<ResourceStoragePostRequest> _validator;
        public ResourceStoragePostRequestHandler(
            IRepository<Data.Model.Storage.Resource> storage,           
            ResourceStoragePostRequestValidator validator
            )
        {
            _storage = storage;            
            _validator = validator;
        }
        public async Task<ResourceStoragePostResponse> Handle(ResourceStoragePostRequest request, CancellationToken cancellationToken)
        {
            var response = new ResourceStoragePostResponse();

            var validationResult = _validator.Validate(request);

            if (validationResult.IsValid)
            {
                var resource = new Data.Model.Storage.Resource()
                {
                    Namespace = request.Namespace.ToLower(),
                    Content = request.Content,
                    OwnerId = request.OwnerId,
                    Metadata = new Data.Model.Storage.StorageMetadata()
                    {
                        RequestId = request.RequestId,
                        Tags = new List<Tuple<Data.Model.Storage.MetadataPropertyNames, object>>() {
                             new Tuple<Data.Model.Storage.MetadataPropertyNames, object>(Data.Model.Storage.MetadataPropertyNames.OriginallyCreated, DateTimeOffset.UtcNow)  ,
                             new Tuple<Data.Model.Storage.MetadataPropertyNames, object>(Data.Model.Storage.MetadataPropertyNames.OriginalDataKeys, request.Keys)
                        }
                    },
                };
                resource = await _storage.CreateAsync(resource, cancellationToken);     
                response.Model = resource;
                response.StatusCode = 201 ; // System.Net.HttpStatusCode.Created;
            }
            else
            {                
                response.RequestValidationErrors.AddRange(validationResult.Errors.Select(x => x.ErrorMessage));    
                response.StatusCode = 400; // System.Net.HttpStatusCode.BadRequest;
            }
            return response;
        }
    }

}
