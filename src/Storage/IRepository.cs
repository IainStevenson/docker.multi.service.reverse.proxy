using Data.Model.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
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
        Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> query, string orderBy, int skip = 0,
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
            string orderBy, int skip = 0,
            int take = int.MaxValue);

        /// <summary>
        ///     Retrieve a collection of <see cref="Resource" /> items using the specified expression
        /// </summary>
        /// <param name="query">The retireval expression</param>
        /// <returns>A Collection of retrieved <see cref="Resource" /></returns>
        Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> query);

        /// <summary>
        ///     Retrieve a collection of <see cref="Resource" /> items using a collection of identifiers
        /// </summary>
        /// <param name="ids">The retireval identifiers</param>
        /// <returns>A Collection of retrieved <see cref="Resource" /></returns>
        Task<IEnumerable<T>> GetAsync(IEnumerable<Guid> ids);


        /// <summary>
        ///     REtrieve a single <see cref="Resource" /> by is Id value
        /// </summary>
        /// <param name="id">The <see cref="Resource" /> Id value</param>
        /// <returns>The existing Resource or Null</returns>
        Task<T> GetAsync(Guid id);

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
        Task<IEnumerable<T>> CreateAsync(IEnumerable<T> items);

        /// <summary>
        ///     Deletes none, one or more <see cref="Resource" /> items taht match the query
        /// </summary>
        /// <param name="query">The match expression</param>
        /// <returns>The number of deleted items</returns>
        Task<long> DeleteAsync(Expression<Func<T, bool>> query);

        /// <summary>
        ///     Deletes a single <see cref="Resource" /> by its Id value
        /// </summary>
        /// <param name="id">The <see cref="Resource" /> identifier value</param>
        /// <returns>The number of deleted items</returns>
        Task<long> DeleteAsync(Guid id);

        /// <summary>
        ///     Deletes a collection of <see cref="Resource" /> using a collection of  Id values
        /// </summary>
        /// <param name="ids">The collction of <see cref="Resource" /> identifier values</param>
        /// <returns>The number of deleted items</returns>
        Task<long> DeleteAsync(IEnumerable<Guid> ids);

        /// <summary>
        ///     Updates a single <see cref="Resource" /> Item
        /// </summary>
        /// <param name="item">The item to store</param>
        /// <remarks>The <see cref="Resource" /> is upserted</remarks>
        /// <returns>The item with an added Etag (void)</returns>
        Task<T> UpdateAsync(T item);

        /// <summary>
        ///     Updates one or more <see cref="Resource" /> items
        /// </summary>
        /// <param name="items">the collection of <see cref="Resource" /> items to store.</param>
        /// <remarks>The <see cref="Resource" /> items are all upserted</remarks>
        /// <returns>The item collection with each item with an added Etag</returns>
        Task<IEnumerable<T>> UpdateAsync(IEnumerable<T> items);
    }


    /// <summary>
    /// Provides a simple in memory dictinonary based storage repository for <see cref="WeatherForecast"/> instances.
    /// </summary>
    public class InMemoryResourceRepository : IRepository<Data.Model.Storage.Resource>
    {
        private readonly Dictionary<Guid, Data.Model.Storage.Resource> _storage;
        public InMemoryResourceRepository(Dictionary<Guid, Data.Model.Storage.Resource> storage)
        {
            _storage = storage;
        }

        public Task<Data.Model.Storage.Resource> CreateAsync(Data.Model.Storage.Resource item, CancellationToken cancellationToken)
        {
            item.Etag = Guid.NewGuid().ToString();
            item.Created = DateTimeOffset.UtcNow;
            _storage.Add(item.Id, item);
            return Task.FromResult(item);
        }

        public Task<IEnumerable<Data.Model.Storage.Resource>> CreateAsync(IEnumerable<Data.Model.Storage.Resource> items)
        {
            foreach (var item in items)
            {
                item.Etag = Guid.NewGuid().ToString();
                item.Created = DateTimeOffset.UtcNow;
                _storage.Add(item.Id, item);
            }
            return Task.FromResult(items);
        }

        public Task<long> DeleteAsync(Expression<Func<Data.Model.Storage.Resource, bool>> query)
        {
            var items = _storage.Values.AsQueryable().Where(query);
            long count = 0;
            foreach (var item in items)
            {
                _storage.Remove(item.Id);
                count++;
            }
            return Task.FromResult(count);
        }

        public Task<long> DeleteAsync(Guid id)
        {
            if (_storage.ContainsKey(id))
            {
                _storage.Remove(id);
                return Task.FromResult(1L);
            }
            return Task.FromResult(0L);
        }

        public Task<long> DeleteAsync(IEnumerable<Guid> ids)
        {
            long count = 0;
            foreach (var id in ids)
            {
                _storage.Remove(id);
                count++;
            }
            return Task.FromResult(count);
        }

        public Task<IEnumerable<Data.Model.Storage.Resource>> GetAsync(Expression<Func<Data.Model.Storage.Resource, bool>> query, string orderBy, int skip = 0, int take = int.MaxValue)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Data.Model.Storage.Resource>> GetAsync(Guid ownerId, string namespaceFilter, Guid? resourceIdFilter, string orderBy, int skip = 0, int take = int.MaxValue)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Data.Model.Storage.Resource>> GetAsync(Expression<Func<Data.Model.Storage.Resource, bool>> query)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Data.Model.Storage.Resource>> GetAsync(IEnumerable<Guid> ids)
        {
            throw new NotImplementedException();
        }

        public Task<Data.Model.Storage.Resource> GetAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Data.Model.Storage.Resource> UpdateAsync(Data.Model.Storage.Resource item)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Data.Model.Storage.Resource>> UpdateAsync(IEnumerable<Data.Model.Storage.Resource> items)
        {
            throw new NotImplementedException();
        }

    }
}