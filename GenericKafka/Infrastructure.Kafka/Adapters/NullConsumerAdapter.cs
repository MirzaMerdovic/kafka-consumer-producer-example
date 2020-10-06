using Infrastructure.Kafka.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Kafka.Adapters
{
    public class NullConsumerAdapter : ConsumerAdapter
    {
        public NullConsumerAdapter(IOptions<KafkaOptions> options, IKafkaClientFactory factory, ILogger<NullConsumerAdapter> logger)
            : base(options, factory, logger)
        {
        }

        protected override Dictionary<Type, Func<object, Task>> Handlers => new Dictionary<Type, Func<object, Task>>(0);
    }
}
