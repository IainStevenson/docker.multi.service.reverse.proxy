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
            new ApiResource("myInfo.Api", "myInfo API") {
                ApiSecrets = { new Secret("secret".Sha256()) { } },
                UserClaims = {
                    JwtClaimTypes.Name,
                    JwtClaimTypes.Email
                },
                ShowInDiscoveryDocument = true,
                Scopes = {
                    "myInfo.Api"
                }
            }
        };

        public static IEnumerable<ApiScope> ApiScopes =>
        new List<ApiScope>
        {
            new ApiScope(
                "myInfo.Api",
                "myInfo API") {
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
                AllowedScopes = { "myInfo.Api" }
            },
            // interactive ASP.NET Core MVC client
            new Client
                {
                    ClientId = "myInfo.Mvc",
                    ClientSecrets = { new Secret("secret".Sha256()) },

                    AllowedGrantTypes = GrantTypes.Code,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    // where to redirect to after login
                    RedirectUris = {
                        "https://local.myinfo.world/signin-oidc" ,
                        "https://local.myinfo.world/store/signin-oidc" ,
                        "https://local.myinfo.world/support/signin-oidc"
                    },

                    // where to redirect to after logout
                    PostLogoutRedirectUris = {
                        "https://local.myinfo.world/signout-callback-oidc",
                        "https://local.myinfo.world/store/signout-callback-oidc",
                        "https://local.myinfo.world/support/signout-callback-oidc"
                    },

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "myInfo.Api"
                    }
                },
            //new Client
            //{
            //    ClientId = "myInfo.Mvc",
            //    ClientName = "myInfo Portal client",
            //    ClientSecrets = { new Secret("secret".Sha256()) },

            //    AllowedGrantTypes = GrantTypes.Hybrid,
            //    RequireConsent = false,
            //    RequirePkce = false,

            //    // where to redirect to after login/ out
            //    RedirectUris = {    "https://local.myinfo.world/store/signin-oidc",
            //                        "https://local.myinfo.world/support/signin-oidc"},
            //    FrontChannelLogoutUri = "https://local.myinfo.world/signin-oidc",

            //    AllowedScopes = new List<string>
            //    {
            //        IdentityServerConstants.StandardScopes.OpenId,
            //        IdentityServerConstants.StandardScopes.Profile,
            //        IdentityServerConstants.StandardScopes.Email,
            //        "myInfo.Api"
            //    },
            //    AllowedCorsOrigins = new[] { "myInfo.local"},

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

                RedirectUris =           { "https://local.myinfo.world/subject/callback.html" },
                PostLogoutRedirectUris = { "https://local.myinfo.world/subject/index.html" },
                AllowedCorsOrigins =     { "https://local.myinfo.world" },

                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Email,
                    IdentityServerConstants.StandardScopes.Profile,
                    "myInfo.Api"
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
                    "myInfo.Api"
                },
                ClientSecrets = new [] { new Secret("secret".Sha256())},
                Enabled = true,

            }
        };

        public static List<TestUser> Users => new List<TestUser>
        {
            new TestUser() { Username = "alice", Password  = "alice", SubjectId = Guid.NewGuid().ToString(),
                Claims = new List<Claim>(){ new Claim("email", "sdk.alice@myInfo.local") } },
            new TestUser() { Username = "bob", Password  = "bob", SubjectId = Guid.NewGuid().ToString(),
                Claims = new List<Claim>(){ new Claim("email", "sdk.bob@myInfo.local") } }
        };
    }

}
