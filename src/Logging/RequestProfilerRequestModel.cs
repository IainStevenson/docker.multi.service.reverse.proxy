using Microsoft.AspNetCore.Http;

namespace Logging
{
    public class RequestProfilerRequestModel
    {
        public string Scheme { get; internal set; }
        public HostString Host { get; internal set; }
        public PathString Path { get; internal set; }
        public QueryString QueryString { get; internal set; }
        public IHeaderDictionary Headers { get; internal set; }
        public string Body { get; internal set; }
        public PathString PathBase { get; internal set; }
        public string Method { get; internal set; }
    }
}
