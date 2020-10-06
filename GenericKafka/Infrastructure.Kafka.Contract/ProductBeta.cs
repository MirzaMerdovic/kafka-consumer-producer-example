using System;

namespace Infrastructure.Kafka.Contract
{
    [Serializable]
    public class ProductBeta
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public decimal Price { get; set; }
    }
}
