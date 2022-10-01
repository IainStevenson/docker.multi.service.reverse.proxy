using MediatR;
using Microsoft.AspNetCore.Http;

namespace Api.Domain.Handling.Delete
{
    public class ResourceOutputDeleteRequest : IRequest<ResourceOutputResponse<Data.Model.Response.Resource>>
    {
        public IHeaderDictionary? Headers { get; set; }
    }


}
