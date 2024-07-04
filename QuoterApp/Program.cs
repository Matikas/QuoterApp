using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using QuoterApp.MarketOrders;
using System;
using System.Threading.Tasks;

namespace QuoterApp
{
    class Program
    {
        private static ServiceProvider _serviceProvider;

        static async Task Main(string[] args)
        {
            SetupServices();

            try
            {
                StartMarketOrdersReaderService();

                // Wait for some orders to be read
                await Task.Delay(5000).ConfigureAwait(false);

                AskForQuote();
            }
            finally
            {
                _serviceProvider.Dispose();
            }

            Console.ReadLine();
        }

        static void SetupServices()
        {
            var services = new ServiceCollection();
            services.AddSingleton<IMarketOrderSource, HardcodedMarketOrderSource>()
                .AddSingleton<IMarketOrdersRepository, MarketOrdersRepository>()
                .AddSingleton<MarketOrderSourceReaderService>()
                .AddScoped<IQuoter, YourQuoter>()
                .AddTransient<Policy>((services) => Policy.Handle<Exception>().WaitAndRetryForever(_ => TimeSpan.FromSeconds(1),
                    (ex, _) =>
                    {
                        var logger = services.GetRequiredService<ILogger<RetryPolicy>>();
                        logger.LogError(ex, "Error while reading market orders");
                    }))
                .AddLogging(builder =>
                {
                    builder
                        .AddFilter("Microsoft", LogLevel.Warning)
                        .AddFilter("System", LogLevel.Warning)
                        .AddFilter("QuoterApp.Program", LogLevel.Debug)
                        .AddConsole();
                });
            _serviceProvider = services.BuildServiceProvider();
        }

        static void StartMarketOrdersReaderService()
        {
            _serviceProvider.GetService<MarketOrderSourceReaderService>().Start();
        }

        static void AskForQuote()
        {
            var gq = _serviceProvider.GetService<IQuoter>();
            var qty = 120;

            var quote = gq.GetQuote("DK50782120", qty);
            var vwap = gq.GetVolumeWeightedAveragePrice("DK50782120");

            Console.WriteLine($"Quote: {quote}, {quote / (double)qty}");
            Console.WriteLine($"Average Price: {vwap}");
            Console.WriteLine();
            Console.WriteLine($"Done");
        }
    }
}
