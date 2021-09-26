using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;

namespace ApiTestClient
{
    public class Program
    {

        private static async Task Main(string[] args)
        {            
            var services = new ServiceCollection(); // create service collection
            ConfigureServices(services);            
            var serviceProvider = services.BuildServiceProvider();// create service provider            
            await serviceProvider.GetService<App>().Run(args);// entry to run app

        }
        private static void ConfigureServices(IServiceCollection services)
        {
            // configure logging
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
            });
            // build config
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
#if DEBUG
                .AddUserSecrets("2d71d321-7620-49dc-afee-9e296f1e970c") // this is taken care of in the framework for ASP.NET 
#endif
                .AddEnvironmentVariables()
                .Build();
            services.AddOptions();
            services.AddHttpClient(string.Empty);
            services.Configure<AppSettings>(configuration.GetSection("App"));
            services.AddTransient<App>();
        }
    }
}