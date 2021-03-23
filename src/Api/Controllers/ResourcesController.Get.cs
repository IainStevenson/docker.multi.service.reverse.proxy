using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Handlers.Resource;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Api.Controllers
{

    public partial class ResourcesController
    {

        /// <summary>
        /// GET: api/resources/{identity}/{id:guid}
        /// </summary>
        /// <remarks>
        /// Supports Headers: If-Modified-Since (which is interpreted as New or changed since), If-None-Match
        /// </remarks>
        /// <param name="id">The unique storage identifier of the rsource.</param>
        /// <returns>
        /// Status code 404 Not Found if the resource does not exist
        /// Status code 200 and an instance of <see cref="Data.Model.Response.Resource"/> wrapping the <see cref="Data.Model.Storage.Resource"/> matching the resource identifier .
        /// Status Code 304 Unchanged if the resource was modified (via etag 'If-None-Match' check) or Modified Date 'If-Modified-Since' check
        /// </returns>
        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IActionResult> Get(
            [Required][FromRoute] Guid id)
        {
            
            var request = new ResourceGetOneRequest()
            {
                Id = id,
                Headers = Request.Headers
            };

            var response = await _mediator.Send(request);

            return response.Handle(this);
            
        }


        /// <summary>
        /// GET: api/resources/{identity}/{*namespace}
        /// </summary>
        /// <remarks>
        /// Supports Headers: If-Modified-Since, which is interpreted as New or changed since
        /// </remarks>
        /// <param name="namespace">The collective storage name space of the resources.</param>
        /// <returns>
        /// An array of all of the <see cref="Data.Model.Response.Resource"/> wrapping the existing <see cref="Data.Model.Response.Resource"/> of that namespace. 
        /// When If-Modified-Since is not specified all of the existing resources are returned with status code 200.
        /// When If-Modified-Since is specified with a valid time, only those resources changed since that time are returned with status code of 200. If none have changed since then, a status code of 304 is returned with no body content.
        /// </returns>
        [HttpGet]
        [Route("{*namespace}")]
        public async Task<IActionResult> Get(
            [Required][FromRoute] string @namespace)
        {

            var request = new ResourceGetManyRequest()
            {
                Namespace = @namespace,
                Headers = Request.Headers,
                OwnerId = _ownerId
            };

            var response = await _mediator.Send(request);

            return response.Handle(this);           
        }
    }
}
