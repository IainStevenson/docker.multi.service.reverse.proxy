using Data.Model.Storage;
using System;

namespace Api
{
    /// <summary>
    /// Generic Storage Item
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ItemStorageModel<T> : IStorageItem where T: class
    {
        public Guid Id { get; set; } = Guid.NewGuid();       

        public Guid OwnerId { get; set; }
        public string Etag { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Modified { get; set; }
        public IStorageMetadata Metadata { get; set; }
        public T Item { get; set; }
    }
}
