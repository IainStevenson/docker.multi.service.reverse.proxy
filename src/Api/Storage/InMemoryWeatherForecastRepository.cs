using Storage.MongoDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Api.Storage
{
    /// <summary>
    /// Provides a simple in memory dictinonary based storage repository for <see cref="WeatherForecast"/> instances.
    /// </summary>
    public class InMemoryWeatherForecastRepository : IRepository<ItemStorageModel<WeatherForecastModel>>
    {
        private readonly Dictionary<Guid, ItemStorageModel<WeatherForecastModel>> _storage;
        public InMemoryWeatherForecastRepository(Dictionary<Guid, ItemStorageModel<WeatherForecastModel>> storage)
        {
            _storage = storage;
        }

        public Task<ItemStorageModel<WeatherForecastModel>> CreateAsync(ItemStorageModel<WeatherForecastModel> item)
        {
            item.Etag = Guid.NewGuid().ToString();
            item.Created = DateTimeOffset.UtcNow;
            _storage.Add(item.Id, item);
            return Task.FromResult(item);
        }

        public Task<IEnumerable<ItemStorageModel<WeatherForecastModel>>> CreateAsync(IEnumerable<ItemStorageModel<WeatherForecastModel>> items)
        {
            foreach(var item in items)
            {
                item.Etag = Guid.NewGuid().ToString();
                item.Created = DateTimeOffset.UtcNow;
                _storage.Add(item.Id, item);
            }
            return Task.FromResult(items);
        }

        public Task<long> DeleteAsync(Expression<Func<ItemStorageModel<WeatherForecastModel>, bool>> query)
        {
            var items = _storage.Values.AsQueryable().Where(query);
            long count = 0;
            foreach(var item in items)
            {
                _storage.Remove(item.Id);
                count ++;
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

        public Task<IEnumerable<ItemStorageModel<WeatherForecastModel>>> GetAsync(Expression<Func<ItemStorageModel<WeatherForecastModel>, bool>> query, string orderBy, int skip = 0, int take = int.MaxValue)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ItemStorageModel<WeatherForecastModel>>> GetAsync(Guid ownerId, string namespaceFilter, Guid? resourceIdFilter, string orderBy, int skip = 0, int take = int.MaxValue)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ItemStorageModel<WeatherForecastModel>>> GetAsync(Expression<Func<ItemStorageModel<WeatherForecastModel>, bool>> query)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ItemStorageModel<WeatherForecastModel>>> GetAsync(IEnumerable<Guid> ids)
        {
            throw new NotImplementedException();
        }

        public Task<ItemStorageModel<WeatherForecastModel>> GetAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<ItemStorageModel<WeatherForecastModel>> UpdateAsync(ItemStorageModel<WeatherForecastModel> item)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ItemStorageModel<WeatherForecastModel>>> UpdateAsync(IEnumerable<ItemStorageModel<WeatherForecastModel>> items)
        {
            throw new NotImplementedException();
        }

        //public Task<bool> Discard(WeatherForecast item)
        //{            
        //    if (_storage.ContainsKey(item.Id))
        //    {
        //        if (_storage[item.Id].Etag == item.Etag)
        //        {
        //            _storage.Remove(item.Id);
        //            return Task.FromResult(true);
        //        }
        //        return Task.FromResult(false);
        //    };
        //    return Task.FromResult(false);
        //}

        //public Task<bool> Discard(Guid id)
        //{
        //    if (_storage.ContainsKey(id))
        //    {
        //        _storage.Remove(id);
        //        return Task.FromResult(true);
        //    };
        //    return Task.FromResult(false);
        //}

        //public Task<WeatherForecast> Retrieve(Guid Id)
        //{
        //   return Task.FromResult(_storage[Id]);
        //}

        //public Task<IEnumerable<WeatherForecast>> Retrieve(Expression<Func<WeatherForecast, bool>> query)
        //{
        //    return Task.FromResult(_storage.Values.Where(query));
        //}

        //public Task<WeatherForecast> Store(WeatherForecast item)
        //{
        //    if (_storage.ContainsKey(item.Id))
        //    {
        //        if (_storage[item.Id].Etag == item.Etag)
        //        {
        //            item.Etag = Guid.NewGuid().ToString();
        //            item.Modified = DateTimeOffset.UtcNow;
        //            _storage[item.Id] = item;
        //            return Task.FromResult(item);
        //        }
        //        return Task.FromResult(_storage[item.Id]);
        //    }
        //    else
        //    {
        //        item.ETag = Guid.NewGuid().ToString();
        //        _storage.Add(item.Id, item);
        //    }            
        //    return Task.FromResult(item);
        //}
    }

}
