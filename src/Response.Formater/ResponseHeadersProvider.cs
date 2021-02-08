using Data.Model.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Response.Formater
{
    public class ResponseHeadersProvider : IResponseHeadersProvider
    {
        private IList<string> _unwantedHeadersList;
        public ResponseHeadersProvider(IList<string> unwantedHeadersList)
        {
            _unwantedHeadersList = unwantedHeadersList;
        }
        public Task AddHeadersFromItem<T>(Microsoft.AspNetCore.Http.IHeaderDictionary headers, T resource) where T : IResponseItem
        {
            headers.Add(HeaderKeys.LastModified, $"{(resource.Modified ?? resource.Created):r}");
            headers.Add(HeaderKeys.ETag, resource.Etag);
            return  Task.FromResult(0);
        }

        public Task RemoveUnwantedHeaders(Microsoft.AspNetCore.Http.IHeaderDictionary headers)
        {
            foreach( var header in _unwantedHeadersList)
            {
                if (headers.ContainsKey(header))
                {
                    headers.Remove(header);
                }               
            }
            return Task.FromResult(0);
        }
    }

}
