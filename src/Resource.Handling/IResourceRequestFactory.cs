namespace Resource.Handling
{
    public interface IResourceRequestFactory
    {
        PostResourceRequest CreatePostResourceRequest(string @namespace, dynamic content, string keys, Guid _ownerId, Guid _requestId);//, string scheme, string value, string value1, string value2);
    }
}