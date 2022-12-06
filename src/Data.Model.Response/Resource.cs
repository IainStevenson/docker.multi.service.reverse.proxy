using CSharpVitamins;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Data.Model.Response
{
    /// <summary>
    /// A client side wrapper around stored content.
    /// </summary>
    public class Resource : IResource, IResponseItem
    {

        /// <summary>
        /// The ETag of this vesion of the resource used for cache control and concurrency checking
        /// </summary>
        public string Etag { get; set; } = (ShortGuid)Guid.NewGuid().ToString();
        /// <summary>
        /// The internal storage identifier this resource is addressed by
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();
        /// <summary>
        /// The client provided content for this resource. On GET the whole Content is returnd as stored. On (POST|PUT), if property names are provided as keys, then the content contains only those keys from the stored content, otherwise the whole content is provided.
        /// </summary>
        public dynamic Content { get; set; }
        /// <summary>
        /// The storage name-space, (type). This follows the ,NET namespace paradigm using dotted string notation. Providing a means of a virtual folder system for storing client content
        /// </summary>
        public string Namespace { get; set; }
        /// <summary>
        /// The UTC date and time of creation.
        /// </summary>
        public DateTimeOffset Created { get; set; } = DateTimeOffset.UtcNow;
        /// <summary>
        /// The UTC date and time of last modification.
        /// </summary>
        public DateTimeOffset? Modified { get; set; }

        public IEnumerable<IApiLink> Links { get; set; }
    }
}
