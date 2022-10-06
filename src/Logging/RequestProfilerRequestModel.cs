using Microsoft.AspNetCore.Http;

namespace Logging
{
    public class RequestProfilerRequestModel
    {
        public string Scheme { get;  set; }
        public HostString Host { get;  set; }
        public PathString Path { get;  set; }
        public QueryString QueryString { get;  set; }
        public IHeaderDictionary Headers { get;  set; }
        public string Body { get;  set; }
        public PathString PathBase { get;  set; }
        public string Method { get;  set; }
    }
}
