using Data.Model.Storage;
using MediatR;
using Storage;
using System.Net;

namespace Api.Domain.Storage.Put
{
    public class ResourceStoragePutRequestHandler : IRequestHandler<ResourceStoragePutRequest, ResourceStoragePutResponse>
    {
        private readonly IRepository<Data.Model.Storage.Resource> _storage;

        public ResourceStoragePutRequestHandler(IRepository<Resource> storage)
        {
            _storage = storage;
        }

        public async Task<ResourceStoragePutResponse> Handle(ResourceStoragePutRequest request, CancellationToken cancellationToken)
        {
            var response = new ResourceStoragePutResponse() { };

            Data.Model.Storage.Resource? existingResource = (await _storage.GetAsync(r => r.Id == request.Id
                                                                                && r.OwnerId == request.OwnerId
                                                                                && r.Namespace == request.Namespace
                                                                                )).FirstOrDefault();

            if (existingResource == null)
            {
                response.StatusCode = 404; //HttpStatusCode.NotFound;
                return response;
            }


            if (
                    (existingResource.Modified.HasValue ?
                    existingResource.Modified.Value <= request.UnmodifiedSince :
                    existingResource.Created <= request.UnmodifiedSince) || (request.ETags.Contains(existingResource.Etag))
                    )
            {

                existingResource.Content = request.Content;
                existingResource.Modified = DateTimeOffset.UtcNow;
                existingResource.Metadata.Tags.Add(new Tuple<MetadataPropertyNames, object>(MetadataPropertyNames.ChangeRequestIdentifier, request.RequestId));
                existingResource.Metadata.Tags.Add(new Tuple<MetadataPropertyNames, object>(MetadataPropertyNames.Updated, existingResource.Modified));

                if (!string.IsNullOrWhiteSpace(request.MoveTo))
                {
                    existingResource.Namespace = request.MoveTo;
                    existingResource.Metadata.Tags.Add(new Tuple<MetadataPropertyNames, object>(MetadataPropertyNames.NamespaceRename, existingResource.Namespace));
                }

                existingResource = await _storage.UpdateAsync(existingResource);

                //
                response.Model = existingResource;
                response.StatusCode = 200; //HttpStatusCode.OK;
                return response;

            }
            else
            {
                if (request.ETags.Any())
                {
                    response.RequestValidationErrors.Add($"The resource has None of the specified ETags {string.Join(',', request.Etags)}/r/n");
                }
                if (request.UnmodifiedSince != DateTimeOffset.MaxValue)
                {
                    response.RequestValidationErrors.Add($"The resource has been modified since {request.UnmodifiedSince}");
                }
                response.StatusCode = 412;// HttpStatusCode.PreconditionFailed;
                return response;
            }
        }

    }
}
