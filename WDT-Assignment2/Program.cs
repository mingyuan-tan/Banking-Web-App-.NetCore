using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WDT_Assignment2.Data;

namespace WDT_Assignment2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    SeedData.Initialize(services);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error has occured seeding the DB");
                }
            }
            host.Run();
        }

        // This starts the web server and runs the application 
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    // Start instructions present in Startup.cs 
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureServices(services =>
                {
                    //services.AddHostedService<ResetLoginAttemptsHostedService>();
                    services.AddHostedService<BillPayHostedService>();
                });
    }
}
