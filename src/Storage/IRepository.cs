using Data.Model.Storage;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Storage
{
    public interface IRepository<T> where T : IStorageItem
    {
        /// <summary>
        ///     Retrieve a queryasble collection of <see cref="Resource" /> items using the specified expression
        /// </summary>
        /// <param name="query">The retireval expression</param>
        /// <param name="orderBy">An optional filtering order by</param>
        /// <param name="skip">An optional filtering skip value</param>
        /// <param name="take">An optional filtering take value</param>
        /// <returns>A Collection of retrieved <see cref="Resource" /></returns>
        Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> query, string orderBy, CancellationToken cancellationToken, int skip = 0,
            int take = int.MaxValue);

        /// <summary>
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="namespaceFilter"></param>
        /// <param name="resourceIdFilter"></param>
        /// <param name="orderBy"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> GetAsync(Guid ownerId, string namespaceFilter, Guid? resourceIdFilter,
            string orderBy, CancellationToken cancellationToken, int skip = 0,
            int take = int.MaxValue);

        /// <summary>
        ///     Retrieve a collection of <see cref="Resource" /> items using the specified expression
        /// </summary>
        /// <param name="query">The retireval expression</param>
        /// <returns>A Collection of retrieved <see cref="Resource" /></returns>
        Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> query, CancellationToken cancellationToken);

        /// <summary>
        ///     Retrieve a collection of <see cref="Resource" /> items using a collection of identifiers
        /// </summary>
        /// <param name="ids">The retireval identifiers</param>
        /// <returns>A Collection of retrieved <see cref="Resource" /></returns>
        Task<IEnumerable<T>> GetAsync(IEnumerable<Guid> idss, CancellationToken cancellationToken);


        /// <summary>
        ///     REtrieve a single <see cref="Resource" /> by is Id value
        /// </summary>
        /// <param name="id">The <see cref="Resource" /> Id value</param>
        /// <returns>The existing Resource or Null</returns>
        Task<T> GetAsync(Guid id, CancellationToken cancellationToken);

        /// <summary>
        ///     Stores a single <see cref="Resource" /> Item
        /// </summary>
        /// <param name="item">The item to store</param>
        /// <remarks>The <see cref="Resource" /> is upserted</remarks>
        /// <returns>The item with an added Etag (void)</returns>
        Task<T> CreateAsync(T item, CancellationToken cancellationToken);

        /// <summary>
        ///     Stores one or more <see cref="Resource" /> items
        /// </summary>
        /// <param name="items">the collection of <see cref="Resource" /> items to store.</param>
        /// <remarks>The <see cref="Resource" /> items are all upserted</remarks>
        /// <returns>The item collection with each item with an added Etag</returns>
        Task<IEnumerable<T>> CreateAsync(IEnumerable<T> items, CancellationToken cancellationToken);

        /// <summary>
        ///     Deletes none, one or more <see cref="Resource" /> items taht match the query
        /// </summary>
        /// <param name="query">The match expression</param>
        /// <returns>The number of deleted items</returns>
        Task<long> DeleteAsync(Expression<Func<T, bool>> query, CancellationToken cancellationToken);

        /// <summary>
        ///     Deletes a single <see cref="Resource" /> by its Id value
        /// </summary>
        /// <param name="id">The <see cref="Resource" /> identifier value</param>
        /// <returns>The number of deleted items</returns>
        Task<long> DeleteAsync(Guid id, CancellationToken cancellationToken);

        /// <summary>
        ///     Deletes a collection of <see cref="Resource" /> using a collection of  Id values
        /// </summary>
        /// <param name="ids">The collction of <see cref="Resource" /> identifier values</param>
        /// <returns>The number of deleted items</returns>
        Task<long> DeleteAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken);

        /// <summary>
        ///     Updates a single <see cref="Resource" /> Item
        /// </summary>
        /// <param name="item">The item to store</param>
        /// <remarks>The <see cref="Resource" /> is upserted</remarks>
        /// <returns>The item with an added Etag (void)</returns>
        Task<T> UpdateAsync(T items, CancellationToken cancellationToken);

        /// <summary>
        ///     Updates one or more <see cref="Resource" /> items
        /// </summary>
        /// <param name="items">the collection of <see cref="Resource" /> items to store.</param>
        /// <remarks>The <see cref="Resource" /> items are all upserted</remarks>
        /// <returns>The item collection with each item with an added Etag</returns>
        Task<IEnumerable<T>> UpdateAsync(IEnumerable<T> items, CancellationToken cancellationToken);
    }
}