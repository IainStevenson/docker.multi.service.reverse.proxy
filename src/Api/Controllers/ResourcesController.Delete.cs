using System;
using System.Net;
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
            _logger.LogTrace($"{nameof(ResourcesController)}:{nameof(Delete)}. Procesing delete request.");

            var isUnchangedSince =  _requestHeadersProvider.IfIsUnchangedSince(Request.Headers, DateTimeOffset.MinValue); 
            var isEtags =  _requestHeadersProvider.IfHasEtagMatching(Request.Headers);

            ResourceStorageDeleteRequest resourceStorageDeleteRequest = _resourceRequestFactory.CreateResourceStorageDeleteRequest(@namespace,
                                                                                                            id,
                                                                                                            _ownerId,
                                                                                                            _requestId,
                                                                                                            isUnchangedSince,
                                                                                                            isEtags);

            var resourceStorageDeleteResponse = await _mediator.Send(resourceStorageDeleteRequest);

            _logger.LogTrace($"{nameof(ResourcesController)}{nameof(Delete)}. Processing storage response.");

            ResourceResponseDeleteRequest outputRequest = _resourceResponseFactory.CreateResourceResponseDeleteRequest(
                                                                                        (HttpStatusCode)resourceStorageDeleteResponse.StatusCode,
                                                                                        resourceStorageDeleteResponse.RequestValidationErrors
                    
                                                                                    );

            _logger.LogTrace($"{nameof(ResourcesController)}{nameof(Delete)}. Processing response.");
            ResourceResponse responseOutput = await _mediator.Send(outputRequest);


            _logger.LogTrace($"{nameof(ResourcesController)}{nameof(Delete)}. Handling response.");

            return _resourceResponseHandler.HandleNone(this, responseOutput);            
        }
    }
}
