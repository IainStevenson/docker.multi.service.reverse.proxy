using Microsoft.AspNetCore.Http;

namespace Api.Domain.Handling
{
    public class ResourceOutputResponse<T> : ResourceOutputResponse 
    {
        public T? Model { get; set; }
        public IHeaderDictionary Headers { get; internal set; }
    }
}
