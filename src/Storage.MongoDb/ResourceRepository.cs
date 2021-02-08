using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CSharpVitamins;
using Data.Model.Storage;
using Pluralizer;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Storage.MongoDb
{
    /// <summary>
    ///     Implements <see cref="IStorageContent{T}" /> Content Storage for a the <see cref="Resource" /> type.
   /// </summary>
    public class ResourceRepository : IRepository<Resource>
    {
        private readonly bool? _byPassDocumentValidation = true;
        private readonly Collation _collation = Collation.Simple;
        private readonly ILogger<ResourceRepository> _logger;
        private readonly IMongoCollection<Resource> _mongoCollection;
        private readonly IPluralize _pluralizer;
        public ResourceRepository(IMongoClient client, ILogger<ResourceRepository> logger, IPluralize pluralizer, string databaseName)
        {
            _logger = logger;
            _pluralizer = pluralizer;
            var mongoDatabase = client.GetDatabase(databaseName);
            var collectionName = _pluralizer.Pluralize(typeof(Resource).Name).ToLower();
            _mongoCollection = mongoDatabase.GetCollection<Resource>(collectionName);
        }

        public async Task<IEnumerable<Resource>> GetAsync(Guid ownerId, string namespaceFilter, Guid? resourceIdFilter,
            string orderBy, int skip = 0, int take = int.MaxValue)
        {
            var sort = ApplySort<Resource>(orderBy);

            var builder = Builders<Resource>.Filter;

            var filter = builder.Eq(r => r.OwnerId, ownerId); // ensure its from the specific owner context

            // now add resource type specification if supplied
            if (namespaceFilter != null)
            {
                var regExFilter = new BsonRegularExpression(namespaceFilter, "i");
                filter &= builder.Regex(r => r.Namespace, regExFilter);
            }

            // now add resource specification if supplied
            if (resourceIdFilter.HasValue)
                filter = builder.Eq(r => r.Id, resourceIdFilter);

            var result = _mongoCollection.Find(filter)
                .Sort(sort) // apply sorting
                .Skip(skip) // apply paging start
                .Limit(take); // apply paging size

            return await result.ToListAsync();
        }
        public async Task<IEnumerable<Resource>> GetAsync(Expression<Func<Resource, bool>> filter, string orderBy,
            int skip = 0, int take = int.MaxValue)
        {
            var sort = ApplySort<Resource>(orderBy);

            var result = _mongoCollection.Find(filter)
                .Sort(sort)
                .Skip(skip)
                .Limit(take);

            return await result.ToListAsync();
        }

        public async Task<IEnumerable<Resource>> GetAsync(Expression<Func<Resource, bool>> query)
        {
            FilterDefinition<Resource> filter = new ExpressionFilterDefinition<Resource>(query);
            return await _mongoCollection.Find(filter).ToListAsync();
        }

        public async Task<IEnumerable<Resource>> GetAsync(IEnumerable<Guid> ids)
        {
            FilterDefinition<Resource> filter = new ExpressionFilterDefinition<Resource>(i => ids.Contains(i.Id));
            return (await _mongoCollection.FindAsync(filter)).ToList();
        }

        public async Task<Resource> GetAsync(Guid id)
        {
            FilterDefinition<Resource> filter = new ExpressionFilterDefinition<Resource>(i => i.Id == id);
            return await _mongoCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<Resource> CreateAsync(Resource item)
        {
            item.Etag = $"{(ShortGuid)Guid.NewGuid()}";
            await _mongoCollection.InsertOneAsync(item, new InsertOneOptions
            {
                BypassDocumentValidation = _byPassDocumentValidation
            });

            return item;
        }
       public async Task<IEnumerable<Resource>> CreateAsync(IEnumerable<Resource> items)
        {
            var index = 0;
            foreach (var item in items)
            {
                item.Etag = $"{(ShortGuid)Guid.NewGuid()}";
                item.Metadata.Index = index++;
            }
            await _mongoCollection.InsertManyAsync(items,
                new InsertManyOptions { BypassDocumentValidation = _byPassDocumentValidation, IsOrdered = false });
            return items;
        }

        public async Task<long> DeleteAsync(Expression<Func<Resource, bool>> query)
        {
            var result = await _mongoCollection.DeleteManyAsync(query, new DeleteOptions { Collation = _collation });
            return result.IsAcknowledged ? result.DeletedCount : 0;
        }

        public Task<long> DeleteAsync(Guid id)
        {
            var result = _mongoCollection.DeleteOne(i => i.Id == id, new DeleteOptions { Collation = _collation });
            return Task.FromResult(result.IsAcknowledged ? result.DeletedCount : 0);
        }

        public async Task<long> DeleteAsync(IEnumerable<Guid> ids)
        {
            var result = await _mongoCollection.DeleteManyAsync(item => ids.Contains(item.Id),
                new DeleteOptions { Collation = _collation });
            return result.IsAcknowledged ? result.DeletedCount : 0;
        }

        public async Task<long> DeleteAsync(Resource item)
        {
            var result =
                await _mongoCollection.DeleteOneAsync(i => i.Id == item.Id, new DeleteOptions { Collation = _collation });
            return result.IsAcknowledged ? result.DeletedCount : 0;
        }


        public async Task<Resource> UpdateAsync(Resource item)
        {
            item = await UpdateItemProperties(item);
            var result = await _mongoCollection.ReplaceOneAsync(resource => resource.Id == item.Id,
                item);
            return item;
        }

        private Task<Resource> UpdateItemProperties(Resource item)
        {
            item.Modified = DateTimeOffset.UtcNow;
            item.Etag = $"{(ShortGuid)Guid.NewGuid()}";
            if (item.Metadata.Tags.ContainsKey("Modified"))
            {
                ((List<DateTimeOffset>)item.Metadata.Tags["Modified"]).Add(item.Modified.Value);
            }
            else
            {
                item.Metadata.Tags.Add("Modified", new List<DateTimeOffset>() { item.Modified.Value });
            }
            return Task.FromResult(item);

        }
        public async Task<IEnumerable<Resource>> UpdateAsync(IEnumerable<Resource> items)
        {

            foreach (var item in items)
            {
                //TODO: Investigate use of UpdateManyAsync
                await _mongoCollection.ReplaceOneAsync(resource => resource.Id == item.Id,
                    await UpdateItemProperties(item));
            }
            return items;
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        public SortDefinition<T> ApplySort<T>(string orderBy) where T : IStorageItem
        {
            var builder = Builders<T>.Sort;

            SortDefinition<T> result = null;
            if (string.IsNullOrWhiteSpace(orderBy) || string.IsNullOrEmpty(orderBy))
            {
                result = builder.Descending(r => r.Created);
                return result;
            }

            var orderByClauses = orderBy.Split(',').Select(o => o.Trim().ToLower());
            var first = true;
            foreach (var orderByClause in orderByClauses)
            {
                var clause = orderByClause.ToLower();
                var direction = "asc";

                var clauses = orderByClause.Split(' ');

                if (clauses.Length > 1)
                {
                    clause = clauses.First().ToLower();
                    direction = clauses.Last().Trim().ToLower();
                }

                var isAsc = direction.StartsWith("asc");

                switch (clause)
                {
                    case "created":
                        if (isAsc)
                            result = first ? builder.Ascending(r => r.Created) : result.Ascending(s => s.Created);
                        else
                            result = first ? builder.Descending(r => r.Created) : result.Descending(s => s.Created);
                        break;
                    case "index":
                        if (isAsc)
                            result = first ? builder.Ascending(r => r.Metadata.Index) : result.Ascending(s => s.Metadata.Index);
                        else
                            result = first ? builder.Descending(r => r.Metadata.Index) : result.Descending(s => s.Metadata.Index);
                        break;
                }
                first = false;
            }
            return result;
        }

    }
}