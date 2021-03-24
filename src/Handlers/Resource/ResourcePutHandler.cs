using AutoMapper;
using MediatR;
using Response.Formater;
using Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Handlers.Resource
{
    public class ResourcePutHandler : IRequestHandler<ResourcePutRequest, ResourcePutResponse>
    {
        private readonly IRepository<Data.Model.Storage.Resource> _storage;
        private readonly IRequestHeadersProvider _requestHeadersProvider;
        private readonly IMapper _mapper;
        private readonly IResponseHeadersProvider _responseHeadersProvider;
        private readonly IResponseLinksProvider _responseLinksProvider;
        private readonly IResourceContentModifier<Data.Model.Response.Resource> _resourceModifier;
        private static readonly Dictionary<string, string> EmptyEntityList = new Dictionary<string, string>() { };
        public ResourcePutHandler(IRepository<Data.Model.Storage.Resource> storage,
            IRequestHeadersProvider requestHeadersProvider,
            IMapper mapper,
            IResponseHeadersProvider responseHeadersProvider,
            IResponseLinksProvider responseLinksProvider,
            IResourceContentModifier<Data.Model.Response.Resource> resourceModifier)
        {
            _storage = storage;
            _requestHeadersProvider = requestHeadersProvider;
            _mapper = mapper;
            _responseHeadersProvider = responseHeadersProvider;
            _responseLinksProvider = responseLinksProvider;
            _resourceModifier = resourceModifier;
        }

        public async Task<ResourcePutResponse> Handle(ResourcePutRequest request, CancellationToken cancellationToken)
        {

            var response = new ResourcePutResponse() { };

            Data.Model.Storage.Resource resource = await _storage.GetAsync(request.Id);

            if (resource == null)
            {
                response.StatusCode = HttpStatusCode.NotFound;
                return response;
            }

            var unmodifiedSince = await _requestHeadersProvider.IfUnmodifiedSince(request.Headers)?? DateTimeOffset.MaxValue;
            var etags = await _requestHeadersProvider.IfMatch(request.Headers);

            // only proceed if resource is unmodified since or is one of the etags
            if (
                    (resource.Modified.HasValue ? 
                    resource.Modified.Value <= unmodifiedSince : resource.Created <= unmodifiedSince) ||
                    (etags.Contains(resource.Etag))
                    )
            {

                resource.Content = request.Model;
                resource.Namespace = request.Namespace;
                resource.Modified = DateTimeOffset.UtcNow;

                resource = await _storage.UpdateAsync(resource);

                var responseModel = _mapper.Map<Data.Model.Response.Resource>(resource);


                if (!string.IsNullOrWhiteSpace(request.Keys))
                {
                    responseModel = await _resourceModifier.CollapseContent(responseModel, request.Keys.Split(','));
                }

                var systemKeys = new Dictionary<string, string>() { { "{id}", $"{resource.Id}" } };
                var relatedEntities = EmptyEntityList;
                response.Model.Links = await _responseLinksProvider.BuildLinks(
                                                                request.Scheme,
                                                                request.Host,
                                                                request.Path.TrimEnd('/'),
                                                                systemKeys,
                                                                relatedEntities);

                response.Model = responseModel;
                response.Headers =  _responseHeadersProvider.AddHeadersFromItem(response.Model);
                response.StatusCode = HttpStatusCode.OK;
                return response;

            }
            else
            {
                if (etags.Any())
                {
                    response.RequestValidationErrors.Add($"The resource has None of the specified ETags {string.Join(',', etags)}/r/n");
                }
                if (unmodifiedSince != DateTimeOffset.MaxValue)
                {
                    response.RequestValidationErrors.Add($"The resource has been modified since {unmodifiedSince}");
                }
                response.StatusCode = HttpStatusCode.PreconditionFailed;
                return response;
            }
        }
    }
}
