using Infrastructure.Kafka.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace Infrastructure.Kafka.ConsumerApp
{
    internal class Program
    {
        internal static Task Main(string[] args)
        {
            return
                KafkaHostBuilder<ProductConsumerAdapter>.BuildHost(
                    args,
                    (_, services) => services.AddHostedService<KafkaConsumerHostedService>())
                .RunAsync();
        }
    }
}
