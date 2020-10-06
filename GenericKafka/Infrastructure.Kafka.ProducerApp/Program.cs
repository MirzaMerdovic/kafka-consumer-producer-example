using Infrastructure.Kafka.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace Infrastructure.Kafka.ProducerApp
{
    internal class Program
    {
        internal static Task Main(string[] args)
        {
            return
                KafkaHostBuilder.BuildHost(
                    args,
                    (_, services) => services.AddHostedService<KafkaProducerHostedService>())
                .RunAsync();
        }
    }
}
