using Data.Model.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Response.Formater
{
    public class ResponseHeadersProvider : IResponseHeadersProvider
    {
        private readonly IList<string> _unwantedHeadersList;
        public ResponseHeadersProvider(IList<string> unwantedHeadersList)
        {
            _unwantedHeadersList = unwantedHeadersList;
        }
        public Task<IHeaderDictionary> AddHeadersFromItem<T>(T resource) where T : IResponseItem
        {
            var headers = new Dictionary<string, StringValues>
            {
                { HeaderKeys.LastModified, $"{(resource.Modified ?? resource.Created):r}" },
                { HeaderKeys.ETag, resource.Etag }
            };
            return  Task.FromResult(headers as IHeaderDictionary);
        }

        public Task RemoveUnwantedHeaders(IHeaderDictionary headers)
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
