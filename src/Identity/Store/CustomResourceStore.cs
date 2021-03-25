using IdentityServer4.Models;
using IdentityServer4.Stores;
using Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.Store
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

        private IEnumerable<IdentityResource> GetAllIdentityResources()
        {
            return _dbRepository.All<IdentityResource>().Result;
        }

        private IEnumerable<ApiScope> GetAllApiScopes()
        {
            return _dbRepository.All<ApiScope>().Result;
        }
        public Task<Resources> GetAllResourcesAsync()
        {
            var result = new Resources(GetAllIdentityResources(), GetAllApiResources(), GetAllApiScopes());
            return Task.FromResult(result);
        }

        public Task<IEnumerable<ApiResource>> FindApiResourcesByNameAsync(IEnumerable<string> apiResourceNames)
        {
            var list = _dbRepository.Where<ApiResource>(a => apiResourceNames.Contains(a.Name)).Result;

            return Task.FromResult(list.AsEnumerable());
        }

        public Task<IEnumerable<ApiResource>> FindApiResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            var list = _dbRepository.Where<ApiResource>(a => a.Scopes.Any( b=> scopeNames.Contains(b))).Result;

            return Task.FromResult(list.AsEnumerable());
        }

        public Task<IEnumerable<ApiScope>> FindApiScopesByNameAsync(IEnumerable<string> scopeNames)
        {
            var list = _dbRepository.Where<ApiScope>(a => scopeNames.Contains(a.Name)).Result;

            return Task.FromResult(list.AsEnumerable());
        }

        public Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            var list = _dbRepository.Where<IdentityResource>(a => scopeNames.Contains(a.Name)).Result;

            return Task.FromResult(list.AsEnumerable());
        }

       
    }
}
