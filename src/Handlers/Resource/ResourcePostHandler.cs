﻿using Data.Model.Storage;
using FluentValidation;
using MediatR;
using Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Handlers.Resource
{
    /// <summary>
    /// Handles the posting of a <see cref="Data.Model.Storage.Resource"/> and the return of a <see cref="Data.Model.Response.Resource"/> with <see cref="HttpStatusCode.Created"/> and providing Hateos Links and Location header, or <see cref="HttpStatusCode.BadRequest"></see>
    /// </summary>
    public class ResourcePostHandler : IRequestHandler<ResourcePostRequest, ResourcePostResponse>
    {
        private readonly IRepository<Data.Model.Storage.Resource> _storage;
        private readonly IMapper _mapper;
        private readonly Dictionary<string, string> EmptyEntityList = new() { };
        private readonly IResponseLinksProvider _responseLinksProvider;
        private readonly IResourceContentModifier<Data.Model.Response.Resource> _resourceModifier;
        private readonly IResponseHeadersProvider _responseHeadersProvider;
        private readonly AbstractValidator<ResourcePostRequest> _validator;
        public ResourcePostHandler(IRepository<Data.Model.Storage.Resource> storage,
            IMapper mapper,
            IResponseLinksProvider responseLinksProvider,
            IResourceContentModifier<Data.Model.Response.Resource> resourceModifier,
            IResponseHeadersProvider responseHeadersProvider,
            ResourcePostValidator validator)
        {
            _storage = storage;
            _mapper = mapper;
            _responseLinksProvider = responseLinksProvider;
            _resourceModifier = resourceModifier;
            _responseHeadersProvider = responseHeadersProvider;
            _validator = validator;
        }

        public async Task<ResourcePostResponse> Handle(ResourcePostRequest request, CancellationToken cancellationToken)
        {
            var response = new ResourcePostResponse();

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
                        Tags = new List<Tuple<MetadataPropertyNames, object>>() {
                             new Tuple<MetadataPropertyNames, object>(MetadataPropertyNames.OriginallyCreated, DateTimeOffset.UtcNow)  ,
                             new Tuple<MetadataPropertyNames, object>(MetadataPropertyNames.OriginalDataKeys, request.Keys)
                        }
                    },
                };

                resource = await _storage.CreateAsync(resource, cancellationToken);

                #region abstract out
                var systemKeys = new Dictionary<string, string>() {
                    { "{id}", $"{resource.Id}" }
                };

                var relatedEntities = EmptyEntityList;

                var responseModel = _mapper.Map<Data.Model.Response.Resource>(resource);


                responseModel.Links = await _responseLinksProvider.BuildLinks(
                                                                request.Scheme,
                                                                request.Host,
                                                                request.PathBase.TrimEnd('/'),
                                                                request.Path.TrimEnd('/'),
                                                                systemKeys,
                                                                relatedEntities);

                #endregion
                if (!string.IsNullOrWhiteSpace(request.Keys))
                {
                    responseModel = await _resourceModifier.CollapseContent(responseModel, request.Keys.Split(','));
                }

                response.Model = responseModel;
                response.StatusCode = System.Net.HttpStatusCode.Created;
                #region abstract out too
                response.ResourceUri = new Uri(responseModel.Links?.SingleOrDefault(x => x.Action == "get" && x.Rel == "self")?.Href ?? "\\");
                response.Headers = _responseHeadersProvider.AddHeadersFromItem(responseModel);
                #endregion
            }
            else
            {

                #region abstract out
                response.RequestValidationErrors.AddRange(validationResult.Errors.Select(x => x.ErrorMessage));
                #endregion

                response.StatusCode = System.Net.HttpStatusCode.BadRequest;
            }
            return response;
        }
    }
}