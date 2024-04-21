using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Support
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>

            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole(configure =>
                    {
                        configure.DisableColors = false;
                        configure.IncludeScopes = true;
                        configure.TimestampFormat = "yyyy-MMM-dd hh:mm.ss.fffffZ ";

                        configure.Format = Microsoft.Extensions.Logging.Console.ConsoleLoggerFormat.Systemd;
                    });
                })               
                .ConfigureWebHostDefaults(webBuilder =>
                {                    
                    webBuilder.UseStartup<Startup>();
                });
    }
}
