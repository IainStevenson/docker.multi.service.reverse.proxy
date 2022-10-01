using Api.Domain.Handling.Framework;
using Data.Model;
using Microsoft.AspNetCore.Mvc;
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
        /// Handles the resource output via the controller.
        /// </summary>
        /// <typeparam name="T">The model type of the output.</typeparam>
        /// <param name="controller">The calling controller.</param>
        /// <param name="resourceOutput">The output instance.</param>
        /// <returns>An <see cref="IActionResult"/> delegate.</returns>
        /// <exception cref="ArgumentOutOfRangeException"> Occurs when upstream processing presents Status codes not handled here.</exception>
        public IActionResult Handle<T>(ControllerBase controller, ResourceResponse<T> resourceOutput) where T : IEntity
        {

            _responseHeadersProvider.RemoveUnwantedHeaders(controller.HttpContext.Response.Headers);

            foreach (var h in resourceOutput.Headers)
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

                    var uri = resourceOutput.Links.Single(x => x.Action == "get" && x.Rel == "self")?.Href ?? "\\";
                    return controller.Created(uri, resourceOutput.Model);

                //case HttpStatusCode.Accepted:
                //    break;
                //case HttpStatusCode.NonAuthoritativeInformation:
                //    break;
                //case HttpStatusCode.NoContent:
                //    break;
                //case HttpStatusCode.ResetContent:
                //    break;
                //case HttpStatusCode.PartialContent:
                //    break;
                //case HttpStatusCode.MultiStatus:
                //    break;
                //case HttpStatusCode.AlreadyReported:
                //    break;
                //case HttpStatusCode.IMUsed:
                //    break;
                //case HttpStatusCode.Ambiguous:
                //    break;
                //case HttpStatusCode.Moved:
                //    break;
                //case HttpStatusCode.Found:
                //    break;
                //case HttpStatusCode.RedirectMethod:
                //    break;
                //case HttpStatusCode.NotModified:
                //    break;
                //case HttpStatusCode.UseProxy:
                //    break;
                //case HttpStatusCode.Unused:
                //    break;
                //case HttpStatusCode.RedirectKeepVerb:
                //    break;
                //case HttpStatusCode.PermanentRedirect:
                //    break;
                //    break;
                //case HttpStatusCode.Unauthorized:
                //    break;
                //case HttpStatusCode.PaymentRequired:
                //    break;
                //case HttpStatusCode.Forbidden:
                //    break;
                //case HttpStatusCode.NotFound:
                //    break;
                //case HttpStatusCode.MethodNotAllowed:
                //    break;
                //case HttpStatusCode.NotAcceptable:
                //    break;
                //case HttpStatusCode.ProxyAuthenticationRequired:
                //    break;
                //case HttpStatusCode.RequestTimeout:
                //    break;
                //case HttpStatusCode.Conflict:
                //    break;
                //case HttpStatusCode.Gone:
                //    break;
                //case HttpStatusCode.LengthRequired:
                //    break;
                //case HttpStatusCode.PreconditionFailed:
                //    break;
                //case HttpStatusCode.RequestEntityTooLarge:
                //    break;
                //case HttpStatusCode.RequestUriTooLong:
                //    break;
                //case HttpStatusCode.UnsupportedMediaType:
                //    break;
                //case HttpStatusCode.RequestedRangeNotSatisfiable:
                //    break;
                //case HttpStatusCode.ExpectationFailed:
                //    break;
                //case HttpStatusCode.MisdirectedRequest:
                //    break;
                //case HttpStatusCode.UnprocessableEntity:
                //    break;
                //case HttpStatusCode.Locked:
                //    break;
                //case HttpStatusCode.FailedDependency:
                //    break;
                //case HttpStatusCode.UpgradeRequired:
                //    break;
                //case HttpStatusCode.PreconditionRequired:
                //    break;
                //case HttpStatusCode.TooManyRequests:
                //    break;
                //case HttpStatusCode.RequestHeaderFieldsTooLarge:
                //    break;
                //case HttpStatusCode.UnavailableForLegalReasons:
                //    break;
                //case HttpStatusCode.InternalServerError:
                //    break;
                //case HttpStatusCode.NotImplemented:
                //    break;
                //case HttpStatusCode.BadGateway:
                //    break;
                //case HttpStatusCode.ServiceUnavailable:
                //    break;
                //case HttpStatusCode.GatewayTimeout:
                //    break;
                //case HttpStatusCode.HttpVersionNotSupported:
                //    break;
                //case HttpStatusCode.VariantAlsoNegotiates:
                //    break;
                //case HttpStatusCode.InsufficientStorage:
                //    break;
                //case HttpStatusCode.LoopDetected:
                //    break;
                //case HttpStatusCode.NotExtended:
                //    break;
                //case HttpStatusCode.NetworkAuthenticationRequired:
                //case HttpStatusCode.Continue:
                //    break;
                //case HttpStatusCode.SwitchingProtocols:
                //    break;
                //case HttpStatusCode.Processing:
                //    break;
                //case HttpStatusCode.EarlyHints:
                //    break;

                //    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(resourceOutput.StatusCode), $"The status code {resourceOutput.StatusCode} of the request output was not handled. This is a programming logic error");
            }
        }

        /// <summary>
        /// Handles the resource output via the controller.
        /// </summary>
        /// <param name="controller">The calling controller.</param>
        /// <param name="resourceOutput">The output instance.</param>
        /// <returns>An <see cref="IActionResult"/> delegate.</returns>
        /// <exception cref="ArgumentOutOfRangeException"> Occurs when upstream processing presents Status codes not handled here.</exception>
        public IActionResult Handle<T>(ControllerBase controller, ResourceResponse resourceOutput) where T : IEntity
        {
            throw new NotImplementedException();
        }
    }
}