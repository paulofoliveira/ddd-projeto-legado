using PackageDelivery.Delivery;

namespace PackageDelivery
{
    public partial class App
    {
        public App()
        {
            DBHelper.Init(@"Server=(localdb)\MSSQLLocalDB;Database=DDDLegacyProjects;Trusted_Connection=true;");
        }
    }
}
