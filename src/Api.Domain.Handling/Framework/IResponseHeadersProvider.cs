using Data.Model.Response;
using Microsoft.AspNetCore.Http;

namespace Api.Domain.Handling.Framework
{
    public interface IResponseHeadersProvider
    {
        IHeaderDictionary AddHeadersFromItem<T>(T resource) where T : IResponseEntity;
        void RemoveUnwantedHeaders(IHeaderDictionary headers);
    }
}
