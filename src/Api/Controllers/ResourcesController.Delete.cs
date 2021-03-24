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
        /// DELETE: api/resources/{id}
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
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            _logger.LogTrace($"{nameof(ResourcesController)}:DELETE. Sending request.");

            var request = new ResourceDeleteRequest() {
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
