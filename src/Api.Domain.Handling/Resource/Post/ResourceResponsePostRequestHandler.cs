using Api.Domain.Handling.Framework;
using AutoMapper;
using MediatR;

namespace Api.Domain.Handling.Resource.Post
{
    public class ResourceResponsePostRequestHandler : IRequestHandler<ResourceResponsePostRequest, ResourceResponse<Data.Model.Response.Resource>>
    {
        private readonly IMapper _mapper;
        private readonly Dictionary<string, string> EmptyEntityList = new() { };
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


            if (request.StatusCode != System.Net.HttpStatusCode.Created) return response;

            Data.Model.Response.Resource responseModel = _mapper.Map<Data.Model.Response.Resource>(request.Model);

            if (!string.IsNullOrWhiteSpace(request.Keys))
            {
                responseModel = await _resourceModifier.CollapseContent(responseModel,
                        request.Keys.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
            }

            response.Model = responseModel;
            response.Headers = _responseHeadersProvider.AddHeadersFromItem(responseModel);
            var systemKeys = new Dictionary<string, string>() { { "{id}", $"{request.Model.Id}" }};
            response.Links = await _responseLinksProvider.BuildLinks(
                                                            request.Scheme,
                                                            request.Host,
                                                            request.PathBase,
                                                            request.Path,
                                                            systemKeys,
                                                            EmptyEntityList);



            return response;
        }
    }
}
