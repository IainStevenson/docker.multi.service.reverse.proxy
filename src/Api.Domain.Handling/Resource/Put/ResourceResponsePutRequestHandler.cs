using Api.Domain.Handling.Framework;
using AutoMapper;
using FluentValidation.Results;
using MediatR;
using System.Net;

namespace Api.Domain.Handling.Resource.Put
{
    public class ResourceResponsePutRequestHandler : IRequestHandler<ResourceResponsePutRequest, ResourceResponse<Data.Model.Response.Resource>>
    {
        private readonly IResourceContentModifier<Data.Model.Response.Resource> _resourceModifier;
        private readonly IMapper _mapper;
        private readonly IResponseLinksProvider _responseLinksProvider;
        private readonly IResponseHeadersProvider _responseHeadersProvider;
        private static readonly Dictionary<string, string> EmptyEntityList = new Dictionary<string, string>() { };
        public ResourceResponsePutRequestHandler(
            IResourceContentModifier<Data.Model.Response.Resource> resourceModifier,
            IMapper mapper,
            IResponseLinksProvider responseLinksProvider,
            IResponseHeadersProvider responseHeadersProvider)
        {
            _resourceModifier = resourceModifier;
            _mapper = mapper;
            _responseLinksProvider = responseLinksProvider;
            _responseHeadersProvider = responseHeadersProvider;
        }


        public async Task<ResourceResponse<Data.Model.Response.Resource>> Handle(ResourceResponsePutRequest request, CancellationToken cancellationToken)
        {
            ResourceResponse<Data.Model.Response.Resource> response = new ResourceResponse<Data.Model.Response.Resource>();
            response.StatusCode = request.StatusCode;
            if (request.StatusCode == HttpStatusCode.OK)
            {
                var responseModel = _mapper.Map<Data.Model.Response.Resource>(request.Model);

                if (!string.IsNullOrWhiteSpace(request.Keys))
                {
                    responseModel = await _resourceModifier.CollapseContent(responseModel, request.Keys.Split(','));
                }

                var systemKeys = new Dictionary<string, string>() { { "{id}", $"{request.Model.Id}" } };

                var relatedEntities = EmptyEntityList;
                response.Links = await _responseLinksProvider.BuildLinks(
                                                                request.Scheme,
                                                                request.Host,
                                                                request.PathBase.TrimEnd('/'),
                                                                request.Path.TrimEnd('/'),
                                                                systemKeys,
                                                                relatedEntities);

                response.Model = responseModel;
                response.Headers = _responseHeadersProvider.AddHeadersFromItem(response.Model);
            }
            return response;
        }
    }


}
