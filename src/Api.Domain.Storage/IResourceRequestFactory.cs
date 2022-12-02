using Api.Domain.Storage.Delete;
using Api.Domain.Storage.Get;
using Api.Domain.Storage.Post;
using Api.Domain.Storage.Put;

namespace Api.Domain.Storage
{
    /// <summary>
    /// Factory for generation of resource storage CRUD request objects.
    /// </summary>
    public interface IResourceRequestFactory
    {
        ResourceStoragePostRequest CreateResourceStoragePostRequest(string clientContentNamespace, dynamic clientContent, string clientContentKeys, Guid ownerId, Guid requestId);
        ResourceStorageGetOneRequest CreateResourceGetOneRequest(Guid id, string clientContentNamespace, Guid ownerId, Guid requestId, DateTimeOffset onlyIfModifiedSince,  List<string> onlyIfNotEtags);
        ResourceStorageGetManyRequest CreateResourceStorageGetManyRequest(string clientContentNamespace, Guid ownerId, Guid requestId, DateTimeOffset onlyIfModifiedSince, List<string> onlyIfNotEtags);
        ResourceStoragePutRequest CreateResourceStoragePutRequest(Guid id, string clientContentNamespace, dynamic content, Guid ownerId, Guid requestId, string keys, string moveto, DateTimeOffset unnlessModifiedSince, List<string> unlessNotOfEtags);
        ResourceStorageDeleteRequest CreateResourceStorageDeleteRequest(Guid id, string clientContentNamespace, Guid ownerId, Guid requestId, DateTimeOffset unnlessModifiedSince, List<string> unlessNotOfEtags);
    }
}