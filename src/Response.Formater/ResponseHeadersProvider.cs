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
        public IHeaderDictionary AddHeadersFromItem<T>(T resource) where T : IResponseItem
        {
            var headers = new HeaderDictionary
            {
                { HeaderKeys.LastModified, $"{(resource.Modified ?? resource.Created):r}" },
                { HeaderKeys.ETag, resource.Etag }
            };
            return  headers;
        }

        public void RemoveUnwantedHeaders(IHeaderDictionary headers)
        {
            foreach( var header in _unwantedHeadersList)
            {
                if (headers.ContainsKey(header))
                {
                    headers.Remove(header);
                }               
            }            
        }
    }
}
