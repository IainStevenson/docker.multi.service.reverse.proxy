using AutoMapper;
using MediatR;
using Response.Formater;
using Storage;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Handlers.Resource
{
    public class ResourceGetOneHandler : IRequestHandler<ResourceGetOneRequest, ResourceGetOneResponse>
    {
        private readonly IRepository<Data.Model.Storage.Resource> _storage;
        private readonly IRequestHeadersProvider _requestHeadersProvider;
        private readonly IResponseHeadersProvider _responseHeadersProvider;
        private readonly IMapper _mapper;
        private readonly Dictionary<string, string> EmptyEntityList = new Dictionary<string, string>() { };
        private readonly IResponseLinksProvider _responseLinksProvider;

        public ResourceGetOneHandler(IRepository<Data.Model.Storage.Resource> storage,
          IRequestHeadersProvider requestHeadersProvider,
          IResponseHeadersProvider responseHeadersProvider,
          IMapper mapper,
          IResponseLinksProvider responseLinksProvider)
        {
            _storage = storage;
            _requestHeadersProvider = requestHeadersProvider;
            _responseHeadersProvider = responseHeadersProvider;
            _mapper = mapper;
            _responseLinksProvider= responseLinksProvider;
        }

        public async Task<ResourceGetOneResponse> Handle(ResourceGetOneRequest request, CancellationToken cancellationToken)
        {
            var response = new ResourceGetOneResponse();

            Data.Model.Storage.Resource resource = await _storage.GetAsync(request.Id);


            if (resource == null)
            {
                response.StatusCode = System.Net.HttpStatusCode.NotFound;
                return response;
            }

            var responseModel = _mapper.Map<Data.Model.Response.Resource>(resource);

            var ifNoneMatch = await _requestHeadersProvider.IfNoneMatch(request.Headers);
            if (ifNoneMatch.Any() &&  ifNoneMatch.Contains(resource.Etag))
            {
                response.Headers =  _responseHeadersProvider.AddHeadersFromItem(responseModel);
                response.StatusCode = System.Net.HttpStatusCode.NotModified;
                return response;
            }

            var ifModifiedSince = await _requestHeadersProvider.IfModifiedSince(request.Headers);
            if (ifModifiedSince.HasValue && ifModifiedSince.HasValue)
            {
                var resourceHasNotBeenModifiedSince = !(resource.Modified.HasValue ?
                                                            resource.Modified > ifModifiedSince.Value :
                                                            resource.Created > ifModifiedSince.Value);
                if (resourceHasNotBeenModifiedSince)
                {

                    response.Headers =  _responseHeadersProvider.AddHeadersFromItem(responseModel);
                    response.StatusCode = System.Net.HttpStatusCode.NotModified;
                    return response;
                }

            }


            
            var relatedEntities = EmptyEntityList;
            var systemKeys = new Dictionary<string, string>() { { "{id}", $"{request.Id}" } };

            responseModel.Links = await _responseLinksProvider.BuildLinks(
                                                            request.Scheme,
                                                            request.Host,
                                                            request.Path.TrimEnd('/'),
                                                            systemKeys,
                                                            relatedEntities);

            response.Headers =  _responseHeadersProvider.AddHeadersFromItem(responseModel);
            response.Model = responseModel;
            response.StatusCode = System.Net.HttpStatusCode.OK;
            return response;
        }
    }
}
