using Microsoft.Extensions.Primitives;

namespace Api.Domain.Storage.Get
{
    public class ResourceStorageGetOneResponse: ResourceStorageResponseBase
    {
       
        /// <summary>
        /// The desired payload model
        /// </summary>
        public Data.Model.Storage.Resource Model { get; set; }

        public IDictionary<string, StringValues> Headers { get; set; }
    }

}
