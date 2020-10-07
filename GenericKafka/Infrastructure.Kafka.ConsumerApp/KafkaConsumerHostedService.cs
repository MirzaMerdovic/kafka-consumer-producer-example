using Infrastructure.Kafka.Adapters;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Kafka.ConsumerApp
{
    public class KafkaConsumerHostedService : IHostedService
    {
        private readonly IKafkaConsumerAdapter _consumerAdapter;

        public KafkaConsumerHostedService(IKafkaConsumerAdapter consumerAdapter)
        {
            _consumerAdapter = consumerAdapter ?? throw new ArgumentNullException(nameof(consumerAdapter));
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _consumerAdapter.Consume(cancellationToken).ConfigureAwait(false);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled(cancellationToken);

            return Task.CompletedTask;
        }
    }
}
