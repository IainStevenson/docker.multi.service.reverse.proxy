﻿using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using Api.Domain.Handling;
using Api.Domain.Handling.Post;
using Api.Domain.Storage.Post;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;


namespace Api.Controllers
{
    public partial class ResourcesController
    {
        /// <summary>
        /// Action verb POST : api/resources/{namespace}[?keys=id1[&keys=id2...]
        /// </summary>
        /// <param name="namespace">The required resource storage namespace, an optionally dotted string folder semantic for the client to declare a data type and provides a means of type aggregation. 
        /// The dotted string format is validated and any deviance will result in a return status code of 400-BadRequest.
        /// Namesapce must follow the .NET rules for Namespaces.
        /// </param>
        /// <param name="keys">
        /// This option reduces return response bandwidth. 
        /// An optional list of Content object key property names. These are used to selectively return content in the response in the <see cref="Data.Model.Response.Resource"/> content property. All properties are stored but only these key properties are returned if they are provided in the post call and exist in the content. See <see cref="Response.Formater.ResourceContentModifier"/> for details.
        /// if keys are provided, they must ALL occur as first level (non nested) properties in the content and then only 
        /// those key values are returned in the content and are available for client-server synchronisation purposes. 
        /// If no keys are provided the whole content is returned in the <see cref="Data.Model.Response.Resource"/>.
        /// If any keys provided are missing from the content, the whole content is returned in the <see cref="Data.Model.Response.Resource"/>.
        /// </param>
        /// <param name="content">The client supplied resource Content value. This 'is' the resource to the client.</param>
        /// <returns>
        /// 201 Created with response body content containing an instance of <see cref="Data.Model.Response.Resource"/> 
        /// which is a wrapper around the client content enriched with storage identifier, ETag, time stamps, plus link information in exceess of the standard Location header info to provide a rich HATEOS compliance.
        /// 400 BadRequest if any parameters are invalid.
        /// </returns>
        /// <example>
        /// 
        ///     To create a resource in the callers personal storage area in a virtual folder called 'identity' with the content posted in the body.
        ///     
        ///     keys are 0 1 or more, and if provided and are present in the body content the POST method will return only the provided key properties from the posted content
        ///     to reduce return bandwith and still provide a key(s)/resourceid set. 
        ///     
        ///     This is so that the client can have a record of what the storage identifier is for thier primary keys.
        ///     
        ///     If keys are excluded the whole of the posted content is returned.
        ///     
        ///     POST: {scheme}://{host}{pathBase}{path}[?keys=id]
        ///     body: { id: 1,  forename: 'Milly', familyname: 'Millenium', dob: '2000-01-01T00:00:00' }
        ///     
        /// </example>
        /// <remarks>
        /// returns 
        /// Code 201
        /// Location header: {scheme}://{host}{pathBase}{path}/74A92E58-854B-4897-A78D-49BAAC26CAB1
        /// Body: { etag: "kljadlasldjsljd", content: { id: 1 } , resourceId: '74A92E58-854B-4897-A78D-49BAAC26CAB1' created: '2021-09-29 10:10:10.0000+01:00' }
        /// 
        /// This data allows clients to know for certain that its id of '1' is stored as resource id '74A92E58-854B-4897-A78D-49BAAC26CAB1' for faster retrieval at the location header address.
        /// Keeping a track of client primary key values and server primary key guid's at the client side is optional but recommended for performance reasons.
        /// </remarks>
        [HttpPost]
        [Route("{namespace}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Post(
            [Required][FromRoute] string @namespace,
            [FromQuery] string keys,
            [Required][FromBody] dynamic content
            )
        {
            _logger.LogTrace($"{nameof(ResourcesController)}:{nameof(Post)}. Processing request...");

            ResourceStoragePostRequest resourceRequest = _resourceRequestFactory.CreateResourceStoragePostRequest(
                                                                            @namespace,
                                                                            content,
                                                                            keys,
                                                                            _ownerId,
                                                                            _requestId);

            ResourceStoragePostResponse resourceResponse = await _mediator.Send(resourceRequest);

            _logger.LogTrace($"{nameof(ResourcesController)}:{nameof(Post)}. Processing response output..");

            ResourceOutputPostRequest resourceOutputRequest = _resourceResponseOutputFactory.CreateResourceOutputPostRequest(            
                resourceResponse.Model,
                (HttpStatusCode)resourceResponse.StatusCode,
                Request.Scheme,
                Request.Host.Value,
                Request.PathBase.Value,
                Request.Path.Value,
                keys
            );

            ResourceOutputResponse<Data.Model.Response.Resource> resourceOutput = await _mediator.Send(resourceOutputRequest);

            _logger.LogTrace($"{nameof(ResourcesController)}:{nameof(Post)}. returning output.");

            return  _responseOutputHandler.Handle(this, resourceOutput);
        }
    }
}