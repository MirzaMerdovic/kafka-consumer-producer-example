using Confluent.Kafka;
using Infrastructure.Kafka.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Kafka.Adapters
{
    public interface IKafkaProducerAdapter
    {
        string Topic { get; }
        Task Produce<TValue>(string key, TValue payload) where TValue : class;
    }

    public class ProducerAdapter : IKafkaProducerAdapter
    {
        private const string LogKey = "Producer";

        private readonly ProducerOptions _options;
        private readonly IKafkaClientFactory _factory;
        private readonly ILogger _logger;

        public string Topic { get; internal set; }

        public ProducerAdapter(IOptions<KafkaOptions> options, IKafkaClientFactory factory, ILogger<ProducerAdapter> logger)
        {
            _options = options.Value?.Producer ?? throw new ArgumentNullException(nameof(options));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            Topic = _options.Topic;
        }

        public async Task Produce<TValue>(string key, TValue payload) where TValue : class
        {
            if (_options.Disabled)
            {
                var parameters = new object[] { LogKey, Topic, key, _options.Source };
                var message = "{kafka-diag} for {kafka-diag-topic} and {kafka-diag-key} from {kafka-diag-source} is disabled";
                _logger.LogWarning(message, parameters);

                return;
            }

            if (string.IsNullOrWhiteSpace(Topic))
                throw new ArgumentException("Kafka Topic not set, please check your configuration.");

            var producer = _factory.CreateProducer<string, byte[]>();

            try
            {
                var type = typeof(TValue).Name;

                var message = new Message<string, byte[]>
                {
                    Key = key,
                    Value = payload.ToByteArray(),
                    Headers = new Headers
                    {
                        {"Source", Encoding.UTF8.GetBytes(_options.Source) },
                        {"Type", Encoding.UTF8.GetBytes(type) }
                    }
                };

                var result = await producer.ProduceAsync(Topic, message).ConfigureAwait(false);

                LogMessageProduced(result.Timestamp.UtcDateTime, type);
            }
            catch (ProduceException<string, string> e)
            {
                _logger.LogError(e, "{kafka-diag} failed to consume record for {kafka-diag-topic}", LogKey, e.DeliveryResult.Topic);

                throw;
            }
        }

        private void LogMessageProduced(DateTime timestamp, string type)
        {
            var parameters = new object[] { LogKey, Topic, type, _options.Source, timestamp };
            var logMessage = "{kafka-diag} produced the message for {kafka-diag-topic} with {kafka-diag-type} and {kafka-diag-source} on {kafka-diag-timestamp}";

            _logger.LogInformation(logMessage, parameters);
        }
    }

    internal static class ProducerAdapterExtensions
    {
        public static byte[] ToByteArray(this object value)
        {
            if (value == null)
                return null;

            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, value);
                var bytes = stream.ToArray();

                return bytes;
            }
        }
    }
}
