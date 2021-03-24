using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Handlers.Resource;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Api.Controllers
{
    public partial class ResourcesController
    {
        /// <summary>
        /// GET: api/resources/{namespace}/{id:guid}
        /// </summary>
        /// <remarks>
        /// Supports Headers: If-Modified-Since (which is interpreted as New or changed since), If-None-Match
        /// </remarks>
        /// <param name="namespace">The storage namespace type of the resource.</param>
        /// <param name="id">The unique storage identifier of the resource.</param>
        /// <returns>
        /// Status code 404 Not Found if the resource does not exist in that namespace.
        /// Status code 200 and an instance of <see cref="Data.Model.Response.Resource"/> wrapping the <see cref="Data.Model.Storage.Resource"/> matching the resource identifier .
        /// Status Code 304 Unchanged if the resource was modified (via etag 'If-None-Match' check) or Modified Date 'If-Modified-Since' check
        /// </returns>
        [HttpGet]
        [Route("{namespace}/{id:guid}")]
        public async Task<IActionResult> Get(
            [Required][FromRoute] string @namespace,
            [Required][FromRoute] Guid id
            )
        {

            _logger.LogTrace($"{nameof(ResourcesController)}:GET (One). Sending request.");
            var request = new ResourceGetOneRequest()
            {
                Id = id,
                Namespace = @namespace.ToLower(),
                OwnerId = _ownerId,
                RequestId = _requestId,
                Headers = Request.Headers,
                Scheme = Request.Scheme,
                Host = Request.Host.Value,
                PathBase = Request.PathBase.Value,
                Path = Request.Path.Value

            };

            var response = await _mediator.Send(request);

            _logger.LogTrace($"{nameof(ResourcesController)}:GET (One). Processing rsponse.");

            return response.Handle(this);
        }


        /// <summary>
        /// GET: api/resources/{namespace}
        /// </summary>
        /// <remarks>
        /// Supports Headers: If-Modified-Since (which is interpreted as New or changed since), If-None-Match
        /// </remarks>
        /// <param name="namespace">The storage namespace type of the resource.</param>
        /// <param name="id">The unique storage identifier of the resource.</param>
        /// <returns>
        /// Status code 404 Not Found if the resource does not exist in that namespace.
        /// Status code 200 and an instance of <see cref="Data.Model.Response.Resource"/> wrapping the <see cref="Data.Model.Storage.Resource"/> matching the resource identifier .
        /// Status Code 304 Unchanged if the resource was modified (via etag 'If-None-Match' check) or Modified Date 'If-Modified-Since' check
        /// </returns>
        [HttpGet]
        [Route("{namespace}")]
        public async Task<IActionResult> Get(
            [Required][FromRoute] string @namespace
            )
        {

            _logger.LogTrace($"{nameof(ResourcesController)}:GET (Many). Sending request.");

            var request = new ResourceGetManyRequest()
            {
                Namespace = @namespace.ToLower(),
                Headers = Request.Headers,
                OwnerId = _ownerId,
                RequestId = _requestId,
                Scheme = Request.Scheme,
                Host = Request.Host.Value,
                PathBase = Request.PathBase.Value,
                Path = Request.Path.Value
            };

            var response = await _mediator.Send(request);

            _logger.LogTrace($"{nameof(ResourcesController)}:GET (Many). Processing rsponse.");

            return response.Handle(this);
        }
    }
}
