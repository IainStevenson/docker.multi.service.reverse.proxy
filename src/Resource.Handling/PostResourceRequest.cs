using MediatR;

namespace Resource.Handling
{

    public class PostResourceRequest : IRequest<PostResourceResponse>
    {
        public string Namespace { get; set; } = "my";
        public dynamic Content { get; set; } = new { };
        public string Keys { get; set; } = String.Empty;
        public Guid OwnerId { get; set; } = Guid.Empty;
        public Guid RequestId { get; set; } = Guid.Empty;
        public long Index { get; set; } = 0;
    }
}