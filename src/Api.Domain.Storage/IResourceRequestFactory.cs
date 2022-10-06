using Api.Domain.Storage.Delete;
using Api.Domain.Storage.Get;
using Api.Domain.Storage.Post;
using Api.Domain.Storage.Put;

namespace Api.Domain.Storage
{
    public interface IResourceRequestFactory
    {
        ResourceStorageGetOneRequest CreateResourceGetOneRequest(Guid id, string @namespace, Guid ownerId, Guid requestId, DateTimeOffset ifModifiedSince,  List<string> notEtags);
        ResourceStorageDeleteRequest CreateResourceStorageDeleteRequest(string @namespace, Guid id, Guid ownerId, Guid requestId, DateTimeOffset isUnchangedSince, List<string> isEtags);
        ResourceStorageGetManyRequest CreateResourceStorageGetManyRequest(string @namespace, Guid ownerId, Guid requestId, DateTimeOffset ifModifiedSince, List<string> notEtags);
        ResourceStoragePostRequest CreateResourceStoragePostRequest(string @namespace, dynamic content, string keys, Guid _ownerId, Guid _requestId);
        ResourceStoragePutRequest CreateResourceStoragePutRequest(Guid id, dynamic content, Guid ownerId, Guid requestId, string keys, string @namespace, string moveto, DateTimeOffset unmodifiedSince, List<string> etags);
    }
}