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
        public string SessionId { get; internal set; }
        public string TraceIdentifier { get; internal set; }
        public Exception Exception { get; internal set; }
    }
}
