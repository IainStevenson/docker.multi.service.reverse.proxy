using IdentityModel;
using Storage;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Identity.Storage
{
    public class UserStore : IUserStore
    {
        private readonly IIdentityRepository _repository;
        public UserStore(IIdentityRepository repository)
        {
            
            _repository = repository;
        }

        public async Task<bool> ValidateCredentials(string username, string password)
        {

            var user = await _repository.Single<AppUser>( x=>x.Username == username );
            if (user == null) return false;
            var salt = user.PasswordSalt;
            var hash = user.PasswordHash;
            return !string.IsNullOrEmpty(salt) 
                && !string.IsNullOrEmpty(hash) 
                && AppUser.PasswordValidation(hash, salt, password);
        }

        public async Task<AppUser> FindBySubjectId(string subjectId)
        {
            var user = await _repository.Single<AppUser>(x => x.SubjectId == subjectId);
            return user;
        }

        public async Task<AppUser> FindByUsername(string username)
        {
            var user = await _repository.Single<AppUser>(x => x.Username == username);

            return user;
        }

        public async Task<AppUser> FindByExternalProvider(string provider, string subjectId)
        {
            var user = await _repository.Single<AppUser>(x => x.ProviderSubjectId == subjectId && x.ProviderName == provider);
            return user;
        }

      
        public async Task<AppUser> AutoProvisionUser(string provider, string subjectId, List<Claim> claims)
        {
            // create a list of claims that we want to transfer into our store
            var filtered = new List<Claim>();

            foreach (var claim in claims)
            {
                // if the external system sends a display name - translate that to the standard OIDC name claim
                if (claim.Type == ClaimTypes.Name)
                {
                    filtered.Add(new Claim(JwtClaimTypes.Name, claim.Value));
                }
                // if the JWT handler has an outbound mapping to an OIDC claim use that
                else if (JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.ContainsKey(claim.Type))
                {
                    filtered.Add(new Claim(JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap[claim.Type], claim.Value));
                }
                // copy the claim as-is
                else
                {
                    filtered.Add(claim);
                }
            }
            var ownerId = Guid.NewGuid().ToString();
            var first = filtered.FirstOrDefault(x => x.Type == JwtClaimTypes.GivenName)?.Value;
            var last = filtered.FirstOrDefault(x => x.Type == JwtClaimTypes.FamilyName)?.Value;
            var userName = $"{first}.{last}";
            if (userName == ".") userName = filtered.FirstOrDefault(c => c.Type == JwtClaimTypes.Name)?.Value ?? subjectId;

            // if no display name was provided, try to construct by first and/or last name
            if (!filtered.Any(x => x.Type == JwtClaimTypes.Name))
            {
                if (first != null && last != null)
                {
                    filtered.Add(new Claim(JwtClaimTypes.Name, first + " " + last));
                }
                else if (first != null)
                {
                    filtered.Add(new Claim(JwtClaimTypes.Name, first));
                }
                else if (last != null)
                {
                    filtered.Add(new Claim(JwtClaimTypes.Name, last));
                }

            }


            filtered.Add(new Claim("OwnerId", ownerId));

           
            // check if a display name is available, otherwise fallback to subject id
          


            // create new user
            var user = new AppUser
            {
                SubjectId = ownerId,
                Username = userName,
                ProviderName = provider,
                ProviderSubjectId = subjectId,
                Claims = filtered
            };

            // store it and give it back
            user = await _repository.Add(user);
            return user;
        }

        public async Task<bool> SaveAppUser(AppUser user, string newPasswordToHash = null)
        {
            bool success = true;
            if (!String.IsNullOrEmpty(newPasswordToHash))
            {
                user.PasswordSalt = AppUser.PasswordSaltInBase64();
                user.PasswordHash = AppUser.PasswordToHashBase64(newPasswordToHash, user.PasswordSalt);
            }
            try
            {
                user = await _repository.Update( x=>x.SubjectId == user.SubjectId , user);                
            }
            catch
            {
                success = false;
            }
            return success;
        }
    }
}
