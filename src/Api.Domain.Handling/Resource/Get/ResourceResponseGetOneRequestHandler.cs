using MediatR;

namespace Api.Domain.Handling.Resource.Get
{
    public class ResourceResponseGetOneRequestHandler : IRequestHandler<ResourceResponseGetOneRequest, ResourceResponse<Data.Model.Response.Resource>> { public Task<ResourceResponse<Data.Model.Response.Resource>> Handle(ResourceResponseGetOneRequest request, CancellationToken cancellationToken) { throw new NotImplementedException(); } }


}
