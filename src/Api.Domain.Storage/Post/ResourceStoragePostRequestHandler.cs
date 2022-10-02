using Data.Model.Storage;
using FluentValidation;
using MediatR;
using Storage;

namespace Api.Domain.Storage.Post
{
    public class ResourceStoragePostRequestHandler : IRequestHandler<ResourceStoragePostRequest, ResourceStoragePostResponse>
    {
        private readonly IRepository<Resource> _storage;

        private readonly AbstractValidator<ResourceStoragePostRequest> _validator;
        public ResourceStoragePostRequestHandler(
            IRepository<Resource> storage,
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
                var resource = new Resource()
                {
                    Namespace = request.Namespace.ToLower(),
                    Content = request.Content,
                    OwnerId = request.OwnerId,
                    Metadata = new StorageMetadata() { Tags = new List<Tag>() }
                };

                resource.Metadata.Tags.Add(new Tag() { Name = MetadataPropertyNames.OriginallyCreated, Value = DateTimeOffset.UtcNow });
                resource.Metadata.Tags.Add(new Tag() { Name = MetadataPropertyNames.OriginalDataKeys, Value = request.Keys });

                resource = await _storage.CreateAsync(resource, cancellationToken);
                response.Model = resource;
                response.StatusCode = 201; // System.Net.HttpStatusCode.Created;
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
