using CSharpVitamins;
using System;
using System.Collections.Generic;

namespace Data.Model.Response
{
    /// <summary>
    /// A client side wrapper around stored content.
    /// </summary>
    public class Resource : IResource, IResponseItem
    {
        public Resource()
        {
            Id = Guid.NewGuid();
            Created = DateTimeOffset.UtcNow;
            Etag = (ShortGuid)Guid.NewGuid().ToString();
        }
        /// <summary>
        /// The ETag of this vesion of the ressource used for cache control anc concurrency checking
        /// </summary>
        public string Etag { get; set; }
        /// <summary>
        /// The internal storage identifier this resource is addressed by
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// The HATEOS link information for this resource
        /// </summary>
        public List<IApiLink> Links { get; set; }
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
        public DateTimeOffset Created { get; set; }
        /// <summary>
        /// The UTC date and time of last modification.
        /// </summary>
        public DateTimeOffset? Modified { get; set; }
    }
}
