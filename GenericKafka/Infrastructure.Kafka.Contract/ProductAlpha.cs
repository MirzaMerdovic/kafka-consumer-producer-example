using System;

namespace Infrastructure.Kafka.Contract
{
    [Serializable]
    public class ProductAlpha
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
