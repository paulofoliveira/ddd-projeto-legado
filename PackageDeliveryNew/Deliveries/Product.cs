using PackageDeliveryNew.Common;

namespace PackageDeliveryNew.Deliveries
{
    public class Product : Entity
    {
        public Product(int id, double weightInPounds) : base(id)
        {
            WeightInPounds = weightInPounds;
        }

        public double WeightInPounds { get; }
    }
}