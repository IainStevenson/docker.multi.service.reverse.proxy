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
        /// DELETE: api/resources/{id}/{clientContentNamespace}
        /// </summary>
        /// <remarks>
        /// Supports Headers: If-Unmodified-Since, If-Match
        /// </remarks>
        /// <param name="id">The system identifier of the resource that is to be deleted.</param>
        /// <param name="clientContentNamespace">The client controlled namespace the resource should currently be in.</param>
        /// <returns>
        /// 400 BadRequest
        /// 404 NotFound
        /// 412 PreConditionFailed
        /// 200 OK
        /// </returns>
        [HttpDelete("{id:guid}/{*clientContentNamespace}")]
        public async Task<IActionResult> Delete(
            [FromRoute] Guid id,
            [FromRoute] string clientContentNamespace)
        {
            _logger.LogTrace($"{nameof(ResourcesController)}:{nameof(Delete)}. Procesing delete request.");

            var onlyIfHasRemainedUnchangedSince =  _requestHeadersProvider.IfIsUnchangedSince(Request.Headers, DateTimeOffset.MaxValue); 
            var onlyIfIsOneOfTheseEtags =  _requestHeadersProvider.IfHasEtagMatching(Request.Headers);

            ResourceStorageDeleteRequest resourceStorageDeleteRequest = _resourceRequestFactory.CreateResourceStorageDeleteRequest(
                                    id,
                                    clientContentNamespace,
                                    _ownerId,
                                    _requestId,
                                    onlyIfHasRemainedUnchangedSince,
                                    onlyIfIsOneOfTheseEtags);

            ResourceStorageDeleteResponse resourceStorageDeleteResponse = await _mediator.Send(resourceStorageDeleteRequest);

            _logger.LogTrace($"{nameof(ResourcesController)}{nameof(Delete)}. Processing delete response.");

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
