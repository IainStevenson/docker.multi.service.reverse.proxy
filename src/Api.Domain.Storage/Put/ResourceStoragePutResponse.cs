using System.Diagnostics.CodeAnalysis;

namespace Api.Domain.Storage.Put
{
    [ExcludeFromCodeCoverage]
    public class ResourceStoragePutResponse : ResourceStorageResponseBase
    {
        public Data.Model.Storage.Resource Model { get; set; }       
    }
}
