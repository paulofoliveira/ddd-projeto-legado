using PackageDeliveryNew.Acl;
using PackageDeliveryNew.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageDeliveryNew.Deliveries
{
    public class EstimateCalculator
    {
        private readonly ProductRepository _productRepository;
        private readonly DeliveryRepository _deliveryRepository;
        private readonly AddressResolver _addressResolver;

        public EstimateCalculator()
        {
            _productRepository = new ProductRepository();
            _deliveryRepository = new DeliveryRepository();
            _addressResolver = new AddressResolver();
        }
        public Result<decimal> Calculate(int deliveryId, int? product1Id, int amount1, int? product2Id, int amount2, int? product3Id, int amount3, int? product4Id, int amount4)
        {
            if (product1Id == null && product2Id == null && product3Id == null && product4Id == null)
                return Result.Fail<decimal>("Must provide at least 1 product.");

            var delivery = _deliveryRepository.GetById(deliveryId);

            if (delivery == null)
                throw new Exception($"Delivery is not found for Id: {deliveryId}");

            var distance = _addressResolver.GetDistanceTo(delivery.Address);

            if (distance == null)
                return Result.Fail<decimal>("Address is not found.");

            var productLines = new List<(int? productId, int amount)>
            {
                (product1Id, amount1),
                (product2Id, amount2),
                (product3Id, amount3),
                (product4Id, amount4)
            }
            .Where(p => p.productId != null)
            .Select(p => new ProductLine(_productRepository.GetById(p.productId.Value), p.amount))
            .ToList();


            if (productLines.Any(p => p.Product == null))
                throw new Exception("One of the products is not found.");

            var estimate = delivery.GetEstimate(distance.Value, productLines);

            return Result.Ok(estimate);
        }
    }

    public class AddressResolver
    {
        public double? GetDistanceTo(Address address)
        {
            // Simulando chamada para a API.
            return 15;
        }
    }
}
