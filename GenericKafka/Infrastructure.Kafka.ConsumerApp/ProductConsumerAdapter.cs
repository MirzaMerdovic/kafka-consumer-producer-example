using Infrastructure.Kafka;
using Infrastructure.Kafka.Adapters;
using Infrastructure.Kafka.Options;
using Kafka.Contract;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kafka.ConsumerApp
{
    public class ProductConsumerAdapter : ConsumerAdapter
    {
        public ProductConsumerAdapter(IOptions<KafkaOptions> options, IKafkaClientFactory factory, ILogger<ProductConsumerAdapter> logger)
            : base(options, factory, logger)
        {
        }

        protected override Dictionary<Type, Func<object, Task>> Handlers
            => new Dictionary<Type, Func<object, Task>>
            {
                [typeof(ProductAlpha)] = ProcessAlpha,
                [typeof(ProductBeta)] = ProcessBeta
            };

        private static Task ProcessAlpha(object response)
        {
            ProductAlpha product = response as ProductAlpha;

            if (product == null)
                return Task.CompletedTask;

            Console.WriteLine("Type Alpha consumed");

            return Task.CompletedTask;
        }

        private static Task ProcessBeta(object response)
        {
            ProductBeta product = response as ProductBeta;

            if (product == null)
                return Task.CompletedTask;

            Console.WriteLine("Type Beta consumed");

            return Task.CompletedTask;
        }
    }
}
