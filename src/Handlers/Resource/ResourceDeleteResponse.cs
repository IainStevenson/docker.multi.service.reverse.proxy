using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;

namespace Handlers.Resource
{
    public class ResourceDeleteResponse
    {

        public HttpStatusCode StatusCode { get; set; }
        public List<string> RequestValidationErrors { get; set; }

        public IActionResult Handle(ControllerBase source)
        {
            switch (StatusCode)
            {
                case HttpStatusCode.NotFound:
                    return source.NotFound();

                case HttpStatusCode.Gone:
                    return source.StatusCode((int) HttpStatusCode.Gone, new RequestExceptionModel(RequestValidationErrors));

                case HttpStatusCode.PreconditionFailed:
                    return source.StatusCode((int)HttpStatusCode.PreconditionFailed, new RequestExceptionModel(RequestValidationErrors));

                case HttpStatusCode.OK:
                    return source.Ok();

                default:
                    RequestValidationErrors.Add($"Unhandled condition set in {nameof(ResourceDeleteHandler)} for {nameof(ResourceDeleteResponse)}");
                    return source.StatusCode(500, new RequestExceptionModel(RequestValidationErrors));

            }
        }
    }
}