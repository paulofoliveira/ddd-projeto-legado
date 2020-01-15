using PackageDeliveryNew.Common;
using System.Collections.Generic;

namespace PackageDeliveryNew.Deliveries
{
    public class ProductLine : ValueObject<ProductLine>
    {
        public ProductLine(Product product, int amount)
        {
            Contracts.Require(product != null);
            Contracts.Require(amount >= 0);

            Product = product;
            Amount = amount;
        }

        public Product Product { get; }
        public int Amount { get; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Product;
            yield return Amount;
        }
    }
}