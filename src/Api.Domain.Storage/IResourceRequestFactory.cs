using Api.Domain.Storage.Post;

namespace Api.Domain.Storage
{
    public interface IResourceRequestFactory
    {
        ResourceStoragePostRequest CreateResourceStoragePostRequest(string @namespace, dynamic content, string keys, Guid _ownerId, Guid _requestId);
    }
}