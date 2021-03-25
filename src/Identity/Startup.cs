// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Services;
using Identity.Storage;
using Pluralizer;
using Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson.Serialization.Conventions;

namespace Identity
{
    public class Startup
    {
        private readonly IWebHostEnvironment HostEnvironment;
        public Startup(IWebHostEnvironment environment)
        {
            HostEnvironment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRequestResponseLoggingMiddlewareWithOptions(options => { options.LogSource = "Identity"; });

            var mongoDatabaseConnectionString = "mongodb://storage:storagepass@mongo.mystore.local:27017";
            var mongoDatabaseName = "mystoreIdentity";
            
            
            var builder = services.AddIdentityServer(options =>
            {
                // see https://identityserver4.readthedocs.io/en/latest/topics/resources.html
                options.EmitStaticAudienceClaim = true;
            });
            builder.AddClients()
                        .AddCorsPolicyService<InMemoryCorsPolicyService>() // Add the CORS service
                        .AddIdentityApiResources()
                        .AddPersistedGrants()
                        .AddMongoRepository(mongoDatabaseConnectionString, mongoDatabaseName)
                        .AddProfileService<UserProfileService>()
                        .AddTestUsers(SeedData.Users);


            if (HostEnvironment.IsDevelopment())
            {
                // not recommended for production - you need to store your key material somewhere secure
                builder.AddDeveloperSigningCredential();
            }
            else
            {
                throw new SecurityTokenEncryptionKeyNotFoundException();
            }

            services.AddAuthorization();

            services.AddSingleton<IPluralize, Pluralizer.Pluralizer>();

            services.AddSingleton<IUserStore, UserStore>();

            services.AddAuthentication()
            //// If Google Sign-on is needed 
            //// Install-Package Microsoft.AspNetCore.Authentication.Google 
            //// add the abive nuget package to the project
            //// then uncomment the following lines 
            //    .AddGoogle("Google", options =>
            //    {
            //        options.SignInScheme = "idsrv.external";
            //        options.ClientId = "<your google client id>";
            //        options.ClientSecret = "<your google client secret>";
            //    })
            ;

            services.AddControllersWithViews();            
        }

        public void Configure(IApplicationBuilder app)
        {
            var allTypesWillIgnoreExtraElements = true;

            ConventionRegistry.Register("Ignore extra properties",
                new ConventionPack { new IgnoreExtraElementsConvention(true) },
                type => allTypesWillIgnoreExtraElements
                );


            app.UsePathBase("/identity");

            app.InitializeDatabase();

            if (HostEnvironment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // turn off CORS at this point as IdentityServer witll handle it
            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

            app.UseStaticFiles();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseRouting();

            app.UseIdentityServer();            

            app.UseAuthorization();

            app.UseRequestResponseLogging();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
