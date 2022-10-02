using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Api.Domain.Handling.Resource
{
    public class ResourceResponse<T> : ResourceResponse
    {
        public T? Model { get; set; }
    }
}
