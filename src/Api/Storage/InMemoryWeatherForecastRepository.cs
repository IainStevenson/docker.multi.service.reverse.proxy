using Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Storage
{
    public class InMemoryWeatherForecastRepository : IRepository<WeatherForecast>
    {
        private readonly Dictionary<Guid, WeatherForecast> _items;
        public InMemoryWeatherForecastRepository(Dictionary<Guid, WeatherForecast> items)
        {
            _items = items;
        }

        public Task<bool> Discard(WeatherForecast item)
        {            
            if (_items.ContainsKey(item.Id))
            {
                if (_items[item.Id].ETag == item.ETag)
                {
                    _items.Remove(item.Id);
                    return Task.FromResult(true);
                }
                return Task.FromResult(false);
            };
            return Task.FromResult(false);
        }

        public Task<bool> Discard(Guid id)
        {
            if (_items.ContainsKey(id))
            {
                _items.Remove(id);
                return Task.FromResult(true);
            };
            return Task.FromResult(false);
        }

        public Task<WeatherForecast> Retrieve(Guid Id)
        {
           return Task.FromResult(_items[Id]);
        }

        public Task<IEnumerable<WeatherForecast>> Retrieve(Func<WeatherForecast, bool> query)
        {
            return Task.FromResult(_items.Values.Where(query));
        }

        public Task<WeatherForecast> Store(WeatherForecast item)
        {
            if (_items.ContainsKey(item.Id))
            {
                if (_items[item.Id].ETag == item.ETag)
                {
                    item.ETag = Guid.NewGuid().ToString();
                    item.Modified = DateTimeOffset.UtcNow;
                    _items[item.Id] = item;
                    return Task.FromResult(item);
                }
                return Task.FromResult(_items[item.Id]);
            }
            else
            {
                item.ETag = Guid.NewGuid().ToString();
                _items.Add(item.Id, item);
            }            
            return Task.FromResult(item);
        }
    }

}
