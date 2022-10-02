using Api.Domain.Handling.Framework;
using Data.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.Net;

namespace Api.Domain.Handling.Resource
{
    public class ResourceResponseHandler : IResourceResponseHandler
    {
        private readonly IResponseHeadersProvider _responseHeadersProvider;
        public ResourceResponseHandler(IResponseHeadersProvider responseHeadersProvider)
        {
            _responseHeadersProvider = responseHeadersProvider;
        }
        /// <summary>
        /// Handles the resource output via the controller a response with single item no content.
        /// </summary>
        /// <typeparam name="T">The model type of the output.</typeparam>
        /// <param name="controller">The calling controller.</param>
        /// <param name="resourceOutput">The output instance.</param>
        /// <returns>An <see cref="IActionResult"/> delegate.</returns>
        /// <exception cref="ArgumentOutOfRangeException"> Occurs when upstream processing presents Status codes not handled here.</exception>
        public IActionResult HandleOne<T>(ControllerBase controller, ResourceResponse<T> resourceOutput) where T : IEntity
        {

            _responseHeadersProvider.RemoveUnwantedHeaders(controller.HttpContext.Response.Headers);

            foreach (var h in resourceOutput.Headers ?? (IDictionary<string, StringValues>)(new Dictionary<string, StringValues>()))
            {
                if (!controller.HttpContext.Response.Headers.ContainsKey(h.Key))
                {
                    controller.HttpContext.Response.Headers.Add(h.Key, h.Value);
                }
                else
                {
                    controller.HttpContext.Response.Headers[h.Key] = h.Value;
                }
            }


            switch (resourceOutput.StatusCode)
            {
                case HttpStatusCode.BadRequest:
                    return controller.BadRequest();

                case HttpStatusCode.OK:
                    return controller.Ok(resourceOutput.Model);

                case HttpStatusCode.Created:
                    var uri = resourceOutput.Links?.Single(x => x.Action == "get" && x.Rel == "self")?.Href ?? "\\";
                    return controller.Created(uri, resourceOutput.Model);

                case HttpStatusCode.NoContent:
                    return controller.NoContent();

                case HttpStatusCode.NotFound:
                    return controller.NotFound();

                case HttpStatusCode.Gone:
                    return controller.StatusCode((int)resourceOutput.StatusCode, resourceOutput.RequestValidationErrors);

                case HttpStatusCode.NotModified:
                case HttpStatusCode.Conflict:
                case HttpStatusCode.PreconditionFailed:
                    return controller.StatusCode((int)resourceOutput.StatusCode);

                default:
                    throw new ArgumentOutOfRangeException(nameof(resourceOutput.StatusCode), $"The status code {resourceOutput.StatusCode} of the request output was not handled. This is a programming logic error");
            }
        }

        ///// <summary>
        ///// Handles the resource output via the controller.
        ///// </summary>
        ///// <param name="controller">The calling controller.</param>
        ///// <param name="resourceOutput">The output instance.</param>
        ///// <returns>An <see cref="IActionResult"/> delegate.</returns>
        ///// <exception cref="ArgumentOutOfRangeException"> Occurs when upstream processing presents Status codes not handled here.</exception>
        //public IActionResult Handle<T>(ControllerBase controller, ResourceResponse resourceOutput) where T : IEntity
        //{
        //    throw new NotImplementedException();
        //}

        public IActionResult HandleMany<T>(ControllerBase controller, ResourceResponse<T> resourceOutput) where T : IEnumerable<IEntity>
        {
            _responseHeadersProvider.RemoveUnwantedHeaders(controller.HttpContext.Response.Headers);

            foreach (var h in resourceOutput.Headers ?? (IDictionary<string, StringValues>)(new Dictionary<string, StringValues>()))
            {
                if (!controller.HttpContext.Response.Headers.ContainsKey(h.Key))
                {
                    controller.HttpContext.Response.Headers.Add(h.Key, h.Value);
                }
                else
                {
                    controller.HttpContext.Response.Headers[h.Key] = h.Value;
                }
            }


            switch (resourceOutput.StatusCode)
            {
                case HttpStatusCode.BadRequest:
                    return controller.BadRequest();

                case HttpStatusCode.OK:
                    return controller.Ok(resourceOutput.Model);

               
                case HttpStatusCode.NoContent:
                    return controller.NoContent();
                
                case HttpStatusCode.NotFound:
                    return controller.NotFound();
                
                case HttpStatusCode.Gone:
                    return controller.StatusCode((int)resourceOutput.StatusCode, resourceOutput.RequestValidationErrors);

                case HttpStatusCode.NotModified:
                case HttpStatusCode.Conflict:
                case HttpStatusCode.PreconditionFailed:
                    return controller.StatusCode((int)resourceOutput.StatusCode);
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(resourceOutput.StatusCode), $"The status code {resourceOutput.StatusCode} of the request output was not handled. This is a programming logic error");
            }
        }
        /// <summary>
        /// Handles the resource output via the controller for a response with no content.
        /// </summary>
        /// <param name="controller">The calling controller.</param>
        /// <param name="resourceOutput">The output instance.</param>
        /// <returns>An <see cref="IActionResult"/> delegate.</returns>
        /// <exception cref="ArgumentOutOfRangeException"> Occurs when upstream processing presents Status codes not handled here.</exception>
        /// <summary>
        public IActionResult HandleNone(ControllerBase controller, ResourceResponse resourceOutput)
        {
            _responseHeadersProvider.RemoveUnwantedHeaders(controller.HttpContext.Response.Headers);

            foreach (var h in resourceOutput.Headers?? (IDictionary<string , StringValues>)(new Dictionary<string, StringValues>()))
            {
                if (!controller.HttpContext.Response.Headers.ContainsKey(h.Key))
                {
                    controller.HttpContext.Response.Headers.Add(h.Key, h.Value);
                }
                else
                {
                    controller.HttpContext.Response.Headers[h.Key] = h.Value;
                }
            }


            switch (resourceOutput.StatusCode)
            {
                case HttpStatusCode.BadRequest:
                    return controller.BadRequest();

                case HttpStatusCode.OK:
                    return controller.Ok();

                case HttpStatusCode.NoContent:
                    return controller.NoContent();

                case HttpStatusCode.NotFound:
                    return controller.NotFound();

                case HttpStatusCode.Gone:
                    return controller.StatusCode((int)resourceOutput.StatusCode, resourceOutput.RequestValidationErrors);

                case HttpStatusCode.NotModified:
                case HttpStatusCode.Conflict:
                case HttpStatusCode.PreconditionFailed:
                    return controller.StatusCode((int)resourceOutput.StatusCode);

                default:
                    throw new ArgumentOutOfRangeException(nameof(resourceOutput.StatusCode), $"The status code {resourceOutput.StatusCode} of the request output was not handled. This is a programming logic error");
            }

        }
    }
}