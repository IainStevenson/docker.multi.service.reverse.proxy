using MediatR;

namespace Api.Domain.Handling.Resource.Get
{
    public class ResourceResponseGetManyRequestHandler : IRequestHandler<ResourceResponseGetManyRequest, ResourceResponse<IEnumerable<Data.Model.Response.Resource>>> { public Task<ResourceResponse<IEnumerable<Data.Model.Response.Resource>>> Handle(ResourceResponseGetManyRequest request, CancellationToken cancellationToken) { throw new NotImplementedException(); } }


}
