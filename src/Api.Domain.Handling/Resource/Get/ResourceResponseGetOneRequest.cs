using MediatR;

namespace Api.Domain.Handling.Resource.Get
{
    public class ResourceResponseGetOneRequest : IRequest<ResourceResponse<Data.Model.Response.Resource>> { }


}
