using MediatR;

namespace Api.Domain.Handling.Get
{
    public class ResourceOutputGetManyRequestHandler : IRequestHandler<ResourceOutputGetManyRequest, ResourceOutputResponse<IEnumerable<Data.Model.Response.Resource>>> { public Task<ResourceOutputResponse<IEnumerable<Data.Model.Response.Resource>>> Handle(ResourceOutputGetManyRequest request, CancellationToken cancellationToken) { throw new NotImplementedException(); } }


}
