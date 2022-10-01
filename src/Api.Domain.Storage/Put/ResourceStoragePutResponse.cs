using Microsoft.Extensions.Primitives;

namespace Api.Domain.Storage.Put
{
    public class ResourceStoragePutResponse
    {
        public int StatusCode { get; internal set; }
        public Data.Model.Storage.Resource Model { get; internal set; }
        public IDictionary<string,  StringValues> Headers { get; internal set; }
        /// <summary>
        /// A collection of resource validation error messages
        /// </summary>
        public List<string> RequestValidationErrors { get; set; } = new List<string>();
    }

}
