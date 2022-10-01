using Microsoft.AspNetCore.Http;

namespace Api.Domain.Handling.Resource
{
    public class ResourceResponse<T> : ResourceResponse
    {
        public T? Model { get; set; }
        public IHeaderDictionary Headers { get; internal set; }
    }
}
