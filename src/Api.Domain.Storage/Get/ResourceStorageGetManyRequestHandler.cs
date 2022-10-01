﻿using MediatR;
using Storage;

namespace Api.Domain.Storage.Get
{
    public class ResourceStorageGetManyRequestHandler : IRequestHandler<ResourceStorageGetManyRequest, ResourceStorageGetManyResponse>
    {
        private readonly IRepository<Data.Model.Storage.Resource> _storage;
        //private readonly IRequestHeadersProvider _requestHeadersProvider;
        //private readonly IMapper _mapper;
        //private readonly Dictionary<string, string> EmptyEntityList = new Dictionary<string, string>() { };
        //private readonly IResponseLinksProvider _responseLinksProvider;

        public ResourceStorageGetManyRequestHandler(IRepository<Data.Model.Storage.Resource> storage
          //IRequestHeadersProvider requestHeadersProvider,
          //IMapper mapper,
          //IResponseLinksProvider responseLinksProvider
          //
          )
        {
            _storage = storage;
            //_requestHeadersProvider = requestHeadersProvider;
            //_mapper = mapper;
            //_responseLinksProvider = responseLinksProvider;
        }

        public async Task<ResourceStorageGetManyResponse> Handle(ResourceStorageGetManyRequest request, CancellationToken cancellationToken)
        {
            var response = new ResourceStorageGetManyResponse();

            IEnumerable<Data.Model.Storage.Resource> resources = new List<Data.Model.Storage.Resource>();

            //var ifModifiedSince = await _requestHeadersProvider.IfModifiedSince(request.Headers) ?? DateTimeOffset.MinValue; // if none make modified as default;


            resources = await _storage.GetAsync(r => r.OwnerId == request.OwnerId && r.Namespace == request.Namespace);

            // 

            if (resources.Any())
            {
                // if all of them are unmodified since then return none
                var unmodifiedItems = resources.Where(r =>
                                            r.Modified.HasValue ? r.Modified < request.IfModifiedSince :
                                            r.Created < request.IfModifiedSince);
                if (unmodifiedItems.Count() == resources.Count())
                {
                    response.StatusCode = 304; //System.Net.HttpStatusCode.NotModified;
                    return response;
                }

                // else return modified since items
                var modifiedItems = resources.Where(r =>
                            r.Modified.HasValue ? r.Modified >= request.IfModifiedSince :
                            r.Created > request.IfModifiedSince);

                response.Model = modifiedItems; //_mapper.Map<IEnumerable<Data.Model.Response.Resource>>(modifiedItems);

            }
            else
            {
                response.Model = resources; // _mapper.Map<IEnumerable<Data.Model.Response.Resource>>(resources);
            }

            //var relatedEntities = EmptyEntityList;
            //foreach (var item in response.Model)
            //{
            //    var systemKeys = new Dictionary<string, string>() {
            //        { "{id}", $"{item.Id}" }
            //     };
            //    item.Links = await _responseLinksProvider.BuildLinks(
            //                                                    request.Scheme,
            //                                                    request.Host,
            //                                                    request.PathBase.TrimEnd('/'),
            //                                                    request.Path.TrimEnd('/'),
            //                                                    systemKeys,
            //                                                    relatedEntities);
            //}
            response.StatusCode = 200;//System.Net.HttpStatusCode.OK;
            return response;
        }
    }

}
