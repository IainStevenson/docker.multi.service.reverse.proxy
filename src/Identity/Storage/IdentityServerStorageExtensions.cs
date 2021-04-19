using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.Extensions.DependencyInjection;
using Storage;
using Pluralizer;
using Microsoft.AspNetCore.Builder;
using IdentityServer4.Models;
using IdentityServer4.Test;
using MongoDB.Bson.Serialization;
using System.Security.Claims;
using Identity.Storage.MopngoDB;

namespace Identity.Storage
{
    public static class IdentityServerStorageExtensions
    {

        /// <summary>
        /// Configure ClientId / Secrets
        /// </summary>
        /// <param name="builder">The IdentityServer builder </param>
        /// <param name="configurationOption"></param>
        /// <returns></returns>
        public static IIdentityServerBuilder AddClients(this IIdentityServerBuilder builder)
        {
            builder.Services.AddTransient<IClientStore, CustomClientStore>();
            builder.Services.AddTransient<ICorsPolicyService, InMemoryCorsPolicyService>();

            return builder;
        }
        /// <summary>
        /// Configure API  &  Resources
        /// Note: Api's have also to be configured for clients as part of allowed scope for a given clientID 
        /// </summary>
        /// <param name="builder">The IdentityServer builder </param>
        /// <returns></returns>
        public static IIdentityServerBuilder AddIdentityApiResources(this IIdentityServerBuilder builder)
        {
            builder.Services.AddTransient<IResourceStore, CustomResourceStore>();

            return builder;
        }

        /// <summary>
        /// Configure Grants
        /// </summary>
        /// <param name="builder">The IdentityServer builder </param>
        /// <returns></returns>
        public static IIdentityServerBuilder AddPersistedGrants(this IIdentityServerBuilder builder)
        {
            builder.Services.AddSingleton<IPersistedGrantStore, CustomPersistedGrantStore>();

            return builder;
        }
        public static IIdentityServerBuilder AddMongoRepository(this IIdentityServerBuilder builder, string connectionString, string databaseName)
        {
            builder.Services.AddTransient<IIdentityRepository>(provider =>
            {
                return new IdentityRepository(connectionString, databaseName, provider.GetService<IPluralize>());
            });

            return builder;
        }

        /// <summary>
        /// Configure Classes to ignore Extra Elements (e.g. _Id) when deserializing
        /// As we are using "IdentityServer4.Models" we cannot add something like "[BsonIgnore]"
        /// </summary>
        private static void ConfigureMongoDriver2IgnoreExtraElements()
        {
        
            BsonClassMap.RegisterClassMap<Client>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });
            BsonClassMap.RegisterClassMap<IdentityResource>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });
            BsonClassMap.RegisterClassMap<ApiResource>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });

            BsonClassMap.RegisterClassMap<ApiScope>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });
            BsonClassMap.RegisterClassMap<PersistedGrant>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });

            BsonClassMap.RegisterClassMap<Claim>(cm =>
            {
                cm.SetIgnoreExtraElements(true);
                cm.MapMember(c => c.Issuer);
                cm.MapMember(c => c.OriginalIssuer);
                cm.MapMember(c => c.Properties);
                cm.MapMember(c => c.Subject);
                cm.MapMember(c => c.Type);
                cm.MapMember(c => c.Value);
                cm.MapMember(c => c.ValueType);
                cm.MapCreator(c => new Claim(c.Type, c.Value, c.ValueType, c.Issuer, c.OriginalIssuer, c.Subject));
            });
        }
        /// <summary>
        /// Initialise the database with the <see cref="SeedData"/> if its found to be missing data.
        /// </summary>
        /// <param name="app">The application builder</param>
        /// <returns></returns>
        public static IApplicationBuilder InitializeDatabase(this IApplicationBuilder app, string domain)
        {


            ConfigureMongoDriver2IgnoreExtraElements();

            SeedData.Domain = domain;

            var repository = app.ApplicationServices.GetService<IIdentityRepository>();

            if ((repository.CollectionCount<Client>().Result) == 0)
            {
                foreach (var item in SeedData.Clients)
                {
                    _ = repository.Add(item).GetAwaiter().GetResult();
                }
            }
            if ((repository.CollectionCount<ApiResource>().Result) == 0)
            {
                foreach (var item in SeedData.Apis)
                {
                    _ = repository.Add(item).GetAwaiter().GetResult();
                }
            }
            if ((repository.CollectionCount<ApiScope>().Result) == 0)
            {
                foreach (var item in SeedData.ApiScopes)
                {
                    _ = repository.Add(item).GetAwaiter().GetResult();
                }
            }
            if ((repository.CollectionCount<IdentityResource>().Result) == 0)
            {
                foreach (var item in SeedData.Ids)
                {
                    _ = repository.Add(item).GetAwaiter().GetResult();
                }
            }
            if ((repository.CollectionCount<TestUser>().Result) == 0)
            {
                foreach (var item in SeedData.Users)
                {
                    _ = repository.Add(item).GetAwaiter().GetResult();
                }
            }

            return app;
        }
    }

}
