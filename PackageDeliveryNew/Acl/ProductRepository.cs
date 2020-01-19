using Dapper;
using PackageDeliveryNew.Deliveries;
using PackageDeliveryNew.Utils;
using System;
using System.Data.SqlClient;

namespace PackageDeliveryNew.Acl
{
    public class ProductRepository
    {
        private const double PoundsInKilogram = 2.20462;
        public Product GetById(int id)
        {
            var legacy = GetLegacyProduct(id);
            var product = MapToProduct(legacy);
            return product;
        }

        private Product MapToProduct(ProductLegacy legacy)
        {
            if (legacy.WT == null && legacy.WT_KG == null)
                throw new Exception("Invalid weight in product: " + legacy.NMB_CM);

            var weightInPounds = legacy.WT ?? legacy.WT_KG.Value * PoundsInKilogram;

            //return new Product(legacy.NMB_CM, weightInPounds);
            return null;
        }

        private ProductLegacy GetLegacyProduct(int id)
        {
            using (var conn = new SqlConnection(Settings.ConnectionString))
            {
                var sql = @"SELECT [NMB_CM], [WT], [WT_KG] FROM [dbo].[PRD_TBL] WHERE [NMB_CM] = @id;";
                return conn.QueryFirst<ProductLegacy>(sql, new { id });
            }
        }

        internal class ProductLegacy
        {
            public int NMB_CM { get; set; }
            public double? WT { get; set; }
            public double? WT_KG { get; set; }
        }


    }
}
