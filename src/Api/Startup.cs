using AutoMapper;
using Logging;
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

            services.AddRequestResponseLoggingMiddlewareWithOptions(options => { options.LogSource = "mystore.api"; });

            if (HostEnvironment.IsDevelopment())
            {
                IdentityModelEventSource.ShowPII = true; //Add this line to expose better debugging when token validation fails

            }
            services.AddHttpClient("");


            var identityServiceSettings = new
            {
                Authority = "https://mystore.local/identity",
                RequireHttpsMetadata = true,
                ApiName = "mystore.api"
            };


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

            services.AddCors();

            services.AddDistributedMemoryCache();

            services.AddSingleton<IRequestHeadersProvider>((services) => new RequestHeadersProvider());
            services.AddSingleton<IResponseHeadersProvider>((services) => new ResponseHeadersProvider(new List<string>() { "X-Powered-By" }));


            IMapper mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Data.Model.Storage.Resource, Data.Model.Response.Resource>();

            }).CreateMapper();

            services.AddSingleton(x => mapper);

            services.AddSingleton<IPathResolver, PathResolver>();

            services.AddSingleton<IPluralize, Pluralizer.Pluralizer>();

            services.AddSingleton<IResponseLinksProvider<Data.Model.Response.Resource>>((p) => new ResponseLinksProvider<Data.Model.Response.Resource>());

            services.AddSingleton<IResourceContentModifier<Data.Model.Response.Resource>>((p) => new ResourceContentModifier<Data.Model.Response.Resource>());

            services.AddSingleton<IPathResolver, PathResolver>();

            services.AddScoped<IResourceContentModifier<Data.Model.Response.Resource>, ResourceContentModifier<Data.Model.Response.Resource>>();

            services.AddScoped(NewMongoClient);

            services.AddScoped(NewResourceStorageClient);

            services.AddControllers()
                    .AddNewtonsoftJson(options =>
                    {
                        // Use the default property (Pascal) casing
                        options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
                        options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Include;
                        options.SerializerSettings.DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat;
                    });

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

                var filePath = Path.Combine(System.AppContext.BaseDirectory, "Api.xml");
                c.IncludeXmlComments(filePath);
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("ApiScope", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", "mystore.api"); // TODO: Add to configuration
                });
            });


        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

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
                    .RequireAuthorization("ApiScope");
                // `.RequireAuthorization()` sets all controllers to [Authorize] 
                // therefore Anonymous access is by exception using [AllowAnonymous] on the required element
            });
        }
    }
}