using Microsoft.AspNetCore.Http;

namespace Logging
{
    public class RequestProfilerResponseModel
    {
        public int StatusCode { get;  set; }
        public IHeaderDictionary Headers { get;  set; } 
        public string Body { get;  set; }
    }
}
