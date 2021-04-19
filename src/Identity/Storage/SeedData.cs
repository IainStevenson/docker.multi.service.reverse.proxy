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
        public static string Domain { get; set; } = "localhost";
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
            new ApiResource("myApi", "myApi") {
                ApiSecrets = { new Secret("secret".Sha256()) { } },
                UserClaims = {
                    JwtClaimTypes.Name,
                    JwtClaimTypes.Email
                },
                ShowInDiscoveryDocument = true,
                Scopes = {
                    "myApi"
                }
            }
        };

        public static IEnumerable<ApiScope> ApiScopes =>
        new List<ApiScope>
        {
            new ApiScope(
                "myApi",
                "my API") {
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
                ClientId = "Command",
                ClientName = "Command Line client",
   
                // no interactive user, use the clientid/secret for authentication
                AllowedGrantTypes = GrantTypes.ClientCredentials,

                // secret for authentication
                ClientSecrets =
                {
                    new Secret("secret".Sha256())
                },

                // scopes that client has access to
                AllowedScopes = { "myApi" }
            },
            // interactive ASP.NET Core MVC client
            new Client
                {
                    ClientId = "Mvc",
                    ClientSecrets = { new Secret("secret".Sha256()) },

                    AllowedGrantTypes = GrantTypes.Code,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    // where to redirect to after login
                    RedirectUris = {
                        $"https://{Domain}/signin-oidc" ,
                        $"https://{Domain}/store/signin-oidc" ,
                        $"https://{Domain}/support/signin-oidc"
                    },

                    // where to redirect to after logout
                    PostLogoutRedirectUris = {
                        $"https://{Domain}/signout-callback-oidc",
                        $"https://{Domain}/store/signout-callback-oidc",
                        $"https://{Domain}/support/signout-callback-oidc"
                    },

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "myApi"
                    }
                },

            new Client
            {
                ClientId = "Javascript",
                ClientName = "JavaScript client",
                AllowedGrantTypes = GrantTypes.Code,
                RequirePkce = true,
                RequireClientSecret = false,

                RedirectUris =           { $"https://{Domain}/subject/callback.html" },
                PostLogoutRedirectUris = { $"https://{Domain}/subject/index.html" },
                AllowedCorsOrigins =     { $"https://{Domain}" },

                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Email,
                    IdentityServerConstants.StandardScopes.Profile,
                    "myApi"
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
                    "myApi"
                },
                ClientSecrets = new [] { new Secret("secret".Sha256())},
                Enabled = true,

            }
        };

        public static List<TestUser> Users => new List<TestUser>
        {
            new TestUser() { Username = "alice", Password  = "alice", SubjectId = Guid.NewGuid().ToString(),
                Claims = new List<Claim>(){ new Claim("email", "sdk.alice@local.myInfo.world") } },
            new TestUser() { Username = "bob", Password  = "bob", SubjectId = Guid.NewGuid().ToString(),
                Claims = new List<Claim>(){ new Claim("email", "sdk.bob@local.myInfo.world") } }
        };
    }

}
