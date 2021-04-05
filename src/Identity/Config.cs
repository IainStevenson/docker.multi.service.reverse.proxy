// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace Identity
{

    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),

            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("myInfo.Api", "My API")
            };

        public static IEnumerable<Client> Clients =>
            new List<Client>
            {
                new Client
                {
                    ClientId = "client",

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
                    ClientId = "mvc",
                    ClientSecrets = { new Secret("secret".Sha256()) },

                    AllowedGrantTypes = GrantTypes.Code,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    // where to redirect to after login
                    RedirectUris = {
                        "https://localhost/store/signin-oidc" ,
                        "https://localhost/support/signin-oidc" ,
                        "https://myInfo.local/signin-oidc" ,
                        "https://myInfo.local/store/signin-oidc" ,
                        "https://myInfo.local/support/signin-oidc"
                    },

                    // where to redirect to after logout
                    PostLogoutRedirectUris = {
                        "https://localhost/store/signout-callback-oidc",
                        "https://localhost/support/signout-callback-oidc",
                        "https://myInfo.local/signout-callback-oidc",
                        "https://myInfo.local/store/signout-callback-oidc",
                        "https://myInfo.local/support/signout-callback-oidc"
                    },

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "myInfo.Api"
                    }
                }
            };
    }
}