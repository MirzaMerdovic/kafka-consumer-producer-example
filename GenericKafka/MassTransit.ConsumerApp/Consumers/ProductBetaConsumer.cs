using Kafka.Contract;
using System;
using System.Threading.Tasks;

namespace MassTransit.ConsumerApp.Consumers
{
    public class ProductBetaConsumer : IConsumer<ProductBeta>
    {
        public Task Consume(ConsumeContext<ProductBeta> context)
        {
            Console.WriteLine("Product Beta consumed.");

            return Task.CompletedTask;
        }
    }
}
