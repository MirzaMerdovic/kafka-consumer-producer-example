using Infrastructure.Kafka.Adapters;
using Infrastructure.Kafka.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace Infrastructure.Kafka.Hosting
{
    public static class HostBuilderExtensions
    {
        /// <summary>
        /// Tries to register Kafka dependencies which includes: IKafkaClientClient, IKafkaConsumerAdapter, IKafkaProducerAdapter and required options.
        /// </summary>
        /// <param name="services">Instance of <see cref="IServiceCollection"/>.</param>
        /// <param name="context">Instance of <see cref="HostBuilderContext"/>.</param>
        /// <returns>Modified provided instance of <see cref="IConfigurationBuilder"/>.</returns>
        public static IServiceCollection AddKafka<TConsumerAdapter>(this IServiceCollection services, HostBuilderContext context)
            where TConsumerAdapter : ConsumerAdapter
        {
            ThrowIfNull(services);
            ThrowIfNull(context);

            return
                services
                    .Configure<KafkaOptions>(context.Configuration.GetSection(nameof(KafkaOptions)))
                    .AddSingleton<IKafkaClientFactory, KafkaClientFactory>()
                    .AddTransient<IKafkaConsumerAdapter, TConsumerAdapter>()
                    .AddTransient<IKafkaProducerAdapter, ProducerAdapter>();
        }

        private static void ThrowIfNull<T>(T value) where T : class
        {
            _ = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}
