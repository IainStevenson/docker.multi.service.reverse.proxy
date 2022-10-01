using MediatR;

namespace Api.Domain.Handling.Resource.Delete
{
    public class ResourceResponseDeleteRequestHandler : IRequestHandler<ResourceResponseDeleteRequest, ResourceResponse>
    {
        public async Task<ResourceResponse> Handle(ResourceResponseDeleteRequest request, CancellationToken cancellationToken) { 
            
            var response = new ResourceResponse();

            

            return response;
        }
    }


}
