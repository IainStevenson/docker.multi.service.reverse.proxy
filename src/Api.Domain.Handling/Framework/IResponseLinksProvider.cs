using Data.Model.Response;

namespace Api.Domain.Handling.Framework
{
    /// <summary>
    /// Handles HATEOS (Html as the engine of state) links for clients that require it.
    /// </summary>
    public interface IResponseLinksProvider
    {

        /// <summary>
        /// Builds HEATEOS Links collection to the <see cref="ApiLinks"/> property.
        /// The Url schem is as follows
        /// POST api/resources/weatherforecast creates a resource with a namespace of weatherforecast
        /// POST api/resources/health/bloodpressure creates a resource with a namespace of health/bloodpressure
        /// 
        /// Example paramters
        /// scheme = https
        /// host = localhost
        /// pathBase = api/
        /// path = resources
        /// namespace = health/bloodpressure
        /// systemKey "1234"
        /// 
        /// Rel          Action  HRef
        /// -------------------------------------------------------------------------
        /// entity       post    https://loalhost/api/resources/health/bloodpressure
        /// entity       list    https://loalhost/api/resources/health/bloodpressure
        /// entity       get     https://loalhost/api/resources/1234/health/bloodpressure
        /// entity       put     https://loalhost/api/resources/1234/health/bloodpressure
        /// entity       delete  https://loalhost/api/resources/1234/health/bloodpressure
        /// 
        /// </summary>
        /// <param name="scheme">The HTTP Request Scheme</param>
        /// <param name="host">The Http Request Host</param>
        /// <param name="pathBase">The Http request path base (if used) for generating links</param>
        /// <param name="path">The Http request path for generating links</param>
        /// <param name="systemKey">Additional key information to finalise the link Urls</param>
        /// <returns>A <see cref="List{IApiLink}"/> containg the necessary HATEOAS links.</returns>
        Task<List<IApiLink>> BuildLinks(
            string scheme,
            string host,
            string pathBase,
            string path,
            string contentNamespace,
            string systemKey);
    }
}
