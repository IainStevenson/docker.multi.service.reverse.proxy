using System;
using System.Threading.Tasks;
using Api.Domain.Handling.Resource;
using Api.Domain.Handling.Resource.Delete;
using Api.Domain.Storage.Delete;
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
            _logger.LogTrace($"{nameof(ResourcesController)}:{nameof(Delete)}. Sending request.");

            var isUnchangedSince =  _requestHeadersProvider.IfIsUnchangedSince(Request.Headers, DateTimeOffset.MaxValue); 
            var etags =  _requestHeadersProvider.IfHasEtagMatching(Request.Headers);

            var request = new ResourceStorageDeleteRequest() {
                Namespace = @namespace.ToLower(),
                Id = id,
                OwnerId = _ownerId,
                RequestId = _requestId,        
                IsUnchangedSince = isUnchangedSince,
                Etags = etags
            };

            var response = await _mediator.Send(request);

            ResourceResponseDeleteRequest outputRequest = new ResourceResponseDeleteRequest() { 
                Headers = Request.Headers
            };

            ResourceResponse<Data.Model.Response.Resource> responseOutput = await _mediator.Send(outputRequest);


            _logger.LogTrace($"{nameof(ResourcesController)}DELETE. Processing response.");

            return _resourceResponseHandler.HandleNone(this, responseOutput);            
        }
    }
}
