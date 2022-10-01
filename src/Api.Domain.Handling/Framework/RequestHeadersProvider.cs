using Microsoft.AspNetCore.Http;

namespace Api.Domain.Handling.Framework
{
    public class RequestHeadersProvider : IRequestHeadersProvider
    {
        /// <inheritdoc/>
        public List<string> IfHasEtagMatching(IHeaderDictionary headers)
        {
            var result = new List<string>();
            if (headers.ContainsKey(HeaderKeys.IfMatch))
            {
                result.AddRange($"{headers[HeaderKeys.IfMatch]}".Split(',', StringSplitOptions.RemoveEmptyEntries));
            }
            return result;
        }

        /// <inheritdoc/>
        public List<string> IfDoesNotHaveEtagMatching(IHeaderDictionary headers)
        {
            var result = new List<string>();
            if (headers.ContainsKey(HeaderKeys.IfNoneMatch))
            {
                result.AddRange($"{headers[HeaderKeys.IfNoneMatch]}".Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
            }
            result.Remove("*"); // illogical to exclude all, why bother calling!, Also POST via PUT NOT supported in this API
            return result;
        }

        /// <inheritdoc/>
        public DateTimeOffset IfHasChangedSince(IHeaderDictionary headers, DateTimeOffset defaultValue)
        {
            DateTimeOffset result = defaultValue;
            if (headers.ContainsKey(HeaderKeys.IfModifiedSince))
            {
                var isValid = DateTimeOffset.TryParse(headers[HeaderKeys.IfModifiedSince].ToString(), out DateTimeOffset headerDate);
                if (isValid) result = headerDate;
            }
            return result;
        }

        /// <inheritdoc/>
        public DateTimeOffset IfIsUnchangedSince(IHeaderDictionary headers, DateTimeOffset defaultValue)
        {
            DateTimeOffset result = defaultValue;
            if (headers.ContainsKey(HeaderKeys.IfUnmodifiedSince))
            {
                var isValid = DateTimeOffset.TryParse(headers[HeaderKeys.IfUnmodifiedSince].ToString(), out DateTimeOffset headerDate);
                if (isValid) result = headerDate;
            }
            return result;
        }
    }
}
