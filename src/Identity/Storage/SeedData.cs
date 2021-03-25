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
            new ApiResource("myStore.Api", "myStore API") {
                ApiSecrets = { new Secret("secret".Sha256()) { } },
                UserClaims = {
                    JwtClaimTypes.Name,
                    JwtClaimTypes.Email
                },
                ShowInDiscoveryDocument = true,
                Scopes = {
                    "myStore.Api"
                }
            }
        };

        public static IEnumerable<ApiScope> ApiScopes =>
        new List<ApiScope>
        {
            new ApiScope(
                "myStore.Api",
                "myStore API") {
                ShowInDiscoveryDocument = true,
                UserClaims = {
                    JwtClaimTypes.Name,
                    JwtClaimTypes.Email
                }
            }
        };



        public static IEnumerable<Client> Clients => new List<Client>
        {
            // command line client
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
                AllowedScopes = { "myStore.Api" }
            },
            // interactive ASP.NET Core MVC client
            new Client
                {
                    ClientId = "myStore.Mvc",
                    ClientSecrets = { new Secret("secret".Sha256()) },

                    AllowedGrantTypes = GrantTypes.Code,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    // where to redirect to after login
                    RedirectUris = {
                        "https://mystore.local/signin-oidc" ,
                        "https://mystore.local/store/signin-oidc" ,
                        "https://mystore.local/support/signin-oidc"
                    },

                    // where to redirect to after logout
                    PostLogoutRedirectUris = {
                        "https://mystore.local/signout-callback-oidc",
                        "https://mystore.local/store/signout-callback-oidc",
                        "https://mystore.local/support/signout-callback-oidc"
                    },

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "myStore.Api"
                    }
                },
            //new Client
            //{
            //    ClientId = "myStore.Mvc",
            //    ClientName = "myStore Portal client",
            //    ClientSecrets = { new Secret("secret".Sha256()) },

            //    AllowedGrantTypes = GrantTypes.Hybrid,
            //    RequireConsent = false,
            //    RequirePkce = false,

            //    // where to redirect to after login/ out
            //    RedirectUris = {    "https://myStore.local/store/signin-oidc",
            //                        "https://myStore.local/support/signin-oidc"},
            //    FrontChannelLogoutUri = "https://myStore.local/signin-oidc",

            //    AllowedScopes = new List<string>
            //    {
            //        IdentityServerConstants.StandardScopes.OpenId,
            //        IdentityServerConstants.StandardScopes.Profile,
            //        IdentityServerConstants.StandardScopes.Email,
            //        "myStore.Api"
            //    },
            //    AllowedCorsOrigins = new[] { "myStore.local"},

            //    AllowOfflineAccess = true
            //},
            // JavaScript Client
            new Client
            {
                ClientId = "SDK.Javascript",
                ClientName = "SDK sample JavaScript client",
                AllowedGrantTypes = GrantTypes.Code,
                RequirePkce = true,
                RequireClientSecret = false,

                RedirectUris =           { "http://myStore.local/subject/callback.html" },
                PostLogoutRedirectUris = { "http://myStore.local/subject/index.html" },
                AllowedCorsOrigins =     { "http://myStore.local" },

                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Email,
                    IdentityServerConstants.StandardScopes.Profile,
                    "myStore.Api"
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
                    "myStore.Api"
                },
                ClientSecrets = new [] { new Secret("secret".Sha256())},
                Enabled = true,

            }
        };

        public static List<TestUser> Users => new List<TestUser>
        {
            new TestUser() { Username = "alice", Password  = "alice", SubjectId = Guid.NewGuid().ToString(),
                Claims = new List<Claim>(){ new Claim("email", "sdk.alice@myStore.local") } },
            new TestUser() { Username = "bob", Password  = "bob", SubjectId = Guid.NewGuid().ToString(),
                Claims = new List<Claim>(){ new Claim("email", "sdk.bob@myStore.local") } }
        };
    }

}
