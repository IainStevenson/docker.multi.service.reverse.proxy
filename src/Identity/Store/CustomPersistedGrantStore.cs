using IdentityServer4.Models;
using IdentityServer4.Stores;
using Storage;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.Store
{
    /// <summary>
    /// Handle consent decisions, authorization codes, refresh and reference tokens
    /// </summary>
    public class CustomPersistedGrantStore : IPersistedGrantStore
    {
        protected IIdentityRepository _dbRepository;

        public CustomPersistedGrantStore(IIdentityRepository repository)
        {
            _dbRepository = repository;
        }

        public Task<IEnumerable<PersistedGrant>> GetAllAsync(PersistedGrantFilter filter)
        {
            //IQueryable<PersistedGrant> result = new List<PersistedGrant>(); ;
            //if (!string.IsNullOrWhiteSpace(filter.SubjectId))
            //{
               var result = _dbRepository.Where<PersistedGrant>(i => 
                                    i.SubjectId == (filter.SubjectId?? i.SubjectId) &&
                                    i.SubjectId == (filter.ClientId ?? i.ClientId) &&
                                    i.SubjectId == (filter.Type ?? i.Type) 
                ).Result;
            //}

            return Task.FromResult(result.AsEnumerable());
        }

        public Task<PersistedGrant> GetAsync(string key)
        {
            var result = _dbRepository.Single<PersistedGrant>(i => i.Key == key).Result;
            return Task.FromResult(result);
        }

        public Task RemoveAllAsync(PersistedGrantFilter filter)
        {

            //if (!string.IsNullOrWhiteSpace(filter.SubjectId) &&
            //    !string.IsNullOrWhiteSpace(filter.ClientId) &&
            //    !string.IsNullOrWhiteSpace(filter.Type))
            //{
                _dbRepository.Delete<PersistedGrant>(
                       i =>
                                    i.SubjectId == (filter.SubjectId ?? i.SubjectId) &&
                                    i.SubjectId == (filter.ClientId ?? i.ClientId) &&
                                    i.SubjectId == (filter.Type ?? i.Type)
                                    );
            //}
            //else if (!string.IsNullOrWhiteSpace(filter.SubjectId) && !string.IsNullOrWhiteSpace(filter.ClientId))
            //{
            //    _dbRepository.Delete<PersistedGrant>(i => i.SubjectId == filter.SubjectId && i.ClientId == filter.ClientId);
            //}



            return Task.FromResult(0);


            //    return Task.FromResult(0);

        }

        public Task RemoveAsync(string key)
        {
            _dbRepository.Delete<PersistedGrant>(i => i.Key == key);
            return Task.FromResult(0);
        }

        public Task StoreAsync(PersistedGrant grant)
        {
            _dbRepository.Add(grant);
            return Task.FromResult(0);
        }


    }
}
