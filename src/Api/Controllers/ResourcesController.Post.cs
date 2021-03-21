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
        /// POST : api/resources/storage.name.space[?keys=id1[&keys=id2...]&noLinks=true|(false)
        /// </summary>
        /// <param name="owner">The resource owner identifier, as a string that can be 'my' (for current owner) or a 
        /// fully qualified email address of some other owner relative to the callers token based identity</param>
        /// <param name="namespace">The resource storage namespace, which is a dotted string folder semantic for client 
        /// based resource aggregation</param>
        /// <param name="content">The supplied resource Content value</param>
        /// <param name="keys">optional list of key names used to selectively strip out content returned to the caller. 
        /// if keys are provided, they msut occur as first level (non nested) properties in the content and then only 
        /// those key values are returned in the content for response and client synchronisation purposes. 
        /// Reduces bandwidth. Otherwise the whole content is returned</param>
        /// <returns>
        /// 201 Created with body content contianing the <see cref="Data.Model.Response.Resource"/> 
        /// and enriched with link information in exceess of the Location header info.
        /// </returns>
        /// <example>
        ///     To create a resource in the callers personal storage area in a virtual folder called 'identity' with the content posted in the body.
        ///     
        ///     POST: https://{host}/api/resources/my/identity
        ///     body: { forename: 'Milly', familyname: 'Millenium', dob: '2000-01-01T00:00:00' }
        ///     
        /// </example>
        [HttpPost]
        [Route("{namespace}")]
        public async Task<IActionResult> Post(
            [Required] [FromRoute] string @namespace,
            [Required] [FromBody] dynamic content,
            [FromQuery] string keys
            )
        {
           
            var resource = new Data.Model.Storage.Resource()
            {
                Namespace = @namespace,
                Content = content,
                OwnerId = _ownerId,
                Metadata = new Data.Model.Storage.StorageMetadata()
                {
                    RequestId = _requestId
                },

            };

            resource = await _storage.CreateAsync(resource);

            var systemKeys = new Dictionary<string, string>() { { "{id}", $"{resource.Id}" } };

            var relatedEntities = EmptyEntityList;

            var response = _mapper.Map<Data.Model.Response.Resource>(resource);

            response = await _responseLinksProvider.AddLinks(response,
                                                            Request.Scheme,
                                                            Request.Host.Value,
                                                            Request.Path.Value.TrimEnd('/'),
                                                            systemKeys,
                                                            relatedEntities);

            var location = response.Links?.SingleOrDefault(x => x.Action == "get" && x.Rel == "self")?.Href ?? "";

           

            if (!string.IsNullOrWhiteSpace(keys))
            {
                response = await _resourceModifier.CollapseContent(response, keys.Split(','));
            }
            await _responseHeadersProvider.AddHeadersFromItem(Response.Headers, response);
            return Created(location, response);

        }


    }
}
