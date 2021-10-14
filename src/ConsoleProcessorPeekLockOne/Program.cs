using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ConsoleProcessorPeekLockOne
{
    class Program
    {
        static async Task Main()
        {
            var host = CreateDefaultBuilder().Build();

            // Invoke Worker
            using IServiceScope serviceScope = host.Services.CreateScope();
            IServiceProvider provider = serviceScope.ServiceProvider;
            var workerInstance = provider.GetRequiredService<Worker>();
            await workerInstance.DoWork();

            host.Run();

            Console.WriteLine("Press any key to end the application");
            Console.ReadKey();
        }

        static IHostBuilder CreateDefaultBuilder()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration(app =>
                {
                    app.AddJsonFile("appsettings.json");
                })
                .ConfigureServices(services =>
                {
                    services.AddSingleton<Worker>();
                });
        }


        internal class Worker
        {
            private readonly ILogger<Worker> logger;

            public Worker(
                IConfiguration configuration,
                ILogger<Worker> logger)
            {
                this.logger = logger;
            }

            public async Task DoWork()
            {

            }
        }
    }
}
