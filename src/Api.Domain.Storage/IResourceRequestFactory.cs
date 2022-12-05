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
        ResourceStoragePostRequest CreateResourceStoragePostRequest(Guid ownerId, Guid requestId, string contentNamespace, string contentKeys, dynamic content);
        ResourceStorageGetOneRequest CreateResourceGetOneRequest(Guid ownerId, Guid requestId, Guid id, string contentNamespace, DateTimeOffset onlyIfModifiedSince, List<string> onlyIfNotEtags);
        ResourceStorageGetManyRequest CreateResourceStorageGetManyRequest(Guid ownerId, Guid requestId, string contentNamespace, DateTimeOffset onlyIfModifiedSince, List<string> onlyIfNotEtags);
        ResourceStoragePutRequest CreateResourceStoragePutRequest(Guid ownerId, Guid requestId, Guid id, string contentNamespace, string contentKkeys, dynamic content, string movetoNamespace, DateTimeOffset unnlessModifiedSince, List<string> unlessNotOfEtags);
        ResourceStorageDeleteRequest CreateResourceStorageDeleteRequest(Guid ownerId, Guid requestId, Guid id, string contentNamespace, DateTimeOffset unnlessModifiedSince, List<string> unlessNotOfEtags);
    }
}