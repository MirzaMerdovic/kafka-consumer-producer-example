using Confluent.Kafka;
using Infrastructure.Kafka.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;

namespace Infrastructure.Kafka
{
    /// <summary>
    /// Represent the way to initializes Kafka consumer and producer instances.
    /// </summary>
    /// <remarks>
    /// Instance should have singleton life cycle.
    /// </remarks>
    public interface IKafkaClientFactory
    {
        /// <summary>
        /// Tries to create an instance of <see cref="IProducer{TKey, TValue}"/>.
        /// </summary>
        /// <typeparam name="TKey">Key type</typeparam>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <returns>Instance of <see cref="IProducer{TKey, TValue}"/>.</returns>
        /// <remarks>Calling CreateProducer multiple times will not create multiple instances of <see cref="IProducer{TKey, TValue}"/>,
        /// since they will be cached, assuming that the factory instance has the singleton life-cycle.
        /// </remarks>
        IProducer<TKey, TValue> CreateProducer<TKey, TValue>() where TValue : class;

        /// <summary>
        /// Tries to create an instance of <see cref="IConsumer{TKey, TValue}"/>.
        /// </summary>
        /// <typeparam name="TKey">Key type.</typeparam>
        /// <typeparam name="TValue">Value type.</typeparam>
        /// <returns>Instance of <see cref="IConsumer{TKey, TValue}"/>.</returns>
        /// <remarks>Calling CreateConsumer multiple times will not create multiple instances of <see cref="IConsumer{TKey, TValue}"/>,
        /// since they will be cached, assuming that the factory instance has the singleton life-cycle.
        /// </remarks>
        IConsumer<TKey, TValue> CreateConsumer<TKey, TValue>() where TValue : class;
    }

    public class KafkaClientFactory : IKafkaClientFactory
    {
        private const string KafkaClusterConfigurationKey = "KafkaTechssonCluster";

        private readonly ConcurrentDictionary<string, IClient> _cache = new ConcurrentDictionary<string, IClient>();

        private readonly KafkaOptions _options;
        private readonly ILogger _logger;

        public KafkaClientFactory(IOptions<KafkaOptions> options, ILogger<KafkaClientFactory> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options.Value ?? throw new ArgumentNullException(nameof(logger));

            _ = _options.Consumer ?? throw new ArgumentException("Consumer options are null.");
            _ = _options.Producer ?? throw new ArgumentException("Producer options are null.");
        }

        public IProducer<TKey, TValue> CreateProducer<TKey, TValue>() where TValue : class
        {
            var config = _options.Producer.ToProducerConfig();
            config.BootstrapServers = _options.Nodes;
            var key = $"producer-{config.BootstrapServers}";

            if (_cache.TryGetValue(key, out var producer))
                return (IProducer<TKey, TValue>)producer;

            _cache[key] =
                new ProducerBuilder<TKey, TValue>(config)
                    .SetLogHandler(LogKafkaMessage)
                    .Build();

            _logger.LogInformation("New producer: {kafka-diag-client} created.", key);

            return (IProducer<TKey, TValue>)_cache[key];
        }

        public IConsumer<TKey, TValue> CreateConsumer<TKey, TValue>() where TValue : class
        {
            var config = _options.Consumer.ToConsumerConfig();
            config.BootstrapServers = _options.Nodes;
            var key = $"consumer-{config.BootstrapServers}";

            if (_cache.TryGetValue(key, out var consumer))
                return (IConsumer<TKey, TValue>)consumer;

            _cache[key] =
                new ConsumerBuilder<TKey, TValue>(config)
                    .SetLogHandler(LogKafkaMessage)
                    .Build();

            _logger.LogInformation("New consumer: {kafka-diag-client} created.", key);

            return (IConsumer<TKey, TValue>)_cache[key];
        }

        private void LogKafkaMessage(IClient client, LogMessage log)
        {
            var message = $"Client: {log.Name} failed with: {log.Message}. Source: {log.Facility}. Kafka log level: {log.Level.ToString()}";

            switch (log.Level)
            {
                case SyslogLevel.Emergency:
                case SyslogLevel.Alert:
                case SyslogLevel.Critical:
                    _logger.LogCritical(message);
                    break;

                case SyslogLevel.Error:
                    _logger.LogError(message);
                    break;

                case SyslogLevel.Warning:
                    _logger.LogWarning(message);
                    break;

                case SyslogLevel.Notice:
                case SyslogLevel.Info:
                    _logger.LogInformation(message);
                    break;

                case SyslogLevel.Debug:
                    _logger.LogDebug(message);
                    break;
            }
        }
    }
}
