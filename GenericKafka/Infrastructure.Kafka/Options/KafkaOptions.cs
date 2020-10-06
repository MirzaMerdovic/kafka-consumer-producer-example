namespace Infrastructure.Kafka.Options
{
    public class KafkaOptions
    {
        public string Nodes { get; set; }

        public ConsumerOptions Consumer { get; set; }

        public ProducerOptions Producer { get; set; }
    }

    public class ConsumerOptions
    {
        public bool Disabled { get; set; }

        public string Topic { get; set; }

        public string[] SupportedTypes { get; set; } = new string[0];

        /// <summary>
        /// Client group id string. All clients sharing the same group.id belong to the same group.
        /// </summary>
        public string GroupId { get; set; }

        /// <summary>
        /// Client group session and failure detection timeout.
        /// The consumer sends periodic heartbeats (heartbeat.interval.ms) to indicate its liveness to the broker.
        /// If no hearts are received by the broker for a group member within the session timeout,
        /// the broker will remove the consumer from the group and trigger a rebalance.
        /// The allowed range is configured with the **broker** configuration properties `group.min.session.timeout.ms`
        /// and `group.max.session.timeout.ms`.
        /// </summary>
        /// <remarks>
        /// Also see `max.poll.interval.ms`.
        /// Default: 10000
        /// </remarks>
        public int SessionTimeoutMs { get; set; } = 10000;

        /// <summary>
        /// Default timeout for network requests.
        /// Producer:
        ///     ProduceRequests will use the lesser value of `socket.timeout.ms` and remaining `message.timeout.ms` for the first message in the batch.
        /// Consumer:
        ///     FetchRequests will use `fetch.wait.max.ms` + `socket.timeout.ms`.
        ///     Admin requests will use `socket.timeout.ms` or explicitly set `rd_kafka_AdminOptions_set_operation_timeout()` value.
        /// </summary>
        /// <remarks>
        /// Default value: 60000
        /// </remarks>
        public int SocketTimeoutMs { get; set; } = 60000;
    }

    public class ProducerOptions
    {
        public bool Disabled { get; set; }

        public string Topic { get; set; }

        public string Source { get; set; }

        /// <summary>
        /// Local message timeout. This value is only enforced locally and limits the time a produced message waits for successful delivery.
        /// A time of 0 is infinite.
        /// This is the maximum time librdkafka may use to deliver a message (including retries).
        /// </summary>
        /// <remarks>
        /// Delivery error occurs when either the retry count or the message timeout are exceeded.
        /// Default value: 300000
        /// </remarks>
        public int MessageTimeoutMs { get; set; } = 300000;

        /// <summary>
        /// Default timeout for network requests.
        /// Producer:
        ///     ProduceRequests will use the lesser value of `socket.timeout.ms` and remaining `message.timeout.ms` for the first message in the batch.
        /// Consumer:
        ///     FetchRequests will use `fetch.wait.max.ms` + `socket.timeout.ms`.
        ///     Admin requests will use `socket.timeout.ms` or explicitly set `rd_kafka_AdminOptions_set_operation_timeout()` value.
        /// </summary>
        /// <remarks>
        /// Default value: 60000
        /// </remarks>
        public int SocketTimeoutMs { get; set; } = 60000;

        /// <summary>
        /// How many times to retry sending a failing Message.
        /// </summary>
        /// <remarks>
        /// Retrying may cause reordering unless `enable.idempotence` is set to true.
        /// Default value: 2
        /// </remarks>
        public int MessageSendMaxRetries { get; set; } = 2;

        /// <summary>
        /// When set to `true`, the producer will ensure that messages are successfully produced exactly once and in the original produce order.
        /// The following configuration properties are adjusted automatically (if not modified by the user) when idempotence is enabled:
        ///     `max.in.flight.requests.per.connection=5` (must be less than or equal to 5)
        ///     `retries=INT32_MAX` (must be greater than 0)
        ///     `acks=all`
        ///     `queuing.strategy=fifo`
        /// </summary>
        /// <remarks>
        /// Producer instantiation will fail if user-supplied configuration is incompatible.
        /// Default: false
        /// </remarks>
        public bool EnableIdempotence { get; set; }
    }
}
