using Dapper;
using PackageDeliveryNew.Utils;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageDeliveryNew.Deliveries
{
    public class ProductRepository
    {
        public Product GetById(int id)
        {
            ProductData productData = GetRawData(id);
            Product product = MapData(productData);

            return product;
        }

        private Product MapData(ProductData productData)
        {
            return new Product(productData.ProductID, productData.Name, productData.WeightInPounds);
        }

        public IReadOnlyList<Product> GetAll()
        {
            using (var conn = new SqlConnection(Settings.ConnectionString))
            {
                string sql = @"SELECT * FROM [dbo].[Product]";
                return conn.Query<ProductData>(sql)
                       .Select(p => MapData(p))
                       .ToList();
            }
        }

        private ProductData GetRawData(int id)
        {
            using (var conn = new SqlConnection(Settings.ConnectionString))
            {
                string sql = @"SELECT * FROM [dbo].[Product] WHERE [ProductID] = @id";
                return conn.QueryFirst<ProductData>(sql, new { id });
            }
        }

        private class ProductData
        {
            public int ProductID { get; set; }
            public string Name { get; set; }
            public double WeightInPounds { get; set; }
        }
    }
}
