using IdentityServer4.Models;
using IdentityServer4.Stores;
using Storage;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.Storage
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

        public async Task<IEnumerable<PersistedGrant>> GetAllAsync(PersistedGrantFilter filter)
        {
            var result = await _dbRepository.Where<PersistedGrant>(i => i.SubjectId == filter.SubjectId);
            return result.AsEnumerable();
        }

        public async Task<PersistedGrant> GetAsync(string key)
        {
            var result = await _dbRepository.Single<PersistedGrant>(i => i.Key == key);
            return result;
        }

        public async Task RemoveAllAsync(string subjectId, string clientId)
        {
            await _dbRepository.Delete<PersistedGrant>(i => i.SubjectId == subjectId && i.ClientId == clientId);

        }

        public async Task RemoveAllAsync(string subjectId, string clientId, string type)
        {
            await _dbRepository.Delete<PersistedGrant>(i => i.SubjectId == subjectId && i.ClientId == clientId && i.Type == type);
        }

        public async Task RemoveAllAsync(PersistedGrantFilter filter)
        {
            await _dbRepository.Delete<PersistedGrant>(i => i.SubjectId == filter.SubjectId && i.ClientId == filter.ClientId && i.Type == filter.Type);
        }

        public async Task RemoveAsync(string key)
        {
            await _dbRepository.Delete<PersistedGrant>(i => i.Key == key);

        }

        public async Task StoreAsync(PersistedGrant grant)
        {
            await _dbRepository.Add(grant);

        }
    }
}
