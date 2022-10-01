﻿using Data.Model.Response;
using Microsoft.AspNetCore.Http;

namespace Api.Domain.Handling
{
    public interface IResponseHeadersProvider
    {
        IHeaderDictionary AddHeadersFromItem<T>(T resource) where T : IResponseItem;
        void RemoveUnwantedHeaders(IHeaderDictionary headers);
    }
}
