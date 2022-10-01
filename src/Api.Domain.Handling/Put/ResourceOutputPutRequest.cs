using MediatR;

namespace Api.Domain.Handling.Put
{
    public class ResourceOutputPutRequest : IRequest<ResourceOutputResponse<Data.Model.Response.Resource>> { }


}
