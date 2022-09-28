
using Data.Model.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Response.Formatting
{
    public interface IResponseLinksProvider
    {

        /// <summary>
        /// Builds HEATEOS Links collection to the <see cref="ApiLinks"/> property.
        /// Add the standard POST, GET, PUT And DELETE verbs actions and any additionally supplied sub collection links.
        /// </summary>
        /// <param name="source">The source item to apply the links to</param>
        /// <param name="scheme">The HTTP Request Scheme</param>
        /// <param name="host">The Http Request Host</param>
        /// <param name="pathBase">The Http request path base (if used) for generating links</param>
        /// <param name="path">The Http request path for generating links</param>
        /// <param name="systemKeys">Additional key information to finalise the link Urls</param>
        /// <param name="relatedEntities">Additional and optional (sub-resource) verbs/actions/href's to apply to the outgoing response item</param>
        /// <param name="ownerKeys">Removes all but the ownerKey properties from returned content, unless empty when all content is returned</param>
        /// <returns>A reference to the modified source.</returns>
        Task<List<IApiLink>> BuildLinks(
            string scheme,
            string host,
            string pathBase,
            string path,
            IDictionary<string, string> systemKeys,
            IDictionary<string, string> relatedEntities);
    }
}
