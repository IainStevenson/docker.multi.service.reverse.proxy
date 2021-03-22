using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Handlers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Api.Controllers
{

    public partial class ResourcesController
    {
        /// <summary>
        /// PUT: api/Resource{identity}/{namespace}/{id:guid}
        /// </summary>
        /// <remarks>
        /// Updates an existing resource, and or, optionally moves its namespace. 
        /// Supports Headers: If-Unmodified-Since, If-Match (ETag) as pre-conditions
        /// </remarks>
        /// <param name="id">The resource identifier. Identifies the record that must match if-* qualifiers</param>
        /// <param name="namespace">The resource storage namespace, this updates any previous namespace for the resource</param>
        /// <param name="content">The supplied resource Content value, if this is null and the namespace is provided then a 'move of namespace' occurs without changing the content, if both are null then BadRequest is returned.</param>
        /// <returns>
        /// 404 NotFound - Gone - no exist at all
        /// 412 PreConditionFailed - does not satisfy if-* header qualifiers
        /// 400 BadRequest - Malformed - incomplete - invalid request
        /// 200 OK - Success - returns changed object
        /// </returns>
        [HttpPut()]
        [Route("{id:guid}/{namespace?}")]
        public async Task<IActionResult> Put(
            [Required][FromRoute] Guid id,
            [FromRoute] string @namespace,
            [FromBody] dynamic content)
        {

            var requestResult = await _requestHandler.Handle(
                                                new RequestMessage<dynamic>()
                                                {
                                                    Caller = typeof(ResourcesController),
                                                    RequestType = Request.Method,
                                                    Headers = Request.Headers,
                                                    Query = Request.Query,
                                                    Parameters = new {Id = id, Namespeace = @namespace },
                                                    Model = content,
                                                    Owner = _ownerId,
                                                    RequestId = _requestId

                                                }); 

            return await _responseHandler.Handle(requestResult);
        }
    }
}
