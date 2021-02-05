using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Storage
{
    /// <summary>
    /// Abstracts the storage and retrievel operations to persist addressible information for a client.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRepository<T> where T : IStorageItem
    {
        /// <summary>
        /// Create or udpate an item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        Task<T> Store(T item);
        /// <summary>
        /// Delete an item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        Task<bool> Discard(T item);
        /// <summary>
        /// Delete an item
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> Discard(Guid id);
        /// <summary>
        /// Retrieve an item
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        Task<T> Retrieve(Guid Id);
        /// <summary>
        /// Retrieve items
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> Retrieve(Func<T, bool> query);
    }
}

