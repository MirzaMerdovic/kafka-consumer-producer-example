using Kafka.Contract;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MassTransit.ProducerApp
{
    class Program
    {
        internal static Task Main(string[] args)
        {
            var builder = new HostBuilder();

            builder
                .ConfigureHostConfiguration(builder =>
                {
                    builder.SetBasePath(Directory.GetCurrentDirectory());
                    builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureAppConfiguration(builder =>
                {
                    builder.AddCommandLine(args);
                    builder.AddEnvironmentVariables();
                })
                .ConfigureServices((ctx, services) =>
                {
                    services.AddLogging(x => x.AddConsole());

                    services.AddMassTransit(x =>
                    {
                        x.UsingRabbitMq((context, cfg) => cfg.Host("localhost"));

                        var serviceAddress = new Uri("rabbitmq://localhost/products");
                        x.AddRequestClient<ProductAlpha>(serviceAddress);
                        x.AddRequestClient<ProductBeta>(serviceAddress);
                        x.AddRequestClient<ProductGamma>(serviceAddress);
                    });

                    services.AddHostedService<BusHostedService>();
                    services.AddHostedService<MassTransitProducerHostedService>();
                });

            return builder.Build().RunAsync();
        }
    }
}
