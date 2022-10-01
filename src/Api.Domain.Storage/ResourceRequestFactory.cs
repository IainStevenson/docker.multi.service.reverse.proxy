using Api.Domain.Storage.Get;
using Api.Domain.Storage.Post;
using Api.Domain.Storage.Put;

namespace Api.Domain.Storage
{
    public class ResourceRequestFactory : IResourceRequestFactory
    {
        ///  <inheritdoc/>
        public ResourceStorageGetOneRequest CreateResourceGetOneRequest(Guid id, string @namespace, Guid ownerId, Guid requestId, DateTimeOffset? ifModifiedSince, List<string> etags)
        {
            return new ResourceStorageGetOneRequest() { 
                Id = id, 
                Namespace = @namespace.ToLower(),
                OwnerId = ownerId,
                RequestId = requestId,
                IfModifiedSince = ifModifiedSince, 
                ETags = etags
            };
        }

        ///  <inheritdoc/>
        public ResourceStorageGetManyRequest CreateResourceStorageGetManyRequest(string @namespace, Guid ownerId, Guid requestId, DateTimeOffset? ifModifiedSince, List<string> etags)
        {
            return new ResourceStorageGetManyRequest()
            {
                Namespace = @namespace.ToLower(),
                OwnerId = ownerId,
                RequestId = requestId,
                IfModifiedSince = ifModifiedSince,
                ETags = etags
            };
        }
        ///  <inheritdoc/>
        public ResourceStoragePostRequest CreateResourceStoragePostRequest(string @namespace, dynamic content, string keys, Guid ownerId, Guid requestId)
        {
            return new ResourceStoragePostRequest()
            {
                Namespace = @namespace.ToLower(),
                Content = content,
                Keys = keys,
                OwnerId = ownerId,
                RequestId = requestId
            };
        }
        ///  <inheritdoc/>
        public ResourceStoragePutRequest CreateResourceStoragePutRequest(Guid id, dynamic content, Guid ownerId, Guid requestId, string keys, string @namespace, string moveto, Task<DateTimeOffset?> unmodifiedSince, Task<List<string>> etags)
        {
            return  new ResourceStoragePutRequest()
            {
                Id = id,
                Namespace = @namespace.ToLower(),
                Content = content,
                Keys = keys,
                OwnerId = ownerId,
                RequestId = requestId
            };
        }

        public ResourceStoragePutRequest CreateResourceStoragePutRequest(Guid id, dynamic content, Guid ownerId, Guid requestId, string keys, string @namespace, string moveto, DateTimeOffset unmodifiedSince, List<string> etags)
        {
            return new ResourceStoragePutRequest()
            {
                Id = id,
                Namespace = @namespace.ToLower(),
                Content = content,
                Keys = keys,
                OwnerId = ownerId,
                RequestId = requestId,
                MoveTo = moveto,
                UnmodifiedSince = unmodifiedSince,
                ETags = etags
            };
        }
    }
}