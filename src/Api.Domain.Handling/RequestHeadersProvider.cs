using Microsoft.AspNetCore.Http;


namespace Api.Domain.Handling
{
    public class RequestHeadersProvider : IRequestHeadersProvider
    {


        public Task<List<string>> IfMatch(IHeaderDictionary headers)
        {
            var result = new List<string>();

            if (headers.ContainsKey(HeaderKeys.IfMatch))
            {
                result.AddRange($"{headers[HeaderKeys.IfMatch]}".Split(',', StringSplitOptions.RemoveEmptyEntries));
            }
            return Task.FromResult(result);
        }

        public Task<List<string>> IfNoneMatch(IHeaderDictionary headers)
        {
            var result = new List<string>();
            if (headers.ContainsKey(HeaderKeys.IfNoneMatch))
            {
                result.AddRange($"{headers[HeaderKeys.IfNoneMatch]}".Split(',', StringSplitOptions.RemoveEmptyEntries));
            }
            result.Remove("*"); // illogical to exclude all, why bother calling!, Also POST via PUT NOT supported in this API
            return Task.FromResult(result);
        }

        public Task<DateTimeOffset?> IfModifiedSince(IHeaderDictionary headers)
        {
            DateTimeOffset? result = null;
            if (headers.ContainsKey(HeaderKeys.IfModifiedSince))
            {
                result = DateTimeOffset.Parse($"{headers[HeaderKeys.IfModifiedSince]}");
            }
            return Task.FromResult(result);
        }


        public Task<DateTimeOffset?> IfUnmodifiedSince(IHeaderDictionary headers)
        {
            DateTimeOffset? result = null;
            if (headers.ContainsKey(HeaderKeys.IfUnmodifiedSince))
            {
                result = DateTimeOffset.Parse($"{headers[HeaderKeys.IfUnmodifiedSince]}");
            }
            return Task.FromResult(result);
        }
    }
}
