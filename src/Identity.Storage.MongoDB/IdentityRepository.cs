using MongoDB.Bson;
using MongoDB.Driver;
using Pluralizer;
using Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Identity.Storage.MongoDB
{

    /// <summary>
    /// Provides functionality  to persist "IdentityServer4.Models" into a given MongoDB
    /// </summary>
    public class IdentityRepository : IIdentityRepository
    {
        private readonly IMongoClient _client;
        private readonly IMongoDatabase _database;
        private readonly IPluralize _plurarizer;

        /// <summary>
        /// Constructs the repository
        /// </summary>
        /// <param name="connectionString">The server connection stirng,. NOTE: Does not need the database segment</param>
        /// <param name="databaseName">The server database name</param>
        /// <param name="pluralizer">The collection name pluralizer from generic class type names</param>
        public IdentityRepository(string connectionString, string databaseName, IPluralize pluralizer)
        {
            _client = new MongoClient(connectionString);
            _database = _client.GetDatabase(databaseName);
            _plurarizer = pluralizer;
        }

        private IMongoCollection<T> GetCollection<T>()
        {


            var collectionName = _plurarizer.Pluralize(typeof(T).Name).ToLower();

            try
            {
                return _database.GetCollection<T>(collectionName);
            }
            catch
            {
                try
                {
                    _database.CreateCollection(collectionName);
                }
                catch (Exception)
                {

                    throw;
                }

            }
            finally
            {

            }
            return _database.GetCollection<T>(collectionName);
        }

        public async Task<IQueryable<T>> All<T>() where T : class, new()
        {
            return await Task.Run(() => {
                return GetCollection<T>().AsQueryable();
            });
        }

        public async Task<IQueryable<T>> Where<T>(Expression<Func<T, bool>> expression) where T : class, new()
        {
            return await Task.Run(() => {
                return GetCollection<T>().AsQueryable().Where(expression);
            });
        }

        public async Task<long> Delete<T>(Expression<Func<T, bool>> predicate) where T : class, new()
        {
            var result = await GetCollection<T>().DeleteManyAsync(predicate);
            return result.DeletedCount;

        }
        public async Task<T> Single<T>(Expression<Func<T, bool>> expression) where T : class, new()
        {
            return await Task.Run(() => {
                return GetCollection<T>().AsQueryable().Where(expression).SingleOrDefault();
            });
        }

        public async Task<long> CollectionCount<T>() where T : class, new()
        {
            return await GetCollection<T>().CountDocumentsAsync(new BsonDocument());

        }

        public async Task<T> Add<T>(T item) where T : class, new()
        {
            await GetCollection<T>().InsertOneAsync(item);
            return item;
        }

        public async Task<IEnumerable<T>> Add<T>(IEnumerable<T> items) where T : class, new()
        {
            await GetCollection<T>().InsertManyAsync(items);
            return items;
        }

        public async Task<T> Update<T>(Expression<Func<T, bool>> filter, T item) where T : class, new()
        {
            var result = await GetCollection<T>().ReplaceOneAsync(filter, item);
            if (result.ModifiedCount == 1)
            {
                return item;
            }
            return null;
        }

    }
}
