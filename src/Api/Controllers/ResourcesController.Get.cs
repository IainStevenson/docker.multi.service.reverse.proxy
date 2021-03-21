using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
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
            
            Data.Model.Storage.Resource resource = (
                                                    await _storage.GetAsync(r => r.Id == id
                                                    )
                                                    ).FirstOrDefault();

            if (resource == null)
            {
                return NotFound();
            }

            var ifNoneMatch = await _requestHeadersProvider.IfNoneMatch(Request.Headers);


            if (ifNoneMatch.Contains(resource.Etag))
            {

                await _responseHeadersProvider.AddHeadersFromItem(Response.Headers,
                                                _mapper.Map<Data.Model.Response.Resource>(resource));

                return StatusCode(304);
            }

            var ifModifiedSince = await _requestHeadersProvider.IfModifiedSince(Request.Headers);

            if (ifModifiedSince.HasValue)
            {
                var resourceHasNotBeenModifiedSince = !(resource.Modified.HasValue ?
                                                            resource.Modified > ifModifiedSince.Value :
                                                            resource.Created > ifModifiedSince.Value);
                if (resourceHasNotBeenModifiedSince)
                {

                    await _responseHeadersProvider.AddHeadersFromItem(Response.Headers,
                                                    _mapper.Map<Data.Model.Response.Resource>(resource));

                    return StatusCode(304);
                }

            }

            var response = _mapper.Map<Data.Model.Response.Resource>(resource);

            await _responseHeadersProvider.AddHeadersFromItem(Response.Headers, response);

            return Ok(response);
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

           
            IEnumerable<Data.Model.Storage.Resource> resources = new List<Data.Model.Storage.Resource>();

            var ifModifiedSince = await _requestHeadersProvider.IfModifiedSince(Request.Headers);

            if (ifModifiedSince.HasValue)
            {

                resources = await _storage.GetAsync(
                    r => r.OwnerId == _ownerId &&
                    r.Namespace == @namespace &&
                    r.Modified.HasValue ? r.Modified > ifModifiedSince.Value : r.Created > ifModifiedSince.Value);

                if (!resources.Any())
                {
                    return StatusCode(304); 
                }
            }
            else
            {
                resources = await _storage.GetAsync(r => r.OwnerId == _ownerId && r.Namespace == @namespace);
            }

            return Ok(_mapper.Map<IEnumerable<Data.Model.Response.Resource>>(resources));

        }
    }
}
