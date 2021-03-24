using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Response.Formater
{
    public interface IRequestHeadersProvider
    {
        /// <summary>
        /// The If-Match HTTP request header makes the request conditional. 
        /// For GET and HEAD methods, the server will send back the requested resource only if it matches one of the listed ETags
        /// </summary>
        /// <param name="headers"></param>
        /// <returns>
        /// An <see cref="List"/> set of <see cref="String"/>
        /// </returns>
        Task<List<string>> IfMatch(IHeaderDictionary headers);
        

        /// <summary>
        /// The If-None-Match HTTP request header makes the request conditional. For GET and HEAD methods, 
        /// the server will send back the requested resource, 
        /// with a 200 status, only if it doesn't have an ETag matching the given ones.
        /// When the condition fails for GET and HEAD methods, then the server must return HTTP status code 304 (Not Modified). 
        /// For methods that apply server-side changes, the status code 412 (Precondition Failed) is used. 
        /// Note that the server generating a 304 response MUST generate any of the following header fields that would have been sent in a 200 (OK) response to the same request: Cache-Control, Content-Location, Date, ETag, Expires, and Vary.
        /// ALSO NOTE: The * value is removed as POST VIA PUT is not supported in this API as a policy decision.
        /// </summary>
        /// <param name="headers"></param>
        /// <returns>
        /// An <see cref="List"/> set of <see cref="String"/>
        /// </returns>
        Task<List<string>> IfNoneMatch(IHeaderDictionary headers);

        /// <summary>
        /// The If-Modified-Since request HTTP header makes the request conditional: 
        /// the server will send back the requested resource, with a 200 status, only if it has been last modified after the given date.
        /// If the request has not been modified since, the response will be a 304 without any body
        /// </summary>
        /// <param name="headers"></param>
        /// <returns>
        /// An instance of <see cref="DateTimeOffset"/>. 
        /// </returns>
        Task<DateTimeOffset?> IfModifiedSince(IHeaderDictionary headers);
        /// <summary>
        /// The If-Unmodified-Since request HTTP header makes the request conditional: 
        /// the server will send back the requested resource, or accept it in the case of a POST or another non-safe method, 
        /// only if it has not been last modified after the given date. 
        /// If the resource has been modified after the given date, the response will be a 412 (Precondition Failed) error.
        /// </summary>
        /// <param name="headers"></param>
        /// <returns>
        /// An instance of <see cref="DateTimeOffset"/>. 
        /// </returns>
        Task<DateTimeOffset?> IfUnmodifiedSince(IHeaderDictionary headers);
    }
}
