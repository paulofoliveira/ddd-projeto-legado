using System.Collections.Generic;
using PackageDelivery.Common;
using PackageDeliveryNew.Deliveries;

namespace PackageDelivery.DeliveryNew
{
    public class ChangeProductViewModel : ViewModel
    {
        public IReadOnlyList<Product> Products { get; }
        public Product SelectedProduct { get; set; }
        public Command<Product> OkCommand { get; }
        public Command CancelCommand { get; }

        public override string Caption => "Change product";

        public ChangeProductViewModel()
        {
            var productRepository = new ProductRepository();
            Products = productRepository.GetAll();

            OkCommand = new Command<Product>(p => p != null, _ => DialogResult = true);
            CancelCommand = new Command(() => DialogResult = false);
        }
    }
}