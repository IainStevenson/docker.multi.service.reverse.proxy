using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;

namespace Handlers.Resource
{
    public class ResourcePutResponse
    {
        public HttpStatusCode StatusCode { get; internal set; }
        public Data.Model.Response.Resource Model { get; internal set; }
        public IHeaderDictionary Headers { get; internal set; }
        /// <summary>
        /// A collection of resource validation error messages
        /// </summary>
        public List<string> RequestValidationErrors { get; set; } = new List<string>();
        public IActionResult Handle(ControllerBase source)
        {
            throw new NotImplementedException();
        }
    }
}
