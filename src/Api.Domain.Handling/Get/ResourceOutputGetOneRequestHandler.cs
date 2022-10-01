using MediatR;

namespace Api.Domain.Handling.Get
{
    public class ResourceOutputGetOneRequestHandler : IRequestHandler<ResourceOutputGetOneRequest, ResourceOutputResponse<Data.Model.Response.Resource>> { public Task<ResourceOutputResponse<Data.Model.Response.Resource>> Handle(ResourceOutputGetOneRequest request, CancellationToken cancellationToken) { throw new NotImplementedException(); } }


}
