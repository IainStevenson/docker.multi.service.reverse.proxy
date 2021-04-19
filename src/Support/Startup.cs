using Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace Support
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private readonly Configuration.Options _configuration;
        public Startup(IConfiguration configuration, IHostEnvironment environment)
        {
            Configuration = configuration;
            _configuration = Configuration.Get<Configuration.Options>();
#if DEBUG
            var configfile = $@"/{environment.ContentRootPath}/appsettings.active.json";
            System.IO.File.WriteAllText(configfile, JsonConvert.SerializeObject(_configuration));
#endif
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

            services.AddOptions();

            services.Configure<Configuration.ApiOptions>(options => Configuration.GetSection("Api").Bind(options));

            services.AddControllersWithViews()
                .AddNewtonsoftJson(options =>
                    {
                        // Use the default property (Pascal) casing
                        options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
                        options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Include;
                        options.SerializerSettings.DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat;
                    });

            services.AddHttpClient(string.Empty);

            services.AddRequestResponseLoggingMiddlewareWithOptions(
                options => { options.LogSource = _configuration.RequestResponse.Source; }
                );

            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = _configuration.Authentication.Scheme;
                    options.DefaultChallengeScheme = _configuration.Authentication.ChallengeScheme;
                })
                .AddCookie(_configuration.Authentication.Scheme)
                .AddOpenIdConnect(_configuration.Authentication.ChallengeScheme, options =>
                {
                    options.Authority = _configuration.Authentication.Authority;
                    options.RequireHttpsMetadata = _configuration.Authentication.RequireHttpsMetadata;
                    options.ClientId = _configuration.Authentication.ClientId;
                    options.ClientSecret = _configuration.Authentication.ClientSecret;
                    options.ResponseType = _configuration.Authentication.ResponseType;
                    options.SaveTokens = _configuration.Authentication.SaveTokens;
                    var scopesSetting = _configuration.Authentication.RequiredScopes;
                    var scopes = scopesSetting.Split(',', System.StringSplitOptions.RemoveEmptyEntries).Distinct();
                    foreach (var scope in scopes)
                    {
                        options.Scope.Add(scope);
                    }
                    options.GetClaimsFromUserInfoEndpoint = _configuration.Authentication.GetClaimsFromUserInfoEndpoint;

                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UsePathBase(_configuration.Service.BasePath);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                IdentityModelEventSource.ShowPII = true; // enable PII viewing in dev environment
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseHsts();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseRequestResponseLogging();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}")
                    .RequireAuthorization();
                // `.RequireAuthorization()` sets all controllers to [Authorize] 
                // therefore Anonymous access is by exception using [AllowAnonymous] on the required element
            });
        }
    }
}
