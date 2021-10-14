using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProcessLogger;

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
            await workerInstance.Stop();
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
                    services.AddTransient(p =>
                    {
                        var config = p.GetService<IConfiguration>();
                        return new LogRecord(config["Sql:ConnectionString"]);
                    });
                });
        }


        internal class Worker
        {
            // connection string to your Service Bus namespace
            static string connectionString;

            // name of your Service Bus queue
            static string queueName;


            // the client that owns the connection and can be used to create senders and receivers
            static ServiceBusClient client;

            // the processor that reads and processes messages from the queue
            static ServiceBusProcessor processor;

            private readonly LogRecord logRecord;
            private readonly ILogger<Worker> logger;

            public Worker(
                LogRecord logRecord,
                IConfiguration configuration,
                ILogger<Worker> logger)
            {
                connectionString = configuration["ServiceBus:ConnectionString"];
                queueName = configuration["ServiceBus:QueueName"];
                this.logRecord = logRecord;
                this.logger = logger;
            }

            public async Task DoWork()
            {
                // The Service Bus client types are safe to cache and use as a singleton for the lifetime
                // of the application, which is best practice when messages are being published or read
                // regularly.
                //

                // Create the client object that will be used to create sender and receiver objects
                client = new ServiceBusClient(connectionString);

                // create a processor that we can use to process the messages
                processor = client.CreateProcessor(queueName, new ServiceBusProcessorOptions());

                try
                {
                    // add handler to process messages
                    processor.ProcessMessageAsync += MessageHandler;

                    // add handler to process any errors
                    processor.ProcessErrorAsync += ErrorHandler;

                    // start processing 
                    await processor.StartProcessingAsync();

                    logger.LogInformation("Wait for a minute and then press any key to end the processing");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, ex.Message);
                }
            }

            public async Task Stop()
            {
                try
                {
                    // stop processing 
                    logger.LogInformation("Stopping the receiver...");
                    await processor.StopProcessingAsync();
                    logger.LogInformation("Stopped receiving messages");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, ex.Message);
                }
                finally
                {
                    // Calling DisposeAsync on client types is required to ensure that network
                    // resources and other unmanaged objects are properly cleaned up.
                    await processor.DisposeAsync();
                    await client.DisposeAsync();
                }

            }
            // handle received messages
            private async Task MessageHandler(ProcessMessageEventArgs args)
            {
                string body = args.Message.Body.ToString();
                logger.LogInformation($"{body}");
                await this.logRecord.Store(new LogRecord.Entity(int.Parse(body), nameof(ConsoleProcessorPeekLockOne)));

                // complete the message. messages is deleted from the queue. 
                await args.CompleteMessageAsync(args.Message);
            }

            // handle any errors when receiving messages
            private Task ErrorHandler(ProcessErrorEventArgs args)
            {
                logger.LogInformation(args.Exception.ToString());
                return Task.CompletedTask;
            }
        }
    }
}
