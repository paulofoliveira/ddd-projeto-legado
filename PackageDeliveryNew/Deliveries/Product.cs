using PackageDeliveryNew.Common;
using System;

namespace PackageDeliveryNew.Deliveries
{
    public class Product : Entity
    {
        public Product(int id, string name, double weightInPounds) : base(id)
        {
            //if (weightInPounds < 0)
            //    throw new Exception();

            //if (weightInPounds <= 0)
            //    throw new Exception();

            // Usando Contracts, invertemos a condição. Torna mais legível.

            Contracts.Require(id >= 0);
            Contracts.Require(name != null);
            Contracts.Require(weightInPounds > 0, "Weight must be greater than 0.");

            Name = name;
            WeightInPounds = weightInPounds;
        }

        public string Name { get; set; }
        public double WeightInPounds { get; }
    }
}