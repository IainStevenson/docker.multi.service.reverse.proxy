using MediatR;

namespace Resource.Handling
{
    public class ResourceRequestFactory : IResourceRequestFactory
    {
        public PostResourceRequest CreatePostResourceRequest(string @namespace, dynamic content, string keys, Guid _ownerId, Guid _requestId) 
        {
            return new PostResourceRequest()
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