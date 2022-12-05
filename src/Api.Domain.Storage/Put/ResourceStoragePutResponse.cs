using System.Diagnostics.CodeAnalysis;

namespace Api.Domain.Storage.Put
{
    [ExcludeFromCodeCoverage]
    public class ResourceStoragePutResponse : ResourceStorageResponseBase
    {
        public Data.Model.Storage.Resource Model { get; set; }
        public string ContentNamespace { get; set; } = string.Empty;
        public string ContentKeys { get; set; } = string.Empty;
    }
}
