using FunctionProcessor;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProcessLogger;

[assembly: FunctionsStartup(typeof(Startup))]
namespace FunctionProcessor
{
    class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddTransient(p =>
            {
                var config = p.GetService<IConfiguration>();
                return new LogRecord(config["Sql:ConnectionString"]);
            });
        }
    }
}
