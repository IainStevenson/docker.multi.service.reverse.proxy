using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{

    public partial class ResourcesController 
    {

        /// <summary>
        /// DELETE: api/resources/CEC71C39-6D38-404D-88EB-BD9479F004CA
        /// </summary>
        /// <remarks>
        /// Supports Headers: If-Unmodified-Since, If-Match
        /// </remarks>
        /// <param name="id">The identifier of the resource</param>
        /// <returns>
        /// 400 BadRequest
        /// 404 NotFound
        /// 412 PreConditionFailed
        /// 200 OK
        /// </returns>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {


            //Data.Model.Storage.Resource resource = (await _storage.GetAsync(r =>
            //                   r.Id == id
            //                   )).FirstOrDefault();

            //if (resource == null)
            //{
            //    return NotFound();
            //}

            //var unmodifiedSince = await _requestHeadersProvider.IfUnmodifiedSince(Request.Headers);
            //var etags = await _requestHeadersProvider.IfMatch(Request.Headers);

            //// only proceed if resource is unmodified since and is one of the etags
            //if (
            //        (resource.Modified.HasValue ? resource.Modified.Value <= unmodifiedSince : resource.Created <= unmodifiedSince) ||
            //        (etags.Contains(resource.Etag))
            //        )
            //{

            //    dynamic error = new { Error = "" };
            //    if (etags.Any())
            //    {
            //        error.Error += $"The resource has None of the specified ETags {string.Join(',', etags)}/r/n";
            //    }
            //    if (unmodifiedSince != DateTimeOffset.MinValue)
            //    {
            //        error.Error += $"The resource has been modified since {unmodifiedSince}";
            //    }
            //    await _responseHeadersProvider.AddHeadersFromItem(Response.Headers, _mapper.Map<Data.Model.Response.Resource>(resource));

            //    return StatusCode(412, error);
            //}


            //var count = await _storage.DeleteAsync(id);
            //if (count == 1)
            //    return Ok();

            //return BadRequest(new { Error = $"Delete operation expected 1 record to be deleted but was: {count}" });
            return Ok();
        }
    }
}
