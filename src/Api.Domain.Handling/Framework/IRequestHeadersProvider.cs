using Microsoft.AspNetCore.Http;

namespace Api.Domain.Handling.Framework
{
    public interface IRequestHeadersProvider
    {
        /// <summary>
        /// Used for concurrency control.
        /// The If-Match HTTP request header makes the request conditional. 
        /// For GET and HEAD methods, the server will send back the requested resource only if it matches one of the listed ETags
        /// </summary>
        /// <param name="headers">The HttpRequest headers collection.</param>
        /// <returns>
        /// An <see cref="List"/> set of <see cref="string"/> which is empty if no header is found.
        /// </returns>
        List<string> IfHasEtagMatching(IHeaderDictionary headers);


        /// <summary>
        /// Used for concurrency control.
        /// The If-None-Match HTTP request header makes the request conditional. For GET and HEAD methods, 
        /// the server will send back the requested resource, 
        /// with a 200 status, only if it doesn't have an ETag matching the given ones.
        /// When the condition fails for GET and HEAD methods, then the server must return HTTP status code 304 (Not Modified). 
        /// For methods that apply server-side changes, the status code 412 (Precondition Failed) is used. 
        /// Note that the server generating a 304 response MUST generate any of the following header fields that would have been sent in a 200 (OK) response to the same request: Cache-Control, Content-Location, Date, ETag, Expires, and Vary.
        /// ALSO NOTE: The * value is removed as POST VIA PUT is not supported in this API as a policy decision.
        /// </summary>
        /// <param name="headers">The HttpRequest headers collection.</param>
        /// <returns>
        /// An <see cref="List"/> set of <see cref="string"/> which is empty of no header is found.
        /// </returns>
        List<string> IfDoesNotHaveEtagMatching(IHeaderDictionary headers);

        /// <summary>
        /// Used for concurrency control.
        /// The If-Modified-Since request HTTP header makes the request conditional: 
        /// the server sends back the requested resource, with a 200 status, only if it has been last modified after the given date. 
        /// If the resource has not been modified since, the response is a 304 without any body; 
        /// the Last-Modified response header of a previous request. 
        /// Unlike If-Unmodified-Since, If-Modified-Since can only be used with a GET or HEAD.
        /// </summary>
        /// <param name="headers">The HttpRequest headers collection.</param>
        /// <returns>
        /// An instance of <see cref="DateTimeOffset"/>  which returns the provided default value if no header is found. 
        /// </returns>
        DateTimeOffset IfHasChangedSince(IHeaderDictionary headers, DateTimeOffset defaultValue);

        /// <summary>
        /// Used for concurrency control.
        /// The HyperText Transfer Protocol (HTTP) If-Unmodified-Since request header makes the request for the resource conditional: 
        /// the server will send the requested resource or accept it in the case of a POST or another non-safe method only 
        /// if the resource has not been modified after the date specified by this HTTP header. 
        /// If the resource has been modified after the specified date, the response will be a 412 Precondition Failed error.
        /// </summary>
        /// <param name="headers">The HttpRequest headers collection.</param>
        /// <returns>
        /// An instance of <see cref="DateTimeOffset"/> which returns the provided default value if no header is found. 
        /// </returns>
        DateTimeOffset IfIsUnchangedSince(IHeaderDictionary headers, DateTimeOffset defaultValue);

        /// <summary>
        /// Used for soft deleteion control. If present will instruct to query for only deleted items
        /// </summary>
        /// <param name="headers"></param>
        /// <returns>True if header is present else false</returns>
        bool IfIsDeleted(IHeaderDictionary headers);
    }
}
