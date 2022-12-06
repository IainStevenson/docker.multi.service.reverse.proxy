using Api.Domain.Storage.Delete;
using Api.Domain.Storage.Get;
using Api.Domain.Storage.Post;
using Api.Domain.Storage.Put;

namespace Api.Domain.Storage
{
    public class ResourceRequestFactory : IResourceRequestFactory
    {
        ///  <inheritdoc/>
        public ResourceStorageGetOneRequest CreateResourceGetOneRequest(
            Guid ownerId, 
            Guid requestId, 
            Guid id, 
            string contentNamespace, 
            DateTimeOffset ifModifiedSince, 
            List<string> notEtags, 
            bool ifIsDeleted)
        {
            return new ResourceStorageGetOneRequest()
            {
                OwnerId = ownerId,
                RequestId = requestId,
                Id = id,
                ContentNamespace = contentNamespace ?? "".ToLower(),
                IfModifiedSince = ifModifiedSince,
                IfNotETags = notEtags,
                IfIsDeleted = ifIsDeleted
            };
        }

        public ResourceStorageDeleteRequest CreateResourceStorageDeleteRequest(
                Guid ownerId, 
                Guid requestId, 
                Guid id, 
                string contentNamespace, 
                DateTimeOffset isUnchangedSince, 
                List<string> isEtags)
        {
            return new ResourceStorageDeleteRequest()
            {
                Id = id,
                ContentNamespace = contentNamespace ?? "".ToLower(),
                OwnerId = ownerId,
                RequestId = requestId,
                IsUnchangedSince = isUnchangedSince,
                IsNotETags = isEtags
            };
        }

        ///  <inheritdoc/>
        public ResourceStorageGetManyRequest CreateResourceStorageGetManyRequest(
                Guid ownerId, 
                Guid requestId, 
                string contentNamespace, 
                DateTimeOffset ifModifiedSince, 
                List<string> notEtags, 
                bool ifIsDeleted)
        {
            return new ResourceStorageGetManyRequest()
            {
                ContentNamespace = contentNamespace ?? "".ToLower(),
                OwnerId = ownerId,
                RequestId = requestId,
                IfModifiedSince = ifModifiedSince,
                IfNotETags = notEtags,
                IfIsDelted = ifIsDeleted
            };
        }
        ///  <inheritdoc/>
        public ResourceStoragePostRequest CreateResourceStoragePostRequest(
            Guid ownerId,
            Guid requestId,
            string contentNamespace,
            string keys,
            dynamic content)
        {
            return new ResourceStoragePostRequest()
            {
                ContentNamespace = contentNamespace ?? "".ToLower(),
                Content = content,
                ContentKeys = keys,
                OwnerId = ownerId,
                RequestId = requestId
            };
        }

        ///  <inheritdoc/>
        public ResourceStoragePutRequest CreateResourceStoragePutRequest(
            Guid ownerId, 
            Guid requestId, 
            Guid id, 
            string contentNamespace, 
            string contentKeys, 
            dynamic content, 
            string movetoNamespace, 
            DateTimeOffset unnlessModifiedSince, 
            List<string> unlessNotOfEtags, 
            bool ifIsDeleted)
        {
            return new ResourceStoragePutRequest()
            {
                OwnerId = ownerId,
                RequestId = requestId,
                Id = id,
                ContentNamespace = contentNamespace ?? "".ToLower(),
                ContentKeys = contentKeys,
                Content = content,
                MoveTo = movetoNamespace,
                UnmodifiedSince= unnlessModifiedSince,
                ETags= unlessNotOfEtags,
                IfIsDeleted = ifIsDeleted
            };
        }
      
    }
}