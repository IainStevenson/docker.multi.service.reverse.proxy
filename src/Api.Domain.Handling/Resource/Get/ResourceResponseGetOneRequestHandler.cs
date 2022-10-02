using Api.Domain.Handling.Framework;
using AutoMapper;
using MediatR;

namespace Api.Domain.Handling.Resource.Get
{
    public class ResourceResponseGetOneRequestHandler : IRequestHandler<ResourceResponseGetOneRequest, ResourceResponse<Data.Model.Response.Resource>>
    {
        private readonly IRequestHeadersProvider _requestHeadersProvider;
        private readonly IResponseHeadersProvider _responseHeadersProvider;
        private readonly IMapper _mapper;
        private readonly Dictionary<string, string> EmptyEntityList = new Dictionary<string, string>() { };
        private readonly IResponseLinksProvider _responseLinksProvider;


        public ResourceResponseGetOneRequestHandler(
            IRequestHeadersProvider requestHeadersProvider,
            IResponseHeadersProvider responseHeadersProvider,
            IMapper mapper,
            IResponseLinksProvider responseLinksProvider
            )
        {
            _requestHeadersProvider = requestHeadersProvider;
            _responseHeadersProvider = responseHeadersProvider;
            _mapper = mapper;
            _responseLinksProvider = responseLinksProvider;
        }
        public async Task<ResourceResponse<Data.Model.Response.Resource>> Handle(ResourceResponseGetOneRequest request, CancellationToken cancellationToken)
        {
            ResourceResponse<Data.Model.Response.Resource> response = new ResourceResponse<Data.Model.Response.Resource>();
            var responseModel = _mapper.Map<Data.Model.Response.Resource>(request.Model);

            response.Headers = _responseHeadersProvider.AddHeadersFromItem(responseModel);


            var relatedEntities = EmptyEntityList;
            var systemKeys = new Dictionary<string, string>() { { "{id}", $"{request.Model.Id}" } };

            response.Links = await _responseLinksProvider.BuildLinks(
                                                            request.Scheme,
                                                            request.Host,
                                                            request.PathBase.TrimEnd('/'),
                                                            request.Path.TrimEnd('/'),
                                                            systemKeys,
                                                            relatedEntities);

            response.Headers = _responseHeadersProvider.AddHeadersFromItem(responseModel);
            throw new NotImplementedException();
        }
    }
}
