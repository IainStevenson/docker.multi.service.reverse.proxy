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
        /// PUT: api/Resource{identity}/{namespace}/{id:guid}
        /// </summary>
        /// <remarks>
        /// Supports Headers: If-Unmodified-Since, If-Match
        /// </remarks>
        /// <param name="owner">The resource owner identifier</param>
        /// <param name="namespace">The resource storage namespace, this overwrites any previous namespace for the resource</param>
        /// <param name="id">The resource identifier</param>
        /// <param name="content">The supplied resource Content value</param>
        /// <returns>
        /// 400 BadRequest
        /// 404 NotFound
        /// 412 PreConditionFailed
        /// 204 NoContent
        /// </returns>
        [HttpPut()]
        [Route("{owner}/{namespace}/{id:guid}")]
        public async Task<IActionResult> Put(
            [Required][FromRoute] string owner,
            [Required][FromRoute] string @namespace,
            [Required][FromRoute] Guid id,
            [FromBody] dynamic content)
        {
           
            Data.Model.Storage.Resource resource = (await _storage.GetAsync(r =>
                                      r.Id == id
                                      )).FirstOrDefault();

            if (resource == null)
            {
                return NotFound();
            }

            var unmodifiedSince = await _requestHeadersProvider.IfUnmodifiedSince(Request.Headers);
            var etags = await _requestHeadersProvider.IfMatch(Request.Headers);

            // only proceed if resource is unmodified since and is one of the etags
            if (
                    (resource.Modified.HasValue ? resource.Modified.Value <= unmodifiedSince : resource.Created <= unmodifiedSince) ||
                    (etags.Contains(resource.Etag))
                    )
            {

                dynamic error = new { Error = "" };
                if (etags.Any())
                {
                    error.Error += $"The resource has None of the specified ETags {string.Join(',', etags)}/r/n";
                }
                if (unmodifiedSince != DateTimeOffset.MinValue)
                {
                    error.Error += $"The resource has been modified since {unmodifiedSince}";
                }
                await _responseHeadersProvider.AddHeadersFromItem(Response.Headers, _mapper.Map<Data.Model.Response.Resource>(resource));

                return StatusCode(412, error);
            }

            var currentResourceNamespace = resource.Namespace;

            resource.Content = content;
            resource.Namespace = @namespace;
            resource = await _storage.UpdateAsync(resource);

            var response = _mapper.Map<Data.Model.Response.Resource>(resource);


            if (@namespace != currentResourceNamespace)
            {
                var systemKeys = new Dictionary<string, string>() { { "{id}", $"{resource.Id}" } };
                var relatedEntities = EmptyEntityList;
                response = await _responseLinksProvider.AddLinks(response,
                                                                Request.Scheme,
                                                                Request.Host.Value,
                                                                Request.Path.Value.TrimEnd('/'),
                                                                systemKeys,
                                                                relatedEntities);
                await _responseHeadersProvider.AddHeadersFromItem(Response.Headers, response);
                
            }
            return Ok(response);
        }

    }
}
