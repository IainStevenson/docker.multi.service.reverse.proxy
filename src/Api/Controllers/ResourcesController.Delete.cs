using System;
using System.Threading.Tasks;
using Handlers.Resource;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Api.Controllers
{
    public partial class ResourcesController 
    {
        /// <summary>
        /// DELETE: api/resources/{namespace}/{id}
        /// </summary>
        /// <remarks>
        /// Supports Headers: If-Unmodified-Since, If-Match
        /// </remarks>
        /// <param name="id">The identifier of the resource that is to be deleted.</param>
        /// <returns>
        /// 400 BadRequest
        /// 404 NotFound
        /// 412 PreConditionFailed
        /// 200 OK
        /// </returns>
        [HttpDelete("{namespace}/{id:guid}")]
        public async Task<IActionResult> Delete(
            [FromRoute] string @namespace,
            [FromRoute] Guid id)
        {
            _logger.LogTrace($"{nameof(ResourcesController)}:DELETE. Sending request.");

            var request = new ResourceDeleteRequest() {
                Namespace = @namespace.ToLower(),
                Id = id,
                OwnerId = _ownerId,
                RequestId = _requestId,
                Headers = Request.Headers               
            };

            var response = await _mediator.Send(request);

            _logger.LogTrace($"{nameof(ResourcesController)}DELETE. Processing response.");

            return response.Handle(this);            
        }
    }
}
