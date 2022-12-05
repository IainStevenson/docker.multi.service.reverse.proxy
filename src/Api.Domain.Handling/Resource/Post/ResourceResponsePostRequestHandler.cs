﻿using Api.Domain.Handling.Framework;
using AutoMapper;
using MediatR;

namespace Api.Domain.Handling.Resource.Post
{
    public class ResourceResponsePostRequestHandler : IRequestHandler<ResourceResponsePostRequest, ResourceResponse<Data.Model.Response.Resource>>
    {
        private readonly IMapper _mapper;

        private readonly IResponseLinksProvider _responseLinksProvider;
        private readonly IResourceContentModifier<Data.Model.Response.Resource> _resourceModifier;
        private readonly IResponseHeadersProvider _responseHeadersProvider;

        public ResourceResponsePostRequestHandler(
            IMapper mapper,
            IResponseLinksProvider responseLinksProvider,
            IResourceContentModifier<Data.Model.Response.Resource> resourceModifier,
            IResponseHeadersProvider responseHeadersProvider
            )
        {
            _mapper = mapper;
            _responseLinksProvider = responseLinksProvider;
            _resourceModifier = resourceModifier;
            _responseHeadersProvider = responseHeadersProvider;
        }
        public async Task<ResourceResponse<Data.Model.Response.Resource>> Handle(ResourceResponsePostRequest request, CancellationToken cancellationToken)
        {

            var response = new ResourceResponse<Data.Model.Response.Resource>();
            response.StatusCode = request.StatusCode;
            response.RequestValidationErrors = request.RequestValidationErrors;

            if (request.StatusCode != System.Net.HttpStatusCode.Created) return response;

            Data.Model.Response.Resource responseModel = _mapper.Map<Data.Model.Response.Resource>(request.Model);

            if (!string.IsNullOrWhiteSpace(request.ContentKeys))
            {
                responseModel = await _resourceModifier.CollapseContent(responseModel,
                        request.ContentKeys.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
            }

            response.Model = responseModel;
            response.Headers = _responseHeadersProvider.AddHeadersFromItem(responseModel);

            response.Links = await _responseLinksProvider.BuildLinks(
                                                            request.Scheme,
                                                            request.Host,
                                                            request.PathBase,
                                                            request.Path,
                                                            request.ContentNamespace,
                                                            $"{request.Model.Id}"
                                                            );



            return response;
        }
    }
}
