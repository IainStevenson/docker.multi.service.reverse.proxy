using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Handlers.Resource;
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
        /// <param name="namespace">The required resource storage namespace, an optionally dotted string folder semantic for the client  to declare a data type and provides a means of type aggregation. 
        /// The dotted string format is validated and any deviance will result in a return status code of 400-BadRequest.
        /// Namesapce must follow the .NET rules for Namespaces.
        /// </param>
        /// <param name="content">The client supplied resource Content value. This 'is' the resource to the client.</param>
        /// <param name="keys">
        /// This option reduces return response bandwidth. 
        /// An optional list of Content object key property names. These are used to selectively return content in the response in the <see cref="Data.Model.Response.Resource"/> content property. All properties are stored but only these key properties are returned if they are provided in the post call and exist in the content. See <see cref="Response.Formater.ResourceContentModifier"/> for details.
        /// if keys are provided, they must ALL occur as first level (non nested) properties in the content and then only 
        /// those key values are returned in the content and are available for client-server synchronisation purposes. 
        /// If no keys are provided the whole content is returned in the <see cref="Data.Model.Response.Resource"/>.
        /// If any keys provided are missing from the content, the whole content is returned in the <see cref="Data.Model.Response.Resource"/>.
        /// </param>
        /// <returns>
        /// 201 Created with response body content containing an instance of <see cref="Data.Model.Response.Resource"/> 
        /// which is a wrapper around the client content enriched with storage identifier, ETag, time stamps, plus link information in exceess of the standard Location header info to provide a rich HATEOS compliance.
        /// 400 BadRequest if any parameters are invalid.
        /// </returns>
        /// <example>
        ///     To create a resource in the callers personal storage area in a virtual folder called 'identity' with the content posted in the body.
        ///     
        ///     POST: https://{host}/api/resources/identity
        ///     body: { forename: 'Milly', familyname: 'Millenium', dob: '2000-01-01T00:00:00' }
        ///     
        /// </example>
        [HttpPost]
        [Route("{namespace}")]
        public async Task<IActionResult> Post(
            [Required][FromRoute] string @namespace,
            [Required][FromBody] dynamic content,
            [FromQuery] string keys)
        {

            _logger.LogTrace($"{nameof(ResourcesController)}:POST. Sending request.");

            var request = new ResourcePostRequest()
            {
                Namespace = @namespace.ToLower(),
                Content = content,
                Keys = keys,
                OwnerId = _ownerId,
                RequestId = _requestId,
                Scheme = Request.Scheme,
                Host = Request.Host.Value,
                PathBase = Request.PathBase.Value,
                Path = Request.Path.Value
            };

            var response = await _mediator.Send(request);

            _logger.LogTrace($"{nameof(ResourcesController)}:POST. Processing response.");

            return response.Handle(this);
        }
    }
}