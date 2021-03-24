# HTTP


Several HTTP headers, called conditional headers, lead to conditional requests. These are:


## Http Conditionals and how they are handled

- If-Match : Succeeds if the ETag of the distant resource is equal to one listed in this header. By default, unless the etag is prefixed with 'W/', it performs a strong validation.
- If-None-Match : Succeeds if the ETag of the distant resource is different to each listed in this header. By default, unless the etag is prefixed with 'W/', it performs a strong validation.
- If-Modified-Since : Succeeds if the Last-Modified date of the distant resource is more recent than the one given in this header.
- If-Unmodified-Since : Succeeds if the Last-Modified date of the distant resource is older or the same than the one given in this header.
- If-Range : Similar to If-Match, or If-Unmodified-Since, but can have only one single etag, or one date. If it fails, the range request fails, and instead of a 206 Partial Content response, a 200 OK is sent with the complete resource.


# Dealing with the first upload of a resource

The first upload of a resource is an edge case of the previous. Like any update of a resource, it is subject to a race condition if two clients try to perform at the similar times. To prevent this, conditional requests can be used: by adding If-None-Match with the special value of '*', representing any etag. The request will succeed, only if the resource didn't exist before:


# Resources

https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/If-Match
https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/If-None-Match
https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/If-Modified-Since
https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/If-Unmodified-Since