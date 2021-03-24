using AutoMapper;
using FluentValidation.AspNetCore;
using Logging;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Pluralizer;
using Response.Formater;
using Storage;
using System;
using System.Collections.Generic;
using System.IO;


namespace Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment HostEnvironment { get; }

        public Startup(IWebHostEnvironment env)
        {
            HostEnvironment = env;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var excludedHeaders = new List<string>() { "X-Powered-By" };

            if (HostEnvironment.IsDevelopment())
            {
                IdentityModelEventSource.ShowPII = true; //Add this line to expose better debugging when token validation fails

            }

            IMongoClient NewMongoClient(IServiceProvider services)
            {
                var connectionString = $"mongodb://storage:storagepass@mongo.mystore.local:27017";
                return new MongoClient(new MongoUrl(connectionString));
            }
            
            IRepository<Data.Model.Storage.Resource> NewResourceStorageClient(IServiceProvider services)
            {
                var databaseName = "myStoreData";
                return new Data.Model.Storage.MongoDB.ResourceRepository(
                            services.GetService<IMongoClient>(),
                            services.GetService<ILogger<Data.Model.Storage.MongoDB.ResourceRepository>>(),
                            services.GetService<IPluralize>(),
                            databaseName
                            );
            }
            IMapper mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Data.Model.Storage.Resource, Data.Model.Response.Resource>();
            }).CreateMapper();

            services.AddRequestResponseLoggingMiddlewareWithOptions(options => { options.LogSource = "mystore.api"; });
            services.AddHttpClient("");
            services.AddScoped(NewMongoClient);
            services.AddScoped(NewResourceStorageClient);
            services.AddCors();
            services.AddDistributedMemoryCache();
            services.AddSingleton<IRequestHeadersProvider>((services) => new RequestHeadersProvider());
            services.AddSingleton<IResponseHeadersProvider>((services) => new ResponseHeadersProvider(excludedHeaders));
            services.AddSingleton<IPathResolver, PathResolver>();
            services.AddSingleton<IPluralize, Pluralizer.Pluralizer>();
            services.AddSingleton<IResponseLinksProvider>((p) => new ResponseLinksProvider());
            services.AddSingleton<IResourceContentModifier<Data.Model.Response.Resource>>((p) => new ResourceContentModifier<Data.Model.Response.Resource>());
            services.AddSingleton(x => mapper);
            services.AddControllers()
                    .AddNewtonsoftJson(options =>
                    {
                        // Use the default property (Pascal) casing
                        options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
                        options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Include;
                        options.SerializerSettings.DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat;
                    })
                    .AddFluentValidation(config => { 
                    
                        config.AutomaticValidationEnabled = true;
                        config.RegisterValidatorsFromAssemblyContaining<Handlers.RequestExceptionModel>();
                    });

            services.AddMediatR(typeof(Handlers.Resource.ResourcePostHandler));

            services.AddAuthentication("Bearer")
                        .AddJwtBearer("Bearer", options =>
                        {
                            options.Authority = "https://mystore.local/identity"; //TODO: CONFIG
                            options.Audience = "https://mystore.local/identity/resources"; //TODO: CONFIG
                            options.RequireHttpsMetadata = true;                            //TODO: CONFIG
                            options.TokenValidationParameters = new TokenValidationParameters
                            {
                                ValidateIssuer = true, //TODO: CONFIG
                                ValidateAudience = true, //TODO: CONFIG
                            };
                        });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" }); // TODO Add to config properties

                //var filePath = Path.Combine(System.AppContext.BaseDirectory, "Api.xml");
                //c.IncludeXmlComments(filePath);
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("ApiScope", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", "api1"); // TODO: Add to configuration
                });
            });

            
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            ConfigureMongoDriver2IgnoreExtraElements();

            app.UsePathBase("/api");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors(policy =>
            {
                //policy.WithOrigins("*");
                policy.AllowAnyOrigin();
                policy.AllowAnyHeader();
                policy.AllowAnyMethod();
                //policy.WithMethods("POST","PUT","GET","DELETE");
                //policy.WithExposedHeaders("WWW-Authenticate");
            });

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "api";
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1"); // TODO Add to config properties
            });

            app.UseHttpsRedirection();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                //ForwardedHeaders = ForwardedHeaders.All
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseRequestResponseLogging();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers()
                    .RequireAuthorization("ApiScope")
                ;
                // `.RequireAuthorization()` sets all controllers to [Authorize] 
                // therefore Anonymous access is by exception using [AllowAnonymous] on the required element
            });
        }
        private static void ConfigureMongoDriver2IgnoreExtraElements()
        {
            BsonClassMap.RegisterClassMap<Data.Model.Storage.StorageMetadata>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });
            BsonClassMap.RegisterClassMap<Data.Model.Storage.Resource>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });

        }
    }

}