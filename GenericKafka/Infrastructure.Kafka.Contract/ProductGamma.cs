using System;

namespace Infrastructure.Kafka.Contract
{
    [Serializable]
    public class ProductGamma
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool Active { get; set; }
    }
}