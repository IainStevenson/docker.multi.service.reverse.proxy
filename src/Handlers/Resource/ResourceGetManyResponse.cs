using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;

namespace Handlers.Resource
{
    public class ResourceGetManyResponse
    {
        /// <summary>
        /// Determines the resulting action response from the calling controller
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// The desired payload model
        /// </summary>
        public IEnumerable<Data.Model.Response.Resource> Model { get; set; }

        /// <summary>
        /// A collection of resource validation error messages
        /// </summary>
        public List<string> RequestValidationErrors { get; set; } = new List<string>();
        
        public IActionResult Handle(ControllerBase source)
        {
            switch (StatusCode)
            {
                case HttpStatusCode.NotModified:

                    return source.StatusCode((int)HttpStatusCode.NotModified);

                case HttpStatusCode.NotFound:

                    return source.NotFound();

                case HttpStatusCode.OK:

                    return source.Ok(Model);

                default:
                    RequestValidationErrors.Add($"Unhandled condition set in {nameof(ResourceGetManyHandler)} for {nameof(ResourceGetManyResponse)}");
                    return source.StatusCode(500, new RequestExceptionModel(RequestValidationErrors));
            }
        }
    }
}
