using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace MassTransit.ProducerApp
{
    public class BusHostedService : IHostedService
    {
        private readonly IBusControl _bus;

        public BusHostedService(IBusControl bus)
        {
            _bus = bus;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _bus.StartAsync(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _bus.StartAsync(cancellationToken);
        }
    }
}
