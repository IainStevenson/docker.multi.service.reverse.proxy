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
        /// Action verb PUT: api/Resource/{id:guid}[/{namespace}]
        /// </summary>
        /// <remarks>
        /// Updates an existing resource, and or, optionally moves its namespace. 
        /// Supports Headers: If-Unmodified-Since, If-Match (ETag) as pre-conditions
        /// </remarks>
        /// <param name="id">
        /// The resource identifier. Identifies the record that must match the if-* qualifiers.
        /// </param>
        /// <param name="namespace">
        /// The resource storage namespace, if provided this overwrites the existing namespace for the resource.
        /// </param>
        /// <param name="content">
        /// The supplied resource Content value. 
        /// If this is null and the namespace is provided then a 'move of namespace' occurs without changing the content.
        /// If both are null then a BadRequest is returned.
        /// </param>
        /// <returns>
        /// 404 NotFound - Gone - no exist at all
        /// 412 PreConditionFailed - does not satisfy if-* header qualifiers
        /// 400 BadRequest - Malformed - incomplete - invalid request
        /// 200 OK - Success - returns changed object
        /// </returns>
        [HttpPut()]
        [Route("{id:guid}/{*namespace}")]
        public async Task<IActionResult> Put(
            [Required][FromRoute] Guid id,
            [FromRoute] string @namespace,
            [FromBody] dynamic content,
            [FromQuery] string keys)
        {
            _logger.LogTrace($"{nameof(ResourcesController)}:PUT. Sending request.");

            var request = new ResourcePutRequest()
            {
                Id = id,
                Namespace = @namespace.ToLower(),
                Model = content,

                Keys = keys,
                
                Headers = Request.Headers,
                Query = Request.Query,
                Scheme = Request.Scheme,
                Host = Request.Host.Value,
                Path = Request.Path.Value,
                
                Owner = _ownerId,
                RequestId = _requestId
            };

            var response = await _mediator.Send(request);

            _logger.LogTrace($"{nameof(ResourcesController)}:PUT. Processing response.");

            return response.Handle(this);
        }       
    }
}