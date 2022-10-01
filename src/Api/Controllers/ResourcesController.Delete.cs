using System;
using System.Threading.Tasks;
using Api.Domain.Handling;
using Api.Domain.Handling.Delete;
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

            var unmodifiedSince = await _requestHeadersProvider.IfUnmodifiedSince(Request.Headers) ?? DateTimeOffset.MaxValue; // if none make unmodifiedever as default
            var etags = await _requestHeadersProvider.IfMatch(Request.Headers);

            var request = new ResourceStorageDeleteRequest() {
                Namespace = @namespace.ToLower(),
                Id = id,
                OwnerId = _ownerId,
                RequestId = _requestId,        
                UnmodifiedSince = unmodifiedSince,
                Etags = etags
            };

            var response = await _mediator.Send(request);

            ResourceOutputDeleteRequest outputRequest = new ResourceOutputDeleteRequest() { 
                Headers = Request.Headers
            };

            ResourceOutputResponse<Data.Model.Response.Resource> responseOutput = await _mediator.Send(outputRequest);


            _logger.LogTrace($"{nameof(ResourcesController)}DELETE. Processing response.");

            return _responseOutputHandler.Handle(this, responseOutput);            
        }
    }
}
