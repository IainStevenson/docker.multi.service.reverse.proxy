using Data.Model.Response;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Response.Formater
{
    public interface IResponseHeadersProvider
    {
        IHeaderDictionary AddHeadersFromItem<T>(T resource) where T : IResponseItem;
        void RemoveUnwantedHeaders(IHeaderDictionary headers);
    }
}
