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
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Identity
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private readonly Configuration.Options _configuration;
        private readonly IWebHostEnvironment HostEnvironment;
        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Configuration = configuration;
            _configuration = Configuration.Get<Configuration.Options>();
            var configfile = $@"/{environment.ContentRootPath}/active-configuration.json";
            System.IO.File.WriteAllText(configfile, JsonConvert.SerializeObject(_configuration));
            Log.Logger.Debug($"Logged configuration to {configfile}");
            HostEnvironment = environment;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRequestResponseLoggingMiddlewareWithOptions(options => 
            { 
                options.LogSource = _configuration.RequestResponse.Source; 
            });                        
            
            var builder = services.AddIdentityServer(options =>
            {
                // see https://identityserver4.readthedocs.io/en/latest/topics/resources.html
                options.EmitStaticAudienceClaim = true;
            });
            builder.AddClients()
                        .AddCorsPolicyService<InMemoryCorsPolicyService>() // Add the CORS service
                        .AddIdentityApiResources()
                        .AddPersistedGrants()
                        .AddMongoRepository(_configuration.Mongo.ConnectionString, _configuration.Mongo.DatabaseName)
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
            //.AddGoogle("Google", options =>
            //{
            //    options.SignInScheme = _configuration.Google.SignInScheme;
            //    options.ClientId = _configuration.Google.ClientId  ;
            //    options.ClientSecret = _configuration.Google.ClientSecret ;
            //})
            ;

            services.AddControllersWithViews()
                    .AddNewtonsoftJson(options =>
                    {
                        // Use the default property (Pascal) casing
                        options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
                        options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Include;
                        options.SerializerSettings.DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat;
                    })
                    ;
        }

        public void Configure(IApplicationBuilder app)
        {
            var allTypesWillIgnoreExtraElements = true;

            ConventionRegistry.Register("Ignore extra properties",
                new ConventionPack { new IgnoreExtraElementsConvention(true) },
                type => allTypesWillIgnoreExtraElements
                );


            app.UsePathBase(_configuration.Service.BasePath);

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
