using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Storage
{
    /// <summary>
    /// Basic interface with a few methods for adding, deleting, and querying data. NOTE: Updates not required at this time
    /// </summary>
    public interface IIdentityRepository
    {
        Task<IQueryable<T>> All<T>() where T : class, new();
        Task<IQueryable<T>> Where<T>(Expression<Func<T, bool>> expression) where T : class, new();
        Task<T> Single<T>(Expression<Func<T, bool>> expression) where T : class, new();
        Task<long> Delete<T>(Expression<Func<T, bool>> expression) where T : class, new();
        Task<T> Add<T>(T item) where T : class, new();
        Task<IEnumerable<T>> Add<T>(IEnumerable<T> items) where T : class, new();
        Task<long> CollectionCount<T>() where T : class, new();
        Task<T> Update<T>(Expression<Func<T, bool>> filter, T item) where T : class, new();
    }
}