using MediatR;

namespace Api.Domain.Handling.Get
{
    public class ResourceOutputGetManyRequest : IRequest<ResourceOutputResponse<IEnumerable<Data.Model.Response.Resource>> { }


}
