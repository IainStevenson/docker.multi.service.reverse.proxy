using MediatR;

namespace Api.Domain.Storage.Put
{
    public class ResourceStoragePutRequestHandler : IRequestHandler<ResourceStoragePutRequest, ResourceStoragePutResponse>
    {
        public async Task<ResourceStoragePutResponse> Handle(ResourceStoragePutRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

}
