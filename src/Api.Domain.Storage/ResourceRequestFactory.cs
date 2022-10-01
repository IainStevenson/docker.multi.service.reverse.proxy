using Api.Domain.Storage.Post;

namespace Api.Domain.Storage
{
    public class ResourceRequestFactory : IResourceRequestFactory
    {
        public ResourceStoragePostRequest CreateResourceStoragePostRequest(string @namespace, dynamic content, string keys, Guid _ownerId, Guid _requestId)
        {
            return new ResourceStoragePostRequest()
            {
                Namespace = @namespace.ToLower(),
                Content = content,
                Keys = keys,
                OwnerId = _ownerId,
                RequestId = _requestId
            };
        }
    }
}