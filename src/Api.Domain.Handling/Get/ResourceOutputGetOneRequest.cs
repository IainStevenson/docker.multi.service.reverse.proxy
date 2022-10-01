using MediatR;

namespace Api.Domain.Handling.Get
{
    public class ResourceOutputGetOneRequest : IRequest<ResourceOutputResponse<Data.Model.Response.Resource>> { }


}
