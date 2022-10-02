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
        public async Task<IActionResult> GetOne(
            [Required][FromRoute] string @namespace,
            [Required][FromRoute] Guid id
            )
        {

            _logger.LogTrace($"{nameof(ResourcesController)}:{nameof(GetOne)}. Processing request.");

            var ifModifiedSince =  _requestHeadersProvider.IfHasChangedSince(Request.Headers, DateTimeOffset.MinValue);
            var etags =  _requestHeadersProvider.IfDoesNotHaveEtagMatching(Request.Headers);

            ResourceStorageGetOneRequest resourceGetOneRequest = _resourceRequestFactory.CreateResourceGetOneRequest(id,
                                                                                            @namespace,
                                                                                            _ownerId,
                                                                                            _requestId,
                                                                                            ifModifiedSince,
                                                                                            etags);

            var resourceStorageGetOneResponse = await _mediator.Send(resourceGetOneRequest);

            ResourceResponseGetOneRequest resourceResponseGetOneRequest = _resourceResponseFactory.CreateResourceResponseGetOneRequest(
                                                                                            resourceStorageGetOneResponse.Model,
                                                                                           (HttpStatusCode)resourceStorageGetOneResponse.StatusCode,
                                                                                           Request.Scheme,
                                                                                           Request.Host.Value,
                                                                                           Request.PathBase.Value,
                                                                                           Request.Path.Value
                                                                                       );

            ResourceResponse<Data.Model.Response.Resource> resourceResponse = await _mediator.Send(resourceResponseGetOneRequest);

            _logger.LogTrace($"{nameof(ResourcesController)}:{nameof(GetOne)}. Processing response.");

            return _resourceResponseHandler.HandleOne(this, resourceResponse);
        }


        /// <summary>
        /// GET: api/resources/{namespace}
        /// </summary>
        /// <remarks>
        /// Supports Headers: If-Modified-Since as a valid .NET datetime format (which is interpreted as New or changed since), If-None-Match providing a comma separated list of server generated 'ShortGuid' etags
        /// </remarks>
        /// <param name="namespace">The client elected storage namespace / type of the resource.</param>
        /// <param name="id">The server elected unique storage identifier of the already stored resource.</param>
        /// <returns>
        /// Status code 404 Not Found if the resource does not exist in that namespace.
        /// Status code 200 and an instance of <see cref="Data.Model.Response.Resource"/> wrapping the <see cref="Data.Model.Storage.Resource"/> matching the resource identifier .
        /// Status Code 304 Unchanged if the resource was modified (via etag 'If-None-Match' check) or Modified Date 'If-Modified-Since' check
        /// </returns>
        [HttpGet]
        [Route("{namespace}")]
        public async Task<IActionResult> GetMany(
            [Required][FromRoute] string @namespace
            )
        {

            _logger.LogTrace($"{nameof(ResourcesController)}:{nameof(GetMany)}. Processing request.");

            var ifModifiedSince =  _requestHeadersProvider.IfHasChangedSince(Request.Headers, DateTimeOffset.MinValue);
            var etags =  _requestHeadersProvider.IfDoesNotHaveEtagMatching(Request.Headers);

            ResourceStorageGetManyRequest resourceStorageGetManyRequest = _resourceRequestFactory.CreateResourceStorageGetManyRequest(
                                                                                                            @namespace, 
                                                                                                            _ownerId, 
                                                                                                            _requestId, 
                                                                                                            ifModifiedSince, 
                                                                                                            etags);

            ResourceStorageGetManyResponse resourceStorageGetManyResponse = await _mediator.Send(resourceStorageGetManyRequest);

            _logger.LogTrace($"{nameof(ResourcesController)}:{nameof(GetMany)}. Processing storage response.");
            ResourceResponseGetManyRequest resourceResponseGetManyRequest = _resourceResponseFactory.CreateResourceResponseGetManyRequest(
                                                                                                          resourceStorageGetManyResponse.Model,
                                                                                                         (HttpStatusCode)resourceStorageGetManyResponse.StatusCode,
                                                                                                         Request.Scheme,
                                                                                                         Request.Host.Value,
                                                                                                         Request.PathBase.Value,
                                                                                                         Request.Path.Value
                                                                                                     );

            ResourceResponse<IEnumerable<Data.Model.Response.Resource>> resourceResponse = await _mediator.Send(resourceResponseGetManyRequest);

            _logger.LogTrace($"{nameof(ResourcesController)}:{nameof(GetMany)}. Handling response.");

            return _resourceResponseHandler.HandleMany(this, resourceResponse);

        }
    }
}
