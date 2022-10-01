using MediatR;
using Storage;

namespace Api.Domain.Storage.Get
{
    public class ResourceStorageGetOneRequestHandler : IRequestHandler<ResourceStorageGetOneRequest, ResourceStorageGetOneResponse>
    {
        private readonly IRepository<Data.Model.Storage.Resource> _storage;
        //private readonly IRequestHeadersProvider _requestHeadersProvider;
        //private readonly IResponseHeadersProvider _responseHeadersProvider;
        //private readonly IMapper _mapper;
        //private readonly Dictionary<string, string> EmptyEntityList = new Dictionary<string, string>() { };
        //private readonly IResponseLinksProvider _responseLinksProvider;

        public ResourceStorageGetOneRequestHandler(
            IRepository<Data.Model.Storage.Resource> storage,
          //IRequestHeadersProvider requestHeadersProvider,
          //IResponseHeadersProvider responseHeadersProvider,
          //IMapper mapper,
          //IResponseLinksProvider responseLinksProvider
            )
        {
            _storage = storage;
            //_requestHeadersProvider = requestHeadersProvider;
            //_responseHeadersProvider = responseHeadersProvider;
            //_mapper = mapper;
            //_responseLinksProvider = responseLinksProvider;
        }

        public async Task<ResourceStorageGetOneResponse> Handle(ResourceStorageGetOneRequest request, CancellationToken cancellationToken)
        {
            var response = new ResourceStorageGetOneResponse();

            Data.Model.Storage.Resource? resource = (await _storage.GetAsync(
                        r => r.Id == request.Id
                        && r.OwnerId == request.OwnerId
                        && r.Namespace == request.Namespace
                )).SingleOrDefault();


            if (resource == null)
            {
                response.StatusCode = 404; // System.Net.HttpStatusCode.NotFound;
                return response;
            }

            //var responseModel = _mapper.Map<Data.Model.Response.Resource>(resource);

            //var ifNoneMatch = await _requestHeadersProvider.IfNoneMatch(request.Headers);
            if (request.ETags.Any() && request.ETags.Contains(resource.Etag))
            {
                //response.Headers = _responseHeadersProvider.AddHeadersFromItem(responseModel);
                response.StatusCode = 304; //System.Net.HttpStatusCode.NotModified;
                return response;
            }

            //var ifModifiedSince = await _requestHeadersProvider.IfModifiedSince(request.Headers);
            if (request.IfModifiedSince.HasValue)
            {
                var resourceHasNotBeenModifiedSince = !(resource.Modified.HasValue ?
                                                            resource.Modified > request.IfModifiedSince.Value :
                                                            resource.Created > request.IfModifiedSince.Value);
                if (resourceHasNotBeenModifiedSince)
                {
                    //response.Headers = _responseHeadersProvider.AddHeadersFromItem(responseModel);
                    response.StatusCode = 304; // System.Net.HttpStatusCode.NotModified;
                    return response;
                }
            }



            //var relatedEntities = EmptyEntityList;
            //var systemKeys = new Dictionary<string, string>() {
            //        { "{id}", $"{resource.Id}" }
            //     };

            //responseModel.Links = await _responseLinksProvider.BuildLinks(
            //                                                request.Scheme,
            //                                                request.Host,
            //                                                request.PathBase.TrimEnd('/'),
            //                                                request.Path.TrimEnd('/'),
            //                                                systemKeys,
            //                                                relatedEntities);

            //response.Headers = _responseHeadersProvider.AddHeadersFromItem(responseModel);
            response.Model = resource;
            response.StatusCode = 200; //System.Net.HttpStatusCode.OK;
            return response;
        }
    }

}
