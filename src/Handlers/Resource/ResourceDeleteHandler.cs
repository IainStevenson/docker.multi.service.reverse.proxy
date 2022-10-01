using Api.Domain.Handling.Framework;
using MediatR;
using Storage;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Handlers.Resource
{
    public class ResourceDeleteHandler : IRequestHandler<ResourceDeleteRequest, ResourceDeleteResponse>
    {
        private readonly IRepository<Data.Model.Storage.Resource> _storage;
        private readonly IRequestHeadersProvider _requestHeadersProvider;

        public ResourceDeleteHandler(
            IRepository<Data.Model.Storage.Resource> storage,
            IRequestHeadersProvider requestHeadersProvider)
        {
            _storage = storage;
            _requestHeadersProvider = requestHeadersProvider;
        }

        public async Task<ResourceDeleteResponse> Handle(ResourceDeleteRequest request, CancellationToken cancellationToken)
        {
            var response = new ResourceDeleteResponse() { };

            Data.Model.Storage.Resource resource = (await _storage.GetAsync(r =>
                               r.Id == request.Id
                               && r.OwnerId == request.OwnerId
                               && r.Namespace == request.Namespace
                               )).FirstOrDefault();


            if (resource == null)
            {
                response.StatusCode = HttpStatusCode.NotFound;
                return response;
            }


            var unmodifiedSince = await _requestHeadersProvider.IfUnmodifiedSince(request.Headers) ?? DateTimeOffset.MaxValue; // if none make unmodifiedever as default
            var etags = await _requestHeadersProvider.IfMatch(request.Headers);

            // only proceed if resource is unmodified since or is one of the etags
            if (
                    (resource.Modified.HasValue ? resource.Modified.Value <= unmodifiedSince : resource.Created <= unmodifiedSince) ||
                    (etags.Contains(resource.Etag))
                    )
            {

                var count = await _storage.DeleteAsync(request.Id);
                if (count == 1)
                {
                    response.StatusCode = HttpStatusCode.NoContent;
                    return response;
                }
                response.RequestValidationErrors.Add($"The resource deletion attempt was made but did not happen.");

                response.StatusCode = HttpStatusCode.Gone;
                return response;
            }
            else
            {

                if (etags.Any())
                {
                    response.RequestValidationErrors.Add($"The resource has None of the specified ETags {string.Join(',', etags)}/r/n");
                }
                if (unmodifiedSince != DateTimeOffset.MinValue)
                {
                    response.RequestValidationErrors.Add($"The resource has been modified since {unmodifiedSince}");
                }
                response.StatusCode = HttpStatusCode.PreconditionFailed;
                return response;
            }
        }
    }
}
