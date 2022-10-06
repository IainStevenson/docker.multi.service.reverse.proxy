using System;

namespace Logging
{
    public class RequestProfilerModel
    {
        public string Source { get; set; }
        public DateTimeOffset TimeOfRequest { get; set; }
        public RequestProfilerRequestModel Request { get; set; }
        public RequestProfilerResponseModel Response { get; set; }
        public DateTimeOffset TimeOfResponse { get; set; }
        public string SessionId { get;  set; }
        public string TraceIdentifier { get;  set; }
        public Exception Exception { get;  set; }
        public object Routes {  get;set; }
       
    }
}
