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

            if (request.Model == null) throw new ArgumentNullException(nameof(request.Model));

            ResourceResponse<Data.Model.Response.Resource> response = new ResourceResponse<Data.Model.Response.Resource>();
            
            response.StatusCode = request.StatusCode;
            if (request.StatusCode == HttpStatusCode.OK)
            {
                var responseModel = _mapper.Map<Data.Model.Response.Resource>(request.Model);

                if (!string.IsNullOrWhiteSpace(request.Keys))
                {
                    responseModel = await _resourceModifier.CollapseContent(responseModel, request.Keys.Split(','));
                }

                response.Links = await _responseLinksProvider.BuildLinks(
                                                                request.Scheme,
                                                                request.Host,
                                                                request.PathBase,
                                                                request.Path,
                                                                request.Namespace,
                                                                $"{request.Model.Id}");

                response.Model = responseModel;
                response.Headers = _responseHeadersProvider.AddHeadersFromItem(response.Model);
            }
            return response;
        }
    }


}
