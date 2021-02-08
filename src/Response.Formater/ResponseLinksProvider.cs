using Data.Model.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Response.Formater
{
    public class ResponseLinksProvider<T> : IResponseLinksProvider<T>
        where T: IResponseItem
    {
       
        public Task<T> AddLinks(T source,
            string scheme,
            string host,
            string path,
            IDictionary<string, string> systemKeys,
            IDictionary<string, string> relatedEntities)
        {

            // Fist segment is api
            // secondsegment is '[controller]'
            // other segments are Uri parameters
            var pathSegments = path.Split('/');         

            {
                var links = new List<IApiLink>();
                // add the default and fixed HATEOS links for the entity
                links.Add(new ApiLink() { Rel = "self", Href = $"{scheme}://{host}{path}", Action = "post" });
                links.Add(new ApiLink() { Rel = "self", Href = $"{scheme}://{host}{path}/{{id}}", Action = "put" });
                links.Add(new ApiLink() { Rel = "self", Href = $"{scheme}://{host}{path}/{{id}}", Action = "get" });
                links.Add(new ApiLink() { Rel = "self", Href = $"{scheme}://{host}{path}/{{id}}", Action = "delete" });

                // Add any optional relationship verb liks
                foreach (var item in relatedEntities)
                {
                    links.Add(new ApiLink
                    {
                        Rel = item.Value,
                        Href = $"{scheme}://{host}{path}/{{id}}/{item.Value}",
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

                source.Links = links;
            }
            return Task.FromResult(source);
        }
    }
}
