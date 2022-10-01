using MediatR;

namespace Api.Domain.Handling.Resource.Put
{
    public class ResourceResponsePutRequestHandler : IRequestHandler<ResourceRresponsePutRequest, ResourceResponse<Data.Model.Response.Resource>> { public Task<ResourceResponse<Data.Model.Response.Resource>> Handle(ResourceRresponsePutRequest request, CancellationToken cancellationToken) { throw new NotImplementedException(); } }


}
