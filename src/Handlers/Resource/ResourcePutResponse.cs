using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
            switch (StatusCode)
            {
                case HttpStatusCode.NotFound:
                    return source.NotFound();

                case HttpStatusCode.OK:
                    foreach(var item in Headers)
                    {
                        source.Response.Headers.Add(item.Key, item.Value);
                    }
                    return source.Ok(Model);

                case HttpStatusCode.PreconditionFailed:
                    return source.StatusCode((int)HttpStatusCode.PreconditionFailed, new RequestExceptionModel(RequestValidationErrors));

                default:
                    RequestValidationErrors.Add($"Unhandled condition set in {nameof(ResourcePutHandler)} for {nameof(ResourcePutResponse)}");
                    return source.StatusCode(500, new RequestExceptionModel(RequestValidationErrors));
            }
        }
    }
}
