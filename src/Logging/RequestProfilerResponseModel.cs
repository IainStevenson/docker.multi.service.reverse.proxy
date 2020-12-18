﻿using Microsoft.AspNetCore.Http;

namespace Logging
{
    public class RequestProfilerResponseModel
    {
        public int StatusCode { get; internal set; }
        public HeaderDictionary Headers { get; internal set; }
        public string Body { get; internal set; }
    }
}
