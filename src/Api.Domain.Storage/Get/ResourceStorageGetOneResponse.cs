using Microsoft.Extensions.Primitives;

namespace Api.Domain.Storage.Get
{
    public class ResourceStorageGetOneResponse
    {
        /// <summary>
        /// Determines the handler action status
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// The desired payload model
        /// </summary>
        public Data.Model.Storage.Resource Model { get; set; }

        /// <summary>
        /// A collection of resource validation error messages
        /// </summary>
        public List<string> RequestValidationErrors { get; set; } = new List<string>();

        public IDictionary<string, StringValues> Headers { get; set; }
    }

}
