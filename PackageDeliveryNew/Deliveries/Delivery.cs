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
        public Delivery(int id, Address destination, decimal? costEstimate, IReadOnlyList<ProductLine> lines) : base(id)
        {
            Contracts.Require(id >= 0);
            Contracts.Require(destination != null);
            Contracts.Require(costEstimate >= 0);
            Contracts.Require(lines != null);

            Destination = destination;
            CostEstimate = costEstimate;
            _lines = lines.ToList();
        }

        public Address Destination { get; }
        public decimal? CostEstimate { get; private set; }
        public IReadOnlyList<ProductLine> Lines => _lines.ToList();

        private List<ProductLine> _lines;

        public void RecalculateCostEstimate(double distanceInMiles)
        {
            Contracts.Require(distanceInMiles >= 0, "Invalid distance.");
            Contracts.Require(Lines.Count > 0, "Need at least one product line.");

            //if (distanceInMiles < 0)
            //    throw new Exception("Invalid distance.");

            //if (productLines.Count == 0 || productLines.Count > 4)
            //    throw new Exception("Invalid product line count.");

            var totalWeightInPounds = Lines.Sum(p => p.Product.WeightInPounds * p.Amount);
            var estimate = totalWeightInPounds * distanceInMiles * PricePerMilePerPound + NonConditionalCharge;

            CostEstimate = decimal.Round((decimal)estimate, 2);
        }

        public void DeleteLine(ProductLine line)
        {
            _lines.Remove(line);
        }

        public void AddProduct(Product product, int amount)
        {
            _lines.Add(new ProductLine(product, amount));
        }
    }
}
