using Data.Model.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Storage
{
    /// <summary>
    /// Provides a simple in memory dictinonary based storage repository for <see cref="WeatherForecast"/> instances.
    /// </summary>
    public class InMemoryResourceRepository : IRepository<Resource>
    {
        private readonly Dictionary<Guid, Resource> _storage;
        public InMemoryResourceRepository(Dictionary<Guid, Resource> storage)
        {
            _storage = storage;
        }

        public Task<Resource> CreateAsync(Resource item, CancellationToken cancellationToken)
        {
            item.Etag = Guid.NewGuid().ToString();
            item.Created = DateTimeOffset.UtcNow;
            _storage.Add(item.Id, item);
            return Task.FromResult(item);
        }

        public Task<IEnumerable<Resource>> CreateAsync(IEnumerable<Resource> items, CancellationToken cancellationToken)
        {
            foreach (var item in items)
            {
                item.Etag = Guid.NewGuid().ToString();
                item.Created = DateTimeOffset.UtcNow;
                _storage.Add(item.Id, item);
            }
            return Task.FromResult(items);
        }

        public Task<long> DeleteAsync(Expression<Func<Resource, bool>> query, CancellationToken cancellationToken)
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

        public Task<long> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            if (_storage.ContainsKey(id))
            {
                _storage.Remove(id);
                return Task.FromResult(1L);
            }
            return Task.FromResult(0L);
        }

        public Task<long> DeleteAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken)
        {
            long count = 0;
            foreach (var id in ids)
            {
                _storage.Remove(id);
                count++;
            }
            return Task.FromResult(count);
        }

        public Task<IEnumerable<Resource>> GetAsync(Expression<Func<Resource, bool>> query, string orderBy, CancellationToken cancellationToke, int skip = 0, int take = int.MaxValue)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Resource>> GetAsync(Guid ownerId, string namespaceFilter, Guid? resourceIdFilter, string orderBy, CancellationToken cancellationToken, int skip = 0, int take = int.MaxValue )
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Resource>> GetAsync(Expression<Func<Resource, bool>> query, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Resource>> GetAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Resource> GetAsync(Guid id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Resource> UpdateAsync(Resource item, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Resource>> UpdateAsync(IEnumerable<Resource> items, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

    }
}