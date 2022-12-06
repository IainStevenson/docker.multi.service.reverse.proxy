using MediatR;

namespace Api.Domain.Handling.Resource.Delete
{
    public class ResourceResponseDeleteRequestHandler : IRequestHandler<ResourceResponseDeleteRequest, ResourceResponse>
    {
        public async Task<ResourceResponse> Handle(ResourceResponseDeleteRequest request, CancellationToken cancellationToken)
        {

            var response = new ResourceResponse
            {
                //404 - not found ,410 = gone ,204 deleted nocontent,
                StatusCode = request.StatusCode
            };
            if (request.StatusCode == System.Net.HttpStatusCode.Gone)
            {
                response.RequestValidationErrors = request.RequestValidationErrors;
            }

            return response;
        }
    }


}
