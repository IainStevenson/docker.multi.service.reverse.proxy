using IdentityServer4.Models;
using Storage;
using System.Threading.Tasks;

namespace Identity.Storage
{
    public class CustomClientStore : IdentityServer4.Stores.IClientStore
    {
        protected IIdentityRepository _dbRepository;

        public CustomClientStore(IIdentityRepository repository)
        {
            _dbRepository = repository;
        }

        public async Task<Client> FindClientByIdAsync(string clientId)
        {
            var client = await _dbRepository.Single<Client>(c => c.ClientId == clientId);

            return client;
        }
    }
}
