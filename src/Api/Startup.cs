using Api.Models;
using AutoMapper;
using Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Pluralizer;
using Response.Formater;
using Storage;
using System;
using System.Linq;

namespace Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        private static readonly string[] Summaries = new[]
       {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Gerneate some forecasts to display straight away
            var rng = new Random();
            var forecasts = Enumerable.Range(0, Summaries.Length - 1).Select(index =>

            new Data.Model.Storage.Resource() {  Content = JsonConvert.SerializeObject(new WeatherForecastModel()
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            }
            ).ToDictionary( x => x.Id, x=> x );

            // add them to the services for the controller repository
            services.AddSingleton(forecasts);

            IMapper mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Data.Model.Storage.Resource, Data.Model.Response.Resource>();

            }).CreateMapper();

            services.AddSingleton(x => mapper);

            services.AddSingleton<IPluralize, Pluralizer.Pluralizer>();
            
            services.AddSingleton<IResponseLinksProvider<Data.Model.Response.Resource>, ResponseLinksProvider<Data.Model.Response.Resource>>();
            
            services.AddScoped<IRepository<Data.Model.Storage.Resource>, InMemoryResourceRepository>();

            services.AddControllers()
                    .AddNewtonsoftJson(options =>
                    {
                        // Use the default property (Pascal) casing
                        options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
                        options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Include;
                        options.SerializerSettings.DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat;
                    });

            services.AddRequestResponseLoggingMiddlewareWithOptions(options => { options.LogSource = "Api"; });// TODO: Add to configuration

            services.AddAuthentication("Bearer")// TODO: Add to configuration
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = "https://mystore.local/identity"; //TODO: CONFIG
                    options.Audience = "https://mystore.local/identity/resources"; //TODO: CONFIG
                    options.RequireHttpsMetadata = true;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true, //TODO: CONFIG
                        ValidateAudience = true, //TODO: CONFIG
                    };
                });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("ApiScope", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", "api1");// TODO: Add to configuration
                });
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UsePathBase("/api");// TODO: Add to configuration

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
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
    }
}
