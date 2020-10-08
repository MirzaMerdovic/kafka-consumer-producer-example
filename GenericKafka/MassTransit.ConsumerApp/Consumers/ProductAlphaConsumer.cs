using Kafka.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassTransit.ConsumerApp.Consumers
{
    public class ProductAlphaConsumer : IConsumer<ProductAlpha>
    {
        public Task Consume(ConsumeContext<ProductAlpha> context)
        {
            Console.WriteLine("Product Alpha consumed.");

            return Task.CompletedTask;
        }
    }
}
