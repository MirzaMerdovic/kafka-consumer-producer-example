using Confluent.Kafka;
using Infrastructure.Kafka.Options;
using System;

namespace Infrastructure.Kafka
{
    public static class Extensoins
    {
        public static ConsumerConfig ToConsumerConfig(this ConsumerOptions options)
        {
            _ = options ?? throw new ArgumentNullException(nameof(options));

            return new ConsumerConfig
            {
                GroupId = options.GroupId,
                SessionTimeoutMs = options.SessionTimeoutMs,
                SocketTimeoutMs = options.SocketTimeoutMs
            };
        }

        public static ProducerConfig ToProducerConfig(this ProducerOptions options)
        {
            _ = options ?? throw new ArgumentNullException(nameof(options));

            return new ProducerConfig
            {
                MessageTimeoutMs = options.MessageTimeoutMs,
                SocketTimeoutMs = options.SocketTimeoutMs,
                EnableIdempotence = options.EnableIdempotence,
                MessageSendMaxRetries = options.MessageSendMaxRetries
            };
        }
    }
}
