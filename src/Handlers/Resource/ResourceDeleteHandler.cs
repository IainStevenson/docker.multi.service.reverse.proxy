using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Handlers.Resource
{
    public class ResourceDeleteHandler : IRequestHandler<ResourceDeleteRequest, ResourceDeleteResponse>
    {
        public Task<ResourceDeleteResponse> Handle(ResourceDeleteRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new ResourceDeleteResponse());
        }
    }
}
