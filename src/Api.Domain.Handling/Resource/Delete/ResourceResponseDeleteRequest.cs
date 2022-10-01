using MediatR;
using Microsoft.AspNetCore.Http;

namespace Api.Domain.Handling.Resource.Delete
{
    public class ResourceResponseDeleteRequest : IRequest<ResourceResponse<Data.Model.Response.Resource>>
    {
        public IHeaderDictionary? Headers { get; set; }
    }


}
