using System;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using ProcessLogger;

namespace FunctionProcessor
{
    public class Processor
    {
        private readonly LogRecord logRecord;
        private readonly ILogger<Processor> logger;

        public Processor(
            LogRecord logRecord,
            ILogger<Processor> logger)
        {
            this.logRecord = logRecord;
            this.logger = logger;
        }

        [FunctionName(nameof(AutocompleteFunction))]
        public async Task AutocompleteFunction(
            [ServiceBusTrigger(
                "function-queue-autocomplete", 
                Connection = "ServiceBusConnection",
                AutoComplete = true
                )]
            string myQueueItem)
        {
            try
            {
                string body = myQueueItem;
                logger.LogInformation($"{body}");
                await this.logRecord.Store(new LogRecord.Entity(int.Parse(body), nameof(AutocompleteFunction)));
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
            }
        }

        [FunctionName(nameof(AutocompleteFunctionWithExceptions))]
        public async Task AutocompleteFunctionWithExceptions(
            [ServiceBusTrigger(
                "function-queue-autocomplete-with-exception",
                Connection = "ServiceBusConnection",
                AutoComplete = true
            )]
            string myQueueItem)
        {
            try
            {
                string body = myQueueItem;
                logger.LogInformation($"{body}");
                if (new Random().Next(0, 10) % 4 == 0)
                {
                    throw new Exception("Something happened");
                }
                await this.logRecord.Store(new LogRecord.Entity(int.Parse(body), nameof(AutocompleteFunctionWithExceptions)));
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
                throw;
            }
        }

        [FunctionName(nameof(BrokeredMessageFunction))]
        public async Task BrokeredMessageFunction(
            [ServiceBusTrigger(
                "function-queue-no-autocomplete",
                Connection = "ServiceBusConnection",
                AutoComplete = false
            )]
            Message message,
            MessageReceiver messageReceiver)
        {
            try
            {
                string body = System.Text.Encoding.UTF8.GetString(message.Body, 0, message.Body.Length);
                logger.LogInformation($"{body}");
                await this.logRecord.Store(new LogRecord.Entity(int.Parse(body), nameof(BrokeredMessageFunction)));
                await messageReceiver.CompleteAsync(message.SystemProperties.LockToken);
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
                throw;
            }
        }

        [FunctionName(nameof(BrokeredMessageFunctionWithExceptions))]
        public async Task BrokeredMessageFunctionWithExceptions(
            [ServiceBusTrigger(
                "function-queue-no-autocomplete-with-exceptions",
                Connection = "ServiceBusConnection",
                AutoComplete = false
            )]
            Message message,
            MessageReceiver messageReceiver)
        {
            try
            {
                string body = System.Text.Encoding.UTF8.GetString(message.Body, 0, message.Body.Length);

                logger.LogInformation($"{body}");
                if (new Random().Next(0, 10) % 4 == 0)
                {
                    throw new Exception("Something happened");
                }
                await this.logRecord.Store(new LogRecord.Entity(int.Parse(body), nameof(BrokeredMessageFunctionWithExceptions)));
                await messageReceiver.CompleteAsync(message.SystemProperties.LockToken);
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
                throw;
            }
        }
    }
}
