using System;

namespace Kafka.Contract
{
    [Serializable]
    public class ProductAlpha
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
