using Kafka.Contract;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MassTransit.ProducerApp
{
    public class MassTransitProducerHostedService : IHostedService
    {
        readonly IBusControl _bus;

        private readonly IPublishEndpoint _publisher;

        public MassTransitProducerHostedService(IPublishEndpoint publisher)
        {
            _publisher = publisher;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var random = new Random();
            while(true)
            {
                var number = random.Next(12);

                if (number % 2 == 0)
                {
                    await _publisher.Publish(new ProductAlpha { Id = 1, Name = "Alpha" }, cancellationToken);
                    Console.WriteLine("Publish Alpha");
                }
                else if (number % 3 == 0)
                {
                    await _publisher.Publish(new ProductBeta { Id = 1, Title = "Beta", Price = 5 });
                    Console.WriteLine("Publish Beta");
                }
                else
                {
                    await _publisher.Publish(new ProductGamma { Id = 1, Name = "Gamma", Description = "I am gamma, the product", Active = true });
                    Console.WriteLine("Publish Gamma");
                }

                await Task.Delay(TimeSpan.FromSeconds(5));
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
