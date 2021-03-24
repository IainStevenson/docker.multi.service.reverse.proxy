using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;

namespace Handlers.Resource
{
    public class ResourceGetOneResponse
    {
        /// <summary>
        /// Determines the resulting action response from the calling controller
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// The desired payload model
        /// </summary>
        public Data.Model.Response.Resource Model { get; set; }

        /// <summary>
        /// A collection of resource validation error messages
        /// </summary>
        public List<string> RequestValidationErrors { get; set; } = new List<string>();
        public IHeaderDictionary Headers { get; internal set; }

        public IActionResult Handle(ControllerBase source)
        {
            switch(StatusCode)
            {
                case HttpStatusCode.NotModified:
                    foreach (var item in Headers)
                    {
                        source.Response.Headers.Add(item.Key, item.Value);
                    }
                    return source.StatusCode((int)HttpStatusCode.NotModified);

                case HttpStatusCode.NotFound:

                    return source.NotFound();

                case HttpStatusCode.OK:
                    foreach (var item in Headers)
                    {
                        source.Response.Headers.Add(item.Key, item.Value);
                    }
                    return source.Ok(Model);

                default:
                    RequestValidationErrors.Add($"Unhandled condition set in {nameof(ResourceGetOneHandler)} for {nameof(ResourceGetOneResponse)}");
                    return source.StatusCode(500, new RequestExceptionModel(RequestValidationErrors));
            }
        }
    }
}
