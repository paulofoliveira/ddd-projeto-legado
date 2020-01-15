using PackageDelivery.Delivery;
using PackageDeliveryNew.Utils;

namespace PackageDelivery
{
    public partial class App
    {
        public App()
        {
            var connectionString = @"Server=(localdb)\MSSQLLocalDB;Database=DDDLegacyProjects;Trusted_Connection=true;";

            DBHelper.Init(connectionString);
            Settings.Init(connectionString);
        }
    }
}
