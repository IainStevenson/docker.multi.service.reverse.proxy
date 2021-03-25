using IdentityServer4.Models;
using Storage;
using System.Threading.Tasks;

namespace Identity.Store
{
    public class CustomClientStore : IdentityServer4.Stores.IClientStore
    {
        protected IIdentityRepository _dbRepository;

        public CustomClientStore(IIdentityRepository repository)
        {
            _dbRepository = repository;
        }

        public Task<Client> FindClientByIdAsync(string clientId)
        {
            var client = _dbRepository.Single<Client>(c => c.ClientId == clientId).Result;

            return Task.FromResult(client);
        }
    }
}
