using MediatR;

namespace Api.Domain.Handling.Put
{
    public class ResourceOutputPutRequestHandler : IRequestHandler<ResourceOutputPutRequest, ResourceOutputResponse<Data.Model.Response.Resource>> { public Task<ResourceOutputResponse<Data.Model.Response.Resource>> Handle(ResourceOutputPutRequest request, CancellationToken cancellationToken) { throw new NotImplementedException(); } }


}
