using Microsoft.Extensions.Primitives;
using System.Diagnostics.CodeAnalysis;

namespace Api.Domain.Storage.Get
{
    [ExcludeFromCodeCoverage]
    public class ResourceStorageGetOneResponse: ResourceStorageResponseBase
    {
       
        /// <summary>
        /// The desired payload model
        /// </summary>
        public Data.Model.Storage.Resource? Model { get; set; }

        public IDictionary<string, StringValues> Headers { get; set; }  = new Dictionary<string, StringValues>();
    }

}
