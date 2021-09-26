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
using Newtonsoft.Json;
using Pluralizer;
using Response.Formater;
using Serilog;
using Storage;
using System;


namespace Api
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
#if DEBUG
            var configfile = $@"/{environment.ContentRootPath}/appsettings.active.json";
            System.IO.File.WriteAllText(configfile, JsonConvert.SerializeObject(_configuration));
            Log.Logger.Debug($"Logged configuration to {configfile}");
#endif

            HostEnvironment = environment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var excludedHeaders = _configuration.Headers.Exclude.Split(',', StringSplitOptions.RemoveEmptyEntries);

            if (HostEnvironment.IsDevelopment())
            {
                //Add this line to expose better debugging when token validation fails
                IdentityModelEventSource.ShowPII = true;
            }

            IMongoClient NewMongoClient(IServiceProvider services)
            {

                var connectionString = _configuration.Mongo.ConnectionString;
                return new MongoClient(new MongoUrl(connectionString));
            }

            IRepository<Data.Model.Storage.Resource> NewResourceStorageClient(IServiceProvider services)
            {
                var databaseName = _configuration.Mongo.DatabaseName;
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
            // _configuration.Logging.Source
            services.AddRequestResponseLoggingMiddlewareWithOptions(options =>
            {
                options.LogSource = _configuration.RequestResponse.Source;
            });
            services.AddHttpClient(string.Empty);
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
                    .AddFluentValidation(config =>
                    {

                        config.AutomaticValidationEnabled = true;
                        config.RegisterValidatorsFromAssemblyContaining<Handlers.RequestExceptionModel>();
                    });

            services.AddMediatR(typeof(Handlers.Resource.ResourcePostHandler));

            services.AddAuthentication("Bearer")
                        .AddJwtBearer("Bearer", options =>
                        {
                            options.Authority = _configuration.Identity.Authority;
                            options.Audience = _configuration.Identity.Audience;
                            options.RequireHttpsMetadata = _configuration.Identity.RequireHttpsMetadata;
                            options.TokenValidationParameters = new TokenValidationParameters
                            {
                                ValidateIssuer = _configuration.Identity.TokenValidation.ValidateIssuer,
                                ValidateAudience = _configuration.Identity.TokenValidation.ValidateAudience,
                            };
                        });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(_configuration.Swagger.Version, new OpenApiInfo { Title = _configuration.Swagger.OpenAPIInfo.Title, Version = _configuration.Swagger.OpenAPIInfo.Version });

                //var filePath = Path.Combine(System.AppContext.BaseDirectory, "Api.xml");
                //c.IncludeXmlComments(filePath);
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(_configuration.Authorization.Policy.Name, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim(_configuration.Authorization.Policy.ClaimName, _configuration.Authorization.Policy.ClaimValues.Split(',', StringSplitOptions.RemoveEmptyEntries)); 
                });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            ConfigureMongoDriver2IgnoreExtraElements();

            app.UsePathBase(_configuration.Service.BasePath );

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors(policy =>
            {
                //TODO: Determine requirements and add to Configutation
                // TODO: Find a way of making this dynamic
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
                c.RoutePrefix = _configuration.Swagger.RoutePrefix;
                c.SwaggerEndpoint(_configuration.Swagger.Endpoint, _configuration.Swagger.EndpointName); 
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
                    .RequireAuthorization(_configuration.Authorization.Policy.Name)
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