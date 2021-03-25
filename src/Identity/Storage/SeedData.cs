using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Security.Claims;
using System.Collections.Generic;
using IdentityModel;
using IdentityServer4;
using System;

namespace Identity.Storage
{
    public static class SeedData
    {
        public static IEnumerable<IdentityResource> Ids =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
            };

        public static IEnumerable<ApiResource> Apis =>
        new List<ApiResource>
        {
            new ApiResource("miConsent.Api", "miConsent API") {
                ApiSecrets = { new Secret("secret".Sha256()) { } },
                UserClaims = {
                    JwtClaimTypes.Name,
                    JwtClaimTypes.Email
                },
                ShowInDiscoveryDocument = true,
                Scopes = {
                    "miConsent.Api"
                }
            }
        };

        public static IEnumerable<ApiScope> ApiScopes =>
        new List<ApiScope>
        {
            new ApiScope(
                "miConsent.Api",
                "miConsent API") {
                ShowInDiscoveryDocument = true,
                UserClaims = {
                    JwtClaimTypes.Name,
                    JwtClaimTypes.Email
                }
            }
        };



        public static IEnumerable<Client> Clients => new List<Client>
        {
            new Client
            {
                ClientId = "SDK.Command",
                ClientName = "SDK sample Command Line client",
   
                // no interactive user, use the clientid/secret for authentication
                AllowedGrantTypes = GrantTypes.ClientCredentials,

                // secret for authentication
                ClientSecrets =
                {
                    new Secret("secret".Sha256())
                },

                // scopes that client has access to
                AllowedScopes = { "miConsent.Api" }
            },
            // interactive ASP.NET Core MVC client
            new Client
            {
                ClientId = "miConsent.Mvc",
                ClientName = "miConsent Portal client",
                ClientSecrets = { new Secret("secret".Sha256()) },

                AllowedGrantTypes = GrantTypes.Hybrid,
                RequireConsent = false,
                RequirePkce = false,

                // where to redirect to after login/ out
                RedirectUris = { "https://miconsent.com/portal/signin-oidc" },
                FrontChannelLogoutUri = "https://miconsent.com/portal/signin-oidc",

                AllowedScopes = new List<string>
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    "miConsent.Api"
                },
                AllowedCorsOrigins = new[] { "miconsent.com"},

                AllowOfflineAccess = true
            },
            // JavaScript Client
            new Client
            {
                ClientId = "SDK.Javascript",
                ClientName = "SDK sample JavaScript client",
                AllowedGrantTypes = GrantTypes.Code,
                RequirePkce = true,
                RequireClientSecret = false,

                RedirectUris =           { "http://miconsent.com/subject/callback.html" },
                PostLogoutRedirectUris = { "http://miconsent.com/subject/index.html" },
                AllowedCorsOrigins =     { "http://miconsent.com" },

                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Email,
                    IdentityServerConstants.StandardScopes.Profile,
                    "miConsent.Api"
                }
            },
            new Client{
                ClientId = "postman-api",
                ClientName = "Postman Test Client",
                AllowOfflineAccess = true,
                //AllowAccessTokensViaBrowser = true,
                //RequireConsent = false,
                RedirectUris = new [] {"https://oauth.pstmn.io/v1/callback" },
                PostLogoutRedirectUris = new [] { "https://oauth.pstmn.io/v1/signout-callback-oidc"},
                AllowedCorsOrigins= new [] { "https://oauth.pstmn.io/v1.com"},

                //EnableLocalLogin = true,
                AllowedScopes = new [] {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    "miConsent.Api"
                },
                ClientSecrets = new [] { new Secret("secret".Sha256())},
                Enabled = true,

            }
        };

        public static List<TestUser> Users => new List<TestUser>
        {
            new TestUser() { Username = "alice", Password  = "password", SubjectId = Guid.NewGuid().ToString(),
                Claims = new List<Claim>(){ new Claim("email", "sdk.alice@miConsent.com") } },
            new TestUser() { Username = "bob", Password  = "password", SubjectId = Guid.NewGuid().ToString(),
                Claims = new List<Claim>(){ new Claim("email", "sdk.bob@miConsent.com") } }
        };
    }

}
