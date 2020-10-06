using Infrastructure.Kafka.Adapters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace Infrastructure.Kafka.Hosting
{
    public sealed class KafkaHostBuilder<TConsumerAdapter> where TConsumerAdapter : ConsumerAdapter
    {
        public static IHost BuildHost(string[] args, Action<HostBuilderContext, IServiceCollection> configureDelegate)
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
                    services.AddKafka<TConsumerAdapter>(ctx);
                })
                .ConfigureServices(configureDelegate);

            return builder.Build();
        }
    }

    public sealed class KafkaHostBuilder
    {
        public static IHost BuildHost(string[] args, Action<HostBuilderContext, IServiceCollection> configureDelegate)
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
                    services.AddKafka<NullConsumerAdapter>(ctx);
                })
                .ConfigureServices(configureDelegate);

            return builder.Build();
        }
    }
}
