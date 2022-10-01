using MediatR;

namespace Api.Domain.Handling.Resource.Get
{
    public class ResourceResponseGetManyRequest : IRequest<ResourceResponse<IEnumerable<Data.Model.Response.Resource>>> { }


}
