using Data.Model.Response;

namespace Api.Domain.Handling.Framework
{
    public class ResponseLinksProvider : IResponseLinksProvider
    {

        /// <inheritdoc/>
        public Task<List<IApiLink>> BuildLinks(
            string scheme,
            string host,
            string pathBase,
            string path,
            string contentNamespace,
            string systemKey)
        {        
            var rootPath = $"{scheme}://{host}{pathBase}{path}";            

            var links = new List<IApiLink>();

            // add the default and fixed HATEOAS links for the entity and namespace
            links.Add(new ApiLink() { Rel = contentNamespace, Href = $"{rootPath}/{contentNamespace}", Action = "post" });
            links.Add(new ApiLink() { Rel = contentNamespace, Href = $"{rootPath}/{contentNamespace}", Action = "list" });
            links.Add(new ApiLink() { Rel = contentNamespace, Href = $"{rootPath}/{systemKey}/{contentNamespace}", Action = "get" });
            links.Add(new ApiLink() { Rel = contentNamespace, Href = $"{rootPath}/{systemKey}/{contentNamespace}", Action = "put" });
            links.Add(new ApiLink() { Rel = contentNamespace, Href = $"{rootPath}/{systemKey}/{contentNamespace}", Action = "delete" });

            var relatedLinks = new List<IApiLink>();

            return Task.FromResult(links);
        }
    }
}
