using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Handlers.Resource;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Api.Controllers
{

    public partial class ResourcesController
    {


        /// <summary>
        /// POST : api/resources/storage.name.space[?keys=id1[&keys=id2...]&noLinks=true|(false)
        /// </summary>
        /// <param name="namespace">The resource storage namespace, which is a dotted string folder semantic for client 
        /// based resource aggregation (delcaring data type)</param>
        /// <param name="content">The supplied resource Content value</param>
        /// <param name="keys">optional list of key names used to selectively strip out content returned to the caller. 
        /// if keys are provided, they msut occur as first level (non nested) properties in the content and then only 
        /// those key values are returned in the content for response and client synchronisation purposes. 
        /// Reduces bandwidth. Otherwise the whole content is returned</param>
        /// <returns>
        /// 201 Created with body content contianing the <see cref="Data.Model.Response.Resource"/> 
        /// and enriched with link information in exceess of the Location header info.
        /// 400 BadRequest if parameters are invalid
        /// </returns>
        /// <example>
        ///     To create a resource in the callers personal storage area in a virtual folder called 'identity' with the content posted in the body.
        ///     
        ///     POST: https://{host}/api/resources/identity
        ///     body: { forename: 'Milly', familyname: 'Millenium', dob: '2000-01-01T00:00:00' }
        ///     
        /// </example>
        [HttpPost]
        [Route("{*namespace}")]
        public async Task<IActionResult> Post(
            [Required][FromRoute] string @namespace,
            [Required][FromBody] dynamic content,
            [FromQuery] string keys
            )
        {
            var request = new ResourcePostRequest()
            {
                Namespace = @namespace,
                Content = content,
                Keys = keys,
                OwnerId = _ownerId,
                RequestId = _requestId,
                Scheme = Request.Scheme,
                Host = Request.Host.Value,
                Path = Request.Path.Value
            };

            var response = await _mediator.Send(request);

            return response.Handle(this);
        }
    }
}
