using Infrastructure.Kafka.Adapters;
using Kafka.Contract;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Kafka.ProducerApp
{
    public class KafkaProducerHostedService : IHostedService
    {
        private readonly IKafkaProducerAdapter _producerAdapter;

        public KafkaProducerHostedService(IKafkaProducerAdapter producerAdapter)
        {
            _producerAdapter = producerAdapter ?? throw new ArgumentNullException(nameof(producerAdapter));
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _producerAdapter.Produce(Guid.NewGuid().ToString(), new ProductAlpha { Id = 1, Name = "Alpha" });
            await _producerAdapter.Produce(Guid.NewGuid().ToString(), new ProductBeta { Id = 1, Title = "Beta", Price = 5 });
            await _producerAdapter.Produce(Guid.NewGuid().ToString(), new ProductGamma { Id = 1, Name = "Gamma", Description = "I am gamma, the product", Active = true });
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled(cancellationToken);

            return Task.CompletedTask;
        }
    }
}
