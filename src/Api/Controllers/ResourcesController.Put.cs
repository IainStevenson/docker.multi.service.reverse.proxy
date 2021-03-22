using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Data.Model.Response;
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

            // logic to abstract the processing of requests and thier logic down into the next layer.
            // this should be the same for each verb passing verb specific data.

            // _resourceHandler
            var requestProcessResult = await _resourceHandler.Handle(
                                                ResourcAction.PUT,
                                                new
                                                {
                                                    Id = id,
                                                    Namespace = @namespace,
                                                    Content = content
                                                }); // returns custom result object ResourceProcessResult

            // pick the handler
            var resultHandler = await _requestHandlers.FirstOrDefault(
                x=> x.HttpStatusCode == requestProcessResult.HttpStatusCode);

            // if no handler procvided throw an excpetion
            if (resultHandler == null) throw new ServicesSetupException($"Missing Services for ResourceProcessHandler of {requestProcessResult.HttpsStatusCode}");
            
            return await resultHandler.Handle(requestProcessResult); // returns Task<IActionResult> if the correct response plus optional content and headers


            // request handler logic for PUT


            if (requestProcessResult.HttpStatusCode == HttpStatusCode.NotFound)
            {
                return NotFound(requestProcessResult.Result);
            }
            else if (requestProcessResult.HttpStatusCode == HttpStatusCode.BadRequest)
            {
                return BadRequest(requestProcessResult.Result);
            }
            else if (requestProcessResult.HttpStatusCode == HttpStatusCode.PreconditionFailed)
            {
                return StatusCode(requestProcessResult.HttpStatusCode, requestProcessResult.Result);
            }
            else if (requestProcessResult.HttpSatusCode == HttpStatusCode.OK)
            {
                return Ok(requestProcessResult.Result);
            }

            // Handler code logic
            Data.Model.Storage.Resource resource = (await _storage.GetAsync(r =>
                                  r.Id == id
                                  )).FirstOrDefault();

            var processModel = new RequestErrorModel()
            {
                Controller = nameof(ResourcesController),
                Action = "PUT",
                StatusCode = HttpStatusCode.OK
            };

            if (resource == null)
            {
                processModel.StatusCode = HttpStatusCode.BadRequest;
                processModel.Reason = "";
            }

            if (content == null &&
                string.IsNullOrWhiteSpace(@namespace))
            {
                return BadRequest(); // nulling content is not allowed, delete instead.
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
