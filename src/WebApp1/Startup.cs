using Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using System.IdentityModel.Tokens.Jwt;

namespace WebApp1
{
    public partial class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllersWithViews();
            JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
            services.AddRequestResponseLoggingMiddlewareWithOptions(options => { options.LogSource = "Store"; });
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc";
            })
                .AddCookie("Cookies")
                .AddOpenIdConnect("oidc", options =>
                {
                    options.Authority = "https://mystore.local/identity"; //TODO: CONFIG
                    options.RequireHttpsMetadata = true; //TODO: CONFIG
                    options.ClientId = "mvc"; //TODO: CONFIG
                    options.ClientSecret = "secret"; //TODO: CONFIG
                    options.ResponseType = "code"; //TODO: CONFIG


                    options.SaveTokens = true;

                    options.Scope.Add("openid"); //TODO: CONFIG
                    options.Scope.Add("profile"); //TODO: CONFIG
                    options.Scope.Add("email"); //TODO: CONFIG
                    options.Scope.Add("api1"); //TODO: CONFIG
                    options.GetClaimsFromUserInfoEndpoint = true;
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UsePathBase("/store");
           
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                IdentityModelEventSource.ShowPII = true; // enable PII viewing in dev environment
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

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
