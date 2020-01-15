using PackageDeliveryNew.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PackageDeliveryNew.Deliveries
{
    public class Delivery : Entity
    {
        private const double PricePerMilePerPound = 0.04;
        private const double NonConditionalCharge = 20;
        public Delivery(int id, Address address) : base(id)
        {
            Contracts.Require(id >= 0);
            Contracts.Require(address != null);

            Address = address;
        }

        public Address Address { get; }

        public decimal GetEstimate(double distanceInMiles, List<ProductLine> productLines)
        {
            Contracts.Require(distanceInMiles >= 0, "Invalid distance.");
            Contracts.Require(productLines.Count > 0 && productLines.Count <= 4, "Invalid product line count.");

            //if (distanceInMiles < 0)
            //    throw new Exception("Invalid distance.");

            //if (productLines.Count == 0 || productLines.Count > 4)
            //    throw new Exception("Invalid product line count.");

            var totalWeightInPounds = productLines.Sum(p => p.Product.WeightInPounds * p.Amount);
            var estimate = totalWeightInPounds * distanceInMiles * PricePerMilePerPound + NonConditionalCharge;

            return decimal.Round((decimal)estimate, 2);
        }
    }
}
