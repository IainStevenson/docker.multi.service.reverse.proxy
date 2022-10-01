using MediatR;

namespace Api.Domain.Handling.Resource.Put
{
    public class ResourceResponsePutRequestHandler : IRequestHandler<ResourceResponsePutRequest, ResourceResponse<Data.Model.Response.Resource>>
    {
        public Task<ResourceResponse<Data.Model.Response.Resource>> Handle(ResourceResponsePutRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }


}
