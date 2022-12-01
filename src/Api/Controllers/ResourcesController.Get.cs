using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using Api.Domain.Handling.Resource;
using Api.Domain.Storage.Get;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Api.Domain.Handling.Resource.Get;
using System.Collections.Generic;

namespace Api.Controllers
{
    public partial class ResourcesController
    {
        /// <summary>
        /// GET: api/resources/{id:guid}[/{namespace}]
        /// </summary>
        /// <remarks>
        /// Supports Headers: 
        ///     If-Modified-Since (which is interpreted as is New or changed since), 
        ///     If-None-Match as in has been changed from the provided etag(s)
        /// Both or either of the above may be true and therefore return the item. Otherwise a 404 NotFound will be returned.
        /// </remarks>
        /// <param name="namespace">The client controlled storage namespace type of the resource.</param>
        /// <param name="id">The server controlled unique storage identifier of the resource.</param>
        /// <returns>
        /// Status code 
        ///     404 Not Found if the resource does not exist at all or exist in that namespace.
        ///     200 and an instance of <see cref="Data.Model.Response.Resource"/> wrapping the <see cref="Data.Model.Storage.Resource"/> matching the resource identifier .
        ///     304 Unchanged if the resource was modified (via etag 'If-None-Match' check) or Modified Date 'If-Modified-Since' check
        /// </returns>
        [HttpGet]
        [Route("{id:guid}/{*namespace}")]
        public async Task<IActionResult> GetOne(
            [Required][FromRoute] Guid id,
            [FromRoute] string @namespace
            )
        {

            _logger.LogTrace($"{nameof(ResourcesController)}:{nameof(GetOne)}. Processing request.");

            var ifModifiedSince = _requestHeadersProvider.IfHasChangedSince(Request.Headers, DateTimeOffset.MinValue);
            
            var noneEtags = _requestHeadersProvider.IfDoesNotHaveEtagMatching(Request.Headers);

            ResourceStorageGetOneRequest resourceGetOneRequest = _resourceRequestFactory.CreateResourceGetOneRequest(id,
                                                                                            @namespace,
                                                                                            _ownerId,
                                                                                            _requestId,
                                                                                            ifModifiedSince,
                                                                                            noneEtags);

            var resourceStorageGetOneResponse = await _mediator.Send(resourceGetOneRequest);

            ResourceResponseGetOneRequest resourceResponseGetOneRequest = _resourceResponseFactory.CreateResourceResponseGetOneRequest(
                                                                                            resourceStorageGetOneResponse.Model,
                                                                                           (HttpStatusCode)resourceStorageGetOneResponse.StatusCode
                                                                                       );

            ResourceResponse<Data.Model.Response.Resource> resourceResponse = await _mediator.Send(resourceResponseGetOneRequest);

            _logger.LogTrace($"{nameof(ResourcesController)}:{nameof(GetOne)}. Processing response.");

            return _resourceResponseHandler.HandleOne(this, resourceResponse);
        }


        /// <summary>
        /// GET: api/resources/{namespace}
        /// </summary>
        /// <remarks>
        /// Supports Headers: 
        ///     If-Modified-Since (which is interpreted as is New or changed since), 
        ///     If-None-Match as in has been changed from the provided etag(s)
        /// Both or either of the above may be true and therefore return the items. Otherwise a 404 NotFound will be returned.
        /// </remarks>
        /// <param name="namespace">The client controlled storage namespace type of the resource.</param>
        /// <param name="id">The server controlled unique storage identifier of the resource.</param>
        /// <returns>
        /// Status code:
        ///     404 Not Found if the resource does not exist in that namespace.
        ///     200 and an instance of <see cref="Data.Model.Response.Resource"/> wrapping the <see cref="Data.Model.Storage.Resource"/> matching the resource identifier .
        ///     304 Unchanged if the resource was modified (via etag 'If-None-Match' check) or Modified Date 'If-Modified-Since' check
        /// </returns>
        [HttpGet]
        [Route("{*namespace}")]
        public async Task<IActionResult> GetMany(
            [Required][FromRoute] string @namespace
            )
        {

            _logger.LogTrace($"{nameof(ResourcesController)}:{nameof(GetMany)}. Processing request.");

            var ifModifiedSince = _requestHeadersProvider.IfHasChangedSince(Request.Headers, DateTimeOffset.MinValue);
            var notEtags = _requestHeadersProvider.IfDoesNotHaveEtagMatching(Request.Headers);

            ResourceStorageGetManyRequest resourceStorageGetManyRequest = _resourceRequestFactory.CreateResourceStorageGetManyRequest(
                                                                                                            @namespace,
                                                                                                            _ownerId,
                                                                                                            _requestId,
                                                                                                            ifModifiedSince,
                                                                                                            notEtags);

            ResourceStorageGetManyResponse resourceStorageGetManyResponse = await _mediator.Send(resourceStorageGetManyRequest);

            _logger.LogTrace($"{nameof(ResourcesController)}:{nameof(GetMany)}. Processing storage response.");
            ResourceResponseGetManyRequest resourceResponseGetManyRequest = _resourceResponseFactory.CreateResourceResponseGetManyRequest(
                                                                                                          resourceStorageGetManyResponse.Model,
                                                                                                         (HttpStatusCode)resourceStorageGetManyResponse.StatusCode
                                                                                                     );

            ResourceResponse<IEnumerable<Data.Model.Response.Resource>> resourceResponse = await _mediator.Send(resourceResponseGetManyRequest);

            _logger.LogTrace($"{nameof(ResourcesController)}:{nameof(GetMany)}. Handling response.");

            return _resourceResponseHandler.HandleMany(this, resourceResponse);

        }
    }
}
