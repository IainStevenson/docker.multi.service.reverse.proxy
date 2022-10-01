using MediatR;

namespace Api.Domain.Handling.Delete
{
    public class ResourceOutputDeleteRequestHandler : IRequestHandler<ResourceOutputDeleteRequest, ResourceOutputResponse>
    {
        public async Task<ResourceOutputResponse> Handle(ResourceOutputDeleteRequest request, CancellationToken cancellationToken) { 
            
            var response = new ResourceOutputResponse();

            

            return response;
        }
    }


}
