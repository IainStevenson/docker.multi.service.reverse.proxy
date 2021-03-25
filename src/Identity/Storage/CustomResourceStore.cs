using IdentityServer4.Models;
using IdentityServer4.Stores;
using Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.Storage
{
    public class CustomResourceStore : IResourceStore
    {
        protected IIdentityRepository _dbRepository;

        public CustomResourceStore(IIdentityRepository repository)
        {
            _dbRepository = repository;
        }

        private IEnumerable<ApiResource> GetAllApiResources()
        {
            return _dbRepository.All<ApiResource>().Result;
        }

        private  IEnumerable<IdentityResource> GetAllIdentityResources()
        {
            return _dbRepository.All<IdentityResource>().Result;
        }

        private IEnumerable<ApiScope> GetAllApiScopes()
        {
            return _dbRepository.All<ApiScope>().Result;
        }

        public async Task<Resources> GetAllResourcesAsync()
        {
            var result = new Resources(GetAllIdentityResources(), GetAllApiResources(), GetAllApiScopes());
            return await Task.FromResult(result);
        }

        public async Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            var list = await _dbRepository.Where<IdentityResource>(e => scopeNames.Contains(e.Name));
            return list.AsEnumerable();
        }

        public async Task<IEnumerable<ApiScope>> FindApiScopesByNameAsync(IEnumerable<string> scopeNames)
        {
            var list = await _dbRepository.Where<ApiScope>(e => scopeNames.Contains(e.Name));
            var result = list.ToList();
            return result;
        }

        public async Task<IEnumerable<ApiResource>> FindApiResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            var list = await _dbRepository.Where<ApiResource>(e => e.Scopes.Any( f => scopeNames.Contains(f)) );
            return list.AsEnumerable();
        }

        public async Task<IEnumerable<ApiResource>> FindApiResourcesByNameAsync(IEnumerable<string> apiResourceNames)
        {

            var list = await _dbRepository.Where<ApiResource>(e => apiResourceNames.Contains(e.Name));
            return list.AsEnumerable();
        }
    }
}
