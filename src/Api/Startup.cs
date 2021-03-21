using Api.Models;
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
using Newtonsoft.Json;
using Pluralizer;
using Response.Formater;
using Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

            services.AddRequestResponseLoggingMiddlewareWithOptions(options => { options.LogSource = "api.miconsent.com"; });

            if (HostEnvironment.IsDevelopment())
            {
                IdentityModelEventSource.ShowPII = true; //Add this line to expose better debugging when token validation fails

            }
            services.AddHttpClient("");


            var identityServiceSettings = new
            {
                Authority = "https://miconsent.com/identity",
                RequireHttpsMetadata = true,
                ApiName = "miConsent.Api"
            };



            //static IList<ICollectionValidator> NewCollectionValidators(IServiceProvider services)
            //{
            //    int collectionItemLimit = 100;

            //    return new List<ICollectionValidator>
            //        {
            //            new CollectionCountCollectionValidator(collectionItemLimit),
            //            new KeyPropertiesCollectionValidator()
            //        };
            //}

            //static IList<IParameterValidator> NewParameterValidators(IServiceProvider services)
            //{
            //    return new List<IParameterValidator>
            //        {
            //            new IdentityParameterValidator(),
            //            new NamespaceParameterValidator(),
            //            new OrderByParameterValidator(),
            //            new SkipParameterValidator(),
            //            new TakeParameterValidator()
            //        };
            //}

            IMongoClient NewMongoClient(IServiceProvider services)
            {
                var connectionString = $"mongodb://storage:storagepass@mongo.miconsent.com:27017";
                return new MongoClient(new MongoUrl(connectionString));
            }

            IRepository<Data.Model.Storage.Resource> NewResourceStorageClient(IServiceProvider services)
            {
                var databaseName = "miConsentData";
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

            //services.AddScoped<LogActionFilter>();

            //services.AddScoped<ResponseHeaderGuardFilter>();

            services.AddScoped<IResourceContentModifier<Data.Model.Response.Resource>, ResourceContentModifier<Data.Model.Response.Resource>>();

            //services.AddScoped(NewCollectionValidators);

            //services.AddScoped(NewParameterValidators);

            services.AddScoped(NewMongoClient);

            //services.AddScoped<IWriteRequestValidation, PostValidation>();

            services.AddScoped(NewResourceStorageClient);

            services.AddSingleton<IDictionary<string, string>>(new Dictionary<string, string>() {
                { "miconsent.com/identity","/.well-known/openid-configuration" },
                { "miconsent.com", "" },
                { "miconsent.com/api","/echo/{hostname}" },
                { "mongo.miconsent.com", null }
            });

            //services.AddSingleton<IEchoResponder, EchoResponder>();

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
                            options.Authority = "https://miconsent.com/identity";
                            options.Audience = "https://miconsent.com/identity/resources";
                            options.RequireHttpsMetadata = true;

                            options.TokenValidationParameters = new TokenValidationParameters
                            {
                                ValidateIssuer = true,
                                ValidateAudience = true,
                            };
                        });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" }); // TODO Add to config properties

                var filePath = Path.Combine(System.AppContext.BaseDirectory, "Api.xml");
                c.IncludeXmlComments(filePath);
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
            });
        }
    }
    //public class Startup
    //{
    //    public Startup(IConfiguration configuration)
    //    {
    //        Configuration = configuration;
    //    }

    //    public IConfiguration Configuration { get; }
    //    private static readonly string[] Summaries = new[]
    //   {
    //        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    //    };

    //    // This method gets called by the runtime. Use this method to add services to the container.
    //    public void ConfigureServices(IServiceCollection services)
    //    {
    //        // Gerneate some forecasts to display straight away
    //        var rng = new Random();
    //        var forecasts = Enumerable.Range(0, Summaries.Length - 1).Select(index =>

    //        new Data.Model.Storage.Resource() {  Content = JsonConvert.SerializeObject(new WeatherForecastModel()
    //        {
    //            Date = DateTime.Now.AddDays(index),
    //            TemperatureC = rng.Next(-20, 55),
    //            Summary = Summaries[rng.Next(Summaries.Length)]
    //        })
    //        }
    //        ).ToDictionary( x => x.Id, x=> x );

    //        // add them to the services for the controller repository
    //        services.AddSingleton(forecasts);

    //        IMapper mapper = new MapperConfiguration(cfg =>
    //        {
    //            cfg.CreateMap<Data.Model.Storage.Resource, Data.Model.Response.Resource>();

    //        }).CreateMapper();

    //        services.AddSingleton(x => mapper);

    //        services.AddSingleton<IPluralize, Pluralizer.Pluralizer>();

    //        services.AddSingleton<IResponseLinksProvider<Data.Model.Response.Resource>, ResponseLinksProvider<Data.Model.Response.Resource>>();

    //        services.AddScoped<IRepository<Data.Model.Storage.Resource>, InMemoryResourceRepository>();

    //        services.AddControllers()
    //                .AddNewtonsoftJson(options =>
    //                {
    //                    // Use the default property (Pascal) casing
    //                    options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
    //                    options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Include;
    //                    options.SerializerSettings.DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat;
    //                });

    //        services.AddRequestResponseLoggingMiddlewareWithOptions(options => { options.LogSource = "Api"; });// TODO: Add to configuration

    //        services.AddAuthentication("Bearer")// TODO: Add to configuration
    //            .AddJwtBearer("Bearer", options =>
    //            {
    //                options.Authority = "https://mystore.local/identity"; //TODO: CONFIG
    //                options.Audience = "https://mystore.local/identity/resources"; //TODO: CONFIG
    //                options.RequireHttpsMetadata = true;

    //                options.TokenValidationParameters = new TokenValidationParameters
    //                {
    //                    ValidateIssuer = true, //TODO: CONFIG
    //                    ValidateAudience = true, //TODO: CONFIG
    //                };
    //            });
    //        services.AddAuthorization(options =>
    //        {
    //            options.AddPolicy("ApiScope", policy =>
    //            {
    //                policy.RequireAuthenticatedUser();
    //                policy.RequireClaim("scope", "api1");// TODO: Add to configuration
    //            });
    //        });

    //    }

    //    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    //    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    //    {
    //        app.UsePathBase("/api");// TODO: Add to configuration

    //        if (env.IsDevelopment())
    //        {
    //            app.UseDeveloperExceptionPage();
    //        }

    //        app.UseHttpsRedirection();

    //        app.UseForwardedHeaders(new ForwardedHeadersOptions
    //        {
    //            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
    //        });

    //        app.UseRouting();

    //        app.UseAuthentication();

    //        app.UseAuthorization();

    //        app.UseRequestResponseLogging();

    //        app.UseEndpoints(endpoints =>
    //        {
    //            endpoints.MapControllers()
    //                .RequireAuthorization("ApiScope")
    //                ;
    //            // `.RequireAuthorization()` sets all controllers to [Authorize] 
    //            // therefore Anonymous access is by exception using [AllowAnonymous] on the required element
    //        });
    //    }
    //}
}
