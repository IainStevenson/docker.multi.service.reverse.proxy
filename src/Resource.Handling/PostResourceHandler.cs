using AutoMapper;
using Data.Model.Storage;
using FluentValidation;
using MediatR;
using Storage;

namespace Resource.Handling
{
    /// <summary>
    /// Handles the Persistence layer processing for the incoming resource.
    /// </summary>
    public class PostResourceHandler : IRequestHandler<PostResourceRequest, PostResourceResponse>
    {
        /// <summary>
        /// Persists the resource in the storage media.
        /// </summary>
        private readonly IRepository<Data.Model.Storage.Resource> _storage;
        /// <summary>
        /// Maps the resource data types.
        /// </summary>
        private readonly IMapper _mapper;
        /// <summary>
        /// Validates the incoming resource request data.
        /// </summary>
        private readonly AbstractValidator<PostResourceRequest> _validator;
        public PostResourceHandler(IRepository<Data.Model.Storage.Resource> storage, IMapper mapper, AbstractValidator<PostResourceRequest> validator)
        {
            _storage = storage;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<PostResourceResponse> Handle(PostResourceRequest request, CancellationToken cancellationToken)
        {
            var response = new PostResourceResponse();

            var validationResult = _validator.Validate(request);

            if (!validationResult.IsValid)
            {
                response.RequestValidationErrors = validationResult.Errors.Select(x => x.ErrorMessage);
                return response;
            }

            var resource = new Data.Model.Storage.Resource()
            {
                Namespace = request.Namespace.ToLower(),
                Content = request.Content,
                OwnerId = request.OwnerId,
                Metadata = new StorageMetadata()
                {
                    RequestId = request.RequestId,
                    Tags = new List<Tuple<MetadataPropertyNames, object>>() {
                                { new Tuple<MetadataPropertyNames, object>(MetadataPropertyNames.OriginallyCreated, $"{DateTimeOffset.UtcNow:o}") } ,
                                { new Tuple<MetadataPropertyNames, object>(MetadataPropertyNames.OriginalDataKeys, request.Keys) } ,
                                { new Tuple<MetadataPropertyNames, object>(MetadataPropertyNames.ChangeRequestIdentifier, $"{request.RequestId}") } ,
                            },
                    Index = request.Index,
                },
            };

            resource = await _storage.CreateAsync(resource, cancellationToken);
            var responseModel = _mapper.Map<Data.Model.Response.Resource>(resource);
            response.Model = responseModel;
            response.StatusCode = 201;

            return response;
        }
    }
}