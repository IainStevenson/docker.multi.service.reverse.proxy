using Api.Domain.Storage.Delete;
using Api.Domain.Storage.Get;
using Api.Domain.Storage.Post;
using Api.Domain.Storage.Put;

namespace Api.Domain.Storage
{
    public class ResourceRequestFactory : IResourceRequestFactory
    {
        ///  <inheritdoc/>
        public ResourceStorageGetOneRequest CreateResourceGetOneRequest(Guid id, string @namespace, Guid ownerId, Guid requestId, DateTimeOffset ifModifiedSince, List<string> notEtags)
        {
            return new ResourceStorageGetOneRequest()
            {
                Id = id,
                ContentNamespace = @namespace ?? "".ToLower(),
                OwnerId = ownerId,
                RequestId = requestId,
                IfModifiedSince = ifModifiedSince,
                IfNotETags = notEtags
            };
        }

        public ResourceStorageDeleteRequest CreateResourceStorageDeleteRequest(Guid id, string @namespace, Guid ownerId, Guid requestId, DateTimeOffset isUnchangedSince, List<string> isEtags)
        {
            return new ResourceStorageDeleteRequest()
            {
                Id = id,
                ContentNamespace = @namespace ?? "".ToLower(),
                OwnerId = ownerId,
                RequestId = requestId,
                IsUnchangedSince = isUnchangedSince,
                IsNotETags = isEtags
            };
        }

        ///  <inheritdoc/>
        public ResourceStorageGetManyRequest CreateResourceStorageGetManyRequest(string @namespace, Guid ownerId, Guid requestId, DateTimeOffset ifModifiedSince, List<string> notEtags)
        {
            return new ResourceStorageGetManyRequest()
            {
                ContentNamespace = @namespace ?? "".ToLower(),
                OwnerId = ownerId,
                RequestId = requestId,
                IfModifiedSince = ifModifiedSince,
                IfNotETags = notEtags
            };
        }
        ///  <inheritdoc/>
        public ResourceStoragePostRequest CreateResourceStoragePostRequest(
            string clientContentNamespace, 
            dynamic content, 
            string keys, 
            Guid ownerId, 
            Guid requestId)
        {
            return new ResourceStoragePostRequest()
            {
                ContentNamespace = clientContentNamespace ?? "".ToLower(),
                Content = content,
                ContentKeys = keys,
                OwnerId = ownerId,
                RequestId = requestId
            };
        }
        ///  <inheritdoc/>
        public ResourceStoragePutRequest CreateResourceStoragePutRequest(Guid id, dynamic content, Guid ownerId, Guid requestId, string keys, string @namespace, string moveto, Task<DateTimeOffset?> unmodifiedSince, Task<List<string>> etags)
        {
            return new ResourceStoragePutRequest()
            {
                Id = id,
                ContentNamespace = @namespace ?? "".ToLower(),
                Content = content,
                Keys = keys,
                OwnerId = ownerId,
                RequestId = requestId
            };
        }

        public ResourceStoragePutRequest CreateResourceStoragePutRequest(Guid id, string @namespace, dynamic content, Guid ownerId, Guid requestId, string keys, string moveto, DateTimeOffset unmodifiedSince, List<string> etags)
        {
            return new ResourceStoragePutRequest()
            {
                Id = id,
                ContentNamespace = @namespace ?? "".ToLower(),
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