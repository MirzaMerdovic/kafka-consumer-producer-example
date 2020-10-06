using Confluent.Kafka;
using Infrastructure.Kafka.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Kafka.Adapters
{
    public interface IKafkaConsumerAdapter
    {
        string Topic { get; }
        Task Consume(CancellationToken cancellation);
    }

    public abstract class ConsumerAdapter : IKafkaConsumerAdapter
    {
        private const string LogKey = "Consumer";

        private readonly ConsumerOptions _options;
        private readonly IKafkaClientFactory _factory;
        private readonly ILogger _logger;

        public string Topic { get; internal set; }

        public ConsumerAdapter(IOptions<KafkaOptions> options, IKafkaClientFactory factory, ILogger<ConsumerAdapter> logger)
        {
            _options = options.Value?.Consumer ?? throw new ArgumentNullException(nameof(options));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            Topic = _options.Topic;
        }

        protected abstract Dictionary<Type, Func<object, Task>> Handlers { get; }

        public async Task Consume(CancellationToken cancellation)
        {
            if (_options.Disabled)
            {
                _logger.LogWarning("{kafka-diag} for {kafka-diag-topic} is disabled", LogKey, Topic);

                return;
            }

            if (string.IsNullOrWhiteSpace(Topic))
                throw new ArgumentException("Kafka Topic not set, please check your configuration.");

            var consumer = _factory.CreateConsumer<string, byte[]>();
            consumer.Subscribe(Topic);

            _logger.LogInformation("{kafka-diag} has subscribed to: {Kafka-diag-topic}.", LogKey, Topic);

            try
            {
                while (!cancellation.IsCancellationRequested)
                {
                    ConsumeResult<string, byte[]> result = consumer.Consume(cancellation);

                    string type = result.Message.Headers.GetHeaderValue("Type");
                    string source = result.Message.Headers.GetHeaderValue("Source");

                    _logger.LogInformation("{kafka-diag} received the message with: {kafka-diag-type} and {kafka-diag-source}", LogKey, type, source);

                    if (Handlers.Keys.Count(x => x.Name.Equals(type, StringComparison.InvariantCultureIgnoreCase)) > 1)
                        throw new InvalidOperationException($"The message type: {type} is already registered. Topic should contain only unique message types.");

                    var key = Handlers.Keys.FirstOrDefault(x => x.Name.Equals(type, StringComparison.InvariantCultureIgnoreCase));

                    if (key == null)
                        continue;

                    await Handlers[key](result.Message.Value.ToObject()).ConfigureAwait(false);

                    _logger.LogInformation("{kafka-diag-method} processed {kafka-diag} message successfully.", Handlers[key].Method.Name, LogKey);
                }
            }
            catch (ConsumeException e)
            {
                var parameters = new object[] { LogKey, e.ConsumerRecord.Topic, JsonConvert.SerializeObject(e.Error) };
                var message = "{kafka-diag} failed to receive the message for {kafka-diag-topic}. Kafka {kafka-diag-error}";

                _logger.LogError(e, message, parameters);

                throw;
            }
        }
    }

    internal static class Extensions
    {
        public static string GetHeaderValue(this Headers headers, string key)
        {
            var header = headers.FirstOrDefault(x => x.Key.Equals(key, StringComparison.CurrentCultureIgnoreCase));

            return Encoding.UTF8.GetString(header.GetValueBytes());
        }

        public static object ToObject(this byte[] bytes)
        {
            if (bytes == null)
                return null;

            IFormatter formatter = new BinaryFormatter();

            using (var stream = new MemoryStream(bytes))
            {
                var value = formatter.Deserialize(stream);

                return value;
            }
        }
    }
}
