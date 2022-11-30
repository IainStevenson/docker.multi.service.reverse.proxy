using System.Diagnostics.CodeAnalysis;

namespace Api.Domain.Storage.Get
{
    [ExcludeFromCodeCoverage]
    public class ResourceStorageGetManyResponse : ResourceStorageResponseBase
    {
        /// <summary>
        /// The desired payload model
        /// </summary>
        public IEnumerable<Data.Model.Storage.Resource> Model { get; set; }       
    }
}
