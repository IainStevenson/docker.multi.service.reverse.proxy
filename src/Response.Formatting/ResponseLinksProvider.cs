using Data.Model.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Response.Formatting
{
    public class ResponseLinksProvider : IResponseLinksProvider
    {

        public Task<List<IApiLink>> BuildLinks(
            string scheme,
            string host,
            string pathBase,
            string path,
            IDictionary<string, string> systemKeys,
            IDictionary<string, string> relatedEntities)
        {

            // Fist segment is api
            // secondsegment is '[controller]'
            // other segments are Uri parameters
            var pathSegments = path.Split('/');
            var links = new List<IApiLink>();

            var rootPath = $"{scheme}://{host}{pathBase}{path}";
            // add the default and fixed HATEOS links for the entity
            links.Add(new ApiLink() { Rel = "self", Href = rootPath, Action = "post" });
            links.Add(new ApiLink() { Rel = "self", Href = $"{rootPath}/{{id}}", Action = "put" });
            links.Add(new ApiLink() { Rel = "self", Href = $"{rootPath}/{{id}}", Action = "get" });
            links.Add(new ApiLink() { Rel = "self", Href = $"{rootPath}/{{id}}", Action = "delete" });

            // Add any optional relationship verb liks
            foreach (var item in relatedEntities)
            {
                links.Add(new ApiLink
                {
                    Rel = item.Value,
                    Href = $"{rootPath}/{{id}}/{item.Value}",
                    Action = item.Key
                });
            }

            // Transform by replacement the values to fulfill the links
            foreach (var item in systemKeys)
            {
                foreach (var link in links)
                {
                    link.Href = link.Href.Replace($"{item.Key}", $"{item.Value}");
                }
            }

            return Task.FromResult(links);
        }
    }
}
