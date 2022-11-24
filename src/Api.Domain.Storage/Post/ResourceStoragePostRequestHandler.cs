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
       

        public ResourceStoragePostRequestHandler(IRepository<Resource> storage, ResourceStoragePostRequestValidator validator)
        {
            _storage = storage;
            _validator = validator;
        }
        public async Task<ResourceStoragePostResponse> Handle(ResourceStoragePostRequest request, CancellationToken cancellationToken)
        {
            var response = new ResourceStoragePostResponse();

            var validationResult = _validator.Validate(request);

            if (!validationResult.IsValid)
            {
                response.RequestValidationErrors = validationResult.Errors.Select(x => $"{x.PropertyName}\t{x.ErrorCode}\t{x.ErrorMessage}").ToList();
                response.StatusCode = StatusCodes.BADREQUEST;
            }

            var resource = new Resource()
            {
                Namespace = request.Namespace.ToLower(),
                Content = request.Content,
                OwnerId = request.OwnerId,
                Metadata = new StorageMetadata()
                {
                    Tags = new List<Tag>
                            {
                                new Tag() { Name = MetadataPropertyNames.OriginallyCreated, Value = DateTimeOffset.UtcNow },
                                new Tag() { Name = MetadataPropertyNames.OriginalDataKeys, Value = request.Keys }
                            }
                }
            };

            response.Model = await _storage.CreateAsync(resource, cancellationToken);
            response.StatusCode = StatusCodes.CREATED;
            return response;
        }
    }
}
