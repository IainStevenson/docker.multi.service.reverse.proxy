using Data.Model.Response;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Response.Formater
{
    public interface IResponseHeadersProvider
    {
        Task<IHeaderDictionary> AddHeadersFromItem<T>(T resource) where T : IResponseItem;
        Task RemoveUnwantedHeaders(IHeaderDictionary headers);
    }
}
